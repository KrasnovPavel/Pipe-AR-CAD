// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Runtime.InteropServices;

namespace UnityC3D
{
    /// <summary> Дескриптор геометрического объекта или ограничения в C3D </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct GCMDescriptor : IEquatable<GCMDescriptor>
    {
        /// <summary> ID в системе C3D. </summary>
        /// <remarks> НЕ ТРОГАТЬ! НЕОБХОДИМО ДЛЯ МАРШАЛИНГА. </remarks>
        private readonly UInt32 id;

        /// <summary> Конструктор. </summary>
        /// <param name="id"></param>
        internal GCMDescriptor(uint id)
        {
            this.id = id;
        }

        public bool Equals(GCMDescriptor other)
        {
            return id == other.id;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is GCMDescriptor other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int) id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{id}";
        }
    }
}