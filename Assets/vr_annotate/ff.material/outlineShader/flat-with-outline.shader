// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/StencilTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FillColor ("FillColor", Color) = (1,1,1,0.5) 
        _LineWidth ("LineWidth", Range(0, 1)) = 1
        _LineColor ("LineColor", Color) = (1,0,0,0.6) 
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        ZTest Always

        Stencil
        {
            Ref 1
            Comp notequal
            Pass replace
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FillColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // HACK: fix horizontal disaligment on Vive
                o.vertex.x /= 1.012;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _FillColor;
            }
            ENDCG
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _LineWidth;
            float4 _LineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float4 n = mul(UNITY_MATRIX_MV, float4(v.normal, 0));
                n.z = 0; 
                n.w = 0;
                n.xyz= normalize(n.xyz);

                float4 n2 = mul(UNITY_MATRIX_P, n);
                o.vertex.xy += n2.xy * o.vertex.z * _LineWidth;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _LineColor;
            }
            ENDCG
        }
    }
}
 