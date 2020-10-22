// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.Tubes.Model;
using HoloCore;
using UnityEngine;

namespace HoloCAD.Tubes.View
{
    /// <summary> Виджет выбора двух фланцев для соединения их трубой. </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class Connector : Singleton<Connector>
    {
        /// <summary> Первый выбранный фланец.  </summary>
        public FlangeView FirstFlange;

        /// <summary> Ведётся ли соединение в данный момент. </summary>
        public static bool IsActive => ActiveConnector != null;

        /// <summary> Активный в данный момент соединитель. </summary>
        public static Connector ActiveConnector;

        /// <summary> Запускает соединение. </summary>
        /// <param name="flange"> Первый выбранный фланец. </param>
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

        /// <summary> Завершает соединение. </summary>
        /// <param name="flangeView"> Второй выбранный фланец. </param>
        public static void FinishConnection(FlangeView flangeView)
        {
            if (flangeView == Instance.FirstFlange)
            {
                Destroy(ActiveConnector.gameObject);
                ActiveConnector = null;
                return;
            };

            var tube = new Tube(GCMSystemBehaviour.System, 
                                ActiveConnector.FirstFlange.flange, 
                                flangeView.flange,
                                flangeView.flange.TubeData);
            
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
