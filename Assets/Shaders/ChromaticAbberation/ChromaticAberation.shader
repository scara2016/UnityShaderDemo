Shader "Hidden/ChromaticAberation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ROffset("ROffset", Float) = 0.1
        _GOffset("GOffset", Float) = 0.1
        _BOffset("BOffset", Float) = 0.1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex,i.uv);
                fixed colGS = (col.r +col.b + col.g)/3;
                col = float4(colGS,colGS,colGS,col.a);
                return col;
            }
            ENDCG
        }
        Cull Off ZWrite Off ZTest Always
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            float _ROffset;
            float _GOffset;
            float _BOffset;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centralDirection = i.uv - float2(0.5f,0.5f);
                fixed4 colOG = tex2D(_MainTex, i.uv);
                _ROffset = sin(_Time*6.0f) * _ROffset;
                _GOffset = sin(_Time*8.0f) * _GOffset;
                _BOffset = sin(_Time*9.3f) * _BOffset;
                fixed4 colROffset = tex2D(_MainTex, i.uv + (centralDirection*_ROffset));
                fixed4 colGOffset = tex2D(_MainTex, i.uv + (centralDirection*_GOffset));
                fixed4 colBOffset = tex2D(_MainTex, i.uv + (centralDirection*_BOffset));
                fixed4 col = float4(colROffset.r, colGOffset.g, colBOffset.b, col.a);
                return col;
            }
            ENDCG
        }
    }
}
