Shader "Custom/StencilShaderMaskReform"
{
     Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 0, 0, 1)
        _Alpha("Alpha Value", Range(0, 1)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Geometry" }
        LOD 100

        // アルファブレンド設定（透明度有効）
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Stencil
            {
                Ref 2
                Comp always
                Pass replace
            }

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST; // Tiling/Offset対応
            float4 _Color;
            float _Alpha;

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, i.uv);    //テクスチャマップ取得

                
                fixed4 finalColor = texCol * _Color;    //テクスチャカラーと標準カラー乗算

                finalColor.a *= _Alpha;

                return finalColor;
            }
            ENDCG
        }
    }
}
