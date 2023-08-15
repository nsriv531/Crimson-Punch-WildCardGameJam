Shader "Lowres/Triplanar Lit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Scale ("Scale", Vector) = (1,1,1)
        _LightingHarshness ("Lighting Harshness", Float) = 3.0
        _LightingBoost ("Lighting Boost", Float) = 2.5
        _LightingCrunch ("Lighting Crunch", Float) = 8.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lit

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half3 _Scale;
        half _LightingHarshness;
        half _LightingBoost;
        half _LightingCrunch;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed3, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)

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
            fixed3 c = tex2D (_MainTex, IN.worldPos.xz * _Scale.xz - IN.worldPos.y * _Scale.y).rgb * _Color;
            
            o.Albedo = c.rgb;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
