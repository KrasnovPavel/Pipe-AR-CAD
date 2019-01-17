using System.Collections.Generic;
using HoloCAD.UI;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace HoloCAD
{
    public class InputManager : Singleton<InputManager>  {
        private static readonly Dictionary<string,float> Inputs = new Dictionary<string, float>();

        public static float GetAxis(string axis){
            if(!Inputs.ContainsKey(axis)){
                Inputs.Add(axis, 0);
            }
         
            return Inputs[axis];
        }
     
        public static void SetAxis(string axis, float value){
            if(!Inputs.ContainsKey(axis)){
                Inputs.Add(axis, 0);
            }
         
            Inputs[axis] = value;
        }
    }
}
