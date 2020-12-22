Shader "HoloCAD/Direct" {
    Properties{
        _GridThickness("Grid Thickness", Float) = 0.01
        _GridSpacingY("Grid Spacing Y", Float) = 1.0
        [PerRendererData]_GridColor("Grid Color", Color) = (1, 1, 0, 1)
        [PerRendererData]_BaseColor("Base Color", Color) = (0.0, 0.0, 0.0, 0.0)
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
            #include "UnityCG.cginc"

            // Access Shaderlab properties
            uniform float _GridThickness;
            uniform float _GridSpacingY;
            uniform float _Length;
            uniform float4 _GridColor;
            uniform float4 _BaseColor;

            // Input into the vertex shader
            struct vertexInput {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // Output from vertex shader into fragment shader
            struct vertexOutput {
                float4 pos: SV_POSITION;
                float4 worldPos: TEXCOORD0;
                float3 localPos: POSITION1;
                float3 worldScale: SCALE;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // VERTEX SHADER
            vertexOutput vert(vertexInput input) {
                vertexOutput output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_OUTPUT(vertexOutput, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            
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
                if (frac(input.localPos.y * input.worldScale.y / input.worldScale.z) < _GridThickness ||
                    (input.localPos.x < _GridThickness && input.localPos.x > -_GridThickness) ||
                    (input.localPos.z < _GridThickness && input.localPos.z > -_GridThickness) ||
                    (input.localPos.y > 1 - _GridThickness / (input.worldScale.y / input.worldScale.z)))
                // if (frac(input.localPos.z * input.worldScale.z / input.worldScale.y) < _GridThickness ||
                //     (input.localPos.x < _GridThickness && input.localPos.x > -_GridThickness) ||
                //     (input.localPos.y < _GridThickness && input.localPos.y > -_GridThickness) ||
                //     (input.localPos.z > 1 - _GridThickness / (input.worldScale.z / input.worldScale.y)))
                {
                    return _GridColor;
                }
                return _BaseColor;
            }
            ENDCG
           }
        }
}
