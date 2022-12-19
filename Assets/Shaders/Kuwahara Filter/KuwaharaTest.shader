Shader "Unlit/KuwaharaTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            

            fixed4 kuwaharaBoxKernel(sampler2D MainTex, float2 uv){
                float2 delta = float2(_MainTex_TexelSize.z,_MainTex_TexelSize.w);
			    delta = 1/delta;
                float4 avgColours[4] = {float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0)};
                float avgLum[4] = {0,0,0,0};
                float avgLumSqr[4] = {0,0,0,0};
                float stdDev[4] = {0,0,0,0};
                float4 col;
                float lum;
                float lumSqr;
                [unroll]
                    for(int i=-3;i<=3;i++){
                    [unroll]
                        for(int j=-3;j<=3;j++){
                            col = tex2D(MainTex, uv + float2(i,j) * delta);
                            lum = Luminance(col);
                            lumSqr = lum*lum;
                            if(i>=0&&j>=0){
                                avgColours[0] += col; 
                                avgLum[0] += lum;
                                avgLumSqr[0] += lum*lum;
                            }
                            if(i<=0&&j>=0){
                                avgColours[1] += col; 
                                avgLum[1] += lum;
                                avgLumSqr[1] += lum*lum;
                            }
                            if(i<=0&&j<=0){
                                avgColours[2] += col; 
                                avgLum[2] += lum;
                                avgLumSqr[2] += lum*lum; 
                            }
                            if(i>=0&&j<=0){
                                avgColours[3] += col; 
                                avgLum[3] += lum; 
                                avgLumSqr[3] += lum*lum;
                            }
                    }
                }
                float lowst = 0;
                float4 selectedColour = float4(0,0,0,0);
                [unroll]
                    for(int i=0;i<4;i++){
                        avgColours[i] = avgColours[i]/9;
                        avgLum[i] = avgLum[i]/9;
                        stdDev[i] = abs((avgLumSqr[i]/9)-(avgLum[i]*avgLum[i]));
                        if(stdDev[i]>lowst){
                            selectedColour = avgColours[i];
                            lowst = stdDev[i];
                        }
                    }
                return selectedColour;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = kuwaharaBoxKernel(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
