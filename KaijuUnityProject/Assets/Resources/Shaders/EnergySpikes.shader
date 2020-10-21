Shader "Custom/EnergySpikes"
{
    Properties
    {
        _SpikeColorFinder ("SpikeColorFinder", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_EnergyPalette("Energy Palette (RGB)", 2D) = "white" {}
		_Energy("Energy", Range(0,100)) = 50
		_DamagedColor("DamagedColor", Color) = (1,0,0,1)
		_Damaged("Damaged", float) = 1.0
		_DamagedTime("DamagedTime", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _EnergyPalette;

        struct Input
        {
            float2 uv_MainTex;
        };

        float _Energy;
        fixed4 _SpikeColorFinder;
		fixed4 _DamagedColor;
		float _Damaged;
		float _DamagedTime;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 OutputColor = tex2D(_EnergyPalette, float2(_Energy / 100.0, _Energy / 100.0));
			OutputColor.a = 1.0f;

			half3 delta1 = abs(c.rgb - _SpikeColorFinder);
			if ((delta1.r + delta1.g + delta1.b) < 0.5f)
			{
				c = OutputColor;
			}

			float cycle = sin(_Time.y * 5.0);
			if (_Damaged < _DamagedTime && cycle > 0.5f)
			{
				o.Albedo = c.rgb * 0.5f * _DamagedColor * sin(_Time.y * 5.0);
			}
			else
			{
				o.Albedo = c.rgb;
			}
            // Metallic and smoothness come from slider variables
            o.Alpha = 1.0f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
