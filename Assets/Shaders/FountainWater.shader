Shader "Lowres/Fountain Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color0 ("Color0", Color) = (1,1,1,1)
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Brightness ("Brightness", Float) = 1
        _Speed ("Speed", Float) = 1
        _Tiling ("Tiling", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float3 normal : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            fixed4 _Color0;
            fixed4 _Color1;
            fixed _Brightness;
            fixed _Speed;
            fixed _Tiling;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float normalMult = dot(i.normal, float3(0, 1, 0));
                // sample the texture 
                float angle01 = atan2(i.uv.y - 0.5, i.uv.x - 0.5) / 6.28318530718;
                float dist01 = length(i.uv - 0.5) * 2.0;
                float h = tex2D(_MainTex, float2(angle01, (dist01 + _Time.x * _Speed * (0.5 + (1.0 - normalMult) * 0.5)) * _Tiling)).r;
                float4 col = lerp(_Color0, _Color1, h);
                col.rgb *= _Brightness;
                col.a *= 0.5 + normalMult * 0.5;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col.rgb);
                return col;
                // return float4(i.normal.x, i.normal.y, i.normal.z, 1);
            }
            ENDCG
        }
    }
}
