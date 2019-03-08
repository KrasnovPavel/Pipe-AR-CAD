using System;
using HoloCAD.UI;
using HoloCore.UI;
using JetBrains.Annotations;
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
        /// <summary> Кнопка увеличения диаметра трубы. </summary>
        [Tooltip("Кнопка увеличения диаметра трубы.")]
        [CanBeNull] public Button3D IncreaseDiameterButton;
        
        /// <summary> Кнопка уменьшения диаметра трубы. </summary>
        [Tooltip("Кнопка уменьшения диаметра трубы.")]
        [CanBeNull] public Button3D DecreaseDiameterButton;
        
        /// <summary> Кнопка перехода в режим размещения трубы. </summary>
        [Tooltip("Кнопка перехода в режим размещения трубы.")]
        [CanBeNull] public Button3D StartPlacingButton;

        /// <summary> Объект, управляющий отображением кнопок и информации о фланце. </summary>
        [Tooltip("Объект, управляющий отображением кнопок и информации о фланце.")]
        public StartTubeFragmentControlPanel ControlPanel;
        
        /// <inheritdoc />
        public override float Diameter
        {
            get => _diameter;
            set
            {
                if (Math.Abs(_diameter - value) < float.Epsilon) return;

                _diameter = value;
                Tube.transform.localScale = new Vector3(Diameter, Length, Diameter);
                ControlPanel.Diameter = Diameter;
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
        protected override void InitButtons()
        {
            base.InitButtons();
            if (IncreaseDiameterButton != null) IncreaseDiameterButton.OnClick += delegate { IncreaseDiameter(); };
            if (DecreaseDiameterButton != null) DecreaseDiameterButton.OnClick += delegate { DecreaseDiameter(); };
            if (StartPlacingButton != null)     StartPlacingButton.OnClick     += delegate { StartPlacing(); };
        }

        #region Unity event functions

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
