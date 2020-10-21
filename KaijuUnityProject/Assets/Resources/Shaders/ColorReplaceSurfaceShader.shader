Shader "Custom/ColorReplaceSurfaceShader"
{
	Properties
	{
		_Color1("Color1", Color) = (1,0.0,1,1)
		_Color1Replace("Color1Replace", Color) = (1,0.0,1,1)
		_Color2("Color2", Color) = (1,0.0,1,1)
		_Color2Replace("Color2Replace", Color) = (1,0.0,1,1)
		_Color3("Color3", Color) = (1,0.0,1,1)
		_Color3Replace("Color3Replace", Color) = (1,0.0,1,1)
		_Color4("Color4", Color) = (1,0.0,1,1)
		_Color4Replace("Color4Replace", Color) = (1,0.0,1,1)
		_Color5("Color5", Color) = (1,0.0,1,1)
		_Color5Replace("Color5Replace", Color) = (1,0.0,1,1)
		_DetectionThreshold("DetectionThreshold", float) = 0.01
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
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

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color1;
		fixed4 _Color1Replace;
		fixed4 _Color2;
		fixed4 _Color2Replace;
		fixed4 _Color3;
		fixed4 _Color3Replace;
		fixed4 _Color4;
		fixed4 _Color4Replace;
		fixed4 _Color5;
		fixed4 _Color5Replace;
		float _DetectionThreshold;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

			half3 delta1 = abs(c.rgb - _Color1);
			half3 delta2 = abs(c.rgb - _Color2);
			half3 delta3 = abs(c.rgb - _Color3);
			half3 delta4 = abs(c.rgb - _Color4);
			half3 delta5 = abs(c.rgb - _Color5);
			if ((delta1.r + delta1.g + delta1.b) < _DetectionThreshold)
			{
				c = _Color1Replace;
			}
			else if ((delta2.r + delta2.g + delta2.b) < _DetectionThreshold)
			{
				c = _Color2Replace;
			}
			else if ((delta3.r + delta3.g + delta3.b) < _DetectionThreshold)
			{
				c = _Color3Replace;
			}
			else if ((delta4.r + delta4.g + delta4.b) < _DetectionThreshold)
			{
				c = _Color4Replace;
			}
			else if ((delta5.r + delta5.g + delta5.b) < _DetectionThreshold)
			{
				c = _Color5Replace;
			}
			
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
