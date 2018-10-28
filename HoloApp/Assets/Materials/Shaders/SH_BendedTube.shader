Shader "Tubes/Bended" {
	Properties{
		_GridThickness("Grid Thickness", Float) = 0.01
		_RadialSpacing("Radial Spacing", Float) = 5
		_GridColor("Grid Color", Color) = (0.5, 1.0, 1.0, 1.0)
		_BaseColor("Base Color", Color) = (0.0, 0.0, 0.0, 0.0)
		_Diameter("Diameter", Float) = 0.426
		_BendRadius("Bend Radius", Float) = 1.2
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
		#define PI 3.14159265

		// Access Shaderlab properties
		uniform float _GridThickness;
		uniform float _RadialSpacing;
		uniform float _Diameter;
		uniform float _BendRadius;
		uniform float4 _GridColor;
		uniform float4 _BaseColor;

		// Input into the vertex shader
		struct vertexInput {
			float4 vertex : POSITION;
		};

		// Output from vertex shader into fragment shader
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 worldPos : TEXCOORD0;
			float3 localPos: POSITION1;
			float3 worldScale: SCALE;
		};

		// VERTEX SHADER
		vertexOutput vert(vertexInput input) {
			vertexOutput output;
			output.pos = UnityObjectToClipPos(input.vertex);
			output.worldPos = mul(unity_ObjectToWorld, input.vertex);
			output.localPos = input.vertex.xyz;
			output.worldScale = float3(
				length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
				length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
				length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
			);
			return output;
		}

		// FRAGMENT SHADER
		float4 frag(vertexOutput input) : COLOR {
			float spacing = _RadialSpacing / 180 * PI;
			float lineThickness = _GridThickness * _Diameter;
			float radialPos = frac(atan2(input.localPos.z, input.localPos.x) / spacing);
			float circle = pow(input.localPos.x, 2) + pow(input.localPos.z, 2) - pow(_BendRadius, 2);
			if (input.localPos.y < lineThickness && input.localPos.y > -lineThickness ||
				radialPos > (1 - _GridThickness / 2) ||	radialPos < (_GridThickness / 2) ||
				input.localPos.z < lineThickness ||
				circle < lineThickness * 2 && circle > -lineThickness * 2)
			{
				return _GridColor;
			}
			return _BaseColor;
		}
		ENDCG
	   }
	}
}
