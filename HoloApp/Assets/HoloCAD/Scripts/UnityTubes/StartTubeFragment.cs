// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc cref="TubeFragment"/>
    /// <summary> Класс, реализующий фрагмент фланца трубы. </summary>
    public class StartTubeFragment : TubeFragment, IDisposable //-V3074
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
        
        /// <inheritdoc/>
        public override bool IsPlacing
        {
            get => base.IsPlacing;
            set
            {
                base.IsPlacing = value;
                
                Transform tubeCollider = Tube.transform.Find("Collider"); 
                if (tubeCollider != null) tubeCollider.GetComponent<BoxCollider>().enabled = !IsPlacing;
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
        public override void StartPlacing()
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
                if (Child != null) Child.RemoveThisFragment();
                StartPlacing();
            }
            else
            {
                base.RemoveThisFragment();
            }
        }

        /// <summary> Выход из режима размещения трубы. </summary>
        public override void StopPlacing()
        {
            if (IsPlacing)
            {
                Owner.StopPlacing();
#if ENABLE_WINMD_SUPPORT
                _recognizer.StopCapturingGestures();
#endif
            }
        }
        
        /// <summary> Вызывается при уничтожении по паттерну IDisposable. </summary>
        public void Dispose()
        {
#if ENABLE_WINMD_SUPPORT
            _recognizer.Dispose();
#endif
        }

        #region Unity event functions

        /// <inheritdoc />
        protected override void Awake()
        {
            _camera = Camera.main;
            base.Awake();
            Tube.transform.localPosition = new Vector3(0, 0, -Length);
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            // TODO: Сделать вход в режим перемещения, если труба была создана нажатием на кнопку. 
#if ENABLE_WINMD_SUPPORT
            _recognizer = new GestureRecognizer();
            _recognizer.Tapped += args =>
            {
                StopPlacing();
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

            if (hitInfo.transform.name == "Marker")
            {
                transform.position = hitInfo.transform.position;
            }
            else
            {
                transform.position = hitInfo.point + Vector3.up * 0.02f;
            }
            
            transform.rotation = Quaternion.LookRotation(hitInfo.normal);
        }

        #endregion
    }
}
