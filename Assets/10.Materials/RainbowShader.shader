Shader "Custom/RainbowShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        float _Alpha;

        struct Input
        {
            float2 uv_MainTex;
        };

        half4 ColorFunction(float2 uv)
        {
            float time = _Time.y * 2; // 시간을 기반으로 애니메이션
            float3 rainbow = float3(
                smoothstep(-1.0, 1.0, sin(uv.x + time)),
                smoothstep(-1.0, 1.0, sin(uv.x + time + 2.0 / 3.0 * 3.14159265)),
                smoothstep(-1.0, 1.0, sin(uv.x + time + 4.0 / 3.0 * 3.14159265))
            );
            return half4(rainbow * 0.5 + 0.5, 1.0);
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            half4 color = ColorFunction(IN.uv_MainTex);
            c.rgb *= color.rgb; // 색상 값만 곱함
            o.Albedo = c.rgb;
            o.Alpha = c.a * _Alpha; // 알파 값 적용
        }
        ENDCG
    }
    FallBack "Diffuse"
}
