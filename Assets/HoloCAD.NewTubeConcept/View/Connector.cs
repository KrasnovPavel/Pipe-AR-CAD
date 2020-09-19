﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.NewTubeConcept.Model;
using HoloCore;
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

            var go   = new GameObject();
            var conn = go.AddComponent<Connector>();
            conn.FirstFlange = flange;
            ActiveConnector  = conn;
        }

        public static void FinishConnection(FlangeView flangeView)
        {
            if (flangeView == Instance.FirstFlange) return;

            var tube     = new Tube(GCMSystemBehaviour.System, ActiveConnector.FirstFlange.flange, flangeView.flange);
            var go       = new GameObject();
            var tubeView = go.AddComponent<TubeView>();
            tubeView.EndFlangeView   = ActiveConnector.FirstFlange;
            tubeView.StartFlangeView = flangeView;
            tubeView.tube            = tube;
            tubeView.name            = "Tube";
            Destroy(ActiveConnector.gameObject);
            ActiveConnector = null;
        }

        #region Unity event functions

        private void Awake()
        {
            _renderer          = GetComponent<LineRenderer>();
            _renderer.endWidth = _renderer.startWidth = 0.01f;
            ActiveConnector    = this;
        }

        private void OnDestroy()
        {
            ActiveConnector = null;
        }

        private void Update()
        {
            _renderer.SetPositions(new[]
            {
                FirstFlange.flange.Origin, 
                CameraRaycast.HitPoint
            });
        }

        #endregion

        #region Private definitions

        private LineRenderer _renderer;

        #endregion
    }
}