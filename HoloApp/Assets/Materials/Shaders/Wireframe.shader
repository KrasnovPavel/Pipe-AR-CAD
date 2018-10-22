// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Grid" {
	Properties{
	  _GridThickness("Grid Thickness", Float) = 0.01
	  _Length("Length", Float) = -1
	  _Offset("Offset", Float) = 0.025
	  _GridSpacingXZ("Grid Spacing XZ", Float) = 1.0
	  _GridSpacingY("Grid Spacing Y", Float) = 1.0
	  _GridColour("Grid Colour", Color) = (0.5, 1.0, 1.0, 1.0)
	  _BaseColour("Base Colour", Color) = (0.0, 0.0, 0.0, 0.0)
	}
	SubShader{
		Tags { "Queue" = "Transparent" }
		Pass {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			// Define the vertex and fragment shader functions
			#pragma vertex vert
			#pragma fragment frag

			// Access Shaderlab properties
			uniform float _GridThickness;
			uniform float _GridSpacingY;
			uniform float _Offset;
			uniform float _Length;
			uniform float4 _GridColour;
			uniform float4 _BaseColour;

			// Input into the vertex shader
			struct vertexInput {
				float4 vertex : POSITION;
			};

			// Output from vertex shader into fragment shader
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
				float3 localPos: POSITION1;
			};

			// VERTEX SHADER
			vertexOutput vert(vertexInput input) {
				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.worldPos = mul(unity_ObjectToWorld, input.vertex);
				output.localPos = input.vertex.xyz;
				return output;
			}

			// FRAGMENT SHADER
			float4 frag(vertexOutput input) : COLOR {
				if (frac(input.worldPos.y / _GridSpacingY) < _GridThickness * 4 ||
					(input.localPos.x < _GridThickness && input.localPos.x > -_GridThickness) ||
					(input.localPos.z < _GridThickness && input.localPos.z > -_GridThickness) ||
					(input.localPos.y < 0.003f) ||
					(_Length > 0 && input.localPos.y > _Length - 0.003f))
				{
					return _GridColour;
				}
				return _BaseColour;
			}
			ENDCG
		   }
		}
}
