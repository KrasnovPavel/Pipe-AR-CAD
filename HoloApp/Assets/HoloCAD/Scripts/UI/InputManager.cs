using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace HoloCAD.UI
{
    public class InputManager : MonoBehaviour
    {
        private GestureRecognizer _recognizer;
        void Awake()
        {
#if ENABLE_WINMD_SUPPORT
            _recognizer = new GestureRecognizer();
            _recognizer.Tapped += args =>
            {
               OnClick();
            };
            _recognizer.StartCapturingGestures();
#endif
        }

        private void OnClick()
        {
            RaycastHit _hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out _hit, Mathf.Infinity))
            {
                if (_hit.transform.GetComponent<Button3D>())
                {
                    _hit.transform.GetComponent<Button3D>().OnClick();
                }
            }
        }
        // Use this for initialization
        void Start()
        {

        }
    }
}
