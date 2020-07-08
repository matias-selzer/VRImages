Shader "Dana/Warp1Habitacion"
{
    Properties
    {
		_Tex ("Texture 00", 2D) = "white" {}

        _radio ("Radio habitacion", Range(1,500)) = 5
        _centro("centro Habitacion", Vector) =  (0,0,0)
        
        _posImg("Posicion Foto", Vector) = (0,0,0)
        _pos("Posicion actual", Vector) = (0,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
		Cull off

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

			sampler2D _Tex;
			float4 _Tex_ST;
           
			float3 _posImg;
			float3 _pos;
			float _radio;
			float3 _centro;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Tex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
			static const float PI = 3.14159265359f;

			float2 xyz2uv(float3 pos) {
				float R = length(pos);
				float tita = atan2(pos.y,pos.x);
				float phi = atan2(sqrt(pos.x * pos.x + pos.y * pos.y), pos.z);
				
				float v = phi/PI;
				float u = tita / (2*PI);
				
				float2 uv = {u,v};
				
				return uv;
			}

			float4 uv2xyz(float2 uv) {
				float R = 1;
				float tita = (uv.x * 2 * PI); // - PI);///R;
				float phi = (uv.y * PI); ///R;
				
				float x = R * sin(phi) * cos(tita);
				float y = R * sin(tita) * sin(phi);
				float z = R * cos(phi);
				
				float4 xyz = {x,y,z,1};
				return xyz;
			}

			//proyección SOBRE la habitación (esfera de radio R) centrada en q DEL punto xyz pertenecediente a la esfera centrada en p.
			// Resolví la ecuación |t s| = R
			// luego, r = t * s + p
			float3 habitacion(float3 xyz, float3 p, float3 q, float R) {
				// punto 3D que le corresponde a uv (centrado en post)
				float3 s = xyz;
				float A = dot(p - q, p - q);
				float L = dot(s,s);
				float cos = dot(p - q, s);
				float t = -cos + sqrt(cos * cos - L*A+ L *R*R);
				float3 r = t * s / L + p;
				return r;
			}
			
			//proyección DE h en la habitación (esfera de ragio R) SOBRE la esfera de radio 1 centrada en p
			float3 proyeccion(float3 h, float3 p) {
				return h - p;
			}

			float4 version6(float2 uv, float3 p_t, float3 p_0, sampler2D Texture) {
				float k = ((length(p_t) - 0.0) / _radio)*(1.0 - 0.5)+0.5;
				
				//float RadioHabitacion = _radio;
				float R = _radio;

				float3 q = {0,0,0}; //origen de todo

				//Punto en la habitación
				float3 s = uv2xyz(uv); //Inversa Proyeccion de la proy equirrectangular
				float3 h = habitacion(s, p_t, q, R); //Inversa vista desde P_t
				float3 x = proyeccion(h, p_0); // Vista desde Pt
				float2 uv0 = xyz2uv(x); // Proyección equirrectangular en la textura
				float4 col0 = tex2D(Texture, uv0);

				return col0; //la imagen fuente que de deforma hasta llegar a la target

			}

            fixed4 frag (v2f i) : SV_Target
            {
				float4 col = {0.5,0.5,0.0,1};
				//Tengo que rotar los ejes y cambiarle el signo a las coordenadas ¿No se exactamente porqué?
				float3 p_i = {-_posImg.z, -_posImg.x, -_posImg.y};
				float3 p_t = { -_pos.z,   -_pos.x,    -_pos.y};
				float3 centro = {- _centro.z, -_centro.x, -_centro.y};

				col = version6(i.uv, p_t - centro, p_i - centro, _Tex);

                return col;
            }
            ENDCG
        }
    }
}
