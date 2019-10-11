// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HoloCAD.IO
{
    /// <summary> Коренной объект Json. </summary>
        [Serializable]
        public class ExpTubesArray
        {
            // ReSharper disable once InconsistentNaming
            /// <summary> Массив экспортируемых труб. </summary>
            public List<ExpTube> tubes = new List<ExpTube>();
        }

        /// <summary> Экспортируемый объект трубы. </summary>
        [Serializable]
        public class ExpTube
        {
            // ReSharper disable once InconsistentNaming
            /// <summary> Массив экспортируемых фрагментов трубы. </summary>
            public List<ExpFragment> fragments = new List<ExpFragment>();

            // ReSharper disable once InconsistentNaming
            /// <summary> Диаметр трубы. </summary>
            public double diameter;

            // ReSharper disable once InconsistentNaming
            /// <summary> Толщина стенки трубы. </summary>
            public double width;
            
            // ReSharper disable once InconsistentNaming
            /// <summary> Наименование стандарта. </summary>
            public string standart_name;
        }

        /// <summary> Экспортируемый объект фрагмента трубы. </summary>
        [Serializable]
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class ExpFragment
        {
            public double[] transform;

            /// <summary> Тип фрагмента: Direct или Bended. </summary>
            public string type;

            /// <summary> Длина фрагмента (только если direct). </summary>
            public double length;

            /// <summary> Радиус погиба (только если bended). </summary>
            public double radius;

            /// <summary> Угол погиба (только если bended). </summary>
            public float bendAngle;

            /// <summary> Угол поворота (только если bended) </summary>
            public float rotationAngle;
        }

}