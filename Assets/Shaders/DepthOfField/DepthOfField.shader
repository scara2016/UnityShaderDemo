Shader "Unlit/DepthOfField"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;

        struct VertexDate {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Interpolaters {
           float4 pos : SV_POSITION;
           float2 uv : TEXCOORD0;
        };

        Interpolaters VertexProgram(VertexDate v){
            Interpolaters i;
            i.pos = UnityObjectToClipPos(v.vertex);
            i.uv = v.uv;
            return i;
        }

    ENDCG

    SubShader
    {
        Cull Off
        ZTest Always
        ZWrite Off
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
          
            fixed4 FragmentProgram (Interpolaters i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
