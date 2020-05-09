// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "HoloCAD/Bended" {
    Properties{
        _GridThickness("Grid Thickness", Float) = 0.01
        _RadialSpacing("Radial Spacing", Float) = 5
        [PerRendererData]_GridColor("Grid Color", Color) = (1, 1, 0, 1)
        [PerRendererData]_BaseColor("Base Color", Color) = (0.0, 0.0, 0.0, 0.0)
        [PerRendererData]_Diameter("Diameter", Float) = 1
        [PerRendererData]_BendRadius("Bend Radius", Float) = 1.2
        [PerRendererData]_Angle("Angle", Float) = 3.14159265
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
        #include "UnityCG.cginc"

        // Access Shaderlab properties
        uniform float _GridThickness;
        uniform float _RadialSpacing;
        uniform float _Diameter;
        uniform float _Angle;
        uniform float _BendRadius;
        uniform float4 _GridColor;
        uniform float4 _BaseColor;

        // Input into the vertex shader
        struct vertexInput {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        // Output from vertex shader into fragment shader
        struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 worldPos : TEXCOORD0;
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
            output.localPos = float3(input.vertex.x + _BendRadius, input.vertex.yz) / _Diameter;
            output.worldScale = float3(
                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)) / _Diameter, // scale x axis
                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)) / _Diameter, // scale y axis
                length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z)) / _Diameter // scale z axis
            );
            return output;
        }

        // FRAGMENT SHADER
        float4 frag(vertexOutput input) : COLOR {
            float spacing = _RadialSpacing / 180 * PI;
            float pointAngle = atan2(input.localPos.z, input.localPos.x);
            float radialPos = frac(pointAngle / spacing);
            
            if (pointAngle > _Angle || pointAngle <= 0)
            {
                return float4(0.0, 0.0, 0.0, 0.0);
            }
            
            bool isMiddleLine = input.localPos.y < _GridThickness && input.localPos.y > -_GridThickness;
            bool isRing = radialPos > (1 - _GridThickness / 2) || radialPos < (_GridThickness / 2);
            bool isStartRing = input.localPos.z < _GridThickness;
            bool isEndRing = atan2(input.localPos.z, input.localPos.x) > _Angle - _GridThickness;
            if (isMiddleLine || isRing || isStartRing || isEndRing) 
            {
                return _GridColor;
            }
            return _BaseColor;
        }
        ENDCG
       }
    }
}
