// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using HoloCore;

namespace HoloCAD
{
    public class InputManager : Singleton<InputManager>  
    {
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

        #region Private definitions

        private static readonly Dictionary<string,float> Inputs = new Dictionary<string, float>();

        #endregion
    }
}
