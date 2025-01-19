Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 10)) = 1.5
        _Transparency ("Transparency", Range(0, 1)) = 0.8
        _LightTint ("Light Tint", Color) = (1,1,1,0.1)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        LOD 100

        // Enable transparency
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
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
            float _BlurSize;
            float _Transparency;
            float4 _LightTint;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 color = 0;
                float totalWeight = 0;

                // Improved Gaussian blur with variable sample count
                const int samples = 5;
                const float blurStep = 0.002;

                for (int x = -samples; x <= samples; x++)
                {
                    for (int y = -samples; y <= samples; y++)
                    {
                        float2 offsetUV = float2(x, y) * _BlurSize * blurStep;
                        float weight = exp(-(x * x + y * y) / (2.0 * _BlurSize));
                        color += tex2D(_MainTex, uv + offsetUV) * weight;
                        totalWeight += weight;
                    }
                }

                // Normalize the blurred result
                color = color / totalWeight;

                // Add subtle light tint for the glass effect
                color.rgb += _LightTint.rgb * _LightTint.a;
                
                // Apply transparency
                color.a = _Transparency;

                return color;
            }
            ENDCG
        }
    }
}
