Shader "Custom/StencilShaderDeathModel"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{}
    }
    SubShader
    {
       Tags {"Queue" = "Geometry+1"}

       //アルファを有効
       Blend SrcAlpha OneMinusSrcAlpha

       Pass
       {
           Stencil
           {
               Ref 2
               Comp Equal
           }
           ZTest Always

           CGPROGRAM
           sampler2D _MainTex;
           #pragma vertex vert_img
           #pragma fragment frag
           #include "UnityCG.cginc"

           fixed4 frag(v2f_img i) : SV_Target
           {
               return tex2D(_MainTex, i.uv); 
           }

           ENDCG
       }
    }
    FallBack "Diffuse"
}
