// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using HoloCAD.UnityTubes;
using HoloCore.UI;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Обработчик данных с xbox-геймпада. </summary>
    public class XboxInputManager : MonoBehaviour
    {
        /// <summary> Время после которого зажатая кнопка считается повторно нажатой. </summary>
        public float TimeThreshold = 0.3f;

        /// <summary> Множитель показаний осей. </summary>
        public float TimeFactor = 1;

        #region Unity event functioin

        private void Start()
        {
            _camera = Camera.main;
        }
        
        private void Update()
        {
            CalculatePressingTime();
            
            CheckFireOnce("JoystickA", Click);
            CheckFireOnce("JoystickStart", TubeManager.SaveScene);
            CheckFireOnce("JoystickX", delegate{ TubeManager.CreateTube(); });

            TubeFragment fragment = TubeManager.SelectedTubeFragment;
            if (fragment == null) return;
            
            CheckFireOnce("LeftBumper", fragment.AddBendFragment);
            CheckFireOnce("JoystickBack", fragment.RemoveThisFragment);
            CheckRepeatPressingAxis("DPADHorizontal", TubeManager.SelectNext, 
                                                                  TubeManager.SelectPrevious);
            CheckFireOnce("JoystickY", fragment.TogglePlacing);
            CheckFireOnce("JoystickB", fragment.Owner.CreateTubesConnector);
            
            switch (fragment)
            {
                case StartTubeFragment startFragment:
                    CheckFireOnce("RightBumper", startFragment.AddDirectFragment);
                    CheckRepeatPressingAxis("LeftStickHorizontal", 
                                            startFragment.IncreaseDiameter, 
                                            startFragment.DecreaseDiameter);
                    break;
                case DirectTubeFragment directFragment:
                    CheckAxisChange("LeftStickHorizontal", directFragment.ChangeLength, TimeFactor);
                    break;
                case BendedTubeFragment bendedFragment:
                    CheckFireOnce("RightBumper", bendedFragment.AddDirectFragment);
                    CheckAxisChange("RightStickHorizontal", bendedFragment.ChangeAngle, TimeFactor * 6);
                    CheckAxisChange("LeftStickHorizontal", bendedFragment.TurnAround, TimeFactor * 6);
                    CheckRepeatPressing("DPADVertical", bendedFragment.ChangeRadius);
                    break;
            }
        }

        #endregion
        
        #region Private definitions

        private Camera _camera;

        private readonly Dictionary<string, float> _pressingTime = new Dictionary<string, float>();
        private readonly HashSet<string> _fireOnce = new HashSet<string>();
        
        private void CalculatePressingTime()
        {
            string[] keys = new string[_pressingTime.Count];
            _pressingTime.Keys.CopyTo(keys, 0);
            foreach (string key in keys)
            {
                if (Input.GetAxis(key) < -0.3f || Input.GetAxis(key) > 0.3f)
                {
                    _pressingTime[key] += Time.deltaTime;
                    if (_pressingTime[key] > TimeThreshold && !_fireOnce.Contains(key)) _pressingTime[key] = 0;
                }
                else
                {
                    _pressingTime[key] = 0;
                }
            }
        }
        
        private void CheckRepeatPressing(string buttonName, Action action)
        {
            if (!_pressingTime.ContainsKey(buttonName)) _pressingTime.Add(buttonName, 0);

            if (Math.Abs(_pressingTime[buttonName] - Time.deltaTime) > float.Epsilon) return;
            
            if (Input.GetAxis(buttonName) > 0.3f) action();
        }

        private void CheckRepeatPressingAxis(string buttonName, Action actionPlus, Action actionMinus)
        {
            if (!_pressingTime.ContainsKey(buttonName)) _pressingTime.Add(buttonName, 0);

            if (Math.Abs(_pressingTime[buttonName] - Time.deltaTime) > float.Epsilon) return;

            if      (Input.GetAxis(buttonName) >  0.3f) actionPlus();
            else if (Input.GetAxis(buttonName) < -0.3f) actionMinus();
        }

        private void CheckFireOnce(string buttonName, Action action)
        {
            if (!_fireOnce.Contains(buttonName))
            {
                _pressingTime.Add(buttonName, 0);
                _fireOnce.Add(buttonName);
            }

            if (Math.Abs(_pressingTime[buttonName] - Time.deltaTime) > float.Epsilon) return;
            
            if (Input.GetAxis(buttonName) > 0.3f) action();
        }

        private void CheckAxisChange(string axisName, Action<float> action, float timeFactor = 1)
        {
            if (Input.GetAxis(axisName) < 0.3f || Input.GetAxis(axisName) > 0.3f)
            {
                action(Input.GetAxis(axisName) * Time.deltaTime * timeFactor);
            }
        }
        
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