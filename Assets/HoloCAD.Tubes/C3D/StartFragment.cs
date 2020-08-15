// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.C3D
{
    /// <summary> Фланец (начальный отрезок трубы). </summary>
    public class StartFragment : TubeFragment
    {
        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система C3D. </param>
        /// <param name="diameter"> Диаметр отрезка. </param>
        /// <param name="placement"> Размещение отрезка. </param>
        public StartFragment(GCMSystem sys, float diameter, MbPlacement3D placement) 
            : base(sys, diameter, null)
        {
            MainLCS.Freeze();
            EndPlane.Freeze();
            EndCircle.Freeze();
            MainLCS.Placement = placement;
            EndCircle.Placement = placement;
            EndPlane.Placement = placement;
            Sys.Evaluate();
        }
        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система C3D. </param>
        /// <param name="diameter"> Диаметр отрезка. </param>
        /// <param name="transform"> Размещение отрезка в Unity. </param>
        public StartFragment(GCMSystem sys, float diameter, Transform transform)
            : this (sys, diameter, MbPlacement3D.FromUnity(transform))
        {
            // Do nothing
        }
    }
}