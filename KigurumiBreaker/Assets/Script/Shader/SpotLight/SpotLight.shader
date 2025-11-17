Shader "Custom/SpotLight"
{
    Properties
    {
        _Color("Light Color", Color) = (1,1,1,1)
        _LightPos("Light Position", Vector) = (0, 3, 0, 1)
        _LightDir("Light Direction", Vector) = (0, -1, 0, 0)
        _InnerAngle("Inner Angle", Range(0, 1)) = 0.9
        _OuterAngle("Outer Angle", Range(0,1)) = 0.7
        _Intensity("Light Intensity", Float) = 2.0
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            float4 _Color;
            float4 _LightPos;
            float4 _LightDir;
            float _InnerAngle;
            float _OuterAngle;
            float _Intensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }

            float SpotLight(float3 worldPos, float3 normal)
            {
                float3 L = normalize(_LightPos.xyz - worldPos);

                //ライト方向ベクトル
                float3 D = normalize(-_LightDir.xyz);
                //角度
                float angleDot = dot(L,D);
                //角度による減衰
                float spot = smoothstep(_OuterAngle, _InnerAngle, angleDot);
                //距離減衰
                float dist = length(_LightPos.xyz - worldPos);
                float atten = 1.0 / (1.0 + dist * dist);

                float NdotL = saturate(dot(normal, L));
                return spot * atten * NdotL;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float intensity = SpotLight(i.worldPos, i.normal);
                return _Color * intensity * _Intensity;
            }

            ENDCG
        }

    }


    // Properties
    // {
    //     _Color ("Color", Color) = (1,1,1,1)
    //     _MainTex ("Albedo (RGB)", 2D) = "white" {}
    //     _Glossiness ("Smoothness", Range(0,1)) = 0.5
    //     _Metallic ("Metallic", Range(0,1)) = 0.0
    // }
    // SubShader
    // {
    //     Tags { "RenderType"="Opaque" }
    //     LOD 200

    //     CGPROGRAM
    //     // Physically based Standard lighting model, and enable shadows on all light types
    //     #pragma surface surf Standard fullforwardshadows

    //     // Use shader model 3.0 target, to get nicer looking lighting
    //     #pragma target 3.0

    //     sampler2D _MainTex;

    //     struct Input
    //     {
    //         float2 uv_MainTex;
    //     };

    //     half _Glossiness;
    //     half _Metallic;
    //     fixed4 _Color;

    //     // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    //     // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    //     // #pragma instancing_options assumeuniformscaling
    //     UNITY_INSTANCING_BUFFER_START(Props)
    //         // put more per-instance properties here
    //     UNITY_INSTANCING_BUFFER_END(Props)

    //     void surf (Input IN, inout SurfaceOutputStandard o)
    //     {
    //         // Albedo comes from a texture tinted by color
    //         fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    //         o.Albedo = c.rgb;
    //         // Metallic and smoothness come from slider variables
    //         o.Metallic = _Metallic;
    //         o.Smoothness = _Glossiness;
    //         o.Alpha = c.a;
    //     }
    //     ENDCG
    // }
    // FallBack "Diffuse"
}
