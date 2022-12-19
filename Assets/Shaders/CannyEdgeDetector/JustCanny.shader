Shader "Hidden/CombinedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_HighThreshold("HighThreshold", Float) = 0.7
		_LowThreshold("LowThreshold", Float) = 0.3
		_OutlineColor("OutlineColor", Color) = (0,0,0,1) 
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite On ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#define PI radians(180)
		
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
			sampler2D _CameraDepthTexture;
			sampler2D _NoiseTex;
			float4 _OutlineColor;
			float _HighThreshold;
			float _LowThreshold;

            float sobel(sampler2D tex, float2 uv) {
				//float2 delta = float2(_DeltaX, _DeltaY);
				float2 delta = float2(_MainTex_TexelSize.z,_MainTex_TexelSize.w);
				delta = 1/delta;
				float4 hr = float4(0, 0, 0, 0);
				float4 vt = float4(0, 0, 0, 0);

				hr += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) * 1.0;
				hr += tex2D(tex, (uv + float2(0.0, -1.0) * delta)) * 0.0;
				hr += tex2D(tex, (uv + float2(1.0, -1.0) * delta)) * -1.0;
				hr += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) * 2.0;
				hr += tex2D(tex, (uv + float2(0.0,  0.0) * delta)) * 0.0;
				hr += tex2D(tex, (uv + float2(1.0,  0.0) * delta)) * -2.0;
				hr += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) * 1.0;
				hr += tex2D(tex, (uv + float2(0.0,  1.0) * delta)) * 0.0;
				hr += tex2D(tex, (uv + float2(1.0,  1.0) * delta)) * -1.0;
				hr = hr / 8;
				vt += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) * 1.0;
				vt += tex2D(tex, (uv + float2(0.0, -1.0) * delta)) * 2.0;
				vt += tex2D(tex, (uv + float2(1.0, -1.0) * delta)) * 1.0;
				vt += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) * 0.0;
				vt += tex2D(tex, (uv + float2(0.0,  0.0) * delta)) * 0.0;
				vt += tex2D(tex, (uv + float2(1.0,  0.0) * delta)) * 0.0;
				vt += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) * -1.0;
				vt += tex2D(tex, (uv + float2(0.0,  1.0) * delta)) * -2.0;
				vt += tex2D(tex, (uv + float2(1.0,  1.0) * delta)) * -1.0;
				vt = vt / 8;
				float x=sqrt(hr * hr + vt * vt);
				vt = normalize(vt);
				hr = normalize(hr);
				float angleOfIntensity =(atan2(vt.xyz,hr.xyz));
				//return float2(x,angleOfIntensity);

            	float intensityMag = x;
				float intensityMag1 = 0.0;
				float intensityMag2 = 0.0;
				if(angleOfIntensity>(7*PI/4)&&angleOfIntensity<(PI/4)){
					intensityMag1 = tex2D(_MainTex, (uv + float2(1.0, 0.0) * delta)).r;
					intensityMag2 = tex2D(_MainTex, (uv + float2(-1.0, 0.0) * delta)).r;
				}
				else if(angleOfIntensity>(0.0)&&angleOfIntensity<(PI/2)){
					intensityMag1 = tex2D(_MainTex, (uv + float2(1.0, 0.0) * delta)).r;
					intensityMag2 = tex2D(_MainTex, (uv + float2(-1.0, 0.0) * delta)).r;
				}
				else if(angleOfIntensity>(PI/4)&&angleOfIntensity<(3*PI/4)){
					intensityMag1 = tex2D(_MainTex, (uv + float2(1.0, 0.0) * delta)).r;
					intensityMag2 = tex2D(_MainTex, (uv + float2(-1.0, 0.0) * delta)).r;
				}
				else if(angleOfIntensity>(PI/2)&&angleOfIntensity<(PI)){
					intensityMag1 = tex2D(_MainTex, (uv + float2(1.0, 0.0) * delta)).r;
					intensityMag2 = tex2D(_MainTex, (uv + float2(-1.0, 0.0) * delta)).r;
				}
				else if(angleOfIntensity>(3*PI/4)&&angleOfIntensity<(5*PI/4)){
					intensityMag1 = tex2D(_MainTex, (uv + float2(1.0, 0.0) * delta)).r;
					intensityMag2 = tex2D(_MainTex, (uv + float2(-1.0, 0.0) * delta)).r;
				}
				else if(angleOfIntensity>(PI)&&angleOfIntensity<(3*PI/2)){
					intensityMag1 = tex2D(_MainTex, (uv + float2(1.0, 0.0) * delta)).r;
					intensityMag2 = tex2D(_MainTex, (uv + float2(-1.0, 0.0) * delta)).r;
				}
				else if(angleOfIntensity>(5*PI/4)&&angleOfIntensity<(7*PI/4)){
					intensityMag1 = tex2D(_MainTex, (uv + float2(1.0, 0.0) * delta)).r;
					intensityMag2 = tex2D(_MainTex, (uv + float2(-1.0, 0.0) * delta)).r;
				}
				else if(angleOfIntensity>(3*PI/2)&&angleOfIntensity<(0)){
					intensityMag1 = tex2D(_MainTex, (uv + float2(1.0, 0.0) * delta)).r;
					intensityMag2 = tex2D(_MainTex, (uv + float2(-1.0, 0.0) * delta)).r;
				}
				if(intensityMag<intensityMag1||intensityMag<intensityMag2){
						intensityMag = 0;
				}
				return intensityMag;

			}

            float4 hysterisis(float4 col, float2 uv){
				float2 delta = float2(_MainTex_TexelSize.z,_MainTex_TexelSize.w);
				delta = 1/delta;		
				if(col.g==1){
					return float4(1,1,1,1);
				}
				else if(col.r==1)
				[unroll]
				for(int i=-1;i<=1;i++){
					[unroll]
					for(int j=-1;j<=1;j++){
						float4 edge = tex2D(_MainTex, (uv + float2(i,j) * delta));
						if(edge.g==1){
							return float4(1,1,1,1);
						}
					}
				}
				else{
					return float4(0,0,0,1);
				}
				return float4(0,0,0,1);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float4 ocol = tex2D(_MainTex, i.uv);
				float s = sobel(_MainTex, i.uv);
				float2 uvs = i.uv;
				float4 dt4 = float4(s,s,s,1);
				float4 col = dt4;
				if(s>_HighThreshold){
					dt4 = float4(0,1,0,1);
				}
				
				if(s<_HighThreshold && s>_LowThreshold){
					dt4 = float4(1,0,0,1);
				}
				else if(s<_LowThreshold){
					dt4 = float4(0,0,0,1);
				}
				col = hysterisis(dt4, i.uv);
				return col*_OutlineColor;
				if(col.r ==0){
					return ocol;
				}
				else{
					col = col* _OutlineColor;
					return col;
				}
            }
            ENDCG
        }

    }
}
