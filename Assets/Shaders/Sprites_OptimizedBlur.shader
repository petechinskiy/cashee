Shader "Sprites/OptimizedBlur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BlurRadius ("Blur Radius (px)", Range(0, 8)) = 0
        _BlurStrength ("Blur Strength", Range(0, 1)) = 1

        // SpriteRenderer / UI compatibility
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
            Tags { "LightMode"="UniversalForward" } // harmless in Built-in; helps SRP preview

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _RendererColor;
            float _BlurRadius;
            float _BlurStrength;

            sampler2D _AlphaTex;
            float _EnableExternalAlpha;

            inline fixed4 SampleSpriteTexture(float2 uv)
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
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                // Early out: cheaper than doing taps when radius ~0
                if (_BlurRadius <= 0.001 || _BlurStrength <= 0.001)
                {
                    fixed4 baseC = SampleSpriteTexture(IN.texcoord) * IN.color;
                    baseC.rgb *= baseC.a; // premultiply (Sprite/Default style)
                    return baseC;
                }

                float2 texel = _MainTex_TexelSize.xy;
                float r = _BlurRadius;

                // Fast 5-tap tent blur (center + 4 diagonals) using bilinear filtering.
                // Good quality/perf for UI sprites; keep radius small (0-4px typical).
                fixed4 c0 = SampleSpriteTexture(IN.texcoord);
                fixed4 c1 = SampleSpriteTexture(IN.texcoord + texel * float2( r,  r));
                fixed4 c2 = SampleSpriteTexture(IN.texcoord + texel * float2(-r,  r));
                fixed4 c3 = SampleSpriteTexture(IN.texcoord + texel * float2( r, -r));
                fixed4 c4 = SampleSpriteTexture(IN.texcoord + texel * float2(-r, -r));

                fixed4 blurred = (c0 * 0.4 + (c1 + c2 + c3 + c4) * 0.15);
                fixed4 baseC2 = c0;
                fixed4 mixC = lerp(baseC2, blurred, saturate(_BlurStrength));

                mixC *= IN.color;
                mixC.rgb *= mixC.a; // premultiply
                return mixC;
            }
            ENDCG
        }
    }
}


