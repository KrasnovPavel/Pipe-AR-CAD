// // This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// // PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
//

using System.Runtime.InteropServices;
using GCM_system = System.IntPtr;

// ReSharper disable InconsistentNaming

namespace UnityC3D
{
    /// <summary> Структура записи геометрического ограничения. </summary>
    /// <remarks>
    /// The argument tuples of each constraint type:
    /// { GCM_c_type          GCM_c_arg  ...    GCM_c_arg }
    /// -------------------|-------------------------------
    /// { GCM_COINCIDENT      GCM_geom GCM_geom GCM_alignment }
    /// { GCM_CONCENTRIC      GCM_geom GCM_geom GCM_alignment }
    /// { GCM_PARALLEL        GCM_geom GCM_geom GCM_alignment }
    /// { GCM_PERPENDICULAR   GCM_geom GCM_geom GCM_alignment }  
    /// { GCM_IN_PLACE        GCM_geom GCM_geom GCM_NO_ALIGNMENT }
    /// { GCM_DISTANCE        GCM_geom GCM_geom double GCM_alignment }
    /// { GCM_TANGENT         GCM_geom GCM_geom GCM_alignment GCM_tan_choice }
    /// { GCM_ANGLE           GCM_geom GCM_geom GCM_geom double GCM_alignment } - planar kind of angle
    /// { GCM_ANGLE           GCM_geom GCM_geom GCM_NULL double GCM_alignment } - 3d kind of angle  
    /// { GCM_SYMMETRIC       GCM_geom GCM_geom GCM_geom GCM_alignment }
    /// { GCM_PATTERNED       GCM_geom GCM_geom GCM_geom double GCM_alignment GCM_scale }
    /// { GCM_LINEAR_PATTERN  GCM_geom GCM_geom GCM_geom GCM_alignment }
    /// { GCM_ANGULAR_PATTERN GCM_geom GCM_geom GCM_geom GCM_alignment }
    /// { GCM_TRANSMITTION    not specified }
    /// { GCM_CAM_MECHANISM   not specified }
    /// { GCM_RADIUS          GCM_geom double }
    /// { GCM_UNKNOWN }
    /// Sample of the journal line: (GCM_AddConstraint (GCM_COINCIDENT #1 #2 GCM_CLOSEST) #3)
    /// </remarks>
    internal struct GCMConstraint
    {
        /// <summary> Колличество аргументов. </summary>
        public const int argsN = 5;

        /// <summary> Тип ограничения </summary>
        public GCMConstraintType type;

        /// <summary> Аргументы ограничения. </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public GCM_c_arg[] args;
    }

    /// <summary> Argument of constraint to record in type <c> GCM_c_record </c> </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct GCM_c_arg
    {
        [FieldOffset(0)] public GCMDescriptor geom;
        [FieldOffset(0)] public GCMAlignment alignVal;
        [FieldOffset(0)] public GCMTanChoice tanChoice;
        [FieldOffset(0)] public GCM_angle_type angType;
        [FieldOffset(0)] public GCM_scale scale;
        [FieldOffset(0)] public double dimValue;
        [FieldOffset(0)] public double enumVal;
    }
    
    /// <summary> Варианты выравнивания направлений. </summary>
    public enum GCMAlignment
    {
        /// <summary> Противонаправленные. </summary>
        Opposite = -1,

        /// <summary> Ориентация согласно ближайшего решения. </summary>
        Closest = 0,

        /// <summary> Сонаправленные. </summary>
        Cooriented = 1,

        /// <summary> Нет определенной ориентации </summary>
        NoAlignment = 2,
    }

    /// <summary> Словарь типов ограничения. </summary>
    /// <remarks>
    /// Значения этого перечисления могут быть использованы для постоянного хранения
    /// и останутся неизменными в следующих версиях.
    /// </remarks>
    internal enum GCMConstraintType
    {
        /*
          (!) Do not change the integral constants (they are written to file permanently).
        */
        /// <summary> Не определенный тип. </summary>
        GCM_UNKNOWN = -1,

        /// <summary> Геометрическое совпадение. </summary>
        GCM_COINCIDENT = 0,

        /// <summary> Параллельность двух объектов, имеющих направление. </summary>
        GCM_PARALLEL = 1,

        /// <summary> Перпендикулярность двух объектов, имеющих направление. </summary>
        GCM_PERPENDICULAR = 2,

        /// <summary> Касание двух поверхностей или кривых. </summary>
        GCM_TANGENT = 3,

        /// <summary> Концентричность двух объектов, имеющих ось или центр. </summary>
        GCM_CONCENTRIC = 4,

        /// <summary> Линейный размер между объектами. </summary>
        GCM_DISTANCE = 5,

        /// <summary> Угловой размер между векторными объектами.  </summary>
        GCM_ANGLE = 6,

        /// <summary> Механическая передача. </summary>
        GCM_TRANSMITTION = 9,

        /// <summary> Кулачковый механизм. </summary>
        GCM_CAM_MECHANISM = 10,

        /// <summary> Симметричность. </summary>
        GCM_SYMMETRIC = 11,

        /// <summary> Зависимый объект. </summary>
        GCM_DEPENDENT = 14,

        /// <summary> Элемент паттерна. </summary>
        GCM_PATTERNED = 15,

        /// <summary> Линейный паттерн. </summary>
        GCM_LINEAR_PATTERN = 16,

        /// <summary> Угловой паттерн. </summary>
        GCM_ANGULAR_PATTERN = 17,

        /// <summary> Радиальный размер. </summary>
        GCM_RADIUS = 18,

        /// <summary>  </summary>
        GCM_LAST_CTYPE,

        /// <summary> Deprecated </summary>
        GCM_IN_PLACE = 7
    }

    /// <summary> Варианты касания поверхностей или кривых.  </summary>
    /// <remarks>
    /// Значения этого перечисления могут быть использованы для постоянного
    /// хранения данных приложения и останутся неизменными в следующих версиях.
    /// </remarks>
    public enum GCMTanChoice //-V3059
    {
        /*
          (!) Do not change the constants
        */
        /// <summary> Не выбрано. </summary>
        GCM_TAN_NONE = 0x00,

        /// <summary> Касание в общем случае (контакт точкой). </summary>
        GCM_TAN_POINT = 0x01,

        /// <summary> Касание по образующей прямой (например два цилиндра с параллельными осями). </summary>
        GCM_TAN_LINE = 0x02,

        /// <summary> Касание по окружности (например сфера в конусе). </summary>
        GCM_TAN_CIRCLE = 0x04
    }

    /// <summary> Вариант углового размера. </summary>
    /// <remarks>
    /// Значения этого перечисления могут быть использованы для постоянного
    /// хранения данных приложения и останутся неизменными в следующих версиях.
    /// </remarks>
    public enum GCM_angle_type
    {
        /// <summary> Неопределен </summary>
        GCM_NONE_ANGLE = 0,

        /// <summary> Угол для планарных соединений (0 .. 360 градусов) </summary>
        GCM_2D_ANGLE = 1,

        /// <summary> Угол в пространстве (0 .. 180 градусов) </summary>
        GCM_3D_ANGLE = 2,

        /// <summary> Угол для планарных соединений (0 .. 360 градусов) </summary>
        GCM_PLANAR_ANGLE = GCM_2D_ANGLE
    }

    /// <summary> Тип связи между элементами в паттерне. </summary>
    public enum GCM_scale
    {
        GCM_NO_SCALE = 0,

        /// <summary> Шаг между элементами константен. Паттерн не масштабируется (не растягивается). </summary>
        GCM_RIGID = 1,

        /// <summary> Шаг между элементами линейно масштабируется при растяжениях. </summary>
        GCM_LINEAR_SCALE = 2
    }
}