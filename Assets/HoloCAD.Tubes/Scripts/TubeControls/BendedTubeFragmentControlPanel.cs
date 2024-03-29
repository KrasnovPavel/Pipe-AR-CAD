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
    [RequireComponent(typeof(BendedTubeFragment))]
    public class BendedTubeFragmentControlPanel : TubeFragmentControlPanel 
    {
        /// <summary> Кнопка увеличения угла погиба. </summary>
        [Tooltip("Кнопка увеличения угла погиба.")]
        [CanBeNull] public Button3D IncreaseAngleButton;
        
        /// <summary> Кнопка уменьшения угла погиба. </summary>
        [Tooltip("Кнопка уменьшения угла погиба.")]
        [CanBeNull] public Button3D DecreaseAngleButton;
        
        /// <summary> Кнопка поворота погиба по часовой стрелке. </summary>
        [Tooltip("Кнопка поворота погиба по часовой стрелке.")]
        [CanBeNull] public Button3D TurnClockwiseButton;
        
        /// <summary> Кнопка поворота погиба против часовой стрелки. </summary>
        [Tooltip("Кнопка поворота погиба против часовой стрелки.")]
        [CanBeNull] public Button3D TurnAnticlockwiseButton;
        
        /// <summary> Кнопка смены радиуса погиба. </summary>
        [Tooltip("Кнопка смены радиуса погиба.")]
        [CanBeNull] public Button3D ChangeRadiusButton;

        /// <inheritdoc/>
        public override void OnSelect()
        {
            base.OnSelect();
            GamepadController.SubscribeToAxis(GamepadController.InputAxis.RightStickHorizontal,
                                              null,
                                              _fragment.ChangeAngle,
                                              4);
            GamepadController.SubscribeToAxis(GamepadController.InputAxis.LeftStickHorizontal,
                                              null,
                                              _fragment.TurnAround,
                                              4);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.DPADUp,
                                               null,
                                               _fragment.ChangeRadius);
        }

        /// <inheritdoc/>
        public override void OnDeselect()
        {
            base.OnDeselect();
            GamepadController.UnsubscribeFromAxis(GamepadController.InputAxis.RightStickHorizontal,
                                                  null,
                                                  _fragment.ChangeAngle,
                                                  4);
            GamepadController.UnsubscribeFromAxis(GamepadController.InputAxis.LeftStickHorizontal,
                                                  null,
                                                  _fragment.TurnAround,
                                                  4);
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.DPADUp,
                                                   null,
                                                   _fragment.ChangeRadius);
        }

        /// <inheritdoc />
        protected override void CalculateBarPosition()
        {
            if (ButtonBar == null) return;
            Vector3 endPointPosition = ButtonBar.parent.position;

            Vector3 direction = _fragment.Diameter * 1.1f * (_camera.transform.position - endPointPosition).normalized;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.back, direction);
            ButtonBar.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
            ButtonBar.position = new Vector3(direction.x, 0, direction.z) + endPointPosition;
        }

        protected override void InitButtons()
        {
            base.InitButtons();
            if (IncreaseAngleButton != null)
            {
                IncreaseAngleButton.OnClick = delegate { _fragment.ChangeAngle(Steps.Angular); };
            }

            if (DecreaseAngleButton != null)
            {
                DecreaseAngleButton.OnClick = delegate { _fragment.ChangeAngle(-Steps.Angular); };
            }

            if (TurnClockwiseButton != null)
            {
                TurnClockwiseButton.OnClick = delegate { _fragment.TurnAround(Steps.Angular); };
            }

            if (TurnAnticlockwiseButton != null)
            {
                TurnAnticlockwiseButton.OnClick = delegate { _fragment.TurnAround(-Steps.Angular); };
            }

            if (ChangeRadiusButton != null)
            {
                ChangeRadiusButton.OnClick = delegate { _fragment.ChangeRadius(); };
            }
        }

        /// <inheritdoc />
        protected override void ExpandSettings()
        {
            base.ExpandSettings();
            if (ButtonBar == null) return;
            
            foreach (Transform child in ButtonBar)
            {
                child.gameObject.SetActive(true);
            }

            if (TextLabel != null)
            {
                TextLabel.gameObject.SetActive(true);
                TextLabel.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            }

            if (ExpandButton != null)
            {
                ExpandButton.transform.localPosition = new Vector3(0f, 0.25f, 0f);
                ExpandButton.Text = "Скрыть";
            }
            
        }

        /// <inheritdoc />
        protected override void HideSettings()
        {
            base.HideSettings();
            if (ButtonBar == null) return;
            
            foreach (Transform child in ButtonBar)
            {
                child.gameObject.SetActive(false);
            }

            if (TextLabel != null)
            {
                TextLabel.gameObject.SetActive(true);
                TextLabel.transform.localPosition = new Vector3(0f, 0.07f, 0f);
            }

            if (ExpandButton != null)
            {
                ExpandButton.gameObject.SetActive(true);
                ExpandButton.transform.localPosition = Vector3.zero;
                ExpandButton.Text = "Настройки";
            }
        }

        #region Unity event functions

        protected void Awake()
        {
            _fragment = GetComponent<BendedTubeFragment>();
            BaseFragment = _fragment;
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            _camera = Camera.main;
            RotationController = GetComponent<GrabRotator>();
            if (RotationController != null)
            {
                RotationController.PropertyChanged += delegate
                                                          (object sender, PropertyChangedEventArgs args)
                                                      {
                                                          if (args.PropertyName == nameof(GrabRotator.Angle))
                                                          {
                                                              _fragment.RotationAngle = RotationController.Angle;
                                                          }
                                                      };
                RotationController.Radius = _fragment.Diameter;
            }
            _fragment.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                                         {
                                             switch (args.PropertyName)
                                             {
                                                 case nameof(BendedTubeFragment.Angle):
                                                 case nameof(BendedTubeFragment.RotationAngle):
                                                     SetText();
                                                     break;
                                                 case nameof(BendedTubeFragment.Diameter):
                                                     if (RotationController != null)
                                                     {
                                                         RotationController.Radius = _fragment.Diameter / 2 * 1.5f;
                                                     }

                                                     break;
                                             }
                                         };
            CheckIsButtonsEnabled(_fragment);
            SetText();
        }

        #endregion

        #region Private definitions

        private Camera _camera;
        private BendedTubeFragment _fragment;
        [CanBeNull] public GrabRotator RotationController;

        private void SetText()
        {
            if (TextLabel != null) TextLabel.text = $"A:{_fragment.Angle:0.00}° B:{_fragment.RotationAngle:0.00}°";
        }

        #endregion
    }
}