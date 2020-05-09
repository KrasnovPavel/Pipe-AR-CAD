Shader "Custom/SH_Button3D" {
	Properties {
		_MainColor ("MainColor", Color) = (0.5, 0.5, 0.5, 0.5)
		_HighLightColor ("HighLightColor" , Color) = (1, 1, 1, 1)
		[PerRendererData]_Fade("Fade", float) = 1
		[PerRendererData]_Alpha("Alpha", float) = 1
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
		uniform float  _Alpha;
		
		// Input into the vertex shader
		struct vertexInput {
			float4 vertex: POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
		};
		
		// Output from vertex shader into fragment shader
		struct vertexOutput {
			float4 pos: SV_POSITION;
            float3 localPos : TEXCOORD;
            UNITY_VERTEX_OUTPUT_STEREO
		};

		// VERTEX SHADER
		vertexOutput vert(appdata_full input) {
			vertexOutput output;
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_INITIALIZE_OUTPUT(vertexOutput, output);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
            output.pos = UnityObjectToClipPos (input.vertex);
			output.localPos = input.vertex;
			return output;
		}
		
		float4 frag(vertexOutput input) : COLOR {
		    float I = pow(abs(input.localPos.y), 4) + pow(abs(input.localPos.z), 4);
            return (_MainColor + _HighLightColor * I / _Fade) * _Alpha;
		}
	    ENDCG
	    }
	}
}
