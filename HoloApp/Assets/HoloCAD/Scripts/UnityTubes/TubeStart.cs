using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary>
    /// Класс, реализующий фланец трубы.
    /// </summary>
    public class TubeStart : BaseTube
    {
        private const float Length = 0.03f;
        private bool _isPlacing;
        private GestureRecognizer _recognizer;
        public GameObject TextDiameter;

        /// <inheritdoc />
        protected override void Start()
        {
            base.Start();
            EndPoint.transform.localPosition = new Vector3(0, 0, Length);

            StandardName = TubeLoader.GetStandardNames()[0];
            Data = TubeLoader.GetAvailableTubes(StandardName)[0];
            
            TubeManager.AddTube(this);
            TubeManager.SelectTube(this);
        }

        private void Awake()
        {
#if ENABLE_WINMD_SUPPORT
            _isPlacing = true;
            _recognizer = new GestureRecognizer();
            _recognizer.Tapped += args =>
            {
                if (_isPlacing)
                {
                    _isPlacing = false;
                    _recognizer.StopCapturingGestures();
                }
            };
            _recognizer.StartCapturingGestures();
#endif
        }
    
        /// <inheritdoc />
        protected override void Update()
        {
            base.Update();
            Tube.transform.localScale = new Vector3(Data.diameter, Length, Data.diameter);
            if (Data.diameter < 0)
            {
                Data.diameter = 0.05f;
            }
            LabelText.text = "Диаметр: " + Data.diameter.ToString("0.000") + "м.";
    
            Tube.GetComponent<MeshCollider>().enabled = !_isPlacing;
            TubeFactory.Instance.MapCollider.enabled = _isPlacing;
            TubeFactory.Instance.MapRenderer.enabled = _isPlacing;
            if (_isPlacing)
            {
                Place();
            }
        }

        /// <inheritdoc />
        protected override void CalculateSizeLine()
        {
            base.CalculateSizeLine();
            float x = EndPoint.transform.localPosition.x;
            float y = EndPoint.transform.localPosition.y;
            float z = EndPoint.transform.localPosition.z;

            SizeLine.positionCount = 6;
            SizeLine.SetPosition(0, new Vector3(x - ((Data.diameter / 2) * Mathf.Cos(30)), y - ((Data.diameter / 2) * Mathf.Cos(60)), transform.position.z));
            SizeLine.SetPosition(1, new Vector3(x + ((Data.diameter / 2) * Mathf.Cos(30)), y + ((Data.diameter / 2) * Mathf.Cos(60)), transform.position.z));

            SizeLine.SetPosition(2, new Vector3(x + (Data.diameter), y - Data.diameter, transform.position.z));
            SizeLine.SetPosition(3, new Vector3(x + (Data.diameter), y - Data.diameter, transform.position.z - Length));
            SizeLine.SetPosition(4, new Vector3(x + ((Data.diameter / 2) * Mathf.Cos(30)), y + ((Data.diameter / 2) * Mathf.Cos(60)), transform.position.z - Length));
            SizeLine.SetPosition(5, new Vector3(x - ((Data.diameter / 2) * Mathf.Cos(30)), y - ((Data.diameter / 2) * Mathf.Cos(60)), transform.position.z - Length));

            TextDiameter.transform.position = new Vector3(x + (Data.diameter*3/4) + (Data.diameter / 4), 0, transform.position.z + Data.diameter);
            TextDiameter.GetComponent<TextMesh>().text = "Диаметр: " + Data.diameter.ToString("0.000") + "м.";

            TextDiameter.transform.rotation = Camera.main.transform.rotation;
        }

        private void Place()
        {
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;
    
            RaycastHit hitInfo;
            if (!Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f)) return;
            
            transform.position = hitInfo.point + Vector3.up * 0.02f;
            transform.rotation = Quaternion.LookRotation(hitInfo.normal);
        }
    
        /// <inheritdoc />
        protected override void InputDown(GameObject obj, InputEventData eventData)
        {
            base.InputDown(obj, eventData);
            
            switch (obj.name)
            {
                case "IncreaseDiameterButton":
                    Data = TubeLoader.GetBigger(Data, StandardName);
                    CalculateSizeLine();
                    break;
                case "DecreaseDiameterButton":
                    Data = TubeLoader.GetSmaller(Data, StandardName);
                    CalculateSizeLine();
                    break;
                case "PlacingButton":
                    _isPlacing = true;
                    _recognizer.StartCapturingGestures();
                    break;
            }
        }
    }
}
