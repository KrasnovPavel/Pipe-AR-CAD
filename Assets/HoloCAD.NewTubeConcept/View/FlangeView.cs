using System;
using UnityEngine;
using HoloCAD.NewTubeConcept.Model;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(SurfaceMagnetism), typeof(Collider))]
    public class FlangeView : MonoBehaviour, IMixedRealityPointerHandler
    {
        public Flange flange;

        public void StartPlacement()
        {
            _placementSolver.enabled = true;
        }

        public void EndPlacement()
        {
            _placementSolver.enabled = false;
        }

        public void StartConnection()
        {
            Connector.StartConnection(this);
        }

        #region Unity event functions

        private void Awake()
        {
            _placementSolver = GetComponent<SurfaceMagnetism>();
            _placementSolver.enabled = false;
        }

        private void Start()
        {
            if (flange == null)
            {
                flange = new Flange(GCMSystemBehaviour.System);
            }
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                flange.Move(transform.position, transform.forward);
                transform.hasChanged = false;
            }
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

        #endregion
    }
}