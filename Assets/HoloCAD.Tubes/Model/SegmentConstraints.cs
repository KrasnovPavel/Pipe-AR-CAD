// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityC3D;

namespace HoloCAD.Tubes.Model
{
    /// <summary> Класс расширения. Создаёт новые виды ограничений для отрезков. </summary>
    public static class SegmentConstraints
    {
        /// <summary> Устанавливает принадлежность отрезка к плоскости. </summary>
        /// <param name="sys"> Система ограничений. </param>
        /// <param name="segment"> Отрезок. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <returns> Объект ограничения. </returns>
        public static GCMConstraint MakeCoincident(this GCMSystem sys, Segment segment, GCMPlane plane)
        {
            return sys.MakeCoincident(segment.Line, plane);
        }

        /// <summary> Устанавливает параллельность отрезка и линии. </summary>
        /// <param name="sys"> Система ограничений. </param>
        /// <param name="segment"> Отрезок. </param>
        /// <param name="line"> Линия. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public static GCMConstraint MakeParallel(this GCMSystem sys, Segment segment, GCMLine line,
                                                 GCMAlignment   alignment = GCMAlignment.NoAlignment)
        {
            return sys.MakeParallel(segment.Line, line, alignment);
        }

        /// <summary> Устанавливает параллельность отрезка и плоскости. </summary>
        /// <param name="sys"> Система ограничений. </param>
        /// <param name="segment"> Отрезок. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public static GCMConstraint MakeParallel(this GCMSystem sys, Segment segment, GCMPlane plane,
                                                 GCMAlignment   alignment = GCMAlignment.NoAlignment)
        {
            return sys.MakeParallel(segment.Line, plane, alignment);
        }

        /// <summary> Устанавливает перпендикулярность отрезка и плоскости. </summary>
        /// <param name="sys"> Система ограничений. </param>
        /// <param name="segment"> Отрезок. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public static GCMConstraint MakePerpendicular(this GCMSystem sys, Segment segment, GCMPlane plane,
                                                      GCMAlignment   alignment = GCMAlignment.NoAlignment)
        {
            return sys.MakePerpendicular(segment.Line, plane, alignment);
        }
    }
}
