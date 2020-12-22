// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloCAD.Tubes.Model;
using HoloCore;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TMPro;
using UnityEngine;

namespace HoloCAD.Tubes.View
{
    /// <summary> Виджет отображения фланца. </summary>
    [RequireComponent(typeof(Collider), typeof(TapRecognizer), typeof(SurfaceMagnetism))]
    [RequireComponent(typeof(ObjectManipulator))]
    public class FlangeView : MonoBehaviour, IMixedRealityPointerHandler //-V3072
    {
        /// <summary> Модель фланца. </summary>
        // ReSharper disable once InconsistentNaming
        public Flange flange;

        /// <summary> Кнопка включения перемещения. </summary>
        public Interactable MoveToggle;

        /// <summary> Кнопка включения вращения. </summary>
        public Interactable RotateToggle;

        public Transform Cylinder;

        /// <summary> Панель кнопок. </summary>
        public Transform ButtonBar;

        /// <summary> Панель вывода диаметра. </summary>
        public TextMeshPro DiameterLabel;

        /// <summary> Кнопка соединения фланцев. </summary>
        [CanBeNull] public GameObject ConnectButton;

        /// <summary> Кнопка удаления трубы. </summary>
        [CanBeNull] public GameObject DeleteTubeButton;

        /// <summary> Запускает режим перемещения с прилипанием к поверхностям. </summary>
        public void StartPlacement()
        {
            if (!_isStarted)
            {
                _placementOnStart = true;
                return;
            }

            _manipulator.enabled                           = false;
            _collider.enabled                              = false;
            SceneController.Instance.EnableSpatialCollider = true;
            SceneController.Instance.EnableSpatialRenderer = true;
            _tapRecognizer.enabled                         = true;
            _placementSolver.enabled                       = true;
        }

        /// <summary> Останавливает режим перемещения с прилипанием к поверхностям. </summary>
        public void EndPlacement()
        {
            _manipulator.enabled                           = true;
            _collider.enabled                              = true;
            SceneController.Instance.EnableSpatialCollider = false;
            SceneController.Instance.EnableSpatialRenderer = false;
            _tapRecognizer.enabled                         = false;
            _placementSolver.enabled                       = false;
        }

        /// <summary> Запускает режим соединения с другой трубой. </summary>
        public void StartConnection()
        {
            if (flange.Owner != null) return;

            Connector.StartConnection(this);
        }

        /// <summary> Удаляет трубу связанную с данным фланцем. </summary>
        public void DeleteTube()
        {
            flange.Owner?.Dispose();
        }

        /// <summary> Удаляет данный фланец (вместе с трубой). </summary>
        public void DeleteFlange()
        {
            flange.Owner?.Dispose();
            flange.Dispose();
        }

        #region Unity event functions

        private void Awake()
        {
            _collider                =  GetComponent<Collider>();
            _tapRecognizer           =  GetComponent<TapRecognizer>();
            _tapRecognizer.enabled   =  false;
            _tapRecognizer.Tap       += EndPlacement;
            _placementSolver         =  GetComponent<SurfaceMagnetism>();
            _placementSolver.enabled =  false;
            _manipulator             =  GetComponent<ObjectManipulator>();
        }

        private void Start()
        {
            if (flange == null)
            {
                flange = new Flange(GCMSystemBehaviour.System);
            }

            flange.PropertyChanged += FlangeOnPropertyChanged;
            flange.Disposed        += delegate { Destroy(gameObject); };

            MoveToggle.OnClick.AddListener(delegate { ChangeManipulationMode(MoveToggle); });
            RotateToggle.OnClick.AddListener(delegate { ChangeManipulationMode(RotateToggle); });

            _isStarted = true;
            if (_placementOnStart) StartPlacement();
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                flange.Move(transform.position, transform.forward);
                transform.hasChanged = false;
            }
        }

        private void LateUpdate()
        {
            if (_redrawRequested)
            {
                Redraw();
                _redrawRequested = false;
            }
        }

        private void OnDestroy()
        {
            _tapRecognizer.Tap     -= EndPlacement;
            flange.PropertyChanged -= FlangeOnPropertyChanged;
        }

        #endregion

        #region MRTK event functions

        /// <summary> Обработчик события клика по трубе для MRTK. </summary>
        /// <param name="eventData"></param>
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (Connector.IsActive)
            {
                Connector.FinishConnection(this);
            }
        }

        /// <summary> Обработчик события нажатия на трубу для MRTK. </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }


        /// <summary> Обработчик события перетягивания трубы для MRTK. </summary>
        /// <param name="eventData"></param>
        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        /// <summary> Обработчик события отпускания нажатия на трубу для MRTK. </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        public void IncreaseDiameter()
        {
            flange.TubeData = TubeLoader.GetBigger(flange.TubeData);
        }

        public void DecreaseDiameter()
        {
            flange.TubeData = TubeLoader.GetSmaller(flange.TubeData);
        }

        #endregion

        #region Private definitions

        private SurfaceMagnetism  _placementSolver;
        private TapRecognizer     _tapRecognizer;
        private Collider          _collider;
        private ObjectManipulator _manipulator;
        private bool              _isStarted;
        private bool              _placementOnStart;
        private bool              _redrawRequested;

        /// <summary> Переключается между режимами перемещения. </summary>
        /// <param name="sender"></param>
        private void ChangeManipulationMode(Interactable sender)
        {
            if (sender == MoveToggle)
            {
                RotateToggle.IsToggled                 = false;
                _manipulator.TwoHandedManipulationType = sender.IsToggled ? TransformFlags.Move : 0;
            }
            else if (sender == RotateToggle)
            {
                MoveToggle.IsToggled                   = false;
                _manipulator.TwoHandedManipulationType = sender.IsToggled ? TransformFlags.Rotate : 0;
            }
        }

        private void FlangeOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _redrawRequested = true;
            if (e.PropertyName == nameof(flange.Owner))
            {
                if (flange.Owner == null)
                {
                    if (ConnectButton    != null) ConnectButton.SetActive(true);
                    if (DeleteTubeButton != null) DeleteTubeButton.SetActive(false);
                }
                else
                {
                    if (ConnectButton    != null) ConnectButton.SetActive(false);
                    if (DeleteTubeButton != null) DeleteTubeButton.SetActive(true);
                }
            }
        }

        private void Redraw()
        {
            ((BoxCollider) _collider).size = new Vector3(flange.Diameter,           flange.Diameter, 0.001f);
            Cylinder.localScale            = new Vector3(flange.Diameter,           flange.Diameter, 0.02f);
            ButtonBar.localPosition        = new Vector3(-(flange.Diameter + 0.1f), 0,               0.05f);
            DiameterLabel.text             = $"D: {flange.Diameter:f3}м.";
        }

        #endregion
    }
}
