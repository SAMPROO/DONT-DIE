Shader "Custom/CharacterShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
     	_ColorMask ("Color Mask", 2D) = "black" {}
     	_PatternMask ("Pattern Mask", 2D) = "black" {}
     	_PatternColor ("Pattern Color", color) = (0,0,0,0)
     	_PatternGlossiness("Pattern Smoothness", Range(0,1)) = 0.5


        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ColorMask;
        sampler2D _PatternMask;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _PatternGlossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _PatternColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
        	float useColor = tex2D(_ColorMask, IN.uv_MainTex).r;
        	float usePattern = tex2D(_PatternMask, IN.uv_MainTex).r;
        	fixed4 color = lerp(_Color, _PatternColor, usePattern);

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = lerp(color, c, useColor);
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = lerp(_Glossiness, _PatternGlossiness, usePattern);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
