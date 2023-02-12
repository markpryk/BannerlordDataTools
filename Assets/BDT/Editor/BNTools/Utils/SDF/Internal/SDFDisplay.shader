Shader "Sprites/SDFDisplay" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}

        [HDR]_FaceColor("Face Color", Color) = (1,1,1,1)
        _FaceDilate("Face Dilate", Range(-1,1)) = 0

        [Header(Outline)]
        [Toggle(OUTLINE_ON)] _EnableOutline("Enable Outline", Float) = 0
        [HDR]_OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Thickness", Range(0,1)) = 0
        _OutlineSoftness("Outline Softness", Range(0,1)) = 0

        [Header(Underlay)]
        [KeywordEnum(Off, On)] UNDERLAY("Underlay", Float) = 0
        [HDR]_UnderlayColor("Border Color", Color) = (0,0,0,.5)
        _UnderlayOffsetX("Border OffsetX", Range(-1,1)) = 0
        _UnderlayOffsetY("Border OffsetY", Range(-1,1)) = 0
        _UnderlayDilate("Border Dilate", Range(-1,1)) = 0
        _UnderlaySoftness("Border Softness", Range(0,1)) = 0

        [Header(Other)]
        _GradientScale("Gradient Scale", float) = 20
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass {

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #pragma shader_feature __ OUTLINE_ON
            #pragma shader_feature UNDERLAY_OFF UNDERLAY_ON

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            half4 _FaceColor;
            fixed _FaceDilate;
            half _GradientScale;

#if defined(OUTLINE_ON)
            fixed _OutlineSoftness;
            fixed _OutlineWidth;
            half4 _OutlineColor;
#endif
#if !defined(UNDERLAY_OFF)
            fixed _UnderlayOffsetX;
            fixed _UnderlayOffsetY;
            fixed _UnderlayDilate;
            fixed _UnderlaySoftness;
            half4 _UnderlayColor;
#endif

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target{
                half scale = 1.0 / (_GradientScale * fwidth(i.uv));
                half bias = 0.5 - _FaceDilate / 2;

                // Compute density value
                fixed d = tex2D(_MainTex, i.uv).a;

                // Compute result color
                half4 c = _FaceColor * saturate((d - bias) * scale + 0.5);

                // Append outline
#ifdef OUTLINE_ON
                if (_OutlineWidth > 0) {
                    half outlineFade = max(_OutlineSoftness, fwidth(i.uv * _GradientScale));
                    half ol_from = min(1, bias + _OutlineWidth / 2 + outlineFade / 2);
                    half ol_to = max(0, bias - _OutlineWidth / 2 - outlineFade / 2);
                    c = lerp(_FaceColor, _OutlineColor, saturate((ol_from - d) / outlineFade));
                    c *= saturate((d - ol_to) / outlineFade);
                }
#endif

                // Append underlay (drop shadow)
#if defined(UNDERLAY_ON)
                {
                    half ul_from = max(0, bias - _UnderlayDilate - _UnderlaySoftness / 2);
                    half ul_to = min(1, bias - _UnderlayDilate + _UnderlaySoftness / 2);
                    float2 underlayUV = i.uv - float2(_UnderlayOffsetX, _UnderlayOffsetY);
                    d = tex2D(_MainTex, underlayUV).a;
#if defined(UNDERLAY_ON)
                    c += float4(_UnderlayColor.rgb, 1) * (_UnderlayColor.a * (1 - c.a)) *
                        saturate((d - ul_from) / (ul_to - ul_from));
#endif
                }
#endif

                return c * i.color;
            }
            ENDCG
        }
    }
}
