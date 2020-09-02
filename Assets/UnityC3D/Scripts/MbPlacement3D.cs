// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityC3D
{
    /// <summary> Размещение объекта в системе координат C3D. </summary>
    /// <remarks> Аналог UnityEngine.Transform, но в правостронней системе координат. </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct MbPlacement3D
    {
        #region Private definitions

        /// <summary> Положение начала локальной системы координат. </summary>
        /// <remarks> НЕ ТРОГАТЬ. НЕОБХОДИМО ДЛЯ МАРШАЛИНГА. </remarks>
        private MbVector3D _origin;

        /// <summary> Направление первой оси. </summary>
        /// <remarks> НЕ ТРОГАТЬ. НЕОБХОДИМО ДЛЯ МАРШАЛИНГА. </remarks>
        private MbVector3D _axisX;

        /// <summary> Направление второй оси. </summary>
        /// <remarks> НЕ ТРОГАТЬ. НЕОБХОДИМО ДЛЯ МАРШАЛИНГА. </remarks>
        private MbVector3D _axisY;

        /// <summary> Направление третьей оси. </summary>
        /// <remarks> НЕ ТРОГАТЬ. НЕОБХОДИМО ДЛЯ МАРШАЛИНГА. </remarks>
        private MbVector3D _axisZ;

        /// <remarks> НЕ ТРОГАТЬ. НЕОБХОДИМО ДЛЯ МАРШАЛИНГА. </remarks>
        private Byte _flag;

        #endregion

        /// <summary> Положение начала локальной системы координат. </summary>
        public Vector3 Origin
        {
            get => _origin.ToUnity();
            set => _origin = MbVector3D.FromUnity(value);
        }

        /// <summary> Направление оси X. </summary>
        public Vector3 AxisX
        {
            get => _axisX.ToUnity();
            set => _axisX = MbVector3D.FromUnity(value);
        }

        /// <summary> Направление оси Y. </summary>
        public Vector3 AxisY
        {
            get => _axisZ.ToUnity();
            set => _axisZ = MbVector3D.FromUnity(value);
        }

        /// <summary> Направление оси Z. </summary>
        public Vector3 AxisZ
        {
            get => _axisY.ToUnity();
            set => _axisY = MbVector3D.FromUnity(value);
        }

        /// <summary> Применяет это размещение к переданному Transform. </summary>
        /// <param name="tr"></param>
        public void Apply(Transform tr)
        {
            tr.position = Origin;
            tr.LookAt(Origin + AxisZ, AxisY);
        }

        /// <summary> Создаёт размещение из Transform. </summary>
        /// <param name="tr"> Исходный Transform. </param>
        /// <returns> Новое размещение. </returns>
        public static MbPlacement3D FromUnity(Transform tr)
        {
            return new MbPlacement3D
            {
                _origin = MbVector3D.FromUnity(tr.position),
                _axisX  = MbVector3D.FromUnity(tr.right),
                _axisZ  = MbVector3D.FromUnity(tr.up),
                _axisY  = MbVector3D.FromUnity(tr.forward),
            };
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{_origin} {_axisX} {_axisY} {_axisZ}";
        }

        public bool Equals(MbPlacement3D other, double eps = 0.0001)
        {
            return _origin.Equals(other._origin, eps)
                   && _axisX.Equals(other._axisX, eps)
                   && _axisY.Equals(other._axisY, eps)
                   && _axisZ.Equals(other._axisZ, eps);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is MbPlacement3D other && Equals(other);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _origin.GetHashCode();
                hashCode = (hashCode * 397) ^ _axisX.GetHashCode();
                hashCode = (hashCode * 397) ^ _axisY.GetHashCode();
                hashCode = (hashCode * 397) ^ _axisZ.GetHashCode();
                return hashCode;
            }
        }
    }
}