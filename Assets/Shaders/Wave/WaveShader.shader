Shader "Unlit/WaveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude ("Amplitude", Float) = 1
        _Wavelength ("Wavelength", Float) = 10
        _Speed ("Speed", Float) = 1
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Amplitude, _Wavelength, _Speed;

            float4 WaveSum(float4 vertex){
                float x = vertex.x;
                float y = vertex.y;
                float2 xy = float2(x,y);
                float4 vertWavePos = vertex;
                vertWavePos.xy = sin(xy);
                return vertWavePos;
            }

            v2f vert (appdata v)
            {
                v2f o;
                float4 vertex = v.vertex;
                float k = 2* UNITY_PI / _Wavelength;
                float f = k * (vertex.x - _Speed * _Time.y);
                vertex.y = _Amplitude*sin(f);
                float3 tangent = normalize(float3(1,k*_Amplitude*cos(f),0));
                float3 normal = float3(-tangent.y, tangent.x, 0);
                vertex = UnityObjectToClipPos(vertex);
                o.vertex = vertex;
                o.normal = normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
