Shader "Sprites/BackgroundBlurMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Mask (Sprite Alpha)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _BlurTint ("Blur Tint", Color) = (1,1,1,1)
        _BlurStrength ("Blur Strength", Range(0, 1)) = 1

        // SpriteRenderer / UI compatibility (kept for SpriteRenderer)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _RendererColor;

            sampler2D _AlphaTex;
            float _EnableExternalAlpha;

            sampler2D _BackgroundBlurTex;
            float4 _BackgroundBlurTex_TexelSize;

            fixed4 _BlurTint;
            float _BlurStrength;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float4 screenPos: TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            inline fixed4 SampleMask(float2 uv)
            {
                fixed4 c = tex2D(_MainTex, uv);
                #if ETC1_EXTERNAL_ALPHA
                fixed4 a = tex2D(_AlphaTex, uv);
                c.a = lerp(c.a, a.r, _EnableExternalAlpha);
                #endif
                return c;
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color * _RendererColor;
                OUT.screenPos = ComputeScreenPos(OUT.vertex);

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                fixed4 mask = SampleMask(IN.uv);
                fixed alpha = mask.a * IN.color.a;
                if (alpha <= 0.001) discard;

                float2 screenUV = (IN.screenPos.xy / IN.screenPos.w);

                // Sample blurred background in screen space.
                fixed4 bg = tex2D(_BackgroundBlurTex, screenUV);

                fixed s = saturate(_BlurStrength);
                fixed4 tinted = bg * _BlurTint;

                // UI часто ставит Image Color = black для затемнения.
                // Поэтому берём RGB только из _BlurTint, а от vertex color используем ТОЛЬКО alpha.
                fixed outA = alpha * s * _BlurTint.a;

                fixed4 outC;
                outC.rgb = tinted.rgb * outA; // premultiplied
                outC.a = outA;
                return outC;
            }
            ENDCG
        }
    }
    Fallback Off
}


