// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using MathExtensions;

namespace UnityC3D
{
    public static class GeometryUtils
    {
        /// <summary> Возвращает расстояние между точкой и прямой. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="line"> Прямая </param>
        /// <returns></returns>
        public static float DistanceLinePoint(GCMPoint point, GCMLine line)
        {
            return Geometry.DistancePointLine(point.Origin, line.Origin, line.Direction);
        }

        /// <summary> Возвращает расстояние между двумя прямыми. </summary>
        /// <remarks> Если прямые не параллельны, то возврящает 0 </remarks>
        /// <param name="line1"> Первая прямая. </param>
        /// <param name="line2"> Вторая прямая. </param>
        /// <returns></returns>
        public static float DistanceLines(GCMLine line1, GCMLine line2)
        {
            return Geometry.DistanceLines(line1.Origin, line1.Direction, line2.Origin, line2.Direction);
        }
        
        /// <summary> Возвращает расстояние между точкой и плоскостью. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <returns></returns>
        public static float DistancePointPlane(GCMPoint point, GCMPlane plane)
        {
            return Geometry.DistancePointPlane(point.Origin, plane.Origin, plane.Normal);
        }

        /// <summary> Возвращает расстояние между прямой и плоскостью. </summary>
        /// <remarks> Если прямая и плоскость не параллельны, то возврящает 0 </remarks>
        /// <param name="line"> Прямая </param>
        /// <param name="plane"> Плоскость. </param>
        /// <returns></returns>
        public static float DistanceLinePlane(GCMLine line, GCMPlane plane)
        {
            return Geometry.DistanceLinePlane(line.Origin, line.Direction, plane.Origin, plane.Normal);
        }

        /// <summary> Возвращает расстояние между двумя плоскостями. </summary>
        /// <remarks> Если плоскости не параллельны, то возврящает 0 </remarks>
        /// <param name="plane1"> Первая плоскость. </param>
        /// <param name="plane2"> Вторая плоскость. </param>
        /// <returns></returns>
        public static float DistancePlanes(GCMPlane plane1, GCMPlane plane2)
        {
            return Geometry.DistancePlanes(plane1.Origin, plane1.Normal, plane2.Origin, plane1.Normal);
        }
    }
}