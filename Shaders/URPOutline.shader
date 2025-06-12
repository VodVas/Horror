// // Shader "Custom/URPOutline"
// // {
// //     Properties
// //     {
// //         _OutlineColor ("Outline Color", Color) = (0,0,0,1)
// //         _OutlineWidth ("Outline Width", Float) = 0.002
// //         _ZOffset ("Z Offset", Float) = 0.0001
// //     }
    
// //     SubShader
// //     {
// //         Tags 
// //         { 
// //             "RenderType" = "Opaque"
// //             "RenderPipeline" = "UniversalPipeline"
// //             "Queue" = "Geometry"
// //         }
        
// //         Pass
// //         {
// //             Name "Outline"
// //             Tags { "LightMode" = "UniversalForward" }
            
// //             Cull Front
// //             ZWrite On
// //             ZTest LEqual
            
// //             Stencil
// //             {
// //                 Ref 1
// //                 Comp Always
// //                 Pass Replace
// //             }
            
// //             HLSLPROGRAM
// //             #pragma vertex OutlineVertex
// //             #pragma fragment OutlineFragment
// //             #pragma multi_compile_instancing
// //             #pragma target 3.5
            
// //             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
// //             CBUFFER_START(UnityPerMaterial)
// //                 float4 _OutlineColor;
// //                 float _OutlineWidth;
// //                 float _ZOffset;
// //             CBUFFER_END
            
// //             struct Attributes
// //             {
// //                 float4 positionOS : POSITION;
// //                 float3 normalOS : NORMAL;
// //                 UNITY_VERTEX_INPUT_INSTANCE_ID
// //             };
            
// //             struct Varyings
// //             {
// //                 float4 positionCS : SV_POSITION;
// //                 UNITY_VERTEX_INPUT_INSTANCE_ID
// //             };
            
// //             Varyings OutlineVertex(Attributes input)
// //             {
// //                 Varyings output = (Varyings)0;
                
// //                 UNITY_SETUP_INSTANCE_ID(input);
// //                 UNITY_TRANSFER_INSTANCE_ID(input, output);
                
// //                 float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
// //                 float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                
// //                 positionWS += normalWS * _OutlineWidth;
// //                 output.positionCS = TransformWorldToHClip(positionWS);
                
// //                 output.positionCS.z -= _ZOffset * output.positionCS.w;
                
// //                 return output;
// //             }
            
// //             half4 OutlineFragment(Varyings input) : SV_Target
// //             {
// //                 UNITY_SETUP_INSTANCE_ID(input);
// //                 return _OutlineColor;
// //             }
// //             ENDHLSL
// //         }
// //     }
    
// //     FallBack "Hidden/Universal Render Pipeline/FallbackError"
// // }




//                 Shader "Custom/URPOutline"
// {
//     Properties
//     {
//         _OutlineColor ("Outline Color", Color) = (0,0,0,1)
//         _OutlineWidth ("Outline Width", Float) = 0.002
//         _ZOffset ("Z Offset", Float) = 0.0001
//         [HideInInspector] _ColorMask ("Color Mask", Float) = 15
//     }
    
//     SubShader
//     {
//         Tags 
//         { 
//             "RenderType" = "Opaque"
//             "RenderPipeline" = "UniversalPipeline"
//             "Queue" = "Geometry+1"
//         }
        
//         Pass
//         {
//             Name "Outline"
//             Tags { "LightMode" = "UniversalForward" }
            
//             Cull Front
//             ColorMask [_ColorMask]
            
//             HLSLPROGRAM
//             #pragma vertex OutlineVertex
//             #pragma fragment OutlineFragment
//             #pragma multi_compile_instancing
//             #pragma target 3.5
            
//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
//             CBUFFER_START(UnityPerMaterial)
//                 float4 _OutlineColor;
//                 float _OutlineWidth;
//                 float _ZOffset;
//                 float _ColorMask;
//             CBUFFER_END
            
//             struct Attributes
//             {
//                 float4 positionOS : POSITION;
//                 float3 normalOS : NORMAL;
//                 UNITY_VERTEX_INPUT_INSTANCE_ID
//             };
            
//             struct Varyings
//             {
//                 float4 positionCS : SV_POSITION;
//                 UNITY_VERTEX_INPUT_INSTANCE_ID
//             };
            
//             Varyings OutlineVertex(Attributes input)
//             {
//                 Varyings output = (Varyings)0;
                
//                 UNITY_SETUP_INSTANCE_ID(input);
//                 UNITY_TRANSFER_INSTANCE_ID(input, output);
                
//                 // Calculate outline position
//                 float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
//                 float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                
//                 // Offset position along normal
//                 positionWS += normalWS * _OutlineWidth;
                
//                 // Transform to clip space
//                 output.positionCS = TransformWorldToHClip(positionWS);
                
//                 // Apply Z offset to prevent z-fighting
//                 output.positionCS.z -= _ZOffset * output.positionCS.w;
                
//                 return output;
//             }
            
//             half4 OutlineFragment(Varyings input) : SV_Target
//             {
//                 UNITY_SETUP_INSTANCE_ID(input);
//                 return _OutlineColor;
//             }
//             ENDHLSL
//         }
//     }
    
//     FallBack "Hidden/Universal Render Pipeline/FallbackError"
// }


Shader "Custom/URPOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 0.002
        _ZOffset ("Z Offset", Float) = 0.0001
        _ZTest ("ZTest", Int) = 8 // Always
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry+1"
        }
        
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Front
            ColorMask [_ColorMask]
            ZTest [_ZTest]
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment
            #pragma multi_compile_instancing
            #pragma target 3.5
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
                float _ZOffset;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            Varyings OutlineVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                positionWS += normalWS * _OutlineWidth;
                
                output.positionCS = TransformWorldToHClip(positionWS);
                output.positionCS.z += _ZOffset * output.positionCS.w;
                
                return output;
            }
            
            half4 OutlineFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}