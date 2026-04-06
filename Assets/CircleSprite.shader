Shader "Custom/RadialFillSprite_URP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0,1)) = 1
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _FillAmount;
            float4 _Color;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            float2 center = float2(0.5, 0.5);

            half4 frag (Varyings i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 dir = i.uv - center;

                float angle = atan2(dir.y, dir.x);
                angle = (angle + PI) / (2 * PI);

                // rotate so it starts from top
                angle = frac(angle + 0.25);

                if (angle > _FillAmount)
                    discard;

                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return col * _Color;
            }

            ENDHLSL
        }
    }
}