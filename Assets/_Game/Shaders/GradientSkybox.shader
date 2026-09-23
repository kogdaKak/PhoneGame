// Небо в оригинале — почти плоский циан с лёгким переходом к горизонту.
// Процедурный скайбокс Unity сюда не годится: он рисует солнце и атмосферу.
Shader "RichRun/Gradient Skybox"
{
    Properties
    {
        _TopColor ("Небо вверху", Color) = (0.251, 0.863, 0.933, 1)
        _HorizonColor ("У горизонта", Color) = (0.208, 0.776, 0.922, 1)
        _Offset ("Сдвиг горизонта", Range(-0.5, 0.5)) = 0
        _Power ("Резкость перехода", Range(0.2, 4)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "PreviewType" = "Skybox"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 direction : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _TopColor;
                half4 _HorizonColor;
                float _Offset;
                float _Power;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                // Меш скайбокса центрирован на камере, поэтому позиция и есть направление взгляда.
                output.direction = input.positionOS.xyz;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float height = normalize(input.direction).y - _Offset;
                float t = pow(saturate(height), _Power);
                return lerp(_HorizonColor, _TopColor, t);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
