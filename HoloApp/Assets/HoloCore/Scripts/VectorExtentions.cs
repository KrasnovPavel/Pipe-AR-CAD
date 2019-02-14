using UnityEngine;

namespace HoloCore
{
    public static class VectorExtentions
    {
        public static float ProjectOn(this Vector3 vector, Vector3 axis)
        {
            return Mathf.Cos(Vector3.Angle(vector, axis) * Mathf.Deg2Rad) * vector.magnitude / axis.magnitude;
        }
    }
}