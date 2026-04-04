Shader "Custom/URP_AfterImage"
{
    Properties
    {
        [HDR] _Color("Color", Color) = (1,0,0,1)
        _Alpha("Alpha", Range(0, 1)) = 1
        _RimLightMul("RimLightMul", Range(0, 10)) = 0.5
        _RimLightPow("RimLightPow", Range(0, 10)) = 1.5
        _Intensity("Intensity", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 viewDirWS  : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Alpha;
                float _RimLightMul;
                float _RimLightPow;
                float _Intensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // 위치 및 노멀 변환
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                output.normalWS = normalInputs.normalWS;
                
                // 시선 방향 계산
                output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 viewDir = normalize(input.viewDirWS);
                float3 normal = normalize(input.normalWS);

                // 림라이트 계산 (중앙은 투명하고 외곽만 빛나는 효과)
                float NdotV = saturate(dot(normal, viewDir));
                float rim = pow((1.0 - NdotV) * _RimLightMul, _RimLightPow);
                
                half4 col;
                // 기본 색상에 Intensity(강도)를 더해 발광 효과 유도
                col.rgb = _Color.rgb * _Intensity;
                col.a = saturate(_Alpha * rim);

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}