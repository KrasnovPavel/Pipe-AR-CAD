// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityEngine;

namespace MathExtensions
{
    public static class VectorExtensions
    {
        /// <summary> Проецирует вектор на ось. </summary>
        /// <param name="vector"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float ProjectOn(this Vector3 vector, Vector3 axis)
        {
            return Mathf.Cos(Vector3.Angle(vector, axis) * Mathf.Deg2Rad) * vector.magnitude / axis.magnitude;
        }

        /// <summary> Проверяет векторы на коллинеарность. </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsCollinear(this Vector3 first, Vector3 second)
        {
            return Math.Abs(NormAngle(Vector3.Angle(first, second))) < float.Epsilon;
        }

        /// <summary> Проверяет векторы на перпендикулярность. </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsPerpendicular(this Vector3 first, Vector3 second)
        {
            return Math.Abs(NormAngle(Vector3.Angle(first, second)) - 90) < float.Epsilon;
        }

        /// <summary> Проверяет равенство двух векторов с учетом неточности значений типа float. </summary>
        /// <param name="vector"></param>
        /// <param name="other"></param>
        /// <param name="eps"> Допустимая погрешность. </param>
        /// <returns></returns>
        public static bool FloatEquals(this Vector3 vector, Vector3 other, float eps = float.Epsilon)
        {
            return Math.Abs(vector.x - other.x) < eps
                   && Math.Abs(vector.y - other.y) < eps
                   && Math.Abs(vector.z - other.z) < eps;
        }

        /// <summary> Возвращает угол, нормированный в интервал [0, 180) градусов. </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static float NormAngle(float angle)
        {
            if (angle >= 180) angle -= 360;
            if (angle < 0) angle = -angle;
            if (Math.Abs(angle - 180) < float.Epsilon) angle = 0;

            return angle;
        }
    }
}