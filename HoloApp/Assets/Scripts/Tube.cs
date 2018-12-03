using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloCAD
{
    public class Tube : BaseTube
    {
        private float _length;
        private float _buttonBarOffset;

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
                Label.GetComponent<TextMesh>().text = "Длина: " + _length.ToString("0.00") + "м.";
            }
        }

        // Use this for initialization
        protected new void Start()
        {
            base.Start();
            Length = 0.5f;
            ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Diameter;
            TubeManager.SelectTube(this);
        }

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
