//Fast graph rendering for UI (used in the debug menu)
Shader "BeyondReach/UI/SimpleGraph"
{
	Properties
	{
		//Pixel snap can be used to fix vlurry UI
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{			
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		//Disable unnecessary functionality
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Off
		//Additive blending, able to layer graphs
		Blend One OneMinusSrcAlpha

		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
				
			#include "UnityCG.cginc"

			//Basic appdata struct, uv used for graph position calculations
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			//Basic v2f struct, uv used for graph position calculations
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//Simple vert function
			v2f vert(appdata v)
			{
				v2f o;

				//Transform vertices and pass uv
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				//Handle pixel snap
				#ifdef PIXELSNAP_ON
					o.vertex = UnityPixelSnap(v.vertex);
				#endif

				return o;
			}

			//Colour used for rendering
			fixed4 _GraphColor;
			
			//Array containing the samples for the graph. Maximum 512 values. Might not work on some graphics cards and has to be adjusted then
			fixed GraphValues[512];
			fixed GraphValues_Length;

			//Calculate the graph colour, threshold colour or default value
			fixed4 frag(v2f i) : SV_Target
			{
				//Final colour
				fixed4 color = _GraphColor;

				//Get the sample value for the x coordinate of the current fragment
				float graphValue = GraphValues[floor(i.uv.x * GraphValues_Length)];
					
				//Set the alpha for everything underneath the tip of the bar to a low value...
				if (graphValue - i.uv.y > 0.02)
					color.a = 0.1;
				//...and the everything above the value to 0
				if (i.uv.y > graphValue)
					color.a = 0;

				//Multiply the rgb value with the alpha value for the final result since we blend additively 
				color.rgb *= color.a;
				return color;
			}

			ENDHLSL
		}
	}
}