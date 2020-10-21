//Fast graph rendering for UI (used in the debug menu)
Shader "BeyondReach/UI/FPSGraph"
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
			fixed4 _Below16Color;
			fixed4 _Below33Color;
			fixed4 _DefaultColor;

			//Thresholds used for selecting the right colour during rendering. [0;1], uv.v coordinate
			fixed _16MS;
			fixed _33MS;
			fixed _Average;
			
			//Array containing the samples for the graph. Maximum 512 values. Might not work on some graphics cards and has to be adjusted then
			fixed GraphValues[512];
			fixed GraphValues_Length;

			//Calculate the graph colour, threshold colour or default value
			fixed4 frag(v2f i) : SV_Target
			{
				//Final colour
				fixed4 color = fixed4(1,1,1,1);

				//Get the sample value for the x coordinate of the current fragment
				float graphValue = GraphValues[floor(i.uv.x * GraphValues_Length)];
					
				//Assign the base colour for the current graph value
				if (graphValue < _16MS)
					color *= _Below16Color;
				else if (graphValue < _33MS)
					color *= _Below33Color;
				else
					color *= _DefaultColor;
				//Set the alpha for everything underneath the tip of the bar to a low value...
				if (graphValue - i.uv.y > 0.02)
					color.a = 0.1;
				//...and the everything above the value to 0
				if (i.uv.y > graphValue)
					color.a = 0;

				//Set the colour to the one used by the average bar if the pixel is part of it
				if (i.uv.y < _Average && i.uv.y > _Average - 0.02)
					color = fixed4(1, 1, 1, 1);
				//Set the colour to the one used by the below33 bar if the pixel is part of it
				else if (i.uv.y < _33MS && i.uv.y > _33MS - 0.02)
					color = _Below33Color;
				//Set the colour to the one used by the below66 bar if the pixel is part of it
				else if (i.uv.y < _16MS && i.uv.y > _16MS - 0.02)
					color = _Below16Color;

				//Multiply the rgb value with the alpha value for the final result since we blend additively 
				color.rgb *= color.a;
				return color;
			}

			ENDHLSL
		}
	}
}