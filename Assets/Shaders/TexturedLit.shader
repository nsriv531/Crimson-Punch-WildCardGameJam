Shader "Lowres/Textured Lit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _LightingHarshness ("Lighting Harshness", Float) = 3.0
        _LightingBoost ("Lighting Boost", Float) = 2.5
        _LightingCrunch ("Lighting Crunch", Float) = 8.0
        _Brightness ("Brightness", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lit

        sampler2D _MainTex;

        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed3, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        float _LightingHarshness;
        float _LightingBoost;
        float _LightingCrunch;
        float _Brightness;
        
        struct Input
        {
            float2 uv_MainTex;
        };

        half4 LightingLit(SurfaceOutput s, half3 lightDir, half atten)
        {
            float NdotL = dot(s.Normal, lightDir);
            half4 c;
            c.rgb = s.Albedo * floor(max(0, pow(NdotL * atten, _LightingHarshness) * _LightingBoost) * _LightingCrunch) / _LightingCrunch;
            c.a = s.Alpha;
            return c;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed3 c = tex2D (_MainTex, IN.uv_MainTex).rgb * UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * _Brightness;
            o.Albedo = c.rgb;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
