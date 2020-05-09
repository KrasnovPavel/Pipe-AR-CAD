// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.UnityTubes;
using HoloCore;
using HoloCore.UI;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс, связывающий нажатия на геймпад с функциями общими для всей сцены. </summary>
    public class GamepadBindings : MonoBehaviour
    {
        #region Unity event functions

        private void Start()
        {
            _camera = Camera.main;
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickA, null, Click);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickStart, null, TubeManager.SaveScene);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickBack, null, TubeManager.LoadScene);
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
            
            SimpleButton button = hitInfo.transform.GetComponent<SimpleButton>();
            if (button != null)
            {
                button.Press();
                button.Release(true);
                return;
            }

            TubeClickerReceiver receiver = hitInfo.transform.GetComponent<TubeClickerReceiver>();
            if (receiver != null)
            {
                receiver.Click();
                return;
            }

            Transform tube = hitInfo.transform.Find("Tube");
            if (tube != null) receiver = tube.GetComponent<TubeClickerReceiver>();
            if (receiver != null) receiver.Click();
        }

        #endregion

    }
}