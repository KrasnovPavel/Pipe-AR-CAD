// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloCAD.Tubes.UnityTubes;
using HoloCore;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.Tubes.TubeControls
{
    /// <inheritdoc />
    /// <summary> Класс, отображающий кнопки и информацию о фланце. </summary>
    /// <remarks> </remarks>
    [RequireComponent(typeof(StartTubeFragment)), RequireComponent(typeof(LineRenderer))]
    public class StartTubeFragmentControlPanel : TubeFragmentControlPanel
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
        
        /// <summary> Кнопка создания новой трубы. </summary>
        [Tooltip("Кнопка создания новой трубы.")]
        [CanBeNull] public Button3D CreateTubeButton;
        
        /// <inheritdoc />
        protected override void CalculateBarPosition()
        {
            if (ButtonBar == null) return;
            Vector3 barPosition = Vector3.zero;
            Vector3 cameraPosition = _camera.position - transform.position; 

            barPosition.x = (cameraPosition.ProjectOn(transform.right) > 0) ? ( _fragment.Diameter/2 + 0.3f) 
                                                                            : (-_fragment.Diameter/2 - 0.3f);
            
            barPosition.y = (cameraPosition.ProjectOn(transform.up) > 0) ? ( _fragment.Diameter/2 + 0.2f) 
                                                                         : (-_fragment.Diameter/2 - 0.2f);

            if (ButtonBar.localPosition.FloatEquals(barPosition)) return;
            
            ButtonBar.localPosition = barPosition;
            CalculateLine();
        }
        
        /// <inheritdoc/>
        public override void OnSelect()
        {
            base.OnSelect();
            GamepadController.SubscribeToRepeatPressing(GamepadController.InputAxis.LeftStickLeft, 
                                                        null, 
                                                        _fragment.DecreaseDiameter);
            GamepadController.SubscribeToRepeatPressing(GamepadController.InputAxis.LeftStickRight, 
                                                        null, 
                                                        _fragment.IncreaseDiameter);
        }

        /// <inheritdoc/>
        public override void OnDeselect()
        {
            base.OnDeselect();
            GamepadController.UnsubscribeFromRepeatPressing(GamepadController.InputAxis.LeftStickLeft, 
                                                            null, 
                                                            _fragment.DecreaseDiameter);
            GamepadController.UnsubscribeFromRepeatPressing(GamepadController.InputAxis.LeftStickRight, 
                                                            null, 
                                                            _fragment.IncreaseDiameter);
        }

        protected override void InitButtons()
        {
            base.InitButtons();
            if (CreateTubeButton != null)
            {
                CreateTubeButton.OnClick += delegate { TubeManager.CreateTube().StartPlacing(); };
            }
            if (IncreaseDiameterButton != null)
            {
                IncreaseDiameterButton.OnClick += delegate { _fragment.IncreaseDiameter(); };
            }
            if (DecreaseDiameterButton != null)
            {
                DecreaseDiameterButton.OnClick += delegate { _fragment.DecreaseDiameter(); };
            }
            if (StartPlacingButton != null)
            {
                StartPlacingButton.OnClick += delegate { _fragment.StartPlacing(); };
            }
        }

        #region Unity event functions

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _fragment = GetComponent<StartTubeFragment>();
            BaseFragment = _fragment;
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            // ReSharper disable once PossibleNullReferenceException
            _camera = Camera.main.transform;
            _fragment.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(_fragment.Diameter))
                {
                    SetText();
                    CalculateLine();
                }
            };
            
            CheckIsButtonsEnabled(_fragment);
            SetText();
            CalculateLine();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            _lineRenderer.enabled = false;
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            _lineRenderer.enabled = true;
        }

        #endregion

        #region Private definitions

        private Transform _camera;
        private StartTubeFragment _fragment;
        private LineRenderer _lineRenderer;

        private void SetText()
        {
            if (TextLabel != null) TextLabel.text = $"Ø:{_fragment.Diameter:0.000}м";
        }
        
        /// <summary> Расчет выносной линии. </summary>
        private void CalculateLine()
        {
            if (ButtonBar == null) return;
            
            Vector3 cameraPosition = _camera.transform.position - transform.position;
            Vector3 barPosition = ButtonBar.localPosition;
            Vector3[] linePositions = { Vector3.zero, Vector3.zero, Vector3.zero };

            barPosition.x += (cameraPosition.ProjectOn(transform.right) > 0) ? -0.25f : +0.25f;
            barPosition.y -= 0.07f;
            float angle = Vector3.Angle(Vector3.left, barPosition);
            if (cameraPosition.ProjectOn(transform.up) > 0)
            {
                angle = 360f - angle;
            }
            // ReSharper disable once Unity.InefficientMultiplicationOrder
            linePositions[0] = Quaternion.Euler(0f, 0f, angle) * Vector3.right * _fragment.Diameter / 2;
            linePositions[1] = barPosition;
            linePositions[2] = barPosition 
                               + ((cameraPosition.ProjectOn(transform.right) > 0) ? new Vector3(0.5f, 0f, 0f)
                                   : new Vector3(-0.5f, 0f, 0f));
            _lineRenderer.SetPositions(linePositions);
            _lineRenderer.startWidth = _lineRenderer.endWidth = 0.1f * _fragment.Diameter/2;
        }

        #endregion

    }
}
