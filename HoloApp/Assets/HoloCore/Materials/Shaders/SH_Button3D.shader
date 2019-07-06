Shader "Custom/SH_Button3D" {
	Properties {
		_MainColor ("MainColor", Color) = (0.5, 0.5, 0.5, 0.5)
		_HighLightColor ("HighLightColor" , Color) = (1, 1, 1, 1)
		[PerRendererData]_Fade("Fade", float) = 1
	}
	SubShader {
	    Tags { "Queue" = "Transparent" }
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha

	    CGPROGRAM	
	    
		#pragma vertex vert
		#pragma fragment frag
		
        #include "UnityCG.cginc"
		
		uniform float4 _MainColor;
		uniform float4 _HighLightColor;
		uniform float  _Fade;
		
		// Input into the vertex shader
		struct vertexInput {
			float4 vertex: POSITION;
		};
		
		// Output from vertex shader into fragment shader
		struct vertexOutput {
			float4 pos: SV_POSITION;
            float3 localPos : TEXCOORD;
		};

		// VERTEX SHADER
		vertexOutput vert(appdata_full input) {
			vertexOutput output;
            output.pos = UnityObjectToClipPos (input.vertex);
			output.localPos = input.vertex;
			return output;
		}
		
		float4 frag(vertexOutput input) : COLOR {
		    float I = pow(abs(input.localPos.y*2), 4) + pow(abs(input.localPos.z*2), 4);
            return _MainColor + _HighLightColor * I / _Fade;
		}
	    ENDCG
	    }
	}
}
