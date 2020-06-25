Shader "Dana/Blending8tex"
{

	/*       y
	 *       ^
	 *       |010   110
	 *       |
	 *       |000   100
	 *   z=0 |--------> x
      y
	 *       ^
	 *       |011   111
	 *       |
	 *       |001   101
	 *   z=1 |--------> x
	 * 
	 * */
	
    Properties
    {
        _Tex000 ("Texture 000", 2D) = "white" {}
        _Tex001 ("Texture 001", 2D) = "white" {}
        _Tex010 ("Texture 010", 2D) = "white" {}
        _Tex011 ("Texture 011", 2D) = "white" {}
        _Tex100 ("Texture 100", 2D) = "white" {}
        _Tex101 ("Texture 101", 2D) = "white" {}
        _Tex110 ("Texture 110", 2D) = "white" {}
        _Tex111 ("Texture 111", 2D) = "white" {}
        _x ("pos x", Range(0,1)) = 0.5
		_y ("pos y", Range(0,1)) = 0.5
		_z ("pos z", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _Tex000;
			float4 _Tex000_ST;
			sampler2D _Tex001;
			float4 _Tex001_ST;
			sampler2D _Tex010;
			float4 _Tex010_ST;
			sampler2D _Tex011;
			float4 _Tex011_ST;
			sampler2D _Tex100;
			float4 _Tex100_ST;
			sampler2D _Tex101;
			float4 _Tex101_ST;
			sampler2D _Tex110;
			float4 _Tex110_ST;
			sampler2D _Tex111;
			float4 _Tex111_ST;
			float _x;
			float _y;
			float _z;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Tex000);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			float expoInOut(float t) {
				float salida = 0;
				if (t < 0.5) {
					salida = 0.5 * pow(2.0, 20.0 * t - 10.0);
				}
				else {
					salida = -0.5 * pow(2.0, 10.0 - t * 20.0) + 1.0;
				}
				//return t;
				return  salida;
			}

			float cubicInOut(float t) {
				// cubicInOut
				float o = +(t > 0.5);
				t = 2.0 * (t - o);
				return 0.5 * t * t * t + o;
			}

			float backInOut(float t) {
				float s = 1.70158 * 1.525;
				float sign = (t > 0.5) - (t < 0.5);
				t = (2 * t - 1)*sign - 1;
				return 0.5 * (1 + sign + sign * t * t * (t + s + s * t));
			}

            fixed4 frag (v2f i) : SV_Target
            {
            // sample the texture
			

			//circInOut 
			/*
			float t = _x;
			float sign = (t > 0.5) - (t < 0.5);
			t = 2 * t - 1 - sign;
			salida = 0.5 * (1 + sign * sqrt(1 - t * t));
			*/

			//quadInOut
			/*
			float t = _x;
			float sign = (t > 0.5) - (t < 0.5);
			t = 2 * t - 1 - sign;
			salida= 0.5 * (1 + sign * (1 - t * t));
			*/

			// quintInOut
			/*
			float t = _x;
			float o = +(t > 0.5);
			t = 2.0 * (t - o);
			float s = t * t;
			salida= 0.5 * s * s * t + o;
			*/
						
			float x = _x;
			float y = _y;
			float z = _z;
			//float x = expoInOut(_x);
			//float y = expoInOut(_y);
			//float z = expoInOut(_z);
			//float x = cubicInOut(_x);
			//float y = cubicInOut(_y);
			//float z = cubicInOut(_z);
			//float x = backInOut(_x);
			//float y = backInOut(_y);
			//float z = backInOut(_z);

            fixed4 col_d00 = lerp(tex2D(_Tex000, i.uv), tex2D(_Tex100, i.uv), x);   
			fixed4 col_d10 = lerp(tex2D(_Tex010, i.uv), tex2D(_Tex110, i.uv), x);  
			fixed4 col_dd0 = lerp(col_d00, col_d10, y);

			fixed4 col_d01 = lerp(tex2D(_Tex001, i.uv), tex2D(_Tex101, i.uv), x);
            fixed4 col_d11 = lerp(tex2D(_Tex011, i.uv), tex2D(_Tex111, i.uv), x);
            fixed4 col_dd1 = lerp(col_d01, col_d11, y);

			fixed4 col = lerp(col_dd0, col_dd1, z);
			//fixed4 col = col_d00;
            return col;
            }
            ENDCG
        }
    }
			
}
