Shader "Custom/Blur"
{
    Properties
    {
        _Count("Count", Range(4.0, 16.0)) = 8.0
        _BlurRadius("BlurRadius", Float) = 0

        _MainTex("Texture", 2D) = "white"{}
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
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_CUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            Name "Dafault"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPAHACLIP

            #include "UnityCG.cginc"
            //C:\Users\ü∫ñ{ ê¬ìÏ\Documents\GitHub\KigurumiBreaker\KigurumiBreaker\Assets\Script\Scene\TitleSceneFolder\UI
            #include "Assets/Script/Scene/TitleSceneFolder/UI/Uvutil.cginc"

            int _Count;
            float _Strength;

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            float _BlurRadius;
            float4 _MainTex_TexelSize;
            
            float _scaleFactor;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(o.worldPosition);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float2 center = float2(0.5, 0.5);
                #if defined(USE_ATLAS)

                center = MeshUVtoAtlasUV(center, _MainTex_TexelSize.zw, _textureRect);
                #endif

                float2 direction = (center - i.uv) * _BlurRadius * _scaleFactor;
                fixed4 resultColor = fixed4(0,0,0,0);
                float2 uv = i.uv;

                for(int index = 0; index < _Count; index++)
                {
                    fixed4 color = tex2D(_MainTex, uv);
                    #if defined(USE_ATLAS)

                    color *= IsInner(uv, _MainTex_TexelSize.zw, _textureRect);
                    #endif

                    resultColor += color;
                    uv += direction;
                }

                fixed4 finalColor = resultColor / _Count;
                finalColor *= i.color;
                return finalColor;
            }

            ENDCG

        }
    }
    FallBack "Diffuse"
}
