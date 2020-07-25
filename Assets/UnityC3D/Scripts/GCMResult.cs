using System.Diagnostics.CodeAnalysis;

namespace UnityC3D
{
    /// <summary> Диагностические коды 3d-решателя. </summary>
    /// <remarks>
    /// GCM_result перечисляет значения, возвращаемые вызовами API компонента GCM, 
    /// включая диагностические коды решения геометрических ограничений. Значения данного типа 
    /// возвращаются такими функциями, как <c>GCM_Evaluate</c> и <c>GCM_EvaluationResult</c>.
    /// </remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum GCMResult
    {
        /// <summary> Код неопределенного результата или состояния. </summary>
        GCM_RESULT_None = 0,

        /// <summary> Успешный результат вызова API компонента GCM. </summary>
        GCM_RESULT_Ok = 1,

        /// <summary> Ограничение или система ограничения решены. </summary>
        GCM_RESULT_Satisfied = GCM_RESULT_Ok,

        /// <summary> Ограничение переопределяет систему и противоречит другим условиям. </summary>
        GCM_RESULT_Overconstrained = 2,

        /// <summary> Заданы ограничения для пары фиксированных объектов. </summary>
        GCM_RESULT_MatedFixation = 3,

        /// <summary> Неудачная попытка перемещения фиксированного объекта (равно, как объекта жестко-связанного с фиксированным). </summary>
        GCM_RESULT_DraggingFailed = 4,

        /// <summary> Ограничение(я) не решено (по неизвестным причинам). </summary>
        GCM_RESULT_Not_Satisfied = 5,

        /// <summary> Ограничение(я) не разрешимо. </summary>
        GCM_RESULT_Unsolvable = 6,

        /// <summary> Ограничение GCM_DEPENDENT не вычислено или ее независимые аргументы находятся вне области решений. </summary>
        /// <remarks> Ситуация возникает, когда функция GCM_dependent_func возвращает false. </remarks>
        GCM_RESULT_DependentConstraintUnsolved = 7,

        /// <summary> Неизвестная ошибка, как правило, не связанная с процессом решения. </summary>
        GCM_RESULT_Error = 8,

        /// <summary> Опция выравнивания не подходит для данного типа ограничения. </summary>
        GCM_RESULT_InappropriateAlignment = 9,

        /// <summary> Геометрический тип аргумента не подходит для данного ограничения. </summary>
        GCM_RESULT_InappropriateArgument = 10,

        /*
          Additional message codes.
        */
        /// <summary> Несовместные типы аргументов ограничения. </summary>
        GCM_RESULT_IncompatibleArguments = 3001,

        /// <summary> Угловая опция несовместима со степенью свободы соединения. </summary>
        /// <remarks>
        /// Планарный тип угла применим только для соединения, оставляющего единственную степень свободы вращения
        /// </remarks>
        GCM_RESULT_InconsistentAngleType,

        /// <summary> Величина ориентации несовместна с другими сопряжениями. </summary>
        GCM_RESULT_InconsistentAlignment,

        /// <summary> Ограничение дублирует другое. </summary>
        GCM_RESULT_Duplicated,

        /// <summary> Неразрешимая циклическая зависимость. </summary>
        GCM_RESULT_CyclicDependence,

        /// <summary> Объект является зависимым от двух и более ограничений 'GCM_DEPENDED'. </summary>
        GCM_RESULT_MultiDependedGeom,

        /// <summary> Избыточное ограничение между зависимыми объектами. </summary>
        GCM_RESULT_OverconstrainingDependedGeoms,

        /// <summary> Зависимый аргумент ограничения 'GCM_DEPENDED' не может быть зафиксирован. </summary>
        GCM_RESULT_DependedGeomCantBeFixed,

        /// <summary> В ограничении не заданы аргументы (пустые аргументы). </summary>
        GCM_RESULT_InvalidArguments,

        /// <summary> Для сопряжения касание - опция выбора по окружности или по образующей не поддреживается </summary>
        mtResCode_UnsupportedTangencyChoice,

        /// <summary> Для данной пары поверхностей касание по окружности геометрически не возможно </summary>
        mtResCode_IsNoPossibleForCircTanChoice,

        /// <summary> Механическая передача вращения компонентов с совпадающими осями не поддерживается </summary>
        mtResCode_CoaxialMtGearTransmissionIsNotAvalable,

        /// <summary>
        /// В сборке присутствуют сопряжения (геометрические условия), создающие зависимость движения толкателя
        /// от движения кулачка, помимо самого кулачкового механизма
        /// </summary>
        mtResCode_NoSeparatedSolutionForCamGear,

        /// <summary> Задана циклическая зависимость для двух или более кулачковых механизмов </summary>
        mtResCode_CyclicDependenceForTwoOrMoreCamGears,

        /// <summary> Заданные сопряжения для толкателя не соответствую его оси движения </summary>
        mtResCode_InconsistentFollowerAxis,

        /// <summary> Не соблюдаются условия планарного угла (векторы сторон угла должны быть перпендикулярны оси). </summary>
        GCM_RESULT_InconsistentPlanarAngle,
        /*    
          ATTENTION: New error messages should be added only before this line.
        */

        /*
          Сообщения о некорректных результатах вызовов API решателя (не вычислительные). 
        */
        /// <summary> Данное ограничение должно быть управляющим размером. </summary>
        GCM_RESULT_ItsNotDrivingDimension,

        /// <summary> Обращение к недействительному объекту. </summary>
        GCM_RESULT_Unregistered,
        GCM_RESULT_InternalError,

        /// <summary> Процесс вычислений был прерван по запросу приложения. </summary>
        GCM_RESULT_Aborted,

        /// <summary> The last error code of user for mates (adding before this line) </summary>
        GCM_RESULT_Last_
    }
}