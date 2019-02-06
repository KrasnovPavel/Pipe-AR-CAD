using System;
using HoloToolkit.Unity.InputModule;
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
        private const float Length = 0.03f;
        private Camera _camera;
#if ENABLE_WINMD_SUPPORT
        private GestureRecognizer _recognizer;
#endif

        /// <inheritdoc />
        protected override void Start()
        {
            _camera = Camera.main;
            base.Start();
            EndPoint.transform.localPosition = new Vector3(0, 0, Length);
            TubeManager.SelectTubeFragment(this);
        }

        private void Awake()
        {
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
        public override float Diameter
        {
            get { return _diameter; }
            set
            {
                if (Math.Abs(_diameter - value) < float.Epsilon) return;

                _diameter = value;
                Tube.transform.localScale = new Vector3(Diameter, Length, Diameter);
                LabelText.text = $"Диаметр: {Diameter:0.000}м.";
            }
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
    
        /// <inheritdoc />
        protected override void InputDown(GameObject obj, InputEventData eventData)
        {
            base.InputDown(obj, eventData);
            
            switch (obj.name)
            {
                case "IncreaseDiameterButton":
                    Owner.SelectBiggerDiameter();
                    break;
                case "DecreaseDiameterButton":
                    Owner.SelectSmallerDiameter();
                    break;
                case "PlacingButton":
                    Owner.StartPlacing();
#if ENABLE_WINMD_SUPPORT
                    _recognizer.StartCapturingGestures();
#endif
                    break;
            }
        }
    }
}
