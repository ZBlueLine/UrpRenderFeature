Shader "EsShaders/LinearToGamma"
{
    Properties
    {
		_MainTex("Main Texture", 2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
		ZTest Always
		ZWrite Off Cull Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

            inline half3 LinearToGammaSpace (half3 linRGB)
            {
                linRGB = max(linRGB, half3(0.h, 0.h, 0.h));
                // An almost-perfect approximation from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
                return max(1.055h * pow(linRGB, 0.416666667h) - 0.055h, 0.h);

                // Exact version, useful for debugging.
                //return half3(LinearToGammaSpaceExact(linRGB.r), LinearToGammaSpaceExact(linRGB.g), LinearToGammaSpaceExact(linRGB.b));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
				half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                col = half4(LinearToGammaSpace(col.xyz), 1);
                return col;
            }
            ENDHLSL
        }
    }
}
