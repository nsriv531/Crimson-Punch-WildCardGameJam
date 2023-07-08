Shader "Lowres/Screen Shader"
{
    Properties
    {
        _CamTex ("Camera Texture", 2D) = "white" {}
        _UITex ("UI Texture", 2D) = "white" {}
        _ColourGrading("Colour Grading", Float) = 16.0
        _DitherTex("Dither", 2D) = "white" {}
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _Additive("Additive Colour", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _CamTex;
            float4 _CamTex_ST;
            float4 _CamTex_TexelSize;

            sampler2D _UITex;
            float4 _UITex_ST;

            sampler2D _DitherTex;
            float4 _DitherTex_TexelSize;

            fixed _ColourGrading;
            fixed3 _Additive;
            fixed3 _Tint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _CamTex);
                return o;
            }

            float lerp_inv(float a, float b, float v) {
                return (v - a) / (b - a);
            }

            float brightness(float3 col) {
                return (col.r + col.g + col.b) / 3.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 col = tex2D(_CamTex, i.uv);

                // col.r = 1.0;

                float dither_threshold = tex2D(_DitherTex, float2(i.uv.x * (_CamTex_TexelSize.z / _DitherTex_TexelSize.z), i.uv.y * (_CamTex_TexelSize.w / _DitherTex_TexelSize.w))).r;

                float l = brightness(col);

                float above = ceil(l * _ColourGrading) / _ColourGrading;
                float below = floor(l * _ColourGrading) / _ColourGrading;

                float dither = lerp_inv(below, above, l);

                col = lerp(col * (1 + below - l), col * (1 + above - l), step(dither_threshold, dither));
                // col = lerp(fixed3(0,0,0), fixed3(1,1,1), step(dither_threshold, l));


                fixed4 ui_col = tex2D(_UITex, i.uv);
                //col = ui_col.a > 0.5? ui_col.rgb : col;//* ui_col + (1 - ui_col.a) * col;
                col = ui_col.a * ui_col + (1 - ui_col.a) * col;
                col.rgb *= _Tint;
                col.rgb += _Additive;

                return fixed4(col.r, col.g, col.b, 1.0);
            }
            ENDCG
        }
    }
}
