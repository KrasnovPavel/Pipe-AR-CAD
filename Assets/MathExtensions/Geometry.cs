// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityEngine;

namespace MathExtensions
{
    public static class Geometry
    {
        /// <summary> Возвращает расстояние между точкой и прямой. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="lineOrigin"> Точка на прямой. </param>
        /// <param name="lineDirection"> Направляющий вектор прямой. </param>
        /// <returns></returns>
        public static float DistancePointLine(Vector3 point, Vector3 lineOrigin, Vector3 lineDirection)
        {
            var originToPoint = point - lineOrigin;
            return (float)Math.Sin(Vector3.Angle(originToPoint, lineDirection) / 180f * Math.PI) * originToPoint.magnitude;
        }

        /// <summary> Возвращает расстояние между двумя прямыми. </summary>
        /// <remarks> Если прямые не параллельны, то возвращает 0 </remarks>
        /// <param name="l1Origin"> Точка на первой прямой. </param>
        /// <param name="l1Direction"> Направляющий вектор первой прямой. </param>
        /// <param name="l2Origin"> Точка на второй прямой. </param>
        /// <param name="l2Direction"> Направляющий вектор второй прямой. </param>
        /// <returns></returns>
        public static float DistanceLines(Vector3 l1Origin, Vector3 l1Direction, Vector3 l2Origin, Vector3 l2Direction)
        {
            if (l1Direction.IsCollinear(l2Direction))
            {
                return DistancePointLine(l1Origin, l2Origin, l2Direction);
            }

            return DistancePointPlane(l1Origin, l2Origin, Vector3.Cross(l1Direction, l2Direction));
        }

        /// <summary> Возвращает расстояние между точкой и плоскостью. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="planeOrigin"> Точка на плоскости. </param>
        /// <param name="planeNormal"> Нормаль к плоскости. </param>
        /// <returns></returns>
        public static float DistancePointPlane(Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
        {
            var originToPoint = point - planeOrigin;
            return (float)Math.Abs(Math.Cos(Vector3.Angle(originToPoint, planeNormal) / 180f * Math.PI)) * originToPoint.magnitude;
        }

        /// <summary> Возвращает расстояние между прямой и плоскостью. </summary>
        /// <remarks> Если прямая и плоскость не параллельны, то возвращает 0 </remarks>
        /// <param name="lineOrigin"> Точка на прямой. </param>
        /// <param name="lineDirection"> Направляющий вектор прямой. </param>
        /// <param name="planeOrigin"> Точка на плоскости. </param>
        /// <param name="planeNormal"> Нормаль к плоскости. </param>
        /// <returns></returns>
        public static float DistanceLinePlane(Vector3 lineOrigin, Vector3 lineDirection, Vector3 planeOrigin, Vector3 planeNormal)
        {
            if (!lineDirection.IsPerpendicular(planeNormal)) return 0;

            return DistancePointPlane(lineOrigin, planeOrigin, planeNormal);
        }

        /// <summary> Возвращает расстояние между двумя плоскостями. </summary>
        /// <remarks> Если плоскости не параллельны, то возвращает 0 </remarks>
        /// <param name="pl1Origin"> Точка на первой плоскости. </param>
        /// <param name="pl1Normal"> Нормаль к первой плоскости. </param>
        /// <param name="pl2Origin"> Точка на второй плоскости. </param>
        /// <param name="pl2Normal"> Нормаль к второй плоскости. </param>
        /// <returns></returns>
        public static float DistancePlanes(Vector3 pl1Origin, Vector3 pl1Normal, Vector3 pl2Origin, Vector3 pl2Normal)
        {
            if (!pl1Normal.IsCollinear(pl2Normal)) return 0;

            return DistancePointPlane(pl1Origin, pl2Origin, pl2Normal);
        }
    }
}