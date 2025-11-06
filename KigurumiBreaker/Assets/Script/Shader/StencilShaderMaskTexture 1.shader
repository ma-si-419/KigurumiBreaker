Shader "Custom/StencilShaderMaskTexture 1"
{
    Properties
	{
		[Enum(Off,0,Front,1,Back,2)]_CullMode("Cull Mode", Int) = 2
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("Main Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0)
		_MainBendingStrength("Main Bending Strength", Range( 0.2 , 1)) = 1
		_MainBendingMultiplier("Main Bending Multiplier", Float) = 1
		_LeafBendingStrength("Leaf Bending Strength", Range( 0 , 1)) = 1
		_LeafBendingMultiplier("Leaf Bending Multiplier", Float) = 1
		_LeafDownwardStrength("Leaf Downward Strength", Float) = 0.15
		_LeafForwardStrength("Leaf Forward Strength", Float) = 0.5
		_LeafNoiseScale("Leaf Noise Scale", Range( 0.1 , 5)) = 1
		_LeafFlutterSpeed("Leaf Flutter Speed", Float) = 1
		_WindDirectionRandomness("Wind Direction Randomness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "DisableBatching" = "True" "Queue" = "Geometry"}
		Cull [_CullMode]

		Stencil
		{
			Ref 2
			Comp always
			Pass replace
		}

		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows nolightmap  dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform int _CullMode;
		uniform sampler2D FFE_Wind_Mask;
		uniform float _LeafFlutterSpeed;
		uniform float _LeafNoiseScale;
		uniform float _LeafDownwardStrength;
		uniform float _LeafBendingStrength;
		uniform float _LeafBendingMultiplier;
		uniform float FFE_Leaf_Flutter;
		uniform float FFE_Wind_Speed;
		uniform float _MainBendingStrength;
		uniform float _MainBendingMultiplier;
		uniform float FFE_Wind_Strength;
		uniform float _LeafForwardStrength;
		uniform float3 FFE_Wind_Direction;
		uniform float _WindDirectionRandomness;
		uniform sampler2D _MainTex;
		uniform float4 _Color;
		uniform float _Cutoff = 0.5;


		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float mulTime262 = _Time.y * _LeafFlutterSpeed;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 temp_output_258_0 = ( ase_worldPos / ( 6.0 * _LeafNoiseScale ) );
			float2 panner213 = ( mulTime262 * float2( 0,0.4 ) + temp_output_258_0.xy);
			float LeafBendingStrength234 = ( _LeafBendingStrength * _LeafBendingMultiplier * FFE_Leaf_Flutter );
			float mulTime289 = _Time.y * ( 1.0 * FFE_Wind_Speed );
			float3 objToWorld126 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float2 temp_output_128_0 = (objToWorld126).xz;
			float2 lerpResult160 = lerp( ( temp_output_128_0 / 35.0 ) , ( temp_output_128_0 / 12.0 ) , 0.5);
			float2 panner132 = ( mulTime289 * float2( 0.15,0.05 ) + lerpResult160);
			float MainBendingStrength242 = ( _MainBendingStrength * _MainBendingMultiplier * FFE_Wind_Strength );
			float temp_output_165_0 = (( -0.2 + ( 1.3 * MainBendingStrength242 ) ) + (tex2Dlod( FFE_Wind_Mask, float4( panner132, 0, 0.0) ).r - 0.0) * (( 0.8 + ( 0.6 * MainBendingStrength242 ) ) - ( -0.2 + ( 1.3 * MainBendingStrength242 ) )) / (1.0 - 0.0));
			float lerpResult162 = lerp( 0.0 , ( temp_output_165_0 * MainBendingStrength242 ) , MainBendingStrength242);
			float MainBending210 = ( temp_output_165_0 * lerpResult162 );
			float2 panner199 = ( mulTime262 * float2( -0.2,0 ) + temp_output_258_0.xy);
			float4 appendResult207 = (float4(0.0 , ( tex2Dlod( FFE_Wind_Mask, float4( panner213, 0, 0.0) ).g * _LeafDownwardStrength * LeafBendingStrength234 * MainBending210 ) , ( tex2Dlod( FFE_Wind_Mask, float4( panner199, 0, 0.0) ).a * _LeafForwardStrength * LeafBendingStrength234 * MainBending210 ) , 0.0));
			float saferPower146 = abs( v.texcoord1.xy.y );
			float UV2WindMask227 = ( ( pow( saferPower146 , 8.0 ) * 0.6 ) + v.texcoord1.xy.y );
			float4 lerpResult233 = lerp( float4( 0,0,0,0 ) , ( v.color.g * appendResult207 ) , UV2WindMask227);
			float3 worldToObjDir206 = mul( unity_WorldToObject, float4( lerpResult233.xyz, 0 ) ).xyz;
			float3 LeafBending204 = worldToObjDir206;
			float ifLocalVar269 = 0;
			if( FFE_Wind_Direction.x == 0.0 )
				ifLocalVar269 = (float)1;
			float ifLocalVar270 = 0;
			if( FFE_Wind_Direction.z == 0.0 )
				ifLocalVar270 = (float)1;
			float3 lerpResult273 = lerp( FFE_Wind_Direction , float3(1,0,0) , ( ifLocalVar269 * ifLocalVar270 ));
			float3 worldToObjDir275 = normalize( mul( unity_WorldToObject, float4( lerpResult273, 0 ) ).xyz );
			float3 lerpResult276 = lerp( worldToObjDir275 , lerpResult273 , _WindDirectionRandomness);
			float3 WindDirection277 = lerpResult276;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 rotatedValue133 = RotateAroundAxis( float3( 0,0,0 ), ase_vertex3Pos, normalize( WindDirection277 ), radians( ( MainBending210 * 22.0 ) ) );
			float3 MainBendingRotation250 = ( rotatedValue133 - ase_vertex3Pos );
			v.vertex.xyz += ( LeafBending204 + ( UV2WindMask227 * MainBendingRotation250 ) );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float4 tex2DNode2 = tex2D( _MainTex, i.uv_texcoord );
			o.Albedo = ( tex2DNode2 * _Color ).rgb;
			float temp_output_252_0 = 0.0;
			float3 temp_cast_1 = (temp_output_252_0).xxx;
			o.Specular = temp_cast_1;
			o.Smoothness = temp_output_252_0;
			o.Alpha = 1;
			clip( ( tex2DNode2.a * _Color.a ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
