Shader "UI/GrabPassBackgroundBlur"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _BlurRadiusPx("Blur Radius (px)", Range(0, 20)) = 6
        _BlurStrength("Blur Strength", Range(0, 3)) = 1
        _BlurTint("Blur Tint", Color) = (1,1,1,1)
        _Opacity("Opacity", Range(0, 1)) = 1

        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
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

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        GrabPass
        {
            "_UIGrabTexture"
        }

        Pass
        {
            Name "UIGrabPassBackgroundBlur"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 uv            : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 grabPos       : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            half4 _ClipRect;

            sampler2D _UIGrabTexture;

            float _BlurRadiusPx;
            float _BlurStrength;
            fixed4 _BlurTint;
            float _Opacity;

            sampler2D _AlphaTex;
            float _EnableExternalAlpha;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);

                #ifdef UNITY_HALF_TEXEL_OFFSET
                    OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1, 1);
                #endif

                OUT.uv = IN.texcoord;
                OUT.color = IN.color * _Color;
                OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);
                return OUT;
            }

            fixed3 SampleGrab(float2 uv)
            {
                return tex2D(_UIGrabTexture, uv).rgb;
            }

            fixed3 Blur9(float2 uv, float2 px)
            {
                fixed3 c0 = SampleGrab(uv);

                fixed3 cAxis =
                    SampleGrab(uv + px * float2( 1,  0)) +
                    SampleGrab(uv + px * float2(-1,  0)) +
                    SampleGrab(uv + px * float2( 0,  1)) +
                    SampleGrab(uv + px * float2( 0, -1));

                fixed3 cDiag =
                    SampleGrab(uv + px * float2( 1,  1)) +
                    SampleGrab(uv + px * float2(-1,  1)) +
                    SampleGrab(uv + px * float2( 1, -1)) +
                    SampleGrab(uv + px * float2(-1, -1));

                return c0 * 0.20 + cAxis * 0.125 + cDiag * 0.075;
            }

            inline fixed4 SampleUIMask(float2 uv)
            {
                fixed4 c = tex2D(_MainTex, uv);
                #if ETC1_EXTERNAL_ALPHA
                    fixed4 a = tex2D(_AlphaTex, uv);
                    c.a = lerp(c.a, a.r, _EnableExternalAlpha);
                #endif
                return c;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 mask = (SampleUIMask(IN.uv) + _TextureSampleAdd) * IN.color;
                fixed alpha = mask.a * saturate(_Opacity);

                #ifdef UNITY_UI_CLIP_RECT
                    alpha *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip(alpha - 0.001);
                #endif

                if (alpha <= 0.001)
                    return 0;

                float2 screenUV = IN.grabPos.xy / IN.grabPos.w;

                float strength = max(0.0, _BlurStrength);

                float2 px = (_BlurRadiusPx / _ScreenParams.xy);

                fixed3 c0 = SampleGrab(screenUV);

                fixed3 b1 = Blur9(screenUV, px);

                fixed3 b2 = Blur9(screenUV, px * 2.0);
                fixed3 b3 = Blur9(screenUV, px * 3.0);

                fixed amount = saturate(strength);
                fixed t2 = saturate(strength - 1.0);
                fixed t3 = saturate(strength - 2.0);

                fixed3 blurred = lerp(b1, b2, t2);
                blurred = lerp(blurred, b3, t3);

                fixed3 mixed = lerp(c0, blurred, amount);

                mixed *= _BlurTint.rgb;

                return fixed4(mixed, alpha * _BlurTint.a);
            }
            ENDCG
        }
    }
}


