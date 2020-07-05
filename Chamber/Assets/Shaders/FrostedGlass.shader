
Shader "Blur"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BlurMask ("Blur Strength Mask", 2D) = "white" {} 
        _MaxLod ("Max Blur", Range(0.0, 7.0)) = 0
        _DepthLodMult ("Depth Multiplier (distance from here to background)", Float) = 0
        [Toggle(WRITE_DEPTH)] _WriteDepth ("Write Depth", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="HDUnlitShader"
            "Queue" = "Transparent+0"
        }
        
        Pass
        {
            ZWrite [_WriteDepth]
        
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #define REQUIRE_DEPTH_TEXTURE 1

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

            struct VerIn
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VerOut
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPosition : TEXCOORD1;
                float3 wsPosition : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BlurMask;
            float4 _BlurMask_ST;
            
            float _MaxLod;
            float _DepthLodMult;
            

            VerOut vert(VerIn v)
            {
                VerOut o;
                o.vertex = TransformWorldToHClip(TransformObjectToWorld(v.vertex.xyz));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPosition = ComputeScreenPos(mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), v.vertex)), _ProjectionParams.x);
                o.wsPosition = mul(UNITY_MATRIX_M,v.vertex).xyz;
                return o;
            }


            float4 frag(VerOut verOut) : SV_Target
            {
                half4 texCol = tex2D(_MainTex, verOut.uv);
                half maskVal = tex2D(_BlurMask, verOut.uv).r;
                
                // DOES NOT FUCKING WORK BUT WHY?????
                // bgDepth = linear distance to pixel behind this one
                float bgDepth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(verOut.screenPosition.xy), _ZBufferParams);
                // bgDepth = linear distance to fragment
                float fragDepth = length(mul(UNITY_MATRIX_V, float4(verOut.wsPosition, 1.0)).xyz).x;
                float depthDif = bgDepth - fragDepth;
                
                float fragLod = maskVal * _MaxLod;

                int lodLow = floor(fragLod);
                int lodHigh = min(_MaxLod, lodLow + 1);
                float blurWeigth = fragLod - lodLow;

                float4 screenPosition = float4(verOut.screenPosition.xy / verOut.screenPosition.w, 0, 0);

                float exposure =  GetInverseCurrentExposureMultiplier();
                half4 blurColLow = half4(SampleCameraColor(screenPosition.xy, lodLow) * exposure, 1);
                half4 blurColHigh = half4(SampleCameraColor(screenPosition.xy, lodHigh) * exposure, 1);

                 
                half4 blurColMix = lerp(blurColLow, blurColHigh, blurWeigth);
                half4 final = lerp(blurColMix, texCol, texCol.a);

                return final;
            }
            ENDHLSL
        }
    }
}
