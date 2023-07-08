Shader "Lowres/Textured Lit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _LightingHarshness ("Lighting Harshness", Float) = 3.0
        _LightingBoost ("Lighting Boost", Float) = 2.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lit

        sampler2D _MainTex;
        
        fixed4 _Color;
        float _LightingHarshness;
        float _LightingBoost;
        
        struct Input
        {
            float2 uv_MainTex;
        };

        half4 LightingLit(SurfaceOutput s, half3 lightDir, half atten)
        {
            float NdotL = dot(s.Normal, lightDir);
            half4 c;
            c.rgb = s.Albedo * max(0, pow(NdotL, _LightingHarshness) * _LightingBoost);
            c.a = s.Alpha;
            return c;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
