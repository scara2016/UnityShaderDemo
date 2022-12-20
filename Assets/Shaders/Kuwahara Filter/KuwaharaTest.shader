Shader "Unlit/KuwaharaTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Range("Range", float) = 1
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
            float _Range;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 improvedKuwaharaBoxKernel(sampler2D mainTex, float2 uv, float range, float2 origin)
            {
                float2 delta = float2(_MainTex_TexelSize.z,_MainTex_TexelSize.w);
                origin = range*origin;
			    delta = 1/delta;
                float4 col;
                float lum = 0;
                float4 avgColor = float4(0,0,0,0);
                float avgLum = 0;
                for(int i=-range;i<range;i++)
                {
                    for (int j=-range;j<range;j++)
                    {
                        col = tex2D(mainTex,uv+(origin*delta+(float2(i,j)*delta)));
                        avgColor += col;
                        avgLum += Luminance(col);
                    }   
                }
                avgColor = avgColor/((3+((range-1)*2))*(3+((range-1)*2)));
                avgLum = avgLum/((3+((range-1)*2))*(3+((range-1)*2)));
                float avgLumSqr = 0;
                float stdDev = 0;
                
                 for(int i=-range;i<range;i++)
                 {
                      for (int j=-range;j<range;j++)
                      {
                          col = tex2D(mainTex,uv+(origin*delta+(float2(i,j)*delta)));
                          lum += Luminance(col);
                          avgLumSqr +=  (lum-avgLum)*(lum-avgLum);
                      }   
                 }
                 stdDev = sqrt(avgLumSqr/((3+((range-1)*2))*(3+((range-1)*2))));
                return fixed4(avgColor.x,avgColor.y,avgColor.z,stdDev);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 colImproved[4];
                colImproved[0] = improvedKuwaharaBoxKernel(_MainTex, i.uv, _Range, float2(1,1));
                colImproved[1] = improvedKuwaharaBoxKernel(_MainTex, i.uv, _Range, float2(1,-1));
                colImproved[2] = improvedKuwaharaBoxKernel(_MainTex, i.uv, _Range, float2(-1,1));
                colImproved[3] = improvedKuwaharaBoxKernel(_MainTex, i.uv, _Range, float2(-1,-1));
                float lowst = colImproved[0];
                fixed4 readyColor;
                for(int i =0;i<4;i++)
                {
                    if(colImproved[i].w<lowst)
                    {
                        lowst = colImproved[i];
                        readyColor = fixed4(colImproved[i].x, colImproved[i].y,colImproved[i].z,1);
                    }
                }
                
                return readyColor;
                
            }
            
            
            ENDCG
        }
    }
}
