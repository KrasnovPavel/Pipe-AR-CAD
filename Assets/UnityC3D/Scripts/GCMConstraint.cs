// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using GCM_system = System.IntPtr;

// ReSharper disable InconsistentNaming

namespace UnityC3D
{
    /// <summary> Геометрическое ограничение. </summary>
    public class GCMConstraint
    {
        internal GCMConstraint(GCMDescriptor descriptor, GCMObject obj1, GCMObject obj2,
            GCMConstraintType type)
        {
            Descriptor = descriptor;
            Obj1 = obj1;
            Obj2 = obj2;
            Type = type;
        }

        /// <summary> Дескриптор ограничения. </summary>
        internal readonly GCMDescriptor Descriptor;

        /// <summary> Первый объект. </summary>
        public readonly GCMObject Obj1;

        /// <summary> Второй объект. </summary>
        public readonly GCMObject Obj2;

        /// <summary> Тип огрпничения. </summary>
        public readonly GCMConstraintType Type;
    }

    /// <summary> Словарь типов ограничения. </summary>
    /// <remarks>
    /// Значения этого перечисления могут быть использованы для постоянного хранения
    /// и останутся неизменными в следующих версиях.
    /// </remarks>
    public enum GCMConstraintType
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
}