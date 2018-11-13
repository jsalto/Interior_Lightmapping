Shader "Custom/InvisibleShadowCaster" {
	SubShader {
		Tags { "Queue" = "Transparent" "LightMode" = "ShadowCaster" "RenderType"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
		UsePass "Standard/SHADOWCASTER"
	} 
	FallBack off
}
