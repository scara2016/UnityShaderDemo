Shader "Hidden/BoxBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float4 _MainTex_TexelSize;
			
            float4 boxBlur(sampler2D tex, float2 uv) {
                float2 delta = float2(_MainTex_TexelSize.z,_MainTex_TexelSize.w);
			    delta = 1/delta;
                float4 col = float4(0,0,0,0);
				[unroll]
                for(int i = -2; i<=2;i++){
                    [unroll]
                    for(int j = -2; j<=2;j++){
                        col+= tex2D(tex, uv + float2(i,j) * delta);
                    }
                }
                col = col/25;
                return col;

            }
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = boxBlur(_MainTex, i.uv);
                
                return col;
            }
            ENDCG
        }
    }
}
