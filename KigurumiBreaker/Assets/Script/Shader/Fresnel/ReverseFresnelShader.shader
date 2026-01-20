Shader "Custom/ReverseFresnelShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{}
        _Color("Color", Color) = (0.2, 0.8, 1, 1)
        _OuterColor("Outer Color", Color) = (1, 0.4, 0.2, 1)
        _FresnelPower("Fresnel Power", Range(0, 10)) = 4
        _OuterPower("Outer Power", Range(0, 10)) = 2
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
            float4 _OuterColor;
            float _FresnelPower;
            float _OuterPower;
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
               float invFresnel = 1.0 - fresnel;  //反転フレネル

               float4 tex = tex2D(_MainTex, i.uv);

               //内側の色
               float3 innerColor = tex.rgb * _Color.rgb;

               //外側の色
               float3 outerColor = _OuterColor.rgb;

               float edge = pow(fresnel, _OuterPower);

               //色をブレンド
               float3 finalColor = lerp(innerColor, outerColor, edge);

               //中央が発光
               float3 emission = invFresnel * _Color.rgb * _EmissionStrenght;

               //外側程透明になる
               float alpha = saturate(invFresnel * _Transparency);

               return float4(finalColor.rgb + emission, alpha);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
