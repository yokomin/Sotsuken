Shader "Hidden/FrostBlur"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE
#include "UnityCG.cginc"
		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};
	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};
	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}
	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	// 二項係数に基づく重み
#define WS 256.0
#define W0 (70.0 / WS)
#define W1 (56.0 / WS)
#define W2 (28.0 / WS)
#define W3 (8.0 / WS)
#define W4 (1.0 / WS)
	ENDCG

		// パス0...水平ぼかし
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		fixed4 frag(v2f i) : SV_Target
	{
		float2 scale = _MainTex_TexelSize.xy;
		fixed4 col = W0 * tex2D(_MainTex, i.uv);
		col += W1 * tex2D(_MainTex, i.uv + scale * float2(1, 0));
		col += W1 * tex2D(_MainTex, i.uv + scale * float2(-1, 0));
		col += W2 * tex2D(_MainTex, i.uv + scale * float2(2, 0));
		col += W2 * tex2D(_MainTex, i.uv + scale * float2(-2, 0));
		col += W3 * tex2D(_MainTex, i.uv + scale * float2(3, 0));
		col += W3 * tex2D(_MainTex, i.uv + scale * float2(-3, 0));
		col += W4 * tex2D(_MainTex, i.uv + scale * float2(4, 0));
		col += W4 * tex2D(_MainTex, i.uv + scale * float2(-4, 0));
		return col;
	}
		ENDCG
	}

		// パス1...垂直ぼかし
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		fixed4 frag(v2f i) : SV_Target
	{
		float2 scale = _MainTex_TexelSize.xy;
		fixed4 col = W0 * tex2D(_MainTex, i.uv);
		col += W1 * tex2D(_MainTex, i.uv + scale * float2(0, 1));
		col += W1 * tex2D(_MainTex, i.uv + scale * float2(0, -1));
		col += W2 * tex2D(_MainTex, i.uv + scale * float2(0, 2));
		col += W2 * tex2D(_MainTex, i.uv + scale * float2(0, -2));
		col += W3 * tex2D(_MainTex, i.uv + scale * float2(0, 3));
		col += W3 * tex2D(_MainTex, i.uv + scale * float2(0, -3));
		col += W4 * tex2D(_MainTex, i.uv + scale * float2(0, 4));
		col += W4 * tex2D(_MainTex, i.uv + scale * float2(0, -4));
		return col;
	}
		ENDCG
	}
	}
}
