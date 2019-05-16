// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, реализующий фрагмент фланца трубы. </summary>
    public class StartTubeFragment : TubeFragment
    {
        /// <inheritdoc />
        public override float Diameter
        {
            get => base.Diameter;
            set
            {
                if (Math.Abs(base.Diameter - value) < float.Epsilon) return;

                base.Diameter = value;
                Tube.transform.localScale = new Vector3(Diameter, Length, Diameter);
            }
        }

        /// <summary> Устанавливает следующий из доступных диаметров труб. </summary>
        public void IncreaseDiameter()
        {
            Owner.SelectBiggerDiameter();
        }
        /// <summary> Устанавливает предыдущий из доступных диаметров труб. </summary>
        public void DecreaseDiameter()
        {
            Owner.SelectSmallerDiameter();
        }

        /// <summary> Переход в режим размещения трубы. </summary>
        public void StartPlacing()
        {
            Owner.StartPlacing();
#if ENABLE_WINMD_SUPPORT
            _recognizer.StartCapturingGestures();
#endif
        }

        /// <inheritdoc />
        public override void RemoveThisFragment()
        {
            if (TubeManager.AllTubes.Count == 1)
            {
                TubeFragment next = Owner.GetNextFragment(this);
                if (next != null) next.RemoveThisFragment();
                StartPlacing();
            }
            else
            {
                base.RemoveThisFragment();
            }
        }

        #region Unity event functions

        /// <inheritdoc />
        protected override void Start()
        {
            _camera = Camera.main;
            base.Start();
            EndPoint.transform.localPosition = new Vector3(0, 0, Length);
            TubeManager.SelectTubeFragment(this);
            
#if ENABLE_WINMD_SUPPORT
            Owner.StartPlacing();
            _recognizer = new GestureRecognizer();
            _recognizer.Tapped += args =>
            {
                if (IsPlacing)
                {
                    Owner.StopPlacing();
                    _recognizer.StopCapturingGestures();
                }
            };
            _recognizer.StartCapturingGestures();
#endif
        }


        /// <inheritdoc />
        protected override void Update()
        {
            base.Update();
            
            if (IsPlacing)
            {
                Place();
            }
        }

        #endregion

        #region Private definitions

        private const float Length = 0.03f;
        private Camera _camera;
#if ENABLE_WINMD_SUPPORT
        private GestureRecognizer _recognizer;
#endif
        
        /// <summary> Перемещает фланец в точку на которую смотрит пользователь. </summary>
        private void Place()
        {
            Vector3 headPosition = _camera.transform.position;
            Vector3 gazeDirection = _camera.transform.forward;
    
            RaycastHit hitInfo;
            if (!Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f)) return;
            
            transform.position = hitInfo.point + Vector3.up * 0.02f;
            transform.rotation = Quaternion.LookRotation(hitInfo.normal);
        }

        #endregion
    }
}
