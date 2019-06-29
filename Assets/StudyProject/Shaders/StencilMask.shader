Shader "Custom/StencilMask"
{
	Properties
	{
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		Cull Off
		ZWrite Off
		Stencil
	{
		Pass IncrSat
	}
		ColorMask 0

		Pass
	{
		CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
		struct v2f
	{
		float4 pos: SV_POSITION;
	};
	v2f vert(appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	half4 frag(v2f i) : SV_Target
	{
		return 0.0;
	}
		ENDCG
	}
	}
		Fallback Off
}