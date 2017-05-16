// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "framefield/UnlitTextureIgnoreZNoTint" {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_color ("Color", Color) = (1, 1, 1 ,1)
}
    
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Fog {Mode Off} 
	LOD 100
        
	Lighting Off
	ZWrite Off
    ZTest Always
	//AlphaTest Off
	Cull Off
    //Blend One One
	//Blend SrcAlpha OneMinusSrcAlpha 
    Blend SrcAlpha OneMinusSrcAlpha, One One
    //BlendOp LogicalCopy
    //Blend Off
	
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_nicest
		

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _color;

		#include "UnityCG.cginc"


		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		    fixed4 color : COLOR;            
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			//UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
            float4 color: COLOR;
		};

		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.color = v.color;
			return o;
		}

		float4 frag(v2f i) : COLOR 
		{			
			float u = i.uv.x;
			float v = i.uv.y;
			float4 c =tex2D(_MainTex, float2(u,v)).rgba;
			return c * _color * i.color;
		} 
              
		ENDCG
	}
} 
    
FallBack "Unlit/Texture"
}
