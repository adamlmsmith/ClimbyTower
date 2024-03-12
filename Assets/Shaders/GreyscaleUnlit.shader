Shader "Custom/Grayscale" 
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _EffectAmount ("Effect Amount", Range (0, 1)) = 1.0
        _Color ("Main Color", Color) = (1,1,1,1)
    }

    Category
    {
        ZWrite On
        Alphatest Greater 0.5
        Cull Off

        SubShader
        {
            Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
            LOD 200
  
            CGPROGRAM
            #pragma surface surf Lambert alpha
  
            sampler2D _MainTex;
            uniform float _EffectAmount;
            fixed4 _Color;
              
            struct Input
            {
                float2 uv_MainTex;
            };
  
            void surf (Input IN, inout SurfaceOutput o)
            {
                half4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = lerp(c.rgb, dot(c.rgb, _Color.rgb), _EffectAmount);
                o.Alpha = c.a;
            }
  
            ENDCG
        }
    }

    Fallback "Unlit/Transparent"
}