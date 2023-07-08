// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Retro/Gradient Skybox"
{
	Properties
	{
        _HorizonColor("Horizon Color", Color) = (1, 1, 1, 1)
		_DarkColor("Dark Horizon Color", Color) = (1, 1, 1, 1)
		_Color0("Colour 1", Color) = (1, 1, 1, 1)
        _B0("Boundary 1", Float) = 0.25
		_Color1("Colour 2", Color) = (1, 1, 1, 1)
        _B1("Boundary 2", Float) = 0.50
		_Color2("Colour 3", Color) = (1, 1, 1, 1)
        _B2("Boundary 3", Float) = 0.75
        _Color3("Colour 4", Color) = (1, 1, 1, 1)
        _Intensity("Intensity", Float) = 1
		_XInfluence("Tilt", Float) = 0.25
		_Tint("Tint", Color) = (1, 1, 1, 1)
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata
	{
		float4 position : POSITION;
		float3 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 position : SV_POSITION;
		float3 texcoord : TEXCOORD0;
		float4 posWorld : TEXCOORD1;
	};

	half3 _Color0;
	half3 _Color1;
	half3 _Color2;
    half3 _Color3;
    half3 _HorizonColor;
	half3 _DarkColor;
	half3 _Tint;

    float _Intensity;
	float _XInfluence;
    float _B0;
    float _B1;
    float _B2;

	v2f vert(appdata v)
	{
		v2f o;
		o.position = UnityObjectToClipPos(v.position);
		o.texcoord = v.texcoord;
		o.posWorld = mul(unity_ObjectToWorld, v.position);
		return o;
	}

    float range(float val, float min, float max) {
        return (val - min) / (max - min);
    }

    float within(float val, float min, float max) {
        return min <= val && max >= val;
    }

	float3 frag(v2f i) : COLOR
	{
		float3 diff = normalize(i.posWorld.xyz - _WorldSpaceCameraPos.xyz);
		if (i.texcoord.y < 0) return lerp(_HorizonColor, _DarkColor, clamp((-normalize(diff).x + 1) / 2, 0, 1));;

        float y = i.texcoord.y - i.texcoord.x * _XInfluence + _XInfluence;

        float3 c = (0, 0, 0);
        c += _HorizonColor * (y < 0);
        c += lerp(_Color0, _Color1, range(y, 0, _B0)) * within(y, 0, _B0);
        c += lerp(_Color1, _Color2, range(y, _B0, _B1)) * within(y, _B0, _B1);
        c += lerp(_Color2, _Color3, range(y, _B1, _B2)) * within(y, _B1, _B2);
        c += _Color3 * (y > _B2);
        //c += lerp(_Color3, _Color3, range(y, _B2, 1)) * step(y, _B2);
        return c * _Tint * _Intensity;

        /*float3 c = lerp(_Color0, _Color1, i.texcoord.y / _Middle) * step(i.texcoord.y, _Middle);
        c += lerp(_Color1, _Color2, (i.texcoord.y - _Middle) / (1 - _Middle)) * step(_Middle, i.texcoord.y);
        return c * _Intensity;*/
	}

	ENDCG

	SubShader
	{
		Tags { "RenderType" = "Background" "Queue" = "Background" }
		Pass
		{
			ZWrite Off
			Cull Off
			Fog { Mode Off }
			CGPROGRAM
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma vertex vert
				#pragma fragment frag
			ENDCG
		}
	}
	CustomEditor "HorizonWithSunSkyboxInspector"
}