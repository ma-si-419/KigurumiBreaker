Shader "Custom/UiOutLine"
{

    Properties
    {
        // UI Image / Sprite のテクスチャ
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

    
        // ※ 現状未使用
        _scale ("scale", Range(0, 30)) = 1
        _Expand ("Outline Expand", Float) = 1

        // UI の Tint Color（Image.color）
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        // 描画するカラーチャンネル
        _ColorMask ("Color Mask", Float) = 15
    
        // UI Alpha Clip 用トグル
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {

        Tags
        {
            // UI 用の透明描画設定
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

    //UI Mask / ScrollRect 用 Stencil 設定
    Stencil
    {
        Ref [_Stencil]
        Comp [_StencilComp]
        Pass [_StencilOp]
        ReadMask [_StencilReadMask]
        WriteMask [_StencilWriteMask]
    }

    
    Cull Off
    Lighting Off
    ZWrite Off
    ZTest [unity_GUIZTestMode]

    // C# から設定される Blend
    Blend [_SrcFactor] [_DstFactor]

    ColorMask [_ColorMask]

    Pass
    {
        Name "Default"
        CGPROGRAM

        //シェーダー設定
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0

        // Unity 標準 UI 用 include
        #include "UnityCG.cginc"
        #include "UnityUI.cginc"

        // UI Mask / AlphaClip 対応
        #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
        #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

        // ===== 頂点入力 =====
        struct appdata_t
        {
            float4 vertex : POSITION;   // 頂点座標
            float4 color  : COLOR;      // Image.color
            float2 texcoord : TEXCOORD0;// UV
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        // ===== 頂点 → フラグメント =====
        struct v2f
        {
            float4 vertex : SV_POSITION; // クリップ座標
            fixed4 color : COLOR;        // Tint * 頂点カラー
            float2 texcoord : TEXCOORD0; // UV
            float4 worldPosition : TEXCOORD1; // UI Mask 用
            float4 mask : TEXCOORD2;     // RectMask2D 用
            UNITY_VERTEX_OUTPUT_STEREO
        };

        // ===== テクスチャ / UI 共通変数 =====
        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _TextureSampleAdd;
        float4 _ClipRect;
        float4 _MainTex_ST;
        float _UIMaskSoftnessX;
        float _UIMaskSoftnessY;

        // Canvas サイズに対するスケール補正（C# から設定）
        float _Expand;
        float _scale;
        float2 _scaleFactor;

        // ===== カスタム用 =====
        float4 _MainTex_TexelSize; // 1px の UV サイズ
        float4 _OutlineColor;     // アウトライン色（HDR対応）


        // ===== HSV → RGB 変換 =====
        float3 HSVtoRGB(float3 c)
        {
            float4 k = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            float3 p = abs(frac(c.xxx + k.xyz) * 6.0 - k.www);
            return c.z * lerp(k.xxx, saturate(p - k.xxx), c.y);
        }

        // ===== 頂点シェーダー =====
        v2f vert(appdata_t v)
        {
            v2f OUT;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

            // クリップ空間へ変換
            float4 vPosition = UnityObjectToClipPos(v.vertex);
            OUT.vertex = vPosition;
            OUT.worldPosition = v.vertex;

            // UI Mask 用ピクセルサイズ計算
            float2 pixelSize = vPosition.w;
            pixelSize /= abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

            // RectMask2D の範囲を安全に clamp
            float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);

            // UV 変換
            OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);

            // Mask 情報
            OUT.mask = float4(
                v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw,
                0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy))
            );

            // Image.color を乗算
            OUT.color = v.color * _Color;
            return OUT;
        }

        // ===== 輝度計算（Sobel 用）=====
        half luminance(float4 c)
        {
            // 人間の視覚に近い輝度計算 + Alpha 反映
            return (c.r * 0.298912 + c.g * 0.586611 + c.b * 0.114478) * c.a;
        }

        // ===== Sobel フィルタでアウトライン抽出 =====
        fixed4 sobel(v2f IN)
        {
            // テクスチャ 1px 分の UV オフセット
            float dx = _MainTex_TexelSize.x * _scaleFactor.x;
            float dy = _MainTex_TexelSize.y * _scaleFactor.y;

            // 周囲 3x3 ピクセルをサンプリング
            half4 c00rgba = tex2D(_MainTex, IN.texcoord + half2(-dx, -dy));
            half4 c01rgba = tex2D(_MainTex, IN.texcoord + half2(-dx, 0.0));
            half4 c02rgba = tex2D(_MainTex, IN.texcoord + half2(-dx, dy));
            half4 c10rgba = tex2D(_MainTex, IN.texcoord + half2(0.0, -dy));
            half4 c12rgba = tex2D(_MainTex, IN.texcoord + half2(0.0, dy));
            half4 c20rgba = tex2D(_MainTex, IN.texcoord + half2(dx, -dy));
            half4 c21rgba = tex2D(_MainTex, IN.texcoord + half2(dx, 0.0));
            half4 c22rgba = tex2D(_MainTex, IN.texcoord + half2(dx, dy));

            // RGB 輝度
            half c00 = luminance(c00rgba);
            half c01 = luminance(c01rgba);
            half c02 = luminance(c02rgba);
            half c10 = luminance(c10rgba);
            half c12 = luminance(c12rgba);
            half c20 = luminance(c20rgba);
            half c21 = luminance(c21rgba);
            half c22 = luminance(c22rgba);

            // Sobel フィルタ（X/Y）
            half sxColor = c00 * -1 + c10 * -2 + c20 * -1 + c02 + c12 * 2 + c22;
            half syColor = c00 * -1 + c01 * -2 + c02 * -1 + c20 + c21 * 2 + c22;

            // Alpha 用 Sobel
            half sxAlpha = c00rgba.a * -1 + c10rgba.a * -2 + c20rgba.a * -1
                         + c02rgba.a + c12rgba.a * 2 + c22rgba.a;
            half syAlpha = c00rgba.a * -1 + c01rgba.a * -2 + c02rgba.a * -1
                         + c20rgba.a + c21rgba.a * 2 + c22rgba.a;

            // 勾配強度
            half outlineRGB   = sqrt(sxColor * sxColor + syColor * syColor);
            half outlineAlpha = sqrt(sxAlpha * sxAlpha + syAlpha * syAlpha);

            // RGB / Alpha の強い方を使用
            half outline = max(outlineRGB, outlineAlpha);

            // 0～1 に制限（※ここを外すと発光を強くできる）
            //outline = saturate(outline);
            outline = outline * 20.0;

            // アウトライン色を出力
            return half4
            (
                _OutlineColor.rgb,
                _OutlineColor.a * outline * IN.color.a
            );
        }

        // ===== フラグメントシェーダー =====
        half4 frag(v2f IN) : SV_Target
        {
            // Sobel によるアウトライン描画
            half4 color = sobel(IN);

            // RectMask2D 対応
            #ifdef UNITY_UI_CLIP_RECT
            half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
            color.a *= m.x * m.y;
            #endif

            // Alpha Clip
            #ifdef UNITY_UI_ALPHACLIP
            clip (color.a - 0.001);
            #endif

            return color;
        }
        ENDCG
    }
}

}
