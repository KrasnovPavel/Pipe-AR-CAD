using System;
using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using UnityEngine;
using HoloCAD.UI;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, реализующий участок погиба трубы. </summary>
    public class BendedTubeFragment : TubeFragment
    {
        private List<Mesh> _meshes;
        private float _radius;
    
        private bool _useSecondRadius;
        private int _angle = MeshFactory.DeltaAngle;
        private static readonly int ShaderDiameter = Shader.PropertyToID("_Diameter");
        private static readonly int ShaderBendRadius = Shader.PropertyToID("_BendRadius");

        /// <summary> Угол погиба. </summary>
        public int Angle
        {
            get { return _angle; }
            set
            {
                if (value < MeshFactory.DeltaAngle || value > 180)
                {
                    return;
                }
                _angle = value;
    
                SetMesh();
                Label.GetComponent<TextMesh>().text = $"Угол погиба: {_angle}°";
            }
        }
    
        /// <summary> Флаг, указывающий какой из двух радиусов погиба используется. <c>true</c> если второй.  </summary>
        public bool UseSecondRadius
        {
            get { return _useSecondRadius; }
            set
            {
                _useSecondRadius = value;
                Radius = _useSecondRadius ? Owner.Data.second_radius : Owner.Data.first_radius;
                SetMesh();
            }
        }

        /// <summary> Используемый радиус погиба. </summary>
        public float Radius
        {
            get { return _radius; }
            protected set
            {
                _radius = value;
                
                Tube.GetComponent<MeshRenderer>().material.SetFloat(ShaderBendRadius, Radius);
            }
        }

        /// <inheritdoc />
        public override float Diameter
        {
            get { return _diameter; }
            set
            {
                if (Math.Abs(_diameter - value) < float.Epsilon) return;

                _diameter = value;
                _meshes = MeshFactory.CreateMeshes(Owner.Data);
                Radius = _useSecondRadius ? Owner.Data.second_radius : Owner.Data.first_radius;
                Tube.GetComponent<MeshRenderer>().material.SetFloat(ShaderDiameter, Diameter);
                ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Diameter;
                SetMesh();
            }
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            _meshes = MeshFactory.CreateMeshes(Owner.Data);
            UseSecondRadius = false;
            Angle = 90;
            TubeManager.SelectTubeFragment(this);
        }

        /// <inheritdoc />
        protected override void InputDown(GameObject obj, InputEventData eventData)
        {
            base.InputDown(obj, eventData);

            switch (obj.name)
            {
                case "IncreaseAngleButton":
                    Angle += MeshFactory.DeltaAngle;
                    break;
                case "DecreaseAngleButton":
                    Angle -= MeshFactory.DeltaAngle;
                    break;
                case "ClockwiseButton":
                    transform.localRotation *= Quaternion.Euler(0, 0, MeshFactory.DeltaAngle);
                    break;
                case "AnticlockwiseButton":
                    transform.localRotation *= Quaternion.Euler(0, 0, -MeshFactory.DeltaAngle);
                    break;
                case "ChangeRadiusButton":
                    UseSecondRadius = !UseSecondRadius;
                    break;
            }
        }
    
        /// <summary> Отображает соответствующий меш. </summary>
        private void SetMesh()
        {
            Tube.transform.localPosition = new Vector3(-Radius, 0, 0);
    
            Quaternion rot = Quaternion.Euler(0, -Angle, 0);
            Vector3 pos = new Vector3(Radius, 0, 0);
            EndPoint.transform.localPosition = rot * pos - pos;
            EndPoint.transform.localRotation = rot;
            const int numberOfAngles = 180 / MeshFactory.DeltaAngle;
            Tube.GetComponent<MeshFilter>().mesh = _meshes[Angle / MeshFactory.DeltaAngle - 1 + (UseSecondRadius ? numberOfAngles : 0)];
            Tube.GetComponent<MeshCollider>().sharedMesh = Tube.GetComponent<MeshFilter>().mesh;
        }
    }
}
