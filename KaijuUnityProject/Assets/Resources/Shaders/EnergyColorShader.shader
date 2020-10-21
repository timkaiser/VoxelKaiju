﻿Shader "Custom/EnergyColorShader"
{
	Properties
	{
		_EnergyPalette("Energy Palette (RGB)", 2D) = "white" {}
		_Energy("Energy", Range(0,100)) = 50
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _EnergyPalette;

			struct Input
			{
				float2 uv_MainTex;
			};

			float _Energy;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 OutputColor = tex2D(_EnergyPalette, float2(_Energy / 100.0, _Energy / 100.0));
				OutputColor.a = 1.0f;

				o.Albedo = OutputColor.rgb;
				// Metallic and smoothness come from slider variables
				o.Alpha = 1.0f;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
