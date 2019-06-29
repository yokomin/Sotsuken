Shader "Custom/FrostComposition"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)

		_MainTex("Diffuse", 2D) = "white" {}
	_Noise("Noise", 2D) = "black" {}

	_Range("Range", Float) = 0.025
		_Blur("Blur", Range(0.0, 1.0)) = 0.5
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Cull Off

		CGINCLUDE
#include "UnityCG.cginc"
		half4 _Color;
	sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _GrabTex;
	sampler2D _FrostTex0;
	sampler2D _FrostTex1;
	sampler2D _FrostTex2;
	sampler2D _FrostTex3;
	sampler2D _Noise;
	float4 _Noise_ST;
	half _Range;
	float _Blur;
	ENDCG

		Pass
	{
		CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag

		struct v2f
	{
		float4 pos : SV_POSITION;
		float3 uv : TEXCOORD;
		float4 screenPos : TEXCOORD1;
	};

	v2f vert(appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		o.screenPos = ComputeGrabScreenPos(o.pos);
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		float2 uv = i.screenPos.xy / i.screenPos.w;
		float2 frostUV = tex2D(_Noise, i.uv * _Noise_ST.xy + _Noise_ST.zw).xy;

		frostUV -= 0.5;
		frostUV *= _Range;
		frostUV += uv;
		// frostUV = uv;

		float t = pow(_Blur, 0.5) * 4;
		float4 frost = smoothstep(1, 0, t) * tex2D(_GrabTex, frostUV);
		frost += smoothstep(1, 0, abs(t - 1)) * tex2D(_FrostTex0, frostUV);
		frost += smoothstep(1, 0, abs(t - 2)) * tex2D(_FrostTex1, frostUV);
		frost += smoothstep(1, 0, abs(t - 3)) * tex2D(_FrostTex2, frostUV);
		frost += smoothstep(1, 0, 4 - t) * tex2D(_FrostTex3, frostUV);

		half4 diffuse = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);

		half alpha = _Color.a * diffuse.a;

		return half4(frost.xyz + (diffuse.rgb * _Color.rgb * alpha), 1);
	}
		ENDCG
	}
	}
		Fallback Off
}