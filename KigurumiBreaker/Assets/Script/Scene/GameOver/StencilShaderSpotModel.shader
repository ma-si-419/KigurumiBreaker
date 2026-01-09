Shader "Custom/StencilShaderSpotModel"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{}
        _Color("Color", Color) = (0,0,0,1)
        _Alpha("Alpha Value", Range(0, 1)) = 1.0
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
            float4 _Color;
            float _Alpha;

           fixed4 frag(v2f_img i) : SV_Target
           {
               _Color.a = _Alpha;     //アルファ値好きなように変えてくれ
               return _Color; 
           }

           ENDCG
        }
       
    }
}
