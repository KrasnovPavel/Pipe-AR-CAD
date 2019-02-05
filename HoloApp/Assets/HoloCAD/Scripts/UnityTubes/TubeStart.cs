using HoloToolkit.Unity.InputModule;
using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using UnityEngine.XR.WSA.Input;
#endif

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
        private Camera _camera;
#if ENABLE_WINMD_SUPPORT
        private GestureRecognizer _recognizer;
#endif

        /// <inheritdoc />
        protected override void Start()
        {
            _camera = Camera.main;
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
            LabelText.text = "Диаметр: " + Data.diameter.ToString("0.000") + "м.";
    
            Tube.GetComponent<MeshCollider>().enabled = !_isPlacing;
            TubeFactory.ShowGrid(_isPlacing);
            if (_isPlacing)
            {
                Place();
            }
        }

        private void Place()
        {
            Vector3 headPosition = _camera.transform.position;
            Vector3 gazeDirection = _camera.transform.forward;
    
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
                    break;
                case "DecreaseDiameterButton":
                    Data = TubeLoader.GetSmaller(Data, StandardName);
                    break;
                case "PlacingButton":
                    _isPlacing = true;
#if ENABLE_WINMD_SUPPORT
                    _recognizer.StartCapturingGestures();
#endif
                    break;
            }
        }
    }
}
