// Вода из видео — плоский насыщенный циан с бледными разводами.
// Никакого освещения ей не нужно: unlit дешевле и точнее попадает в стиль.
Shader "RichRun/Stylized Water"
{
    Properties
    {
        _BaseColor ("Цвет воды", Color) = (0.169, 0.682, 0.925, 1)
        _FoamColor ("Цвет разводов", Color) = (0.60, 0.93, 1.0, 1)
        _WaveTex ("Текстура волн", 2D) = "black" {}
        _Strength ("Сила разводов", Range(0, 1)) = 0.5
        // Порог отсекает низ диапазона канала, контраст растягивает остаток до единицы.
        _Threshold ("Порог узора", Range(0, 1)) = 0.16
        _Contrast ("Контраст узора", Range(0.5, 12)) = 4
        _Speed1 ("Скорость слоя 1 (xy)", Vector) = (0.012, 0.020, 0, 0)
        _Speed2 ("Скорость слоя 2 (xy)", Vector) = (-0.017, 0.011, 0, 0)
        _Scale2 ("Масштаб слоя 2", Range(0.2, 3)) = 0.7
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_WaveTex);
            SAMPLER(sampler_WaveTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _WaveTex_ST;
                half4 _BaseColor;
                half4 _FoamColor;
                float _Strength;
                float _Threshold;
                float _Contrast;
                float4 _Speed1;
                float4 _Speed2;
                float _Scale2;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _WaveTex);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float time = _Time.y;

                // Два слоя с разной скоростью и масштабом — рисунок не зацикливается на глаз.
                half a = SAMPLE_TEXTURE2D(_WaveTex, sampler_WaveTex, input.uv + _Speed1.xy * time).r;
                half b = SAMPLE_TEXTURE2D(_WaveTex, sampler_WaveTex, input.uv * _Scale2 + _Speed2.xy * time).r;

                // Текстуры воды в проекте синие: узор сидит в красном канале и занимает
                // лишь часть диапазона (у water_brighter это 0.12..0.59). Поэтому берём
                // максимум слоёв, а не произведение, и растягиваем порогом с контрастом —
                // иначе разводов не видно вовсе.
                half pattern = max(a, b);
                pattern = saturate((pattern - _Threshold) * _Contrast);

                return lerp(_BaseColor, _FoamColor, pattern * _Strength);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
