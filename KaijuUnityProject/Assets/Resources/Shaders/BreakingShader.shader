/* HEADER:
 * This is the main shader for paintable objects. It renders the object with normal map and projects the outlines of the components onto is.
 */

Shader "Custom/BreakingShader" { //source https://docs.unity3d.com/Manual/SL-SurfaceShaderExamples.html

	Properties{
		_MainTex("Texture", 2D) = "white" {}								// Main Texture. 
		_Breaks("Breaks", 2D) = "clear" {}									// Outlines of the components
		_BreakThreshold("Crack Alpha Threshold", Range(0.001, 1.0)) = 1		// Value that determines how much of the original texture can be seen

	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			CGPROGRAM
			#pragma surface surf Lambert

		// Standart Surface shader with normal map:
		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalMap;
		};

		sampler2D _MainTex;
		sampler2D _Breaks;

		float _BreakThreshold;

		void surf(Input IN, inout SurfaceOutput o) {
			float4 breaks = tex2D(_Breaks, IN.uv_MainTex);
			o.Albedo = breaks.a > _BreakThreshold-0.0 ? float4(tex2D(_MainTex, IN.uv_MainTex).rgb * 0.2 ,1) : tex2D(_MainTex, IN.uv_MainTex);
		}
		ENDCG
	}
		Fallback "Diffuse"
}