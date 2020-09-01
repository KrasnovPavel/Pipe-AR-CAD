// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace UnityC3D
{
    /// <summary> Структура полей данных, представляющих геометрический объект. </summary>
    /// <remarks>
    /// Эта простая структура данных представляет варианты геометрических типов, с которыми работает решатель.
    /// { GCM_POINT    origin } - simple point
    /// { GCM_SPHERE   origin radiusA } - center and radius of a sphere
    /// { GCM_LINE     origin axisZ } - point and direction of a line;
    /// { GCM_PLANE    origin axisZ } - point and normal of a plane;
    /// { GCM_CIRCLE   origin axisZ radiusA } - center, rotation axis and radius;
    /// { GCM_CYLINDER origin axisZ radiusA } - center, rotation axis and radius;
    /// { GCM_CONE     origin axisZ radiusA radiusB } - center, rotation axis and two radiuses;
    /// { GCM_TORUS    origin axisZ radiusA radiusB };
    /// { GCM_LCS      origin axisZ axisX axisY } - local coordinate system that specify a solid position.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct GCMGeometry
    {
        /// <summary> Тип геометрии. </summary>
        internal GCMGeomType type;

        /// <summary> Точка позиционирования геометрического объекта. </summary>
        internal MbVector3D origin;

        /// <summary> Направляющий вектор прямой или вектор нормали плоскости. </summary>
        internal MbVector3D axisZ;

        /// <summary> Ось X локальной системы координат. </summary>
        internal MbVector3D axisX;

        /// <summary> Ось Y локальной системы координат. </summary>
        internal MbVector3D axisY;

        /// <summary> Радиус окружности, сферы или цилиндра либо радиус основания конуса, "большой" радиус тора. </summary>
        internal double radiusA;

        /// <summary> "Малый" радиус тора или конуса. </summary>
        internal double radiusB;
    }

    /// <summary>
    /// Словарь типов геометрических примитивов.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum GCMGeomType
    {
        /// <summary> Пустой геометрический объект. </summary>
        GCM_NULL_GTYPE = 0,

        /// <summary> Точка. </summary>
        GCM_POINT,

        /// <summary> Прямая. </summary>
        GCM_LINE,

        /// <summary> Плоскость. </summary>
        GCM_PLANE,

        /// <summary> Цилиндр. </summary>
        GCM_CYLINDER,

        /// <summary> Конус. </summary>
        GCM_CONE,

        /// <summary> Сферическая поверхность. </summary>
        GCM_SPHERE,

        /// <summary> Тороидальная поверхность. </summary>
        GCM_TORUS,

        /// <summary> Окружность. </summary>
        GCM_CIRCLE,

        /// <summary> Система координат. </summary>
        GCM_LCS,

        /// <summary> Точка и пара ортонормированных векторов. </summary>
        GCM_MARKER,

        /// <summary> Сплайновая кривая. </summary>
        GCM_SPLINE,

        /// <summary> Unit vector (internal use only) </summary>
        GCM_VECTOR,

        /// <summary> Point with unit vector (internal use only) </summary>
        GCM_AXIS,

        /// <summary>  Геометрический тип, не поддерживаемый решателем. </summary>
        GCM_UNKNOWN_GTYPE,

        /// <summary> Количество типов. </summary>
        GCM_LAST_GTYPE,
    }
}