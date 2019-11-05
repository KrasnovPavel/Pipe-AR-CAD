// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using HoloCAD.UnityTubes;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс, отображающий информацию об отростке. </summary>
    public class OutgrowthControlPanel : TubeFragmentControlPanel
    {
        /// <summary> Кнопка, двигающая отросток к концу родительской трубы. </summary>
        [Tooltip("Кнопка, двигающая отросток к концу родительской трубы.")]
        [CanBeNull] public Button3D ForwardButton;
        
        /// <summary> Кнопка, двигающая отросток к началу родительской трубы. </summary>
        [Tooltip("Кнопка, двигающая отросток к началу родительской трубы.")]
        [CanBeNull] public Button3D BackwardButton;

        /// <summary> Кнопка увеличения угла под которым отросток выходит из родительской трубы. </summary>
        [Tooltip("Кнопка увеличения угла под которым отросток выходит из родительской трубы.")]
        [CanBeNull] public Button3D IncreaseAngle;

        /// <summary> Кнопка уменьшения угла под которым отросток выходит из родительской трубы. </summary>
        [Tooltip("Кнопка уменьшения угла под которым отросток выходит из родительской трубы.")]
        [CanBeNull] public Button3D DecreaseAngle;

        /// <summary>
        /// Кнопка вращения отростка вокруг родительской трубы по часовой стрелке, если смотреть с конца трубы.
        /// </summary>
        [Tooltip("Кнопка вращения отростка вокруг родительской трубы по часовой стрелке, если смотреть с конца трубы.")]
        [CanBeNull] public Button3D ClockwiseButton;

        /// <summary>
        /// Кнопка вращения отростка вокруг родительской трубы против часовой стрелки, если смотреть с конца трубы.
        /// </summary>
        [Tooltip("Кнопка вращения отростка вокруг родительской трубы против часовой стрелки, если смотреть с конца трубы.")]
        [CanBeNull] public Button3D AnticlockwiseButton;

        /// <inheritdoc/>
        public override void OnSelect()
        {
            GamepadController.SubscribeToAxis(GamepadController.InputAxis.LeftStickVertical, 
                                              GamepadController.InputAxis.RightTrigger, 
                                              _outgrowth.Move);
            GamepadController.SubscribeToAxis(GamepadController.InputAxis.LeftStickHorizontal,
                                              GamepadController.InputAxis.RightTrigger,
                                              _outgrowth.TurnAround,
                                              4);
            GamepadController.SubscribeToAxis(GamepadController.InputAxis.RightStickHorizontal,
                                              GamepadController.InputAxis.RightTrigger,
                                              _outgrowth.ChangeAngle,
                                              4);
        }

        /// <inheritdoc/>
        public override void OnDeselect()
        {
            GamepadController.UnsubscribeFromAxis(GamepadController.InputAxis.LeftStickVertical, 
                                                  GamepadController.InputAxis.RightTrigger,
                                                  _outgrowth.Move);
            GamepadController.UnsubscribeFromAxis(GamepadController.InputAxis.LeftStickHorizontal, 
                                                  GamepadController.InputAxis.RightTrigger,
                                                  _outgrowth.TurnAround,
                                                  4);
            GamepadController.UnsubscribeFromAxis(GamepadController.InputAxis.RightStickHorizontal, 
                                                  GamepadController.InputAxis.RightTrigger,
                                                  _outgrowth.ChangeAngle,
                                                  4);
        }

        /// <inheritdoc />
        protected override void CalculateBarPosition()
        {
            // Do nothing
        }

        /// <inheritdoc />
        protected override void InitButtons()
        {
            if (ForwardButton != null)
            {
                ForwardButton.OnClick += delegate { _outgrowth.Move(Steps.Linear); };
            }
            if (BackwardButton != null)
            {
                BackwardButton.OnClick += delegate { _outgrowth.Move(-Steps.Linear); };
            }
            if (IncreaseAngle != null)
            {
                IncreaseAngle.OnClick += delegate { _outgrowth.ChangeAngle(Steps.Angular); };
            }
            if (DecreaseAngle != null)
            {
                DecreaseAngle.OnClick += delegate { _outgrowth.ChangeAngle(-Steps.Angular); };
            }
            if (ClockwiseButton != null)
            {
                ClockwiseButton.OnClick += delegate { _outgrowth.TurnAround(Steps.Angular); };
            }
            if (AnticlockwiseButton != null)
            {
                AnticlockwiseButton.OnClick += delegate { _outgrowth.TurnAround(-Steps.Angular); };
            }
        }

        #region Unity event functions

        protected void Awake()
        {
            _outgrowth = transform.parent.parent.parent.GetComponent<Outgrowth>();
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            _outgrowth.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                switch (args.PropertyName)
                {
                    case nameof(Outgrowth.Position):
                        CheckMoveButtons();
                        SetText();
                        break;
                    case nameof(Outgrowth.Angle):
                        bool canIncrease = Math.Round(_outgrowth.Angle + Steps.Angular) <= Outgrowth.MaxAngle;
                        bool canDecrease = Math.Round(_outgrowth.Angle - Steps.Angular) >= Outgrowth.MinAngle;

                        if (IncreaseAngle  != null) IncreaseAngle.SetEnabled(canIncrease);
                        if (DecreaseAngle  != null) DecreaseAngle.SetEnabled(canDecrease);
                        SetText();
                        break;
                }
            };
            _outgrowth.Fragment.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(DirectTubeFragment.Length))
                {
                    CheckMoveButtons();
                    SetText();
                }
            };
            SetText();
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            OnSelect();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            OnDeselect();
        }

        #endregion

        #region Private definitions

        private Outgrowth _outgrowth;

        private void CheckMoveButtons()
        {
            float maxDistance = _outgrowth.ParentFragment.Length - _outgrowth.Fragment.Diameter / 2;
            float minDistance = _outgrowth.Fragment.Diameter / 2;
            bool canForward = _outgrowth.Position + Steps.Linear < maxDistance;
            bool canBackward = _outgrowth.Position - Steps.Linear > minDistance;

            if (ForwardButton != null) ForwardButton.SetEnabled(canForward);
            if (BackwardButton != null) BackwardButton.SetEnabled(canBackward);
        }

        private void SetText()
        {
            if (TextLabel != null) TextLabel.text = $"D:{_outgrowth.Position:0.000} A:{_outgrowth.Angle:0.00}";
        }

        #endregion
    }
}