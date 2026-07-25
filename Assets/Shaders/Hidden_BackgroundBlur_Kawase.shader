Shader "Hidden/BackgroundBlur/Kawase"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Offset; // xy are uv offsets

            fixed4 frag(v2f_img i) : SV_Target
            {
                // 4-tap Kawase blur using diagonal samples.
                float2 o = _Offset.xy;
                fixed4 c1 = tex2D(_MainTex, i.uv + float2( o.x,  o.y));
                fixed4 c2 = tex2D(_MainTex, i.uv + float2(-o.x,  o.y));
                fixed4 c3 = tex2D(_MainTex, i.uv + float2( o.x, -o.y));
                fixed4 c4 = tex2D(_MainTex, i.uv + float2(-o.x, -o.y));
                return (c1 + c2 + c3 + c4) * 0.25;
            }
            ENDCG
        }
    }
    Fallback Off
}


