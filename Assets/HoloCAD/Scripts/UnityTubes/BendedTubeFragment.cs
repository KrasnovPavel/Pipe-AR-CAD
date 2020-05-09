// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, реализующий участок погиба трубы. </summary>
    public class BendedTubeFragment : TubeFragment
    {
        /// <summary> Prefab коллайдера сегмента погиба. </summary>
        [Tooltip("Prefab коллайдера сегмента погиба.")] 
        [CanBeNull] public GameObject ColliderPrefab;

        /// <summary> Материал последнего кольца трубы. </summary>
        [Tooltip("Материал последнего кольца трубы.")] 
        public Material EndRingMaterial;

        /// <summary> Стартовый флаг, указывающий какой из двух радиусов погиба используется. <c>true</c> если второй.  </summary>
        public bool StartUseSecondRadius;
        
        /// <summary> Стартовый угол погиба. </summary>
        public float StartAngle = 90f;
        
        /// <summary> Угол погиба. </summary>
        public float Angle
        {
            get => _angle;
            set
            {
                if (value < MinAngle || value > MaxAngle || Math.Abs(_angle - value) < float.Epsilon) return;

                _angle = value;
                SetColliders(_angle);
                Tube.GetComponent<MeshRenderer>().material.SetFloat(ShaderAngle, _angle / 180f * (float) Math.PI);
                SetEndpoint();
                OnPropertyChanged();
            }
        }
    
        /// <summary> Флаг, указывающий какой из двух радиусов погиба используется. <c>true</c> если второй.  </summary>
        public bool UseSecondRadius
        {
            get => _useSecondRadius;
            set
            {
                if (_useSecondRadius == value) return;
                
                _useSecondRadius = value;
                Radius = _useSecondRadius ? Owner.Data.second_radius : Owner.Data.first_radius;
                SetMesh();
                OnPropertyChanged();
            }
        }

        /// <summary> Используемый радиус погиба. </summary>
        public float Radius
        {
            get => _radius;
            protected set
            {
                if (Math.Abs(_radius - value) < float.Epsilon) return;
                
                _radius = value;
                
                Tube.GetComponent<MeshRenderer>().material.SetFloat(ShaderBendRadius, Radius);
                SetEndpoint();
                OnPropertyChanged();
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
                EndPoint.GetComponent<MeshRenderer>().material.SetFloat(ShaderDiameter, Diameter);
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
        public float RotationAngle
        {
            get
            {
                float result = _rotationAngle;
                if (Mathf.Abs(result) > 360f)
                {
                    result -= Mathf.Ceil(result / 360f) * 360f;
                }

                if (result < 0)
                {
                    result = 360f + result;
                }

                if (result > 180f)
                {
                    result = -360f + result;
                }

                return result;
            }
            set
            {
                if (Math.Abs(_rotationAngle - value) < float.Epsilon) return;
                _rotationAngle = value;
                transform.localRotation = Quaternion.Euler(0, 0, _rotationAngle);
                OnPropertyChanged();
            }
        }

        /// <summary> Изменяет радиус погиба на новый, если он соответствует одному из допустимых радиусов. </summary>
        /// <param name="radius">Новый радиус</param>
        public void SetRadius(float radius)
        {
            // функция проверяет, является ли новый радиус допустимым, и в этом случае его устанавливает
            if (Math.Abs(_radius - radius) < float.Epsilon) return;
            if (Math.Abs(Owner.Data.first_radius - radius) > float.Epsilon &&
                Math.Abs(Owner.Data.second_radius - radius) > float.Epsilon) return;
            StartUseSecondRadius = !(Math.Abs(Owner.Data.first_radius - radius) < float.Epsilon);
        }


        /// <summary> Увеличивает угол погиба. </summary>
        public void ChangeAngle(float delta)
        {
            Angle += delta;
        }

        /// <summary> Поворачивает погиб по часовой стрелке. </summary>
        /// <param name="deltaAngle"> Угол поворота. </param>
        public void TurnAround(float deltaAngle)
        {
            RotationAngle += deltaAngle;
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

            Color color;

            if (IsSelected) color = SelectedTubeGridColor;
            else color = IsColliding ? CollidingTubeGridColor : DefaultTubeGridColor;

            EndPoint.GetComponent<MeshRenderer>().material.SetColor(GridColor, color);
        }

        #region Unity event functions

        /// <inheritdoc />
        protected override void Start()
        {
            CreateColliders();
            base.Start();
            _meshes = MeshFactory.GetMeshes(Owner.Data);
            UseSecondRadius = StartUseSecondRadius;
            Angle = StartAngle;
        }

        #endregion

        #region Private defintions

        /// <summary> Список мешей для погиба данного диаметра. </summary>
        /// <remarks> Содержит три меша: погиб первого радиуса, погиб второго радиуса, плоское кольцо. </remarks>
        private List<Mesh> _meshes;

        /// <summary> Список плоских выпуклых коллайдеров. </summary>
        private readonly List<GameObject> _colliders = new List<GameObject>();

        private readonly List<TubeFragmentCollider> _collidersComponents = new List<TubeFragmentCollider>();

        /// <summary> Минимальный угол погиба. </summary>
        private const float MinAngle = 5f;

        /// <summary> Максимальный угол погиба. </summary>
        private const float MaxAngle = 180f;

        private const float DeltaAngle = 15f;

        private static readonly int ShaderDiameter = Shader.PropertyToID("_Diameter");
        private static readonly int ShaderBendRadius = Shader.PropertyToID("_BendRadius");
        private static readonly int ShaderAngle = Shader.PropertyToID("_Angle");
        private bool _useSecondRadius;
        private float _angle = MinAngle;
        private float _radius;
        private float _rotationAngle;

        /// <summary> Отображает соответствующий меш. </summary>
        private void SetMesh()
        {
            Tube.GetComponent<MeshFilter>().mesh = _meshes[UseSecondRadius ? 1 : 0];
            Tube.GetComponent<MeshCollider>().sharedMesh = Tube.GetComponent<MeshFilter>().mesh;
            EndPoint.GetComponent<MeshFilter>().mesh = _meshes.Last();
            EndPoint.GetComponent<MeshRenderer>().material = EndRingMaterial;

            for (int i = 0; i < _colliders.Count; i++)
            {
                _colliders[i].GetComponent<SphereCollider>().radius = Diameter / 2;
                float shiftAngle = (2 * i + 2) / 2f * DeltaAngle;
                Vector3 shiftVector = Vector3.zero.RotateAround(new Vector3(-Radius, 0f, 0f),
                                                                Quaternion.Euler(0, -shiftAngle, 0));
                _colliders[i].transform.localPosition = shiftVector;
            }
        }

        /// <summary> Включает или выключает необходимое число коллайдеров. </summary>
        /// <param name="newAngle"> Новый угол поворота. </param>
        private void SetColliders(float newAngle)
        {
            int newPos = (int) Math.Floor(newAngle / DeltaAngle);

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
            for (int i = 0; i <= 180 / DeltaAngle; i++)
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
            int angle = (int) Math.Floor(Angle / DeltaAngle);
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
