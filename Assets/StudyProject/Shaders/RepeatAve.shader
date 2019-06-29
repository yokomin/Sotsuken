Shader "Custom/RepeatAve"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Diffuse", 2D) = "white" {}
	_Noise("Noise", 2D) = "gray" {}
	_Range("Range", Float) = 0.025
		_Blur("Blur", Float) = 0.005
		[KeywordEnum(R0,R1,R2,R3,R4,R5,R6,R7,R8)] _Repeat("Repeat", Float) = 0
		_Size("Size", int) = 3
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" }
		Cull Off

		CGINCLUDE
#pragma target 3.0
#include "UnityCG.cginc"
		ENDCG

		GrabPass{ "_FirstGrabTexture" }
		pass
	{
		CGPROGRAM
#pragma multi_compile _ _REPEAT_R1 _REPEAT_R2 _REPEAT_R3 _REPEAT_R4 _REPEAT_R5 _REPEAT_R6 _REPEAT_R7 _REPEAT_R8
#pragma vertex vertAve
#pragma fragment fragAveFirst
#if _REPEAT_R1 | _REPEAT_R2 | _REPEAT_R3 | _REPEAT_R4 | _REPEAT_R5 | _REPEAT_R6 | _REPEAT_R7 | _REPEAT_R8
#define _AVE
#endif
#include "RepeatAve.cginc"
			sampler2D _FirstGrabTexture;
		half4 fragAveFirst(v2fAve i) : SV_Target
		{
			float2 uv = i.screenPos.xy / i.screenPos.w;
			half4 frost = 0.0;
#ifdef _AVE
			for (int m = -(_Size - 1) / 2; m <= (_Size - 1) / 2; m++)
			{
				for (int n = -(_Size - 1) / 2; n <= (_Size - 1) / 2; n++)
				{
					frost += tex2D(_FirstGrabTexture, uv + float2(_Blur * m, _Blur * n));
				}
			}
			frost /= _Size * _Size;
#else
			frost = tex2D(_FirstGrabTexture, uv);
#endif
			return half4(frost.xyz, 1);
		}
			ENDCG
	}


	GrabPass{}
		pass
	{
		CGPROGRAM
#pragma multi_compile _ _REPEAT_R2 _REPEAT_R3 _REPEAT_R4 _REPEAT_R5 _REPEAT_R6 _REPEAT_R7 _REPEAT_R8
#pragma vertex vertAve
#pragma fragment fragAve
#if _REPEAT_R2 | _REPEAT_R3 | _REPEAT_R4 | _REPEAT_R5 | _REPEAT_R6 | _REPEAT_R7 | _REPEAT_R8
#define _AVE
#endif
#include "RepeatAve.cginc"
			ENDCG
	}

	GrabPass{}
		pass
	{
		CGPROGRAM
#pragma multi_compile _ _REPEAT_R3 _REPEAT_R4 _REPEAT_R5 _REPEAT_R6 _REPEAT_R7 _REPEAT_R8
#pragma vertex vertAve
#pragma fragment fragAve
#if _REPEAT_R3 | _REPEAT_R4 | _REPEAT_R5 | _REPEAT_R6 | _REPEAT_R7 | _REPEAT_R8
#define _AVE
#endif
#include "RepeatAve.cginc"
			ENDCG
	}
	/*
	GrabPass{}
		pass
	{
		CGPROGRAM
#pragma multi_compile _ _REPEAT_R4 _REPEAT_R5 _REPEAT_R6 _REPEAT_R7 _REPEAT_R8
#pragma vertex vertAve
#pragma fragment fragAve
#if _REPEAT_R4 | _REPEAT_R5 | _REPEAT_R6 | _REPEAT_R7 | _REPEAT_R8
#define _AVE
#endif
#include "RepeatAve.cginc"
			ENDCG
	}
	
	GrabPass{}
		pass
	{
		CGPROGRAM
#pragma multi_compile _ _REPEAT_R5 _REPEAT_R6 _REPEAT_R7 _REPEAT_R8
#pragma vertex vertAve
#pragma fragment fragAve
#if _REPEAT_R5 | _REPEAT_R6 | _REPEAT_R7 | _REPEAT_R8
#define _AVE
#endif
#include "RepeatAve.cginc"
			ENDCG
	}

	GrabPass{}
		pass
	{
		CGPROGRAM
#pragma multi_compile _ _REPEAT_R6 _REPEAT_R7 _REPEAT_R8
#pragma vertex vertAve
#pragma fragment fragAve
#if _REPEAT_R6 | _REPEAT_R7 | _REPEAT_R8
#define _AVE
#endif
#include "RepeatAve.cginc"
			ENDCG
	}

	GrabPass{}
		pass
	{
		CGPROGRAM
#pragma multi_compile _ _REPEAT_R7 _REPEAT_R8
#pragma vertex vertAve
#pragma fragment fragAve
#if _REPEAT_R7 | _REPEAT_R8
#define _AVE
#endif
#include "RepeatAve.cginc"
			ENDCG
	}

	GrabPass{}
		pass
	{
		CGPROGRAM
#pragma multi_compile _ _REPEAT_R8
#pragma vertex vertAve
#pragma fragment fragAve
#if _REPEAT_R8
#define _AVE
#endif
#include "RepeatAve.cginc"
			ENDCG
	}
	*/
	GrabPass{}
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		half4 _Color;
	sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _Noise;
	float4 _Noise_ST;
	sampler2D _GrabTexture;
	half _Range;
	struct v2f
	{
		float4 pos: SV_POSITION;
		float3 uv: TEXCOORD;
		float4 screenPos: TEXCOORD1;
		float3 ray: TEXCOORD2;
	};
	v2f vert(appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		o.screenPos = ComputeScreenPos(o.pos);
		o.ray = UnityObjectToViewPos(v.vertex).xyz * float3(-1, -1, 1);
		return o;
	}
	half4 frag(v2f i) : SV_Target
	{
		i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
	float2 uv = i.screenPos.xy / i.screenPos.w;
	float2 frostUV = tex2D(_Noise, i.uv * _Noise_ST.xy + _Noise_ST.zw).xy;
	frostUV -= 0.5;
	frostUV *= _Range;
	frostUV += uv;
	half4 frost = tex2D(_GrabTexture, frostUV);
	half4 diffuse = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);
	half alpha = _Color.a * diffuse.a;
	return half4(frost.xyz + (diffuse.rgb * _Color.rgb * alpha), 1);
	}
		ENDCG
	}
	}
		Fallback Off
}