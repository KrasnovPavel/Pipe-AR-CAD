// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using UnityEngine;
using HoloCAD.NewTubeConcept.Model;
using HoloCore;
using HoloCore.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(Collider), typeof(TapRecognizer), typeof(SurfaceMagnetism))]
    [RequireComponent(typeof(ObjectManipulator))]
    public class FlangeView : MonoBehaviour, IMixedRealityPointerHandler //-V3072
    {
        public Flange flange;
        public Interactable MoveToggle;
        public Interactable RotateToggle;

        public void StartPlacement()
        {
            _manipulator.enabled = false;
            _collider.enabled = false;
            SceneController.Instance.EnableSpatialCollider = true;
            SceneController.Instance.EnableSpatialRenderer = true;
            _tapRecognizer.enabled = true;
            _placementSolver.enabled = true;
        }

        public void EndPlacement()
        {
            _manipulator.enabled = true;
            _collider.enabled = true;
            SceneController.Instance.EnableSpatialCollider = false;
            SceneController.Instance.EnableSpatialRenderer = false;
            _tapRecognizer.enabled = false;
            _placementSolver.enabled = false;
        }

        public void StartConnection()
        {
            Connector.StartConnection(this);
        }

        #region Unity event functions

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _tapRecognizer = GetComponent<TapRecognizer>();
            _tapRecognizer.enabled = false;
            _tapRecognizer.Tap += EndPlacement;
            _placementSolver = GetComponent<SurfaceMagnetism>();
            _placementSolver.enabled = false;
            _manipulator = GetComponent<ObjectManipulator>();
        }

        private void Start()
        {
            if (flange == null)
            {
                flange = new Flange(GCMSystemBehaviour.System);
            }
            
            MoveToggle.OnClick.AddListener(delegate { ChangeManipulationMode(MoveToggle); });
            RotateToggle.OnClick.AddListener(delegate { ChangeManipulationMode(RotateToggle); });
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                flange.Move(transform.position, transform.forward);
                transform.hasChanged = false;
            }
        }

        private void OnDestroy()
        {
            _tapRecognizer.Tap -= EndPlacement;
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

        #endregion

        #region Private definitions

        private SurfaceMagnetism _placementSolver;
        private TapRecognizer _tapRecognizer;
        private Collider _collider;
        private ObjectManipulator _manipulator;

        private void ChangeManipulationMode(Interactable sender)
        {
            if (sender == MoveToggle)
            {
                RotateToggle.IsToggled = false;
                _manipulator.TwoHandedManipulationType = sender.IsToggled ? TransformFlags.Move : 0;
            }
            else if (sender == RotateToggle)
            {
                MoveToggle.IsToggled = false;
                _manipulator.TwoHandedManipulationType = sender.IsToggled ? TransformFlags.Rotate : 0;
            }
        }

        #endregion
    }
}