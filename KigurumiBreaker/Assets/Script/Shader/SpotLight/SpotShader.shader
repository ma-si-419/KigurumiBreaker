Shader "Custom/SpotShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Intensity("Intensity", Float) = 2.0
        _Range("Light Range", Float) = 5.0
        _Angle("Outer Angle", Range(1,90)) = 30
        _InnerAngle("Inner Angle", Range(0, 90)) = 20

        _LightPos("Light Pos", Vector) = (0,3,0,1)
        _LightDir("Light Dir", Vector) = (0, -1, 0, 0)
    }

    SubShader
    {
        Tags{"Queue" = "Transparent" "RenderType" = "Transparent"}
        ZWrite Off
        Blend SrcAlpha One           //加算
        Cull Front                   //内側表示

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;

            float3 _LightPos;
            float3 _LightDir;

            float _Intensity;
            float _Range;
            float _Angle;
            float _InnerAngle;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;

            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                
                float3 toPixel = i.worldPos - _LightPos;

                float dist = length(toPixel);

                //距離減衰
                float disAtt = saturate(1.0 - dist / _Range);
                disAtt *= disAtt;

                float3 dir = normalize(toPixel);

                float angle = degrees(acos(dot(-dir, _LightDir)));

                //1～0によるスムーズアップの距離減衰
                float angleAtt = saturate((_Angle - angle) / (_Angle - _InnerAngle));

                //全体的な強度
                float intensity = angleAtt * disAtt * _Intensity;

                return float4(_Color.rgb * intensity, intensity);
            }

            ENDCG
        }
    }


    // SubShader
    // {
    //    Tags{"RenderType"="Transparent" "Queue"="Transparent"}
    //     Blend SrcAlpha One       // 加算
    //     Cull Front               // 内側を表示
    //     ZWrite Off

        

    //     CGPROGRAM
    //     #pragma surface surf Lambert alpha:fade nofog

    //     float4 _Color;
    //     float _Intensity;
    //     float _Falloff;

    //     struct Input
    //     {
    //         float2 uv_MainTex;
    //     }

    //     void surf(Input IN, inout SurfaceOutput o)
    //     {
    //         //円錐の先端を明るくする簡易的なフェード
    //         float cone = saturate(1.0 - IN.uv_MainTex.y);

    //         //中心軸方向に向かうほど明るく
    //         float radial = saturate(1.0 - abs(IN.uv_MainTex.x - 0.5) * 2.0);

    //         //距離減衰
    //         float distanceFalloff = pow(saturate(1.0 - IN.uv_MainTex.y), _Falloff);

    //         // 総合的な強度
    //         float intensity = cone * radial * distanceFalloff * _Intensity;

    //         o.Emission = _Color.rgb * intensity;
    //         o.Alpha = intensity;
    //     }

    //     ENDCG
    // }

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
