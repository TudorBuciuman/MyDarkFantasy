Shader"CloudShader" {
    Properties {
        _Color("Main Color", Color) = (1,1,1,0.3)
        _MainTex("Texture", 2D) = "white" {}
    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

ZWrite Off

Lighting Off
        Fog {
Mode Off}

Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            Stencil {
                Ref 1
                Comp Greater
                Pass IncrSat
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 pos : SV_POSITION;
};

sampler2D _MainTex;
float4 _Color;

v2f vert(appdata_t v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    fixed4 texColor = tex2D(_MainTex, i.uv);
    return texColor * _Color;
}
            ENDCG
        }
    }
}
