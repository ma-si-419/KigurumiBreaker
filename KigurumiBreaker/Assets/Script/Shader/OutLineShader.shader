Shader "Custom/OutLineShader"
{
   Properties
   {
	   _MainTex("Texture", 2D) = "white"{}
	   _Color("Color", Color) = (1,1,0.1,0.1)
	   _Alpha("Alpha Value", Range(0,1)) = 1.0
	   _OutLineSize("OutLineSize", Range(0,100)) = 0.02
   }

   SubShader
   {
	   Tags{"RenderType" = "Transparent" "Queue" = "Transparent"}
	   LOD 100

	   //アルファを有効
	   Blend SrcAlpha OneMinusSrcAlpha

	   Pass
	   {
		   Cull Front

		   CGPROGRAM
		   #pragma vertex vert
		   #pragma fragment frag
		   #include "UnityCG.cginc"

		   sampler2D _MainTex;
		   fixed4 _Color;
		   float _Alpha;
		   float _OutLineSize;

		   struct appdata
		   {
			   float4 vertex : POSITION;
               float3 normal : NORMAL;
               float2 uv : TEXCOORD0;
		   };

		   struct v2f
		   {
			   float4 vertex : SV_POSITION;
               float2 uv : TEXCOORD0;
		   };

		   v2f vert(appdata v)
		   {
			   v2f o;
               v.vertex += float4(v.normal * _OutLineSize, 0);
               o.vertex = UnityObjectToClipPos(v.vertex);
               o.uv = v.uv;
               return o;
		   }

		   fixed4 frag(v2f i) : SV_Target
		   {
			   _Color.a = _Alpha;
               return _Color;
		   }

		   ENDCG

	   }

   }

}
