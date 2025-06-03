Shader "Universal Render Pipeline/Custom/MonsterVision"
{
    Properties
    {
        [Header(Base Settings)]
        _MainTex ("Screen Texture", 2D) = "white" {}
        _VisionIntensity ("Vision Intensity", Range(0, 1)) = 1.0
        
        [Header(Color Settings)]
        _HotColor ("Hot Color", Color) = (1, 0.2, 0, 1)
        _ColdColor ("Cold Color", Color) = (0, 0.2, 1, 1)
        _EdgeColor ("Edge Detection Color", Color) = (1, 1, 0, 1)
        _ColorThreshold ("Color Detection Threshold", Range(0, 1)) = 0.5
        
        [Header(Distortion)]
        _DistortionStrength ("Distortion Strength", Range(0, 0.5)) = 0.1
        _DistortionFrequency ("Distortion Frequency", Range(1, 10)) = 3.0
        
        [Header(Pulse Effect)]
        _PulseSpeed ("Pulse Speed", Range(0.1, 5)) = 1.0
        _PulseIntensity ("Pulse Intensity", Range(0, 1)) = 0.3
        
        [Header(Noise)]
        _NoiseScale ("Noise Scale", Range(1, 100)) = 50.0
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.2
        _StaticIntensity ("Static Intensity", Range(0, 1)) = 0.1
        
        [Header(Edge Detection)]
        _EdgeThreshold ("Edge Detection Threshold", Range(0, 0.1)) = 0.01
        _EdgeIntensity ("Edge Intensity", Range(0, 2)) = 1.0
        
        [Header(Vignette)]
        _VignetteIntensity ("Vignette Intensity", Range(0, 2)) = 1.0
        _VignetteSmoothness ("Vignette Smoothness", Range(0.01, 1)) = 0.5
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Overlay"
        }
        
        Pass
        {
            Name "MonsterVisionPass"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ _HIGH_QUALITY
            #pragma target 3.0
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 screenPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _HotColor;
                float4 _ColdColor;
                float4 _EdgeColor;
                float _VisionIntensity;
                float _ColorThreshold;
                float _DistortionStrength;
                float _DistortionFrequency;
                float _PulseSpeed;
                float _PulseIntensity;
                float _NoiseScale;
                float _NoiseIntensity;
                float _StaticIntensity;
                float _EdgeThreshold;
                float _EdgeIntensity;
                float _VignetteIntensity;
                float _VignetteSmoothness;
            CBUFFER_END
            
            // Optimized noise function
            float Hash(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.13);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }
            
            float Noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = Hash(i);
                float b = Hash(i + float2(1.0, 0.0));
                float c = Hash(i + float2(0.0, 1.0));
                float d = Hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            // Fast edge detection using Sobel operator
            float EdgeDetection(float2 uv, float2 texelSize)
            {
                float3 sobel_x = float3(-1, 0, 1);
                float3 sobel_y = float3(-1, 0, 1);
                
                float gx = 0;
                float gy = 0;
                
                for(int x = -1; x <= 1; x++)
                {
                    for(int y = -1; y <= 1; y++)
                    {
                        float2 offset = float2(x, y) * texelSize;
                        float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset).rgb;
                        float lum = dot(col, float3(0.299, 0.587, 0.114));
                        
                        gx += lum * sobel_x[x + 1] * sobel_y[y + 1];
                        gy += lum * sobel_y[x + 1] * sobel_x[y + 1];
                    }
                }
                
                return length(float2(gx, gy));
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.screenPos = output.uv;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Calculate radial distortion
                float2 centerUV = input.uv - 0.5;
                float radialDistance = length(centerUV);
                float distortionAmount = radialDistance * radialDistance * _DistortionStrength;
                
                // Apply wave distortion
                float time = _Time.y;
                float2 distortion = sin(centerUV * _DistortionFrequency + time) * distortionAmount;
                float2 distortedUV = input.uv + distortion;
                
                // Sample main texture with distortion
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
                
                // Calculate luminance for heat vision
                float luminance = dot(color.rgb, float3(0.299, 0.587, 0.114));
                
                // Apply pulse effect
                float pulse = sin(time * _PulseSpeed) * 0.5 + 0.5;
                luminance += pulse * _PulseIntensity * (1.0 - radialDistance);
                
                // Heat vision color mapping
                float heatValue = saturate(luminance + (color.r - color.b) * _ColorThreshold);
                half3 heatColor = lerp(_ColdColor.rgb, _HotColor.rgb, heatValue);
                
                // Edge detection
                float2 texelSize = 1.0 / float2(1920, 1080); // TODO: Pass screen dimensions
                float edges = EdgeDetection(distortedUV, texelSize);
                edges = step(_EdgeThreshold, edges) * _EdgeIntensity;
                
                // Add edge highlight
                heatColor = lerp(heatColor, _EdgeColor.rgb, edges);
                
                // Noise and static
                float2 noiseUV = input.uv * _NoiseScale + time * 0.1;
                float noiseValue = Noise(noiseUV);
                float staticNoise = Hash(input.uv + time) * _StaticIntensity;
                
                heatColor += noiseValue * _NoiseIntensity;
                heatColor = lerp(heatColor, half3(staticNoise, staticNoise, staticNoise), staticNoise);
                
                // Vignette effect
                float vignette = 1.0 - saturate(pow(radialDistance * _VignetteIntensity, 1.0 / _VignetteSmoothness));
                heatColor *= vignette;
                
                // Final composition
                half3 finalColor = lerp(color.rgb, heatColor, _VisionIntensity);
                
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
