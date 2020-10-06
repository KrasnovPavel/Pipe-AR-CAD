// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCore;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept
{
    public class GCMSystemBehaviour : Singleton<GCMSystemBehaviour>
    {
        public static readonly GCMSystem System = new GCMSystem();
        
        public static readonly GCMPlane HorizontalPlane = new GCMPlane(System, Vector3.zero, Vector3.up, System.GroundLCS);

        static GCMSystemBehaviour()
        {
            HorizontalPlane.Freeze();
        }

        private void OnDestroy()
        {
            System.Dispose();
        }
    }
}