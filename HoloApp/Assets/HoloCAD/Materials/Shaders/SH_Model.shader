// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "HoloCAD/Model" {
	Properties{
        _MainTex ("Texture", 2D) = "white" {}
		_MaxDistance("Max Distance", Float) = 10
	}
	SubShader{
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			
		CGPROGRAM
		// Define the vertex and fragment shader functions
		#pragma vertex vert
		#pragma fragment frag
		
        #include "UnityCG.cginc"

		// Access Shaderlab properties
		uniform float _MaxDistance;
        sampler2D _MainTex;
        float4 _MainTex_ST;

		struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 worldPos : TEXCOORD1;
        };

		v2f vert(appdata_base v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            return o;
        }

		// FRAGMENT SHADER
		float4 frag(v2f i) : COLOR {
            fixed4 col = tex2D(_MainTex, i.uv);
		    float c = 1 - distance(i.worldPos, _WorldSpaceCameraPos) / _MaxDistance;
		    return col * c;
		}
		ENDCG
	   }
	}
}
