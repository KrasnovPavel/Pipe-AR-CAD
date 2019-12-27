// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Linq;
using HoloCAD.UnityTubes;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс, рассчитывающий и отображающий расстояние между соединяемыми трубами. </summary>
    [RequireComponent(typeof(LineRenderer))]
    public sealed class TubesConnector : MonoBehaviour
    {
        // ReSharper disable once NotNullMemberIsNotInitialized
        /// <summary> Первая труба </summary>
        [NotNull] public Tube FirstTube;
        
        /// <summary> Вторая труба. </summary>
        [CanBeNull] public Tube SecondTube;
        
        // ReSharper disable once NotNullMemberIsNotInitialized
        /// <summary> Местоположение курсора. </summary>
        [NotNull] public Transform Cursor;
        
        /// <summary> Допустимая ошибка по углу стыка. </summary>
        [Tooltip("Допустимая ошибка по углу стыка.")]
        public float AngleThreshold = 3f;
        
        /// <summary> Допустимая ошибка по расстоянию. </summary>
        [Tooltip("Допустимая ошибка по расстоянию.")]
        public float DistanceThreshold = 0.005f;
        
        public void RemoveThis()
        {
            Destroy(gameObject);
            FirstTube.RemoveTubeConnection();
            SecondTube?.RemoveTubeConnection();

            if (TubeUnityManager.ActiveTubesConnector == this)
            {
                TubeUnityManager.RemoveActiveTubesConnector();
            }
        }

        #region Unity event functions

        /// <summary> Функция, инициализирующая объект в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        private void Start ()
        {
            // ReSharper disable once PossibleNullReferenceException
            _camera = Camera.main.transform;
            _renderer = GetComponent<LineRenderer>();
            _labelsBar = transform.Find("LabelsBar");
            _distanceLabel = _labelsBar.Find("DistanceLabel").GetComponent<TextMesh>();
            _angleLabel = _labelsBar.Find("AngleLabel").GetComponent<TextMesh>();
            _diameterLabel = _labelsBar.Find("DiameterLabel").GetComponent<TextMesh>();
            _removeButton = _labelsBar.Find("RemoveButton").GetComponent<Button3D>();
            
            _removeButton.OnClick += delegate { RemoveThis(); };
        }

        private void Update()
        {
            CheckError();
        }

        #endregion

        #region Private definitions

        private LineRenderer _renderer;
        private Transform _labelsBar;
        private TextMesh _distanceLabel;
        private TextMesh _angleLabel;
        private TextMesh _diameterLabel;
        private Transform _camera;
        private Button3D _removeButton;

        private void CheckError()
        {
            Transform first = FirstTube.Fragments.Last().EndPoint.transform;
            Transform second = (SecondTube == null) ? Cursor : SecondTube.Fragments.Last().EndPoint.transform;
            bool isDiametersEquals = SecondTube == null || FirstTube.Data.Equals(SecondTube.Data);
            Vector3 deltaPos = second.position - first.position; 
            float deltaRot = Vector3.Angle(first.forward, -second.forward);
            
            _labelsBar.position = first.position + deltaPos / 2 + (FirstTube.Data.diameter + 0.05f) * -_camera.forward;
            _labelsBar.LookAt(_camera);

            _distanceLabel.text = $"Δx: {deltaPos.x:0.000}м Δy: {deltaPos.y:0.000}м Δz: {deltaPos.z:0.000}м";
            _angleLabel.text = $"Δα: {deltaRot:0.0}˚";
            _diameterLabel.gameObject.SetActive(!isDiametersEquals);
        
            _renderer.SetPositions(new [] { first.position, second.position });
        }
        
        #endregion
    }
}
