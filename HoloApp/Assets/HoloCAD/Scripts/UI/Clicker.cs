using HoloCAD.UnityTubes;
using HoloCore.UI;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс, реализующий нажатия на объекты. </summary>
    public class Clicker : MonoBehaviour
    {
        #region Unity event functions

        private void Start()
        {
            _camera = Camera.main;
            
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickA, null, Click);
        }

        #endregion

        #region Private definitions

        private Camera _camera;
        
        private void Click()
        {
            Vector3 headPosition = _camera.transform.position;
            Vector3 gazeDirection = _camera.transform.forward;

            RaycastHit hitInfo;
            if (!Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f)) return;
            Debug.Log(hitInfo.transform.name);
            
            SimpleButton button = hitInfo.transform.GetComponent<SimpleButton>();
            if (button != null)
            {
                button.Press();
                button.Release();
            }
            else
            {
                TubeClickerReceiver receiver = hitInfo.transform.GetComponent<TubeClickerReceiver>();
                if (receiver != null)
                {
                    receiver.Click();
                }
                else
                {
                    Transform tube = hitInfo.transform.Find("Tube");
                    if (tube != null) receiver = tube.GetComponent<TubeClickerReceiver>();
                    
                    if (receiver != null) receiver.Click();
                }
            }
        }

        #endregion

    }
}