// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityEngine;

namespace HoloCore
{
    public static class VectorExtensions
    {
        public static float ProjectOn(this Vector3 vector, Vector3 axis)
        {
            return Mathf.Cos(Vector3.Angle(vector, axis) * Mathf.Deg2Rad) * vector.magnitude / axis.magnitude;
        }
    }
}