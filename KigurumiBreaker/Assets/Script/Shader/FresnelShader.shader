Shader "Custom/FresnelShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{}
        _Color("Color", Color) = (0.2, 0.8, 1, 1)
        _FresnelPower("Fresnel Power", Range(0, 1)) = 4
        _EmissionStrenght("Emission Strenght", Range(0, 10)) = 2
        _Transparency("Tranparency", Range(0, 1)) = 0.5
    }


    SubShader
    {
        Tags{"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _FresnelPower;
            float _EmissionStrenght;
            float _Transparency;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float fresnel = pow(1.0 - saturate(dot(i.viewDir, i.worldNormal)), _FresnelPower);

                float4 tex = tex2D(_MainTex, i.uv);   //テクスチャ
                float4 baseColor = tex * _Color; 

                float3 emission = fresnel * _Color.rgb * _EmissionStrenght;   //発光部分調整可能

                float alpha = saturate(fresnel + (1 - _Transparency));    //透明感を足す

                return float4(baseColor.rgb + emission, alpha);
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
