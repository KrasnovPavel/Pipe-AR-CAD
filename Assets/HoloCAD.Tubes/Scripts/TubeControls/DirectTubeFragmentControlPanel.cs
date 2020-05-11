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
    [RequireComponent(typeof(DirectTubeFragment))]
    public class DirectTubeFragmentControlPanel : TubeFragmentControlPanel 
    {
        /// <summary> Кнопка увеличения длины. </summary>
        [Tooltip("Кнопка увеличения длины.")]
        [CanBeNull] public Button3D IncreaseLengthButton;

        /// <summary> Кнопка уменьшения длины. </summary>
        [Tooltip("Кнопка уменьшения длины.")]
        [CanBeNull] public Button3D DecreaseLengthButton;
        
        /// <summary> Кнопка добавления отростка. </summary>
        [Tooltip("Кнопка добавления отростка.")]
        [CanBeNull] public Button3D AddOutgrowthButton;

        /// <inheritdoc/>
        public override void OnSelect()
        {
            base.OnSelect();
            GamepadController.SubscribeToAxis(GamepadController.InputAxis.LeftStickHorizontal, 
                                             null, 
                                             _fragment.ChangeLength);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.DPADUp,
                                               null,
                                               _fragment.AddOutgrowth);
        }

        /// <inheritdoc/>
        public override void OnDeselect()
        {
            base.OnDeselect();
            GamepadController.UnsubscribeFromAxis(GamepadController.InputAxis.LeftStickHorizontal, 
                                                 null, 
                                                  _fragment.ChangeLength);
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.DPADUp,
                                                   null,
                                                   _fragment.AddOutgrowth);
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
            if (IncreaseLengthButton != null)
            {
                IncreaseLengthButton.OnClick += delegate { _fragment.ChangeLength(Steps.Linear); };
            }
            if (DecreaseLengthButton != null)
            {
                DecreaseLengthButton.OnClick += delegate { _fragment.ChangeLength(-Steps.Linear); };
            }
            if (AddOutgrowthButton != null)
            {
                AddOutgrowthButton.OnClick += delegate { _fragment.AddOutgrowth(); };				
            }
        }

        #region Unity event functions

        protected void Awake()
        {
            _fragment = GetComponent<DirectTubeFragment>();
            BaseFragment = _fragment;
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

            if (ExpandButton != null) ExpandButton.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        }

        /// <inheritdoc />
        protected override void HideSettings()
        {
            base.ExpandSettings();
            if (ButtonBar == null) return;
            foreach (Transform child in ButtonBar)
            {
                child.gameObject.SetActive(false);
            }

            if (TextLabel != null) TextLabel.gameObject.SetActive(true);

            if (ExpandButton != null)
            {
                ExpandButton.gameObject.SetActive(true);
                ExpandButton.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            }
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            _camera = Camera.main;
            _fragment.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(DirectTubeFragment.Length))
                {
                    SetText();
                }
            };
            
            CheckIsButtonsEnabled(_fragment);
            SetText();
            _lengthGrabber = GetComponent<GrabExtender>();
            if (_lengthGrabber != null)
            {
                _lengthGrabber.Value = _fragment.Length;
                _lengthGrabber.GrabberScale = _fragment.Diameter * 0.8f;
                _fragment.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                                             {
                                                 switch (args.PropertyName)
                                                 {
                                                     case nameof(TubeFragment.Diameter):
                                                         _lengthGrabber.GrabberScale = _fragment.Diameter * 0.8f;
                                                         break;
                                                     case nameof(DirectTubeFragment.Length):
                                                         _lengthGrabber.Value = _fragment.Length;
                                                         break;
                                                 }
                                             };
                _lengthGrabber.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                                                  {
                                                      if (args.PropertyName == nameof(GrabExtender.Value))
                                                      {
                                                          _fragment.Length = _lengthGrabber.Value;
                                                      }
                                                  };
            }
        }

        #endregion

        #region Private definitions

        private Camera _camera;
        private DirectTubeFragment _fragment;
        private GrabExtender _lengthGrabber;

        private void SetText()
        {
            if (TextLabel != null) TextLabel.text = $"L: {_fragment.Length:0.000}";
        }

        #endregion
    }
}
