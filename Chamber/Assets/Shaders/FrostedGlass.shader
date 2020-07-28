
Shader "FrostedGlass"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        [NoScaleOffset]_BlurMask ("Blur Strength Mask", 2D) = "white" {}
        _BlurStrength ("Blur Strength", Float) = 0.2
        _NearDepthFadeDist ("Near Depth Fade Distance", float) = 1

        [KeywordEnum(DoubleSided, Invert, Normal)]_CullMode("Cull Mode", Float) = 2

        // From generated code
        [HideInInspector]_EmissionColor("Color", Color) = (1, 1, 1, 1)
        [HideInInspector]_RenderQueueType("Vector1", Float) = 5
        [HideInInspector]_StencilRef("Vector1", Int) = 2
        [HideInInspector]_StencilWriteMask("Vector1", Int) = 3
        [HideInInspector]_StencilRefDepth("Vector1", Int) = 32
        [HideInInspector]_StencilWriteMaskDepth("Vector1", Int) = 48
        [HideInInspector]_StencilRefMV("Vector1", Int) = 160
        [HideInInspector]_StencilWriteMaskMV("Vector1", Int) = 176
        [HideInInspector]_StencilRefDistortionVec("Vector1", Int) = 64
        [HideInInspector]_StencilWriteMaskDistortionVec("Vector1", Int) = 64
        [HideInInspector]_StencilWriteMaskGBuffer("Vector1", Int) = 51
        [HideInInspector]_StencilRefGBuffer("Vector1", Int) = 34
        [HideInInspector]_ZTestGBuffer("Vector1", Int) = 4
        [HideInInspector][ToggleUI]_RequireSplitLighting("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_ReceivesSSR("Boolean", Float) = 0
        [HideInInspector]_SurfaceType("Vector1", Float) = 1
        [HideInInspector]_BlendMode("Vector1", Float) = 0
        [HideInInspector]_SrcBlend("Vector1", Float) = 1
        [HideInInspector]_DstBlend("Vector1", Float) = 0
        [HideInInspector]_AlphaSrcBlend("Vector1", Float) = 1  
        [HideInInspector]_AlphaDstBlend("Vector1", Float) = 0
        [HideInInspector][ToggleUI]_ZWrite("Boolean", Float) = 1
        [HideInInspector]_TransparentSortPriority("Vector1", Int) = 0
        [HideInInspector]_CullModeForward("Vector1", Float) = 2
        [HideInInspector][Enum(Front, 1, Back, 2)]_TransparentCullMode("Vector1", Float) = 2
        [HideInInspector]_ZTestDepthEqualForOpaque("Vector1", Int) = 4
        [HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)]_ZTestTransparent("Vector1", Float) = 4
        [HideInInspector][ToggleUI]_TransparentBackfaceEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_AlphaCutoffEnable("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_UseShadowThreshold("Boolean", Float) = 0
        [HideInInspector][ToggleUI]_DoubleSidedEnable("Boolean", Float) = 0
        [HideInInspector][Enum(Flip, 0, Mirror, 1, None, 2)]_DoubleSidedNormalMode("Vector1", Float) = 2
        [HideInInspector]_DoubleSidedConstants("Vector4", Vector) = (1, 1, -1, 0)
    }

    SubShader
    {
        Tags
        {
            "LIGHTMODE"="ForwardOnly" 
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="HDUnlitShader"
            "Queue" = "Transparent+0"
        }
        
        Pass
        {
            Blend [_SrcBlend] [_DstBlend], [_AlphaSrcBlend] [_AlphaDstBlend]
        
            Cull [_CullMode]
        
            ZTest [_ZTestTransparent]
        
            ZWrite [_ZWrite]
        
        
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #define REQUIRE_DEPTH_TEXTURE

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            #define SHADERPASS SHADERPASS_FORWARD_UNLIT
            #pragma multi_compile _ DEBUG_DISPLAY
        
            #if defined(_ENABLE_SHADOW_MATTE) && SHADERPASS == SHADERPASS_FORWARD_UNLIT
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Shadow/HDShadowContext.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/HDShadow.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/PunctualLightCommon.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/HDShadowLoop.hlsl"
            #endif
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
        
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
            // Shader Start

            #define MAX_LOD 7
            #define DEPTH_BLUR_ADJUST 2.7
        
            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
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
            
            float _BlurStrength;
            float _NearDepthFadeDist;

            float EaseOutCubix(float x) {
              return (x = x - 1) * x * x + 1;
            }
            
            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.vertex = TransformWorldToHClip(TransformObjectToWorld(v.vertex.xyz));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPosition = ComputeScreenPos(mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), v.vertex)), _ProjectionParams.x);
                o.wsPosition = mul(UNITY_MATRIX_M, v.vertex).xyz;
                return o;
            }

            float4 frag(VertexOutput o) : SV_Target
            {
                half4 texCol = tex2D(_MainTex, o.uv);
                half maskBlurMult = tex2D(_BlurMask, o.uv).r;

                float3 vsViewDir = normalize(mul((float3x3)UNITY_MATRIX_V, o.wsPosition).xyz);  
                float depthToDist = 1 / -vsViewDir.z; 

                float bgDepth = LinearEyeDepth(SampleCameraDepth(o.screenPosition.xy / o.screenPosition.w), _ZBufferParams);
                float bgDist = bgDepth * depthToDist;
                float fragDist = length(mul(UNITY_MATRIX_V, float4(o.wsPosition, 1.0)).xyz).x;
                float fragBgDist = bgDist - fragDist;

                float fragLod
                  = (_BlurStrength) // Blur strength property
                  * (maskBlurMult) // Multiplier from tex
                  / (sqrt(bgDist)) // Adapt to distance change (camera moves closer/farther)
                  * (1 + (sqrt(fragBgDist) - 1) / DEPTH_BLUR_ADJUST) // Keep blur amount consistent when depth is different
                  * (clamp(EaseOutCubix(fragBgDist / _NearDepthFadeDist), 0, 1)); // Near depth fade

                int lodLow = min(MAX_LOD, floor(fragLod));
                int lodHigh = min(MAX_LOD, lodLow + 1);
                float lowHighWeigth = fragLod - lodLow;

                float4 screenPosition = float4(o.screenPosition.xy / o.screenPosition.w, 0, 0);


                half4 blurColLow = half4(SampleCameraColor(screenPosition.xy, lodLow), 1);
                half4 blurColHigh = half4(SampleCameraColor(screenPosition.xy, lodHigh), 1);

                half4 blurColMix = lerp(blurColLow, blurColHigh, lowHighWeigth);
                half4 final = lerp(blurColMix, texCol, texCol.a);

                return final;
            }
            ENDHLSL
        }
    }
}
