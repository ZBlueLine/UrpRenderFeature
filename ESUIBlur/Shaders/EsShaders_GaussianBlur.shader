Shader "EsShader/GaussianBlur"
{
		Properties
		{
			[HidenInInspector]_MainTex("Base (RGB)", 2D) = "white" {}
		}

		SubShader
		{
			ZWrite Off
			Blend Off

			Pass
			{
				ZTest Off
				Cull Off
				CGPROGRAM
				#pragma vertex vert_Blur
				#pragma fragment frag_Blur
				ENDCG
			}

			Pass
			{
				ZTest Always
				Cull Off

				CGPROGRAM
				#pragma vertex vert_BlurVertical
				#pragma fragment frag_GaussianBlur

				ENDCG
			}

			Pass
			{
				ZTest Always
				Cull Off

				CGPROGRAM

				#pragma vertex vert_BlurHorizontal
				#pragma fragment frag_GaussianBlur

				ENDCG
			}
		}

		CGINCLUDE

		sampler2D _MainTex;
		
		uniform half4 _MainTex_TexelSize;
		uniform half _BlurSize;

		struct VertexInput
		{
			float4 vertex : POSITION;
			half2 texcoord : TEXCOORD0;
		};

		struct VertexOutput_Blur
		{
			float4 pos : SV_POSITION;
			half2 uv20 : TEXCOORD0;
			half2 uv21 : TEXCOORD1;
			half2 uv22 : TEXCOORD2;
			half2 uv23 : TEXCOORD3;
			half2 uv24 : TEXCOORD4;
			half2 uv25 : TEXCOORD5;
			half2 uv26 : TEXCOORD6;
			half2 uv27 : TEXCOORD7;
		};


		static const half3 GaussWeight = half3(0.38774, 0.24477, 0.06136);


		VertexOutput_Blur vert_Blur(VertexInput v)
		{
			VertexOutput_Blur o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv20 = v.texcoord + _MainTex_TexelSize.xy* half2(0.5h, 0.5h) * _BlurSize;
			o.uv21 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, -0.5h) * _BlurSize;
			o.uv22 = v.texcoord + _MainTex_TexelSize.xy * half2(0.5h, -0.5h) * _BlurSize;
			o.uv23 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, 0.5h) * _BlurSize;

			o.uv24 = v.texcoord + _MainTex_TexelSize.xy * half2(0.0h, 0.5h) * _BlurSize;
			o.uv25 = v.texcoord + _MainTex_TexelSize.xy * half2(0.0h, -0.5h) * _BlurSize;
			o.uv26 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, 0.0h) * _BlurSize;
			o.uv27 = v.texcoord + _MainTex_TexelSize.xy * half2(0.5h, 0.0h) * _BlurSize;
			return o;
		}

		fixed4 frag_Blur(VertexOutput_Blur i) : SV_Target
		{
			fixed4 color = (0,0,0,0);

			color += tex2D(_MainTex, i.uv20);
			color += tex2D(_MainTex, i.uv21);
			color += tex2D(_MainTex, i.uv22);
			color += tex2D(_MainTex, i.uv23);
			//color += tex2D(_MainTex, i.uv24);
			//color += tex2D(_MainTex, i.uv25);
			//color += tex2D(_MainTex, i.uv26);
			//color += tex2D(_MainTex, i.uv27);

			return color / 4;
		}

		struct VertexOutput_GaussianBlur
		{
			float4 pos : SV_POSITION;
			half2 uv[5] : TEXCOORD0;
		};

		VertexOutput_GaussianBlur vert_BlurHorizontal(VertexInput v)
		{
			VertexOutput_GaussianBlur o;
			o.pos = UnityObjectToClipPos(v.vertex);
			half2 uv = v.texcoord;
			
			o.uv[0] = uv ;
			o.uv[1] = uv - _MainTex_TexelSize.xy * half2(1.0, 0.0) * _BlurSize;
			o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1.0, 0.0) * _BlurSize;
			o.uv[3] = uv - _MainTex_TexelSize.xy * half2(2.0, 0.0) * _BlurSize;
			o.uv[4] = uv + _MainTex_TexelSize.xy * half2(2.0, 0.0) * _BlurSize;
			return o;
		}

		VertexOutput_GaussianBlur vert_BlurVertical(VertexInput v)
		{
			VertexOutput_GaussianBlur o;
			o.pos = UnityObjectToClipPos(v.vertex);
			half2 uv = v.texcoord;
			
			o.uv[0] = uv ;
			o.uv[1] = uv - _MainTex_TexelSize.xy * half2(0.0, 1.0) * _BlurSize;
			o.uv[2] = uv + _MainTex_TexelSize.xy * half2(0.0, 1.0) * _BlurSize;
			o.uv[3] = uv - _MainTex_TexelSize.xy * half2(0.0, 2.0) * _BlurSize;
			o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0.0, 2.0) * _BlurSize;
			return o;
		}

		half4 frag_GaussianBlur(VertexOutput_GaussianBlur i) : SV_Target
		{
			//return half4(i.uv, 0, 1);
			half4 texCol = tex2D(_MainTex, i.uv[0]) * GaussWeight[0];
			for(int t = 1; t < 3; ++t)
			{
				texCol += tex2D(_MainTex, i.uv[t*2 -1]) * GaussWeight[t];
				texCol += tex2D(_MainTex, i.uv[t*2]) * GaussWeight[t];
			}
			return texCol;
		}

		ENDCG

		FallBack Off
}