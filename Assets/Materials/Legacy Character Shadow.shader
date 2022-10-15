Shader "Unlit/Character Shadow"
    {
    Properties
        {
        _Color ( "Tint", Color ) = ( 1, 1, 1, 1 )
        }
    SubShader
        {
        Tags { "Queue"="Transparent" }

        Pass
            {
            Stencil
                {
                Ref 4
                Comp NotEqual
                Pass Replace
                }

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 position : POSITION; };

            struct v2f { half4 position : SV_POSITION; };
            
            fixed4 _Color;

            v2f vert (appdata v)
                {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                return o;
                }

            fixed4 frag (v2f i) : SV_Target { return _Color; }
            ENDCG
            }
        }
    }
