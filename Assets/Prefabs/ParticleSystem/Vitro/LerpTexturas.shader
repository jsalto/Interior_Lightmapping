Shader "LerpTexturas"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_Metallic("Metallic", Range(0,1)) = 0.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
	_BlendTex("Blend (RGB)", 2D) = "white"
		_BlendAlpha("Blend Alpha", float) = 0

	}
		SubShader
	{
		Tags{ "Queue" = "Geometry-9" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Lighting Off
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma surface surf Standard

		fixed4 _Color;
	sampler2D _MainTex;
	sampler2D _BlendTex;
	half _Glossiness;
	half _Metallic;
	float _BlendAlpha;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed4 c = ((1 - _BlendAlpha) * tex2D(_MainTex, IN.uv_MainTex) + _BlendAlpha * tex2D(_BlendTex, IN.uv_MainTex)) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
	}
	ENDCG
	}

		Fallback "Transparent/VertexLit"
}