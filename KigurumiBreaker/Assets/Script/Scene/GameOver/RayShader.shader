Shader "Custom/RayShader" {
    Properties {
        _MainTex ("Texture", 2D) = "black" {}
        _Bright ("Brighten Power", float) = 1.0
        _BrightTex ("Brighten Alpha Texture", 2D) = "white" {}
        _UVScrollX("UV Scroll X", float) = 0
        _UVScrollY("UV Scroll Y ", float) = 0
    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        ZWrite Off
        BlendOp Add
        Blend SrcAlpha One

        Pass {
            Cull Front
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                
                float _UVScrollX;
                float _UVScrollY;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.texcoord += float2(_UVScrollX, _UVScrollY) * _Time;
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.texcoord);
                    return col;
                }
            ENDCG
        }

        GrabPass{}

        BlendOp Add
        Blend One Zero
        Pass {
            Cull Back
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 texcoord : TEXCOORD0;
                    float4 screentex : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                float _UVScrollX;
                float _UVScrollY;

                sampler2D _GrabTexture;
                float4 _GrabTexture_ST;

                sampler2D _BrightTex;
                float4 _BrightTex_ST;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.texcoord += float2(_UVScrollX, _UVScrollY) * _Time;
                    o.screentex = ComputeScreenPos(o.vertex);
                    return o;
                }

                float _Bright;

                fixed4 frag (v2f i) : SV_Target
                {
                    float4 col = tex2D(_GrabTexture, i.screentex.xy / i.screentex.w);
                    col += col * _Bright * tex2D(_BrightTex, i.texcoord).a;

                    float4 texcolor = tex2D(_MainTex, i.texcoord);
                    col.rgb += texcolor.rgb * texcolor.a;
                    return col;
                }
            ENDCG
        }
    }

}
