using System;
using UnityEngine;
using HoloCAD.UI;
using HoloCore.UI;
using JetBrains.Annotations;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, реализующий прямой участок трубы. </summary>
    public class DirectTubeFragment : TubeFragment
    {
        /// <summary> Кнопка увеличения длины. </summary>
        [Tooltip("Кнопка увеличения длины.")]
        [CanBeNull] public Button3D IncreaseLengthButton;

        /// <summary> Кнопка уменьшения длины. </summary>
        [Tooltip("Кнопка уменьшения длины.")]
        [CanBeNull] public Button3D DecreaseLengthButton;
        
        /// <summary> Длина прямого участка трубы. </summary>
        public float Length
        {
            get => _length;
            set
            {
                if (value <= 0)
                {
                    return;
                }
                _length = value;

                Tube.transform.localScale = new Vector3(Diameter, _length, Diameter);
                EndPoint.transform.localPosition = new Vector3(0, 0, _length);
                Label.GetComponent<TextMesh>().text = $"Длина: {_length:0.00}м.";
            }
        }

        /// <inheritdoc />
        public override float Diameter
        {
            get => _diameter;
            set
            {
                if (Math.Abs(_diameter - value) < float.Epsilon) return;

                _diameter = value;
                Tube.transform.localScale = new Vector3(Diameter, _length, Diameter);
                ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Diameter;
            }
        }

        /// <summary> Увеличивает длину. </summary>
        /// <param name="delta"> Изменение длины. </param>
        public void IncreaseLength(float delta = 0.05f)
        {
            Length += delta;
        }
        
        /// <summary> Уменьшает длину. </summary>
        /// <param name="delta"> Изменение длины. </param>
        public void DecreaseLength(float delta = 0.05f)
        {
            Length -= delta;
        }

        /// <inheritdoc />
        protected override void InitButtons()
        {
            base.InitButtons();
            if (IncreaseLengthButton != null) IncreaseLengthButton.OnClick += delegate { IncreaseLength(); };
            if (DecreaseLengthButton != null) DecreaseLengthButton.OnClick += delegate { DecreaseLength(); };
        }

        #region Unity event functions

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            Length = 0.5f;
            ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Diameter;
            TubeManager.SelectTubeFragment(this);
        }

        #endregion

        #region Private definitions

        private float _length;
        private float _buttonBarOffset;

        #endregion
    }
}
