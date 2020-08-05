// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityC3D
{
    /// <summary> Структура, хранящая вектор в C3D. </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MbVector3D
    {
        /// <summary> X - координата. </summary>
        /// <remarks> НЕ ТРОГАТЬ. НЕОБХОДИМО ДЛЯ МАРШАЛИНГА. </remarks>
        public double X;
        /// <summary> Y - координата. </summary>
        /// <remarks> НЕ ТРОГАТЬ. НЕОБХОДИМО ДЛЯ МАРШАЛИНГА. </remarks>
        public double Y;
        /// <summary> Z - координата. </summary>
        /// <remarks> НЕ ТРОГАТЬ. НЕОБХОДИМО ДЛЯ МАРШАЛИНГА. </remarks>
        public double Z;

        /// <summary> Преобразует из вектора Unity. </summary>
        /// <param name="unityVector"> Вектор. </param>
        public static MbVector3D FromUnity(Vector3 unityVector)
        {
            return new MbVector3D
            {
                X = unityVector.x,
                Y = unityVector.y,
                Z = unityVector.z,
            };
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({X:f2}, {Y:f2}, {Z:f2})";
        }

        /// <summary> Преобразует в вектор Unity. </summary>
        public Vector3 ToUnity()
        {
            return new Vector3((float)X, (float)Y, (float)Z);
        }
    }
}