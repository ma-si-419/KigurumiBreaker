Shader "Custom/ToonShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white"{}
        _SubTex("Emission Texture", 2D) = "black"{}
        _Alpha("Alpha", Range(0,1)) = 0.5
        _EmissionPower("Emission Power", Range(0,5)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _SubTex;
            float4 _MainTex_ST;
            float _Alpha;
            float _EmissionPower;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //ÉgÉDÅ[Éìí≤ÇÃâAâeåvéZ
                half nl = max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz));
                if(nl <= 0.01f) nl = 0.3f;
                else if(nl <= 0.3f) nl = 0.5f;
                else nl = 1.0f;

                //í èÌÉJÉâÅ[
                fixed4 mainCol = tex2D(_MainTex, i.uv);
                mainCol.rgb *= nl;

                //î≠åıÉJÉâÅ[
                fixed4 emisCol = tex2D(_SubTex, i.uv) * _EmissionPower;

                //í èÌêFÇ∆î≠åıêFÇâ¡éZçáê¨
                fixed4 finalCol;
                finalCol.rgb = mainCol.rgb + emisCol.rgb;
                finalCol.a = _Alpha;

                return finalCol;
            }
            ENDCG
        }
    }
}
