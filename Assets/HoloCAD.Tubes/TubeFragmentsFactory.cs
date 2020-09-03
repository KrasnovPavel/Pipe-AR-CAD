// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.Tubes.C3D;
using HoloCAD.Tubes.Unity;
using HoloCore;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes
{
    public class TubeFragmentsFactory : Singleton<TubeFragmentsFactory>
    {
        public static GCMSystem MainSys = new GCMSystem();
        public GameObject DirectTubePrefab;

        public static GameObject CreateDirect(
            GCMSystem sys = null, 
            TubeFragment parent = null, 
            float diameter = 0.01f,
            float length = 0.5f)
        {
            if (sys == null) sys = MainSys;
            
            var f  = new DirectFragment(sys, diameter, length, parent);
            var go = Instantiate(Instance.DirectTubePrefab);
            go.GetComponent<DirectFragmentView>().Fragment = f;
            return go;
        }
    }
}