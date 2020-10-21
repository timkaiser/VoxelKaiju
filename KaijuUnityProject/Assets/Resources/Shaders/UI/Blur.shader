//Fancy UI blur shader
Shader "BeyondReach/UI/Blur"
{
	Properties
	{
		[PerRendererData] _MainTex("_MainTex", 2D) = "white" {}  

		//Opaque texture LOD level
		_LODLevel ("Opaque LOD Level", Range(0,7)) = 1
		//Kernel size
        _Radius ("Blur radius", Range(0, 20)) = 1

		//Stencil support
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		
	}

    Category
    {
        //Queue after opaque objects, however render opaque since we grab-pass and render to texture
        Tags{ "Queue" = "Transparent+1" "RenderType" = "Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True"}
		Blend SrcAlpha OneMinusSrcAlpha
		
        Cull Off
        Lighting Off
        ZWrite Off
     
        SubShader
        {
			//Stencil Operation
			Stencil
			{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp] 
				ReadMask [_StencilReadMask]
				WriteMask [_StencilWriteMask]
			}

            //Gaussian blur
            Pass
            {
                HLSLPROGRAM
				
				//Define functions
                #pragma vertex vert
                #pragma fragment frag
				
				//Tell the graphics API to sacrifice precision for performance
                #pragma fragmentoption ARB_precision_hint_fastest
				
				//Includes
                #include "UnityCG.cginc"
 
				//Structs and vars
                struct appdata
                {
                    float4 vertex : POSITION;
					float4 color : COLOR;
                    float2 uv : TEXCOORD0;
                };
 
                struct v2f
                {
                    float4 pos : POSITION;
					float4 color : COLOR;
                    float2 uv : TEXCOORD0;
                    float4 uvgrab : TEXCOORD1;
                };
 
                sampler2D _ColorPyramidTexture;
				sampler2D _MainTex;

				int _LODLevel;

				float _Radius;
				
				//Vertex function
                v2f vert(appdata v)
                {
                    v2f o;
					
					//Convert vertex pos to clip space
                    o.pos = UnityObjectToClipPos(v.vertex);
					
					//Pass the uv coords
					o.uv = v.uv;

					//Calculate uv coords for the screen grab
                    o.uvgrab.xy = (float2(o.pos.x, o.pos.y) + o.pos.w) * 0.5;
                    o.uvgrab.zw = o.pos.zw;
					
					//Fake adjust the colour since we aren't rendering transparent
					o.color = v.color;

                    return o;
                }
 
				//Fragment function
                half4 frag(v2f i) : COLOR
                { 
					//Get the sprite for masking
					float4 mask = tex2D(_MainTex, i.uv);

					if(mask.r > 0.05f)
					{
						half4 sum = half4(0,0,0,0);
					 
						//Grabpixel function
						#define GRABXYPIXEL(kernelx, kernely) tex2Dlod(_ColorPyramidTexture, float4(float2(max(min(i.uvgrab.x + kernelx, 0.98f), 0.02f), max(min(i.uvgrab.y + kernely, 0.98f), 0.02f)), 0, _LODLevel))
 
						//Add the centre to the samples and start counting the measurements
						sum += GRABXYPIXEL(0.0f, 0.0f);
						int measurments = 1;
 
						//Take additional samples
						for (float range = 0.005f; range <= (0.005f * _Radius); range += 0.005f)
						{
							sum += GRABXYPIXEL(range, range);
							sum += GRABXYPIXEL(range, -range);
							sum += GRABXYPIXEL(-range, range);
							sum += GRABXYPIXEL(-range, -range);
							sum += GRABXYPIXEL(0, range);
							sum += GRABXYPIXEL(0, -range);
							sum += GRABXYPIXEL(range, 0);
							sum += GRABXYPIXEL(-range, 0);
							measurments += 8;
						}
						return float4((((sum / measurments) * 1.0f + (i.color * i.color.a)) / 2.0f).rgb * i.color.rgb, 1.0f);
					}
					else
						return mask;
                }

                ENDHLSL
            }
        }
    }
}