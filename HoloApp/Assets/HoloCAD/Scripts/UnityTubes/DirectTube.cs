using HoloToolkit.Unity.InputModule;
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
        public GameObject TextDiameter;

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
                CalculateSizeLine();
            }
        }

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            Length = 0.5f;
            ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Data.diameter;
            TubeManager.SelectTube(this);
           // TextDiameter.transform.localScale.Set(0.14f, 0.14f, 0.14f);

        }

        /// <inheritdoc />
        protected override void Update()
        {
            CalculateSizeLine();
        }

        /// <inheritdoc />
        protected override void CalculateSizeLine()
        {
            base.CalculateSizeLine();
            float x = EndPoint.transform.localPosition.x;
            float y = EndPoint.transform.localPosition.y;
            float z = EndPoint.transform.localPosition.z;

            SizeLine.positionCount = 6;
            SizeLine.SetPosition(0, new Vector3(x - ((Data.diameter / 2) * Mathf.Cos(30)), y - ((Data.diameter / 2) * Mathf.Cos(60)), this.transform.position.z));
            SizeLine.SetPosition(1, new Vector3(x + ((Data.diameter / 2) * Mathf.Cos(30)), y + ((Data.diameter / 2) * Mathf.Cos(60)), this.transform.position.z));      
            SizeLine.SetPosition(2, new Vector3(x + (Data.diameter), y - Data.diameter, this.transform.position.z));
            SizeLine.SetPosition(3, new Vector3(x + (Data.diameter), y - Data.diameter, this.transform.position.z - Length));
            SizeLine.SetPosition(4, new Vector3(x + ((Data.diameter / 2) * Mathf.Cos(30)), y + ((Data.diameter / 2) * Mathf.Cos(60)), this.transform.position.z - Length));
            SizeLine.SetPosition(5, new Vector3(x - ((Data.diameter / 2) * Mathf.Cos(30)), y - ((Data.diameter / 2) * Mathf.Cos(60)), this.transform.position.z - Length));

            TextDiameter.GetComponent<TextMesh>().fontSize = (int)Mathf.Round(Length * 50);

            TextDiameter.transform.position = new Vector3((Data.diameter) + x, this.transform.position.y + Length /2 , 0);
            TextDiameter.GetComponent<TextMesh>().text = "Длина: " + _length.ToString("0.00") + "м.";

            TextDiameter.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, TextDiameter.transform.rotation.eulerAngles.z);
            SizeLine.transform.rotation = Quaternion.Euler(SizeLine.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
            
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
