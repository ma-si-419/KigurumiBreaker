Shader "Custom/UiOutLine"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white"{}
        _scale("scale", Float) = 1

        _Color("Tint", Color) = (1,1,1,1)

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        
        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
       Tags
       {
           "Queue" = "Transparent"
           "IgnoreProjector" = "True"
           "RenderType" = "Transparent"
           "PreviewType" = "Plane"
           "CanUseSpriteAtlas" = "True"
       }

       Stencil
       {
           Ref[_Stencil]
           Comp[_StencilComp]
           Pass[_StencilOp]
           ReadMask[_StencilReadMask]
           WriteMask[_StencilWriteMask]
       }

       Cull Off
       Lighting Off
       ZWrite Off
       ZTest [unity_GUIZTestMode]
       Blend [_SrcFactor] [_DstFactor]
       ColorMask [_ColorMask]

       Pass
       {
           Name "Default"
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
           #pragma target 2.0

           #include "UnityCG.cginc"
           #include "UnityUI.cginc"

           #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
           #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

           struct appdata_t
           {
               float4 vertex : POSITION;
               float4 color : COLOR;
               float2 texcoord : TEXCOORD0;
               UNITY_VERTEX_INPUT_INSTANCE_ID
           };

           struct v2f
           {
               float4 vertex : SV_POSITION;
               fixed4 color : COLOR;
               float2 texcoord : TEXCOORD0;
               float4 worldPosition : TEXCOORD1;
               float4 mask : TEXCOORD2;
               UNITY_VERTEX_OUTPUT_STEREO
           };

           sampler2D _MainTex;
           fixed4 _Color;
           fixed4 _TextureSampleAdd;
           float4 _ClipRect;
           float4 _MainTex_ST;
           float _UIMaskSoftnessX;
           float _UIMaskSoftnessY;

           float _scaleFactor;

           float4 _MainTex_TexelSize;
           float4 _OutLineColor;

           v2f vert(appdata_t v)
           {
               v2f Out;
               UNITY_SETUP_INSTANCE_ID(v);
               UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(Out);
               float4 vPosition = UnityObjectToClipPos(v.vertex);
               Out.worldPosition = v.vertex;
               Out.vertex = vPosition;

               float2 pixelSize = vPosition.w;
               pixelSize /= float2(1,1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

               float4 cliampedRect = clamp(_ClipRect, -2e10, 2e10);
               float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
               Out.texcoord = TRANSFORM_TEX(v.vertex)
           }
       }

        ENDCG
    }
    FallBack "Diffuse"
}
