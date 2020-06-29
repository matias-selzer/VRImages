Shader "Dana/Warp2Esferico"
{
    Properties
    {
		_SourceTex ("Source Texture", 2D) = "white" {}
        _TargetTex ("Target Texture", 2D) = "white" {}
        _t ("t", Range(0,1)) = 0
        
        _radio ("Radio habitacion", Range(1,50)) = 5
        _pos0("Posicion 0", Vector) = (0,0,0)
        _pos1("Posicion 1", Vector) = (0,0,0)
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

            sampler2D _SourceTex;
            float4 _SourceTex_ST;
            sampler2D _TargetTex;
            float4 _TargetTex_ST;
           
            float _t;
            float3 _pos0;
            float3 _pos1;
            float3 pos;
            float _radio;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _SourceTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
			static const float PI = 3.14159265f;

			float2 xyz2uv(float3 pos) {
				float R = length(pos);//sqrt(pos.x * pos.x + pos.y * pos.y + pos.z * pos.z);
				float tita = atan2(pos.y,pos.x);
				float phi = atan2(sqrt(pos.x * pos.x + pos.y * pos.y), pos.z);
				
				float v = (phi /** R*/)/PI;
				float u = (tita /** R + PI*/) / (2*PI);
				
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
			float3 habitacion(float3 xyz, float3 p, float3 q, float R) {
				float A = length(p - q);
				//encontrar el valor correspondiente en la esfera de Radio R centrada en pos0
				
				// punto 3D que le corresponde a uv (centrado en post)
				float3 s = xyz + p;

				//coseno del angulo de separación entre p0 y r centrados en post
				float cos = (p != q) ? dot(normalize(q - p), normalize(s-p)) : 0;
				
				//magnitud de la distancia de post a la esfera centrada en pos0 de radio R
				float B = - A * cos + sqrt(A*A * (cos*cos-1) + R * R);
				
				//proyeccion de s sobre la esfera de radio R centrada en pos0
				float3 r = B * normalize(s-p) + p;
				return r;
			}
			
			//proyección DE h en la habitación (esfera de ragio R) SOBRE la esfera de radio 1 centrada en p
			float3 proyeccion(float3 h, float3 p) {
				float R = 1;
				return normalize(h - p) * R;
			}

			float4 version6(float2 uv, float3 p_t, float3 p_0, float3 p_1, float t) {
				//float RadioHabitacion = _radio;
				float R = _radio;
				float3 q = {0,0,0}; //origen de todo

			//Punto en la habitación
				float3 s = uv2xyz(uv); //Inversa Proyeccion 
				float3 h = habitacion(s, p_t, q, R); //Inversa vista desde P_t
			//Proyección desde pos0
				float3 x = proyeccion(h, p_0); // Vista desde Pt
				float2 uv0 = xyz2uv(x); // Proyección
				float4 col0 = tex2D(_SourceTex, uv0);

			//Proyección desde pos1
				//h = habitacion(s, p_t, q, R); // Inversa vista desde P1
				x = proyeccion(h, p_1); // Vista desde Pt
				float2 uv1 = xyz2uv(x); // Proyeccion
				float4 col1 = tex2D(_TargetTex, uv1);
				
//				//merge de los dos colores
//				float4 col = ((1-_t)*col0 + _t*col1);
//				return col;
				
				float4 col;
				if (t < 0.5) {
					col = col0;
				} else col = col1;
				
				return col; //la imagen fuente que de deforma hasta llegar a la target

			}

            fixed4 frag (v2f i) : SV_Target
            {

				//uv es la coordenada de tex en la esfera centrada en p_t. Esto también debería venir de afuera. Es la posición actual.
				//si se va inicializar desde afuera comentar esta línea
				pos = (1-_t) * _pos0 + _t * _pos1;

				//Tengo que rotar los ejes y cambiarle el signo a las coordenadas ¿No se exactamente porqué?
				float3 p_0 = {-_pos0.z, -_pos0.x, -_pos0.y};
				float3 p_1 = {-_pos1.z, -_pos1.x, -_pos1.y};
				float3 p_t = { -pos.z,   -pos.x,   -pos.y};

				float dist0 = length(p_t - p_0);
				float dist1 = length(p_t - p_1);
				float dist = dist0 + dist1;
				float t = dist0 / dist;

				
                // sample the texture
                float4 col = {0.5,0.5,0.0,1};
                col = version6(i.uv, p_t, p_0, p_1, t);
                return col;
            }
            ENDCG
        }
    }
}
