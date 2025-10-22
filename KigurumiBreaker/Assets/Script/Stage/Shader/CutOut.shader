Shader "Custom/CutOut"
{
    Properties
    {
        _Color ("Water Tint", Color) = (0.2, 0.5, 0.8, 0.5)
        _MainTex ("Normal (Wave) Map", 2D) = "bump" {}
        _WaveStrength ("Wave Strength", Range(0, 0.1)) = 0.02
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1
        _FresnelPower ("Fresnel Power", Range(0.1, 5)) = 2
        _Transparency ("Transparency", Range(0,1)) = 0.6
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        half4 _Color;
        float _WaveStrength;
        float _WaveSpeed;
        float _FresnelPower;
        float _Transparency;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // --- 波模様（法線マップのスクロール） ---
            float2 uv1 = IN.uv_MainTex + _Time.y * _WaveSpeed * float2(0.05, 0.03);
            float2 uv2 = IN.uv_MainTex - _Time.y * _WaveSpeed * float2(0.03, 0.07);
            float3 n1 = UnpackNormal(tex2D(_MainTex, uv1));
            float3 n2 = UnpackNormal(tex2D(_MainTex, uv2));
            o.Normal = normalize(n1 + n2);

            // --- フレネル反射風効果（視線角で透明度変化） ---
            float fresnel = pow(1 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPower);

            // --- ベースカラー ---
            o.Albedo = _Color.rgb;
            o.Metallic = 0;
            o.Smoothness = 0.8;

            // --- 半透明設定 ---
            o.Alpha = lerp(_Transparency, 1, fresnel); // 角度で透明度変化
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}