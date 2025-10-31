Shader "Custom/EmissionShader"
{
     Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _Alpha("Alpha Value", Range(0,1)) = 1
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionPower ("Emission Power", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        //アルファを有効
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _EmissionColor;
        float _Alpha;
        half _EmissionPower;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;    //基本色テクスチャ
            c.a = _Alpha;
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            o.Emission = _EmissionColor.rgb * _EmissionPower;      //発光カラー
        }
        ENDCG
        
    }

    FallBack "Diffuse"
}
