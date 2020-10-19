// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCore;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes
{
    /// <summary> Компонент, хранящий GCMSystem, относящийся к сцене. </summary>
    public class GCMSystemBehaviour : Singleton<GCMSystemBehaviour>
    {
        /// <summary> Система ограничений сцены. </summary>
        public static readonly GCMSystem System = new GCMSystem();

        /// <summary> Горизонтальная плоскость сцены. </summary>
        public static readonly GCMPlane HorizontalPlane =
            new GCMPlane(System, Vector3.zero, Vector3.up, System.GroundLCS);

        /// <summary> Статический конструктор. </summary>
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
