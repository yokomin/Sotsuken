Shader "Custom/Gaussian"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)

		_MainTex("Diffuse", 2D) = "white" {}
		_Noise("Noise", 2D) = "black" {}

		_Range("Range", Float) = 0.025
		_Sigma("Sigma", Range(0.01, 8.0)) = 1.0
	}

	SubShader
	{
		//Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Cull Off

		GrabPass{ "_Frost" }

		CGINCLUDE
		#include "UnityCG.cginc"

		half4 _Color;

		sampler2D _MainTex;
		float4 _MainTex_ST;

		sampler2D _Frost;
		float4 _Frost_TexelSize;

		sampler2D _Noise;
		float4 _Noise_ST;

		half _Range;
		float _Sigma;

		// 重み計算用関数
		inline float getWeight(float2 xy)
		{
			return exp(-dot(xy, xy) / (2.0 * _Sigma * _Sigma));
		}

		// カーネルサイズ計算用関数
		inline int getKernelN()
		{
			return (int)ceil(_Sigma * sqrt(-log(0.0001)));
		}

		// 最大kernelN...(int)ceil(8 * sqrt(-ln(0.0001)))
		#define KERNEL_N_MAX 25

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
				float3 ray : TEXCOORD2;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.screenPos = ComputeGrabScreenPos(o.pos);
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
				// frostUV = uv;

				int kernelN = getKernelN();

				float2 texelOffset = float2(0, 0);
				float weight = getWeight(texelOffset);
				float weightSum = weight;
				float4 frost = weight * tex2D(_Frost, frostUV);

				[unroll(KERNEL_N_MAX)]
				for (int n = 0; n < kernelN; n++)
				{
					int x = n + 1;

					texelOffset = float2(x, 0);
					weight = getWeight(texelOffset);
					weightSum += 2 * weight;
					frost += weight * tex2D(_Frost, frostUV + texelOffset * _Frost_TexelSize.xy);
					texelOffset = float2(-x, 0);
					frost += weight * tex2D(_Frost, frostUV + texelOffset * _Frost_TexelSize.xy);
				}

				[unroll(KERNEL_N_MAX)]
				for (int m = 0; m < kernelN; m++)
				{
					int y = m + 1;

					texelOffset = float2(0, y);
					weight = getWeight(texelOffset);
					weightSum += 2 * weight;
					frost += weight * tex2D(_Frost, frostUV + texelOffset * _Frost_TexelSize.xy);
					texelOffset = float2(0, -y);
					frost += weight * tex2D(_Frost, frostUV + texelOffset * _Frost_TexelSize.xy);

					[unroll(KERNEL_N_MAX)]
					for (int n = 0; n < kernelN; n++)
					{
						int x = n + 1;

						texelOffset = float2(x, y);
						weight = getWeight(texelOffset);
						weightSum += 4 * weight;
						frost += weight * tex2D(_Frost, frostUV + texelOffset * _Frost_TexelSize.xy);
						texelOffset = float2(-x, y);
						frost += weight * tex2D(_Frost, frostUV + texelOffset * _Frost_TexelSize.xy);
						texelOffset = float2(x, -y);
						frost += weight * tex2D(_Frost, frostUV + texelOffset * _Frost_TexelSize.xy);
						texelOffset = float2(-x, -y);
						frost += weight * tex2D(_Frost, frostUV + texelOffset * _Frost_TexelSize.xy);
					}
				}

				frost /= weightSum;

				half4 diffuse = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);

				half alpha = _Color.a * diffuse.a;

				return half4(frost.xyz + (diffuse.rgb * _Color.rgb * alpha), 1);
			}

		ENDCG
		}
	}

		Fallback Off
}