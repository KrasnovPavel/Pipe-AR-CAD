using System;
using HoloCAD.NewTubeConcept.Model;
using HoloCore;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(LineRenderer))]
    public class Connector : Singleton<Connector>
    {
        public FlangeView FirstFlange;

        public static bool IsActive => ActiveConnector != null;

        public static Connector ActiveConnector;

        public static void StartConnection(FlangeView flange)
        {
            if (ActiveConnector != null)
            {
                Destroy(ActiveConnector.gameObject);
            }
            var go = new GameObject();
            var conn = go.AddComponent<Connector>();
            conn.FirstFlange = flange;
            ActiveConnector = conn;
        }

        public static void FinishConnection(FlangeView flangeView)
        {
            var tube = new Tube(GCMSystemBehaviour.System, ActiveConnector.FirstFlange.flange, flangeView.flange);
            var go = new GameObject();
            var tubeView = go.AddComponent<TubeView>();
            tubeView.EndFlangeView = ActiveConnector.FirstFlange;
            tubeView.StartFlangeView = flangeView;
            tubeView.tube = tube;
            tubeView.name = "Tube";
            Destroy(ActiveConnector.gameObject);
            ActiveConnector = null;
        }

        #region Unity event functions

        private void Awake()
        {
            _renderer = GetComponent<LineRenderer>();
            _renderer.endWidth = _renderer.startWidth = 0.01f;
            ActiveConnector = this;
            var focusProvider = CoreServices.InputSystem?.FocusProvider;
            focusProvider?.SubscribeToPrimaryPointerChanged(OnPrimaryPointerChanged, true);
        }

        private void OnDestroy()
        {
            ActiveConnector = null;
            var focusProvider = CoreServices.InputSystem?.FocusProvider;
            focusProvider?.UnsubscribeFromPrimaryPointerChanged(OnPrimaryPointerChanged);
        }

        private void Update()
        {
            Vector3 first = FirstFlange.flange.Origin;
            Vector3 second = _pointer.Position;
            _renderer.SetPositions(new[] {first, second});
        }

        #endregion

        #region Private definitions

        private LineRenderer _renderer;
        private IMixedRealityPointer _pointer;

        private void OnPrimaryPointerChanged(IMixedRealityPointer oldPointer, IMixedRealityPointer newPointer)
        {
            _pointer = newPointer;
        }

        #endregion
    }
}