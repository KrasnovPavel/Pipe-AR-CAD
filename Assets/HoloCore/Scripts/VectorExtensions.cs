// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityEngine;

namespace HoloCore
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

        /// <summary> Проверяет равенство двух векторов с учетом неточности значений типа float. </summary>
        /// <param name="vector"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool FloatEquals(this Vector3 vector, Vector3 other)
        {
            return Math.Abs(vector.x - other.x) < float.Epsilon
                   && Math.Abs(vector.y - other.y) < float.Epsilon
                   && Math.Abs(vector.z - other.z) < float.Epsilon;
        }
    }
}