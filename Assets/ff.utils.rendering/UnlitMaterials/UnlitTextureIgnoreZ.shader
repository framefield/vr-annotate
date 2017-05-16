// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "framefield/UnlitTextureIgnoreZ" {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_tintAmount ("Tint Amount", Range (0, 1)) = 0
	_tintColor ("Tint Color", Color) = (0.5, 0.5, 0.5 ,0.5)
}
    
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Fog {Mode Off} 
	LOD 100
        
	Lighting Off
	ZWrite Off
    ZTest Always
	AlphaTest Off
	Cull Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _tintColor;
		float _tintAmount;

		#include "UnityCG.cginc"


		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			//UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		};

		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			//UNITY_TRANSFER_FOG(o,o.vertex);
			return o;
		}

		float4 frag(v2f i) : COLOR 
		{			
			float u = i.uv.x;
			float v = i.uv.y;
			float4 c =tex2D(_MainTex, float2(u,v)).rgba;
			return float4(c.rgb +  (_tintColor.rgb - 0.5) * _tintAmount * 2, c.a * _tintColor.a * 2);
		} 
              
		ENDCG
	}
} 
    
FallBack "Unlit/Texture"
}
