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
            : base(sys, diameter, null, true)
        {
            MainLCS.Freeze();
            EndCircle.Freeze();
            EndPlane.Freeze();
            RightAxis.Freeze();

            StartCircle = EndCircle;
            StartPlane  = EndPlane;

            MainLCS.Placement = placement;
            Sys.Evaluate();
        }

        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система C3D. </param>
        /// <param name="diameter"> Диаметр отрезка. </param>
        /// <param name="transform"> Размещение отрезка в Unity. </param>
        public StartFragment(GCMSystem sys, float diameter, Transform transform)
            : this(sys, diameter, MbPlacement3D.FromUnity(transform))
        {
            // Do nothing
        }

        /// <summary> Устанавливает расположение данного фланца </summary>
        /// <param name="placement"> Новое расположение. </param>
        public void SetPlacement(MbPlacement3D placement)
        {
            Sys.Evaluate();
        }
    }
}