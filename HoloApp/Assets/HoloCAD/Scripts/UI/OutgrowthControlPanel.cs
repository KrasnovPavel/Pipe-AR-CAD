// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using HoloCAD.UnityTubes;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс, отображающий информацию об отростке. </summary>
    public class OutgrowthControlPanel : TubeFragmentControlPanel
    {
        /// <summary> Объект, отображающий текстовые данные о участке трубе. </summary>
        [Tooltip("Объект, отображающий текстовые данные о участке трубе.")]
        [CanBeNull] public TextMesh TextLabel;

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


        /// <inheritdoc />
        protected override void CalculateBarPosition()
        {
            // Do nothing
        }

        /// <inheritdoc />
        protected override void CalculateLine()
        {
            // Do nothing
        }

        /// <inheritdoc />
        protected override void SetText()
        {
            Transform t = _outgrowth.transform;
            if (TextLabel != null) TextLabel.text = $"D:{t.localPosition.z:0.000} A:{_outgrowth.Angle}";
        }

        /// <inheritdoc />
        protected override void InitButtons()
        {
            if (ForwardButton != null)
            {
                ForwardButton.OnClick += delegate { _outgrowth.Move(Outgrowth.Step); };
            }
            if (BackwardButton != null)
            {
                BackwardButton.OnClick += delegate { _outgrowth.Move(-Outgrowth.Step); };
            }
            if (IncreaseAngle != null)
            {
                IncreaseAngle.OnClick += delegate { _outgrowth.ChangeAngle(MeshFactory.DeltaAngle); };
            }
            if (DecreaseAngle != null)
            {
                DecreaseAngle.OnClick += delegate { _outgrowth.ChangeAngle(-MeshFactory.DeltaAngle); };
            }
            if (ClockwiseButton != null)
            {
                ClockwiseButton.OnClick += delegate { _outgrowth.TurnAround(MeshFactory.DeltaAngle); };
            }
            if (AnticlockwiseButton != null)
            {
                AnticlockwiseButton.OnClick += delegate { _outgrowth.TurnAround(-MeshFactory.DeltaAngle); };
            }
        }

        /// <inheritdoc />
        protected override void CheckIsButtonsEnabled()
        {
            Vector3 pos = _outgrowth.transform.localPosition;
            float maxDistance = _outgrowth.ParentFragment.Length - _outgrowth.Fragment.Diameter / 2;
            float minDistance = _outgrowth.Fragment.Diameter / 2;
            
            bool canForward  = pos.z + Outgrowth.Step < maxDistance;
            bool canBackward = pos.z - Outgrowth.Step > minDistance;
            bool canIncrease = Math.Round(_outgrowth.Angle + MeshFactory.DeltaAngle) <= Outgrowth.MaxAngle;
            bool canDecrease = Math.Round(_outgrowth.Angle - MeshFactory.DeltaAngle) >= Outgrowth.MinAngle;

            if (ForwardButton  != null) ForwardButton.SetEnabled(canForward);
            if (BackwardButton != null) BackwardButton.SetEnabled(canBackward);
            if (IncreaseAngle  != null) IncreaseAngle.SetEnabled(canIncrease);
            if (DecreaseAngle  != null) DecreaseAngle.SetEnabled(canDecrease);
        }

        #region Unity event functions

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            _outgrowth = transform.parent.parent.parent.GetComponent<Outgrowth>();
        }

        #endregion

        #region Private definitions

        private Outgrowth _outgrowth;

        #endregion
    }
}