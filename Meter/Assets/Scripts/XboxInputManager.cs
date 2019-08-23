// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using UnityEngine;

    /// <summary> Обработчик данных с xbox-геймпада. </summary>
public class XboxInputManager : MonoBehaviour
{
    /// <summary> Время после которого зажатая кнопка считается повторно нажатой. </summary>
    public float TimeThreshold = 0.3f;

    /// <summary> Множитель показаний осей. </summary>
    public float TimeFactor = 1;

    public float RepeatTime = 0.5f;

    //TODO: Механизм определения подключен ли геймпад.
    public bool IsGamepadConnected = false;

    public Meter meter;
    
    #region Unity event functioin

    private void Start()
    {
        _camera = Camera.main;
    }
    
    private void Update()
    {
        if (!IsGamepadConnected) return;
        
        CalculatePressingTime();
        
        CheckRepeatPressing("JoystickA", meter.SaveCurrentData);
        CheckFireOnce("JoystickStart", meter.ChooseFile);
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
    
    #endregion
}