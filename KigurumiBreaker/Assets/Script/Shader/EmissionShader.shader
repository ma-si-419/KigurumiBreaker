Shader "Custom/EmissionShader"
{
     Properties
    {
        _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
        _EmissionPower ("Emission Power", Range(0, 5)) = 1.0
        _Alpha ("Alpha", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // “§–¾‚â‰ÁŽZ”­Œõ‚ð—LŒø‰»
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _EmissionColor;
            float _EmissionPower;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _EmissionColor * _EmissionPower;
                col.a = _Alpha;
                return col;
            }
            ENDCG
        }
    }

    FallBack "Unlit/Color"
}
