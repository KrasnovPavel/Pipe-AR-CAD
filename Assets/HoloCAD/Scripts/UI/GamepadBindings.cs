// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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
            // ReSharper disable once PossibleNullReferenceException
            _camera = Camera.main.transform;
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickA, null, Click);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickStart, null, SceneManager.SaveScene);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickBack, null, SceneManager.LoadScene);
        }

        #endregion

        #region Private definitions

        private Transform _camera;
        
        private void Click()
        {
            Vector3 headPosition = _camera.position;
            Vector3 gazeDirection = _camera.forward;

            RaycastHit hitInfo;
            if (!Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f)) return;
            
            SimpleButton button = hitInfo.transform.GetComponent<SimpleButton>();
            if (button != null)
            {
                button.Press();
                button.Release(true);
            }
        }

        #endregion
    }
}