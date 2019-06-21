// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HoloToolkit.Unity;
using JetBrains.Annotations;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, реализующий участок погиба трубы. </summary>
    public class BendedTubeFragment : TubeFragment
    {
        /// <summary> Prefab коллайдера сегмента погиба. </summary>
        [Tooltip("Prefab коллайдера сегмента погиба.")] 
        [CanBeNull] public GameObject ColliderPrefab;

        /// <summary> Материал цвета трубы по умолчанию. </summary>
        [Tooltip("Материал цвета трубы по умолчанию.")] 
        public Material DefaultTubeMaterial;
        
        /// <summary> Материал цвета выбранной трубы. </summary>
        [Tooltip("Материал цвета выбранной трубы.")] 
        public Material SelectedTubeMaterial;
        
        /// <summary> Материал цвета пересекающейся трубы. </summary>
        [Tooltip("Материал цвета пересекающейся трубы.")] 
        public Material CollidingTubeMaterial;
        
        /// <summary> Угол погиба. </summary>
        public float Angle
        {
            get => _angle;
            set
            {
                if (value < MeshFactory.DeltaAngle || value > 180)
                {
                    return;
                }
                
                SetColliders(_angle);
                _angle = value;
                Tube.GetComponent<MeshRenderer>().material.SetFloat(ShaderAngle, _angle / 180f * (float)Math.PI);
                SetEndpoint();
            }
        }
    
        /// <summary> Флаг, указывающий какой из двух радиусов погиба используется. <c>true</c> если второй.  </summary>
        public bool UseSecondRadius
        {
            get => _useSecondRadius;
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
            get => _radius;
            protected set
            {
                _radius = value;
                
                Tube.GetComponent<MeshRenderer>().material.SetFloat(ShaderBendRadius, Radius);
                SetEndpoint();
            }
        }

        /// <inheritdoc />
        public override float Diameter
        {
            get => base.Diameter;
            set
            {
                if (Math.Abs(base.Diameter - value) < float.Epsilon) return;

                base.Diameter = value;
                _meshes = MeshFactory.GetMeshes(Owner.Data);
                Radius = _useSecondRadius ? Owner.Data.second_radius : Owner.Data.first_radius;
                Tube.GetComponent<MeshRenderer>().material.SetFloat(ShaderDiameter, Diameter);
                SetMesh();
                SetEndpoint();
            }
        }

        /// <inheritdoc />
        public override bool IsPlacing
        {
            get => base.IsPlacing;
            set
            {
                base.IsPlacing = value;

                Tube.GetComponent<MeshCollider>().enabled = !IsPlacing;
                
                if (IsPlacing)
                {
                    foreach (GameObject col in _colliders)
                    {
                        col.SetActive(false);
                    }
                }
                else
                {
                    SetColliders(Angle);
                }
            }
        }

        /// <summary> Угол поворота вокруг оси. </summary>
        public float RotationAngle { get; private set; }

        /// <summary> Увеличивает угол погиба. </summary>
        public void IncreaseAngle(float delta = MeshFactory.DeltaAngle)
        {
            Angle += delta;
        }

        /// <summary> Уменьшает угол погиба. </summary>
        public void DecreaseAngle(float delta = MeshFactory.DeltaAngle)
        {
            Angle -= delta;
        }

        /// <summary> Поворачивает погиб по часовой стрелке. </summary>
        /// <param name="deltaAngle"> Угол поворота. </param>
        public void TurnClockwise(float deltaAngle = MeshFactory.DeltaAngle)
        {
            RotationAngle += deltaAngle;
            transform.localRotation *= Quaternion.Euler(0, 0, deltaAngle);
        }

        /// <summary> Поворачивает погиб против часовой стрелки. </summary>
        /// <param name="deltaAngle"> Угол поворота. </param>
        public void TurnAnticlockwise(float deltaAngle = MeshFactory.DeltaAngle)
        {
            RotationAngle -= deltaAngle;
            transform.localRotation *= Quaternion.Euler(0, 0, -deltaAngle);
        }

        /// <summary> Меняет радиус погиба. </summary>
        public void ChangeRadius()
        {
            UseSecondRadius = !UseSecondRadius;
        }

        /// <inheritdoc/>
        public override void OnTubeCollisionEnter()
        {
            // Так как погиб не является выпуклой фигурой, он требует особенной обработки коллизий.
            // Для этого создается несколько маленьких выпуклых коллайдеров. 
            // Если хоть один коллайдер обнаружил коллизию, значит вся труба в коллизии.
            if (GetNumberOfCollisions() == 1)
            {
                base.OnTubeCollisionEnter();
            }
        }

        /// <inheritdoc/>
        public override void OnTubeCollisionExit()
        {
            if (GetNumberOfCollisions() == 0)
            {
                base.OnTubeCollisionExit();
            }
        }

        /// <inheritdoc/>
        protected override void SetColor()
        {
            base.SetColor();
            
            Material mat;
            
            if (IsSelected) mat = SelectedTubeMaterial;
            else            mat = IsColliding ? CollidingTubeMaterial : DefaultTubeMaterial;

            
            EndPoint.GetComponent<MeshRenderer>().material = mat;
        }

        #region Unity event functions

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            _meshes = MeshFactory.GetMeshes(Owner.Data);
            CreateColliders();
            UseSecondRadius = false;
            Angle = 90;
            TubeManager.SelectTubeFragment(this);
        }

        #endregion
        
        #region Private defintions
        
        /// <summary> Список мешей для погиба данного диаметра. </summary>
        /// <remarks> Содержит три меша: погиб первого радиуса, погиб второго радиуса, плоское кольцо. </remarks>
        private List<Mesh> _meshes;

        /// <summary> Список плоских выпуклых коллайдеров. </summary>
        private readonly List<GameObject> _colliders = new List<GameObject>();
        private readonly List<TubeFragmentCollider> _collidersComponents = new List<TubeFragmentCollider>();

        private static readonly int ShaderDiameter = Shader.PropertyToID("_Diameter");
        private static readonly int ShaderBendRadius = Shader.PropertyToID("_BendRadius");
        private static readonly int ShaderAngle = Shader.PropertyToID("_Angle");
        private bool _useSecondRadius;
        private float _angle = MeshFactory.DeltaAngle;
        private float _radius;
    
        /// <summary> Отображает соответствующий меш. </summary>
        private void SetMesh()
        {
            Tube.GetComponent<MeshFilter>().mesh = _meshes[UseSecondRadius ? 1 : 0];
            Tube.GetComponent<MeshCollider>().sharedMesh = Tube.GetComponent<MeshFilter>().mesh;
            EndPoint.GetComponent<MeshFilter>().mesh = _meshes.Last();
            
            for (int i = 0; i < _colliders.Count; i++)
            {
                _colliders[i].GetComponent<SphereCollider>().radius = Diameter / 2;
                float shiftAngle = (2 * i + 2) / 2f * MeshFactory.DeltaAngle;
                Vector3 shiftVector = Vector3.zero.RotateAround(new Vector3(-Radius, 0f, 0f), 
                                                        Quaternion.Euler(0, -shiftAngle, 0));
                _colliders[i].transform.localPosition = shiftVector;
            }
        }

        /// <summary> Включает или выключает необходимое число коллайдеров. </summary>
        /// <param name="newAngle"> Новый угол поворота. </param>
        private void SetColliders(float newAngle)
        {
            int newPos = (int)Math.Floor(newAngle / MeshFactory.DeltaAngle);

            if (newPos < 3)
            {
                foreach (GameObject col in _colliders)
                {
                    col.SetActive(false);
                }
                
                _colliders[0].SetActive(true);
                
            }
            else
            {
                for (int i = 0; i < _colliders.Count; i++)
                {
                    _colliders[i].SetActive((i < newPos - 2));
                }
            }
        }

        /// <summary> Создаёт коллайдеры. </summary>
        private void CreateColliders()
        {
            for (int i = 0; i <= 180 / MeshFactory.DeltaAngle; i++)
            {
                GameObject newCollider = Instantiate(ColliderPrefab, Tube.transform);
                newCollider.GetComponent<TubeFragmentCollider>().Owner = this;
                _colliders.Add(newCollider);
                _collidersComponents.Add(newCollider.GetComponent<TubeFragmentCollider>());
            }
        }

        /// <summary> Перемещает конечную точку трубы в нужное место. </summary>
        private void SetEndpoint()
        {
            Quaternion rot = Quaternion.Euler(0, -Angle, 0);
            Vector3 pos = new Vector3(Radius, 0, 0);
            EndPoint.transform.localPosition = rot * pos - pos;
            EndPoint.transform.localRotation = rot;
        }

        private int GetNumberOfCollisions()
        {
            int angle = (int)Math.Floor(Angle / MeshFactory.DeltaAngle);
            int numberOfCollisions = 0;
            for (int i = 0; i < _colliders.Count && i < angle - 2; i++)
            {
                if (_colliders[i].activeSelf && _collidersComponents[i].IsInTrigger)
                {
                    numberOfCollisions++;
                }
            }

            return numberOfCollisions;
        }

        #endregion
    }
}
