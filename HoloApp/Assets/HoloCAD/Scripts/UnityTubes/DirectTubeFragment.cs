using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloCAD.UI;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, реализующий прямой участок трубы. </summary>
    public class DirectTubeFragment : TubeFragment
    {
        private float _length;
        private float _buttonBarOffset;

        /// <summary> Длина прямого участка трубы. </summary>
        public float Length
        {
            get { return _length; }
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
            get { return _diameter; }
            set
            {
                if (Math.Abs(_diameter - value) < float.Epsilon) return;

                _diameter = value;
                Tube.transform.localScale = new Vector3(Diameter, _length, Diameter);
                ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Diameter;
            }
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            Length = 0.5f;
            ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Diameter;
            TubeManager.SelectTubeFragment(this);
        }
        
        /// <inheritdoc />
        protected override void InputDown(GameObject obj, InputEventData eventData)
        {
            base.InputDown(obj, eventData);

            switch (obj.name)
            {
                case "IncreaseLengthButton":
                    Length += 0.05f;
                    break;
                case "DecreaseLengthButton":
                    Length -= 0.05f;
                    break;
            }
        }
    }
}
