//Fast UI blur shader
Shader "BeyondReach/UI/FastBlur"
{
	Properties
	{
		[PerRendererData] _MainTex("_MainTex", 2D) = "white" {}  

		_LODLevel ("Opaque LOD Level", Range(0,7)) = 1
	}

    Category
    {
        //Queue after opaque objects, however render opaque since we grab-pass and render to texture
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True"}
		Blend SrcAlpha OneMinusSrcAlpha
		
        Cull Off
        Lighting Off
        ZWrite Off
     
        SubShader
        {
            //Read blur from pyramid texture
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

					if(mask.a > 0.5f)
						return tex2Dlod(_ColorPyramidTexture, float4(i.uvgrab.xy / i.uvgrab.w, 0, _LODLevel)) * i.color;
					else
						return half4(0,0,0,0);
                }

                ENDHLSL
            }
        }
    }
}