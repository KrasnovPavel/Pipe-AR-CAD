using System;
using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using UnityEngine;
using HoloCAD.UI;
using HoloToolkit.Unity;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, реализующий участок погиба трубы. </summary>
    public class BendedTubeFragment : TubeFragment
    {
        private List<Mesh> _meshes;
        private float _radius;
        private List<GameObject> _colliders = new List<GameObject>();
    
        private bool _useSecondRadius;
        private int _angle = MeshFactory.DeltaAngle;
        private static readonly int ShaderDiameter = Shader.PropertyToID("_Diameter");
        private static readonly int ShaderBendRadius = Shader.PropertyToID("_BendRadius");

        /// <summary> Prefab коллайдера сегмента погиба. </summary>
        [Tooltip("Prefab коллайдера сегмента погиба.")]
        public GameObject ColliderPrefab;
        
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
                
                SetColliders(value, _angle);
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
            SetColliders(MeshFactory.DeltaAngle, 0);
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
            Quaternion rot = Quaternion.Euler(0, -Angle, 0);
            Vector3 pos = new Vector3(Radius, 0, 0);
            EndPoint.transform.localPosition = rot * pos - pos;
            EndPoint.transform.localRotation = rot;
            const int numberOfAngles = 180 / MeshFactory.DeltaAngle;
            Tube.GetComponent<MeshFilter>().mesh = _meshes[Angle / MeshFactory.DeltaAngle - 1 + (UseSecondRadius ? numberOfAngles : 0)];
            Tube.GetComponent<MeshCollider>().sharedMesh = Tube.GetComponent<MeshFilter>().mesh;
            
            for (int i = 0; i < _colliders.Count; i++)
            {
                _colliders[i].GetComponent<MeshCollider>().sharedMesh = _meshes[UseSecondRadius ? numberOfAngles : 0];
                
                float shiftAngle = (2 * i + 1) / 2f * MeshFactory.DeltaAngle;
                Vector3 shiftVector = Vector3.zero;
                shiftVector = shiftVector.RotateAround(new Vector3(-Radius, 0f, 0f), 
                                                       Quaternion.Euler(0, -shiftAngle, 0));
                _colliders[i].GetComponent<MeshCollider>().transform.localPosition = shiftVector;
            }
        }

        /// <summary> Создает или удаляет необходимое число коллайдеров. </summary>
        /// <param name="newAngle"> Новый угол поворота. </param>
        /// <param name="oldAngle"> Старый угол поворота. </param>
        private void SetColliders(int newAngle, int oldAngle)
        {
            int newPos = newAngle / MeshFactory.DeltaAngle;
            int oldPos = oldAngle / MeshFactory.DeltaAngle;

            if (newPos < oldPos)
            {
                for (int i = oldPos - 1; i >= newPos; i--)
                {
                    Destroy(_colliders[i]);
                    _colliders.RemoveAt(_colliders.Count - 1);
                }
            }
            else
            {
                for (int i = oldPos + 1; i <= newPos; i++)
                {
                    GameObject newCollider = Instantiate(ColliderPrefab, Tube.transform);
                    newCollider.GetComponent<TubeFragmentCollider>().Owner = this;
                    MeshCollider meshCollider = newCollider.GetComponent<MeshCollider>();
                    meshCollider.convex = true;
                    meshCollider.inflateMesh = true;
                    meshCollider.skinWidth = 0.0000001f;
                    meshCollider.isTrigger = true;
                    newCollider.transform.Rotate(new Vector3(0f, -(i-1) * MeshFactory.DeltaAngle, 0f));
                    _colliders.Add(newCollider);
                }
            }
        }
    }
}
