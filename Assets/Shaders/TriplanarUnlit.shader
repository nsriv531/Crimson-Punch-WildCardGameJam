Shader "Lowres/Triplanar Unlit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Scale ("Scale", Vector) = (1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Unlit noforwardadd noambient

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half3 _Scale;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed3, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)

        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
        {
            return half4(s.Albedo, s.Alpha);
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
