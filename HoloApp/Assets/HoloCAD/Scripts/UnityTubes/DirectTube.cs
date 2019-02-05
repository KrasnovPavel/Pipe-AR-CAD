﻿using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloCAD.UI;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary>
    /// Класс, реализующий прямую трубу.
    /// </summary>
    public class DirectTube : BaseTube
    {
        private float _length;
        private float _buttonBarOffset;

        /// <value> Длина трубы. </value>
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

                Tube.transform.localScale = new Vector3(Data.diameter, _length, Data.diameter);
                EndPoint.transform.localPosition = new Vector3(0, 0, _length);
                Label.GetComponent<TextMesh>().text = "Длина: " + _length.ToString("0.00") + "м.";
            }
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            Length = 0.5f;
            ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Data.diameter;
            TubeManager.SelectTube(this);
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
