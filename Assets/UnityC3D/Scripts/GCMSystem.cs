// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityC3D
{
    /// <summary> Система C3D. </summary>
    [SuppressMessage("ReSharper", "RedundantCast")]
    public class GCMSystem : IDisposable //-V3073
    {
        /// <summary> Система координат закреплённая в начале координат. </summary>
        public readonly GCM_LCS GroundLCS;

        /// <summary> Событие, вызываемое после пересчёта геометрии. </summary>
        public event Action Evaluated;

        /// <summary> Конструктор. </summary>
        public GCMSystem()
        {
            _gcmSystemPtr = GCM_CreateSystem();
            GroundLCS     = new GCM_LCS(this, new GCMDescriptor(0));
        }

        /// <summary> Удаляет все объекты из системы. </summary>
        public void Clear()
        {
            GCM_ClearSystem(_gcmSystemPtr);
        }

        /// <summary> Уничтожает систему. </summary>
        public void Dispose()
        {
            GCM_RemoveSystem(_gcmSystemPtr);
        }

        /// <summary> Пересчитывает систему. </summary>
        /// <remarks> Может завершиться с ошибкой. </remarks>
        /// <returns> Код ошибки. </returns>
        public GCMResult Evaluate()
        {
            var res = GCM_Evaluate(_gcmSystemPtr);
            if (res != GCMResult.GCM_RESULT_Ok)
            {
                Debug.LogWarning(res);
                foreach (var constraint in _gcmConstraints)
                {
                    var cRes = EvaluationResult(constraint);
                    if (cRes != GCMResult.GCM_RESULT_Ok)
                    {
                        Debug.LogWarning($"{cRes}-{constraint.Type}:  {constraint.Obj1}, {constraint.Obj2}, {constraint.Obj3}");
                    }
                }
            }

            Evaluated?.Invoke();
            return res;
        }

        /// <summary> Включает журналирование обращений к системе. </summary>
        /// <param name="filename"></param>
        public void SetJournal([CallerMemberName] string filename = "log")
        {
            GCM_SetJournal(_gcmSystemPtr, filename + ".txt");
        }

        /// <summary> Задаёт совпадение двух точек. </summary>
        /// <param name="first"> Первая точка. </param>
        /// <param name="second"> Вторая точка. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCMPoint first, GCMPoint second)
        {
            return MakeCoincident((GCMObject) first, (GCMObject) second, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт совпадение двух прямых. </summary>
        /// <param name="first"> Первая прямая. </param>
        /// <param name="second"> Вторая прямая. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCMLine      first, GCMLine second,
                                            GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        /// <summary> Задаёт принадлежность точки к прямой. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="line"> Прямая. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCMPoint point, GCMLine line)
        {
            return MakeCoincident((GCMObject) line, (GCMObject) point, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт совпадение двух плоскостей. </summary>
        /// <param name="first"> Первая плоскость. </param>
        /// <param name="second"> Вторая плоскость. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCMPlane     first, GCMPlane second,
                                            GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        /// <summary> Задаёт принадлежность точки к плоскости. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCMPoint point, GCMPlane plane)
        {
            return MakeCoincident((GCMObject) point, (GCMObject) plane, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт принадлежность прямой к плоскости. </summary>
        /// <param name="line"> Прямая. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCMLine line, GCMPlane plane)
        {
            return MakeCoincident((GCMObject) line, (GCMObject) plane, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт совпадение двух окружностей. </summary>
        /// <param name="first"> Первая окружность. </param>
        /// <param name="second"> Вторая окружность. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCMCircle    first, GCMCircle second,
                                            GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakeCoincident((GCMObject) first, (GCMObject) second, alignment);
        }

        /// <summary> Задаёт принадлежность окружности к плоскости. </summary>
        /// <param name="circle"> Окружность. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCMCircle    circle, GCMPlane plane,
                                            GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakeCoincident((GCMObject) circle, (GCMObject) plane, alignment);
        }

        /// <summary> Задаёт совпадение двух локальных систем координат. </summary>
        /// <param name="first"> Первая ЛСК. </param>
        /// <param name="second"> Вторая ЛСК. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCM_LCS first, GCM_LCS second)
        {
            return MakeCoincident((GCMObject) first, (GCMObject) second, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт принадлежность точки началу локальной системы координат. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="lcs"> ЛСК. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeCoincident(GCMPoint point, GCM_LCS lcs)
        {
            return MakeCoincident((GCMObject) point, (GCMObject) lcs, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт концентричность двух окружностей. </summary>
        /// <param name="first"> Первая окружность. </param>
        /// <param name="second"> Вторая окружность. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeConcentric(GCMCircle    first, GCMCircle second,
                                            GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakeConcentric((GCMObject) first, (GCMObject) second, alignment);
        }

        /// <summary> Задаёт принадлежность центра окружности к прямой. </summary>
        /// <param name="circle"> Окружность. </param>
        /// <param name="line"> Прямая. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeConcentric(GCMCircle    circle, GCMLine line,
                                            GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakeConcentric((GCMObject) circle, (GCMObject) line, alignment);
        }

        /// <summary> Задаёт совпадение центра окружности с точкой. </summary>
        /// <param name="circle"> Окружность. </param>
        /// <param name="point"> Точка. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeConcentric(GCMCircle circle, GCMPoint point)
        {
            return MakeConcentric((GCMObject) circle, (GCMObject) point, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт параллельность двух прямых. </summary>
        /// <param name="first"> Первая прямая. </param>
        /// <param name="second"> Вторая прямая. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeParallel(GCMLine      first, GCMLine second,
                                          GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakeParallel((GCMObject) first, (GCMObject) second, alignment);
        }

        /// <summary> Задаёт параллельность двух плоскостей. </summary>
        /// <param name="first"> Первая плоскость. </param>
        /// <param name="second"> Вторая плоскость. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeParallel(GCMPlane     first, GCMPlane second,
                                          GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakeParallel((GCMObject) first, (GCMObject) second, alignment);
        }

        /// <summary> Задаёт параллельность прямой и плоскости. </summary>
        /// <param name="line"> Прямая. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeParallel(GCMLine      line, GCMPlane plane,
                                          GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakeParallel((GCMObject) line, (GCMObject) plane, alignment);
        }

        /// <summary> Задаёт перпендикулярность двух прямых. </summary>
        /// <param name="first"> Первая прямая. </param>
        /// <param name="second"> Вторая прямая. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakePerpendicular(GCMLine      first, GCMLine second,
                                               GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakePerpendicular((GCMObject) first, (GCMObject) second, alignment);
        }

        /// <summary> Задаёт перпендикулярность двух плоскостей. </summary>
        /// <param name="first"> Первая плоскость. </param>
        /// <param name="second"> Вторая плоскость. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakePerpendicular(GCMPlane     first, GCMPlane second,
                                               GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakePerpendicular((GCMObject) first, (GCMObject) second, alignment);
        }

        /// <summary> Задаёт перпендикулярность прямой и плоскости. </summary>
        /// <param name="line"> Прямая. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakePerpendicular(GCMLine      line, GCMPlane plane,
                                               GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return MakePerpendicular((GCMObject) line, (GCMObject) plane, alignment);
        }

        /// <summary> Задаёт расстояние между двумя точками. </summary>
        /// <param name="first"> Первая точка. </param>
        /// <param name="second"> Вторая точка. </param>
        /// <param name="distance"> Расстояние. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetDistance(GCMPoint first, GCMPoint second, float distance)
        {
            return SetDistance((GCMObject) first, (GCMObject) second, distance, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт расстояние между двумя прямыми. </summary>
        /// <param name="first"> Первая прямая. </param>
        /// <param name="second"> Вторая прямая. </param>
        /// <param name="distance"> Расстояние. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetDistance(GCMLine first, GCMLine second, float distance)
        {
            return SetDistance((GCMObject) first, (GCMObject) second, distance, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт расстояние между точкой и прямой. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="line"> Прямая. </param>
        /// <param name="distance"> Расстояние. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetDistance(GCMPoint point, GCMLine line, float distance)
        {
            return SetDistance((GCMObject) point, (GCMObject) line, distance, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт расстояние между двумя плоскостями. </summary>
        /// <param name="first"> Первая плоскость. </param>
        /// <param name="second"> Вторая плоскость. </param>
        /// <param name="distance"> Расстояние. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetDistance(GCMPlane     first, GCMPlane second, float distance,
                                         GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return SetDistance((GCMObject) first, (GCMObject) second, distance, alignment);
        }

        /// <summary> Задаёт расстояние между точкой и плоскостью. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <param name="distance"></param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetDistance(GCMPoint point, GCMPlane plane, float distance)
        {
            return SetDistance((GCMObject) point, (GCMObject) plane, distance, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт расстояние между прямой и плоскостью. </summary>
        /// <param name="line"> Прямая. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <param name="distance"> Расстояние. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetDistance(GCMLine line, GCMPlane plane, float distance)
        {
            return SetDistance((GCMObject) line, (GCMObject) plane, distance, GCMAlignment.NoAlignment);
        }

        /// <summary> Задаёт угол между прямыми. </summary>
        /// <param name="first"> Первая прямая. </param>
        /// <param name="second"> Вторая прямая. </param>
        /// <param name="radians"> Угол в радианах. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetAngle(GCMLine first, GCMLine second, float radians)
        {
            return SetAngle((GCMObject) first, (GCMObject) second, radians);
        }

        /// <summary> Задаёт угол между плоскостями. </summary>
        /// <param name="first"> Первая плоскость. </param>
        /// <param name="second"> Вторая плоскость. </param>
        /// <param name="radians"> Угол в радианах. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetAngle(GCMPlane first, GCMPlane second, float radians)
        {
            return SetAngle((GCMObject) first, (GCMObject) second, radians);
        }

        /// <summary> Задаёт угол между прямой и плоскостью. </summary>
        /// <param name="line"> Прямая. </param>
        /// <param name="plane"> Плоскость. </param>
        /// <param name="radians"> Угол в радианах. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetAngle(GCMLine line, GCMPlane plane, float radians)
        {
            return SetAngle((GCMObject) line, (GCMObject) plane, radians);
        }

        /// <summary> Задаёт угол между окружностями. </summary>
        /// <param name="first"> Первая окружность. </param>
        /// <param name="second"> Вторая окружность. </param>
        /// <param name="radians"> Угол в радианах. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetAngle(GCMCircle first, GCMCircle second, float radians)
        {
            return SetAngle((GCMObject) first, (GCMObject) second, radians);
        }

        /// <summary> Задаёт угол между прямой и окружностью. </summary>
        /// <param name="line"> Прямая. </param>
        /// <param name="circle"> Окружность. </param>
        /// <param name="radians"> Угол в радианах. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetAngle(GCMLine line, GCMCircle circle, float radians)
        {
            return SetAngle((GCMObject) line, (GCMObject) circle, radians);
        }

        /// <summary> Задаёт угол между плоскостью и окружностью. </summary>
        /// <param name="plane"> Плоскость. </param>
        /// <param name="circle"> Окружность. </param>
        /// <param name="radians"> Угол в радианах. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint SetAngle(GCMPlane plane, GCMCircle circle, float radians)
        {
            return SetAngle((GCMObject) plane, (GCMObject) circle, radians);
        }

        public GCMPattern CreateAngularPattern(GCMObject sample, GCMObject axialObject, GCMAlignment alignment)
        {
            return new GCMPattern(this,
                                  GCM_AddAngularPattern(_gcmSystemPtr, sample.Descriptor, axialObject.Descriptor,
                                                        alignment),
                                  sample, axialObject);
        }

        /// <summary> Удаляет переданное ограничение из системы. </summary>
        /// <param name="constraint"> Удаляемое ограничение. </param>
        public void RemoveConstraint(GCMConstraint constraint)
        {
            GCM_RemoveConstraint(_gcmSystemPtr, constraint.Descriptor);
        }

        /// <summary> Возвращает результат вычисления для заданного ограничения. </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public GCMResult EvaluationResult(GCMConstraint constraint)
        {
            return GCM_EvaluationResult(_gcmSystemPtr, constraint.Descriptor);
        }

        #region Internal definitions

        /// <summary> Добавляет точку в систему. </summary>
        /// <param name="point"> Точка. </param>
        /// <param name="parent"> Родительская ЛСК. </param>
        /// <returns> Дескриптор объекта в системе. </returns>
        internal GCMDescriptor AddPoint(Vector3 point, GCMDescriptor? parent = null)
        {
            var o = MbVector3D.FromUnity(point);
            var p = GCM_Point(ref o);

            return parent == null
                       ? GCM_AddGeom(_gcmSystemPtr, ref p)
                       : GCM_SubGeom(_gcmSystemPtr, parent.Value, ref p);
        }

        /// <summary> Добавляет прямую в систему. </summary>
        /// <param name="origin"> Точка на прямой. </param>
        /// <param name="direction"> Направляющий вектор прямой. </param>
        /// <param name="parent"> Родительская ЛСК. </param>
        /// <returns> Дескриптор объекта в системе. </returns>
        internal GCMDescriptor AddLine(Vector3 origin, Vector3 direction, GCMDescriptor? parent = null)
        {
            var o    = MbVector3D.FromUnity(origin);
            var d    = MbVector3D.FromUnity(-direction);
            var line = GCM_Line(ref o, ref d);
            return parent == null
                       ? GCM_AddGeom(_gcmSystemPtr, ref line)
                       : GCM_SubGeom(_gcmSystemPtr, parent.Value, ref line);
        }

        /// <summary> Добавляет плоскость в систему. </summary>
        /// <param name="origin"> Точка на плоскости. </param>
        /// <param name="normal"> Нормаль к плоскости. </param>
        /// <param name="parent"> Родительская ЛСК. </param>
        /// <returns> Дескриптор объекта в системе. </returns>
        internal GCMDescriptor AddPlane(Vector3 origin, Vector3 normal, GCMDescriptor? parent = null)
        {
            var o     = MbVector3D.FromUnity(origin);
            var n     = MbVector3D.FromUnity(-normal);
            var plane = GCM_Plane(ref o, ref n);
            return parent == null
                       ? GCM_AddGeom(_gcmSystemPtr, ref plane)
                       : GCM_SubGeom(_gcmSystemPtr, parent.Value, ref plane);
        }

        /// <summary> Добавляет окружность в систему. </summary>
        /// <param name="origin"> Центр окружности. </param>
        /// <param name="normal"> Нормаль к окружности. </param>
        /// <param name="radius"> Радиус окружности. </param>
        /// <param name="parent"> Родительская ЛСК. </param>
        /// <returns> Дескриптор объекта в системе. </returns>
        internal GCMDescriptor AddCircle(Vector3 origin, Vector3 normal, float radius, GCMDescriptor? parent = null)
        {
            var o      = MbVector3D.FromUnity(origin);
            var n      = MbVector3D.FromUnity(-normal);
            var circle = GCM_Circle(ref o, ref n, radius);
            return parent == null
                       ? GCM_AddGeom(_gcmSystemPtr, ref circle)
                       : GCM_SubGeom(_gcmSystemPtr, parent.Value, ref circle);
        }

        /// <summary> Добавляет локальную систему координат. </summary>
        /// <param name="tr"> Transform из Unity. </param>
        /// <param name="parent"> Родительская ЛСК. </param>
        /// <returns> Дескриптор объекта в системе. </returns>
        internal GCMDescriptor AddLCS(Transform tr, GCMDescriptor? parent = null)
        {
            var p   = MbPlacement3D.FromUnity(tr);
            var lcs = GCM_SolidLCS(ref p);
            return parent == null
                       ? GCM_AddGeom(_gcmSystemPtr, ref lcs)
                       : GCM_SubGeom(_gcmSystemPtr, parent.Value, ref lcs);
        }

        /// <summary> Добавляет локальную систему координат. </summary>
        /// <param name="placement"> Расположение. </param>
        /// <param name="parent"> Родительская ЛСК. </param>
        /// <returns> Дескриптор объекта в системе. </returns>
        internal GCMDescriptor AddLCS(MbPlacement3D placement, GCMDescriptor? parent = null)
        {
            var lcs = GCM_SolidLCS(ref placement);
            return parent == null
                       ? GCM_AddGeom(_gcmSystemPtr, ref lcs)
                       : GCM_SubGeom(_gcmSystemPtr, parent.Value, ref lcs);
        }

        /// <summary> Возвращает дескриптор родительской ЛСК переданного объекта. </summary>
        /// <param name="obj"></param>
        /// <returns> Дескриптор объекта в системе. </returns>
        internal GCMDescriptor GetParent(GCMObject obj)
        {
            return GCM_Parent(_gcmSystemPtr, obj.Descriptor);
        }

        /// <summary> Возвращает расположение в системе переданного объекта. </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal MbPlacement3D GetPlacement(GCMObject obj)
        {
            return GCM_Placement(_gcmSystemPtr, obj.Descriptor);
        }

        /// <summary> Задаёт новое размещение объекта в системе. </summary>
        /// <param name="obj"></param>
        /// <param name="newPlacement"></param>
        internal void SetPlacement(GCMObject obj, MbPlacement3D newPlacement)
        {
            GCM_SetPlacement(_gcmSystemPtr, obj.Descriptor, ref newPlacement);
        }

        /// <summary> Создаёт ограничение радиуса окружности. </summary>
        /// <param name="circle"> Окружность. </param>
        /// <returns> Дескриптор ограничения в системе. </returns>
        internal GCMDescriptor CreateRadiusConstraint(GCMCircle circle)
        {
            return GCM_FixRadius(_gcmSystemPtr, circle.Descriptor);
        }

        /// <summary> Устанавливает радиус окружности. </summary>
        /// <param name="circle"> Окружность. </param>
        /// <param name="newRadius"> Радиуы. </param>
        internal void SetRadius(GCMCircle circle, float newRadius)
        {
            GCM_ChangeDrivingDimension(_gcmSystemPtr, circle.RadiusConstraint, newRadius);
        }

        /// <summary> Возвращает радиус окружности. </summary>
        /// <param name="circle"> Окружность. </param>
        /// <returns></returns>
        internal float GetRadius(GCMCircle circle)
        {
            return (float) GCM_RadiusA(_gcmSystemPtr, circle.Descriptor);
        }

        /// <summary> Замораживает объект в системе. </summary>
        /// <param name="obj"></param>
        internal void Freeze(GCMObject obj)
        {
            GCM_FreezeGeom(_gcmSystemPtr, obj.Descriptor);
        }

        /// <summary> Размораживает объект в системе. </summary>
        /// <param name="obj"></param>
        internal void Free(GCMObject obj)
        {
            GCM_FreeGeom(_gcmSystemPtr, obj.Descriptor);
        }

        /// <summary> Удаляет объект из системы. </summary>
        /// <param name="obj"> Удаляемый объект. </param>
        internal void Remove(GCMObject obj)
        {
            GCM_RemoveGeom(_gcmSystemPtr, obj.Descriptor);
            foreach (var constraint in _gcmConstraints.Where(c => c.Obj1.Equals(obj) || c.Obj2.Equals(obj)))
            {
                RemoveConstraint(constraint);
            }
        }

        internal GCMConstraint AddObjectToPattern(
            GCMPattern   pattern,
            GCMObject    obj,
            float        position,
            GCMAlignment alignment,
            GCMScale     scale)
        {
            var co = _gcmConstraints.Find(c =>
                                              c.Obj1.Equals(pattern.Sample) && c.Obj2.Equals(pattern.AxialObject) &&
                                              c.Obj3.Equals(obj)
                                              && c.Type == GCMConstraintType.GCM_ANGULAR_PATTERN);

            if (co != null) return co;

            var desc = GCM_AddGeomToPattern(_gcmSystemPtr, pattern.Descriptor, obj.Descriptor, position, alignment,
                                            scale);
            co = new GCMConstraint(desc, GCMConstraintType.GCM_ANGULAR_PATTERN, pattern.Sample, pattern.AxialObject,
                                   obj);

            _gcmConstraints.Add(co);
            return co;
        }

        internal void ChangeDrivingDimensions(GCMConstraint constraint, float newValue)
        {
            GCM_ChangeDrivingDimension(_gcmSystemPtr, constraint.Descriptor, newValue);
        }

        /// <summary> Удаляет переданный паттерн из системы. </summary>
        /// <param name="pattern"> Удаляемое паттерн. </param>
        public void RemovePattern(GCMPattern pattern)
        {
            foreach (var constraint in pattern.Constraints)
            {
                _gcmConstraints.Remove(constraint);
            }

            GCM_RemoveConstraint(_gcmSystemPtr, pattern.Descriptor);
        }

        #endregion

        #region Private definitions

        /// <summary> Задаёт совпадение двух любых объектов системы. </summary>
        /// <remarks> Может привести к ошибке при расчете, если ограничение не применимо к переданным объектам. </remarks>
        /// <param name="first"> Первый объект. </param>
        /// <param name="second"> Второй объект. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        private GCMConstraint MakeCoincident(GCMObject first, GCMObject second, GCMAlignment alignment)
        {
            var co = _gcmConstraints.Find(c =>
                                              (c.Obj1.Equals(first) && c.Obj2.Equals(second)
                                               || c.Obj1.Equals(second) && c.Obj2.Equals(first))
                                              && c.Type == GCMConstraintType.GCM_COINCIDENT);

            if (co != null) return co;

            var desc = GCM_AddBinConstraint(_gcmSystemPtr, GCMConstraintType.GCM_COINCIDENT, first.Descriptor,
                                            second.Descriptor, alignment, GCMTanChoice.GCM_TAN_NONE);
            var cons = new GCMConstraint(desc, GCMConstraintType.GCM_COINCIDENT, first, second);
            _gcmConstraints.Add(cons);
            return cons;
        }

        /// <summary> Задаёт концентричность двух любых объектов системы. </summary>
        /// <remarks> Может привести к ошибке при расчете, если ограничение не применимо к переданным объектам. </remarks>
        /// <param name="first"> Первый объект. </param>
        /// <param name="second"> Второй объект. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        private GCMConstraint MakeConcentric(GCMObject first, GCMObject second, GCMAlignment alignment)
        {
            var co = _gcmConstraints.Find(c =>
                                              (c.Obj1.Equals(first) && c.Obj2.Equals(second)
                                               || c.Obj1.Equals(second) && c.Obj2.Equals(first))
                                              && c.Type == GCMConstraintType.GCM_CONCENTRIC);

            if (co != null) return co;

            var desc = GCM_AddBinConstraint(_gcmSystemPtr, GCMConstraintType.GCM_CONCENTRIC, first.Descriptor,
                                            second.Descriptor, alignment, GCMTanChoice.GCM_TAN_NONE);
            var cons = new GCMConstraint(desc, GCMConstraintType.GCM_CONCENTRIC, first, second);
            _gcmConstraints.Add(cons);
            return cons;
        }

        /// <summary> Задаёт параллельность двух любых объектов системы. </summary>
        /// <remarks> Может привести к ошибке при расчете, если ограничение не применимо к переданным объектам. </remarks>
        /// <param name="first"> Первый объект. </param>
        /// <param name="second"> Второй объект. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        private GCMConstraint MakeParallel(GCMObject first, GCMObject second, GCMAlignment alignment)
        {
            var co = _gcmConstraints.Find(c =>
                                              (c.Obj1.Equals(first) && c.Obj2.Equals(second)
                                               || c.Obj1.Equals(second) && c.Obj2.Equals(first))
                                              && c.Type == GCMConstraintType.GCM_PARALLEL);

            if (co != null) return co;

            var desc = GCM_AddBinConstraint(_gcmSystemPtr, GCMConstraintType.GCM_PARALLEL, first.Descriptor,
                                            second.Descriptor, alignment, GCMTanChoice.GCM_TAN_NONE);
            var cons = new GCMConstraint(desc, GCMConstraintType.GCM_PARALLEL, first, second);
            _gcmConstraints.Add(cons);
            return cons;
        }

        /// <summary> Задаёт перпендикулярность двух любых объектов системы. </summary>
        /// <remarks> Может привести к ошибке при расчете, если ограничение не применимо к переданным объектам. </remarks>
        /// <param name="first"> Первый объект. </param>
        /// <param name="second"> Второй объект. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        private GCMConstraint MakePerpendicular(GCMObject first, GCMObject second, GCMAlignment alignment)
        {
            var co = _gcmConstraints.Find(c =>
                                              (c.Obj1.Equals(first) && c.Obj2.Equals(second)
                                               || c.Obj1.Equals(second) && c.Obj2.Equals(first))
                                              && c.Type == GCMConstraintType.GCM_PERPENDICULAR);

            if (co != null) return co;

            var desc = GCM_AddBinConstraint(_gcmSystemPtr, GCMConstraintType.GCM_PERPENDICULAR, first.Descriptor,
                                            second.Descriptor, alignment, GCMTanChoice.GCM_TAN_NONE);
            var cons = new GCMConstraint(desc, GCMConstraintType.GCM_PERPENDICULAR, first, second);
            _gcmConstraints.Add(cons);
            return cons;
        }

        /// <summary> Задаёт касание двух любых объектов системы. </summary>
        /// <param name="first"> Первый объект. </param>
        /// <param name="second"> Второй объект. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <param name="tanChoice"> Вариант касания. </param>
        /// <returns> Объект ограничения. </returns>
        public GCMConstraint MakeTangent(
            GCMObject    first,
            GCMObject    second,
            GCMAlignment alignment,
            GCMTanChoice tanChoice)
        {
            var co = _gcmConstraints.Find(c =>
                                              (c.Obj1.Equals(first) && c.Obj2.Equals(second)
                                               || c.Obj1.Equals(second) && c.Obj2.Equals(first))
                                              && c.Type == GCMConstraintType.GCM_TANGENT);

            if (co != null) return co;

            var desc = GCM_AddBinConstraint(_gcmSystemPtr, GCMConstraintType.GCM_TANGENT, first.Descriptor,
                                            second.Descriptor, alignment, tanChoice);
            var cons = new GCMConstraint(desc, GCMConstraintType.GCM_TANGENT, first, second);
            _gcmConstraints.Add(cons);
            return cons;
        }

        /// <summary> Задаёт расстояние между двумя любыми объектами системы. </summary>
        /// <remarks> Может привести к ошибке при расчете, если ограничение не применимо к переданным объектам. </remarks>
        /// <param name="first"> Первый объект. </param>
        /// <param name="second"> Второй объект. </param>
        /// <param name="distance"> Расстояние. </param>
        /// <param name="alignment"> Опция выравнивания. </param>
        /// <returns> Объект ограничения. </returns>
        private GCMConstraint SetDistance(GCMObject first, GCMObject second, float distance, GCMAlignment alignment)
        {
            var co = _gcmConstraints.Find(c =>
                                              (c.Obj1.Equals(first) && c.Obj2.Equals(second)
                                               || c.Obj1.Equals(second) && c.Obj2.Equals(first))
                                              && c.Type == GCMConstraintType.GCM_DISTANCE);

            if (co == null)
            {
                var desc = GCM_AddDistance(_gcmSystemPtr, first.Descriptor, second.Descriptor, distance, alignment);
                var cons = new GCMConstraint(desc, GCMConstraintType.GCM_DISTANCE, first, second);
                _gcmConstraints.Add(cons);
                return cons;
            }

            GCM_ChangeDrivingDimension(_gcmSystemPtr, co.Descriptor, distance);
            return co;
        }

        /// <summary> Задаёт угол между двумя любыми объектами системы. </summary>
        /// <remarks> Может привести к ошибке при расчете, если ограничение не применимо к переданным объектам. </remarks>
        /// <param name="first"> Первый объект. </param>
        /// <param name="second"> Второй объект. </param>
        /// <param name="angle"> Угол в радианах. </param>
        /// <returns> Объект ограничения. </returns>
        private GCMConstraint SetAngle(GCMObject first, GCMObject second, float angle)
        {
            var co = _gcmConstraints.Find(c =>
                                              (c.Obj1.Equals(first) && c.Obj2.Equals(second)
                                               || c.Obj1.Equals(second) && c.Obj2.Equals(first))
                                              && c.Type == GCMConstraintType.GCM_ANGLE);

            if (co == null)
            {
                var desc = GCM_AddAngle(_gcmSystemPtr, first.Descriptor, second.Descriptor, angle);
                var cons = new GCMConstraint(desc, GCMConstraintType.GCM_ANGLE, first, second);
                _gcmConstraints.Add(cons);
                return cons;
            }

            GCM_ChangeDrivingDimension(_gcmSystemPtr, co.Descriptor, angle);
            return co;
        }

        /// <summary> Указатель на систему в С++. </summary>
        private readonly IntPtr _gcmSystemPtr;

        /// <summary> Список всех ограничений в системе. </summary>
        private readonly List<GCMConstraint> _gcmConstraints = new List<GCMConstraint>();

        #endregion

        #region Extern functions

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Evaluate@@YA?AW4GCM_result@@PEAVMtGeomSolver@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Evaluate@@YA?AW4GCM_result@@PAVMtGeomSolver@@@Z")]
#endif
        private static extern GCMResult GCM_Evaluate(IntPtr gSys);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_CreateSystem@@YAPEAVMtGeomSolver@@XZ")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_CreateSystem@@YAPAVMtGeomSolver@@XZ")]
#endif
        private static extern IntPtr GCM_CreateSystem();

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_RemoveSystem@@YAXPEAVMtGeomSolver@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_RemoveSystem@@YAXPAVMtGeomSolver@@@Z")]
#endif
        private static extern void GCM_RemoveSystem([In] IntPtr gcmSystem);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_ClearSystem@@YAXPEAVMtGeomSolver@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_ClearSystem@@YAXPAVMtGeomSolver@@@Z")]
#endif
        private static extern void GCM_ClearSystem([In] IntPtr gcmSystem);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Placement@@YA?AVMbPlacement3D@@PEAVMtGeomSolver@@UMtObjectId@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Placement@@YA?AVMbPlacement3D@@PAVMtGeomSolver@@UMtObjectId@@@Z")]
#endif
        private static extern MbPlacement3D GCM_Placement(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_SetPlacement@@YAXPEAVMtGeomSolver@@UMtObjectId@@AEBVMbPlacement3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_SetPlacement@@YAXPAVMtGeomSolver@@UMtObjectId@@ABVMbPlacement3D@@@Z")]
#endif
        private static extern void GCM_SetPlacement(IntPtr gSys, GCMDescriptor g, ref MbPlacement3D newPlacement);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_RadiusA@@YANPEAVMtGeomSolver@@UMtObjectId@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_RadiusA@@YANPAVMtGeomSolver@@UMtObjectId@@@Z")]
#endif
        private static extern double GCM_RadiusA(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Point@@YA?AUGCM_g_record@@AEBVMbCartPoint3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Point@@YA?AUGCM_g_record@@ABVMbCartPoint3D@@@Z")]
#endif
        private static extern GCMGeometry GCM_Point(ref MbVector3D origin);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Line@@YA?AUGCM_g_record@@AEBVMbCartPoint3D@@AEBVMbVector3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Line@@YA?AUGCM_g_record@@ABVMbCartPoint3D@@ABVMbVector3D@@@Z")]
#endif
        private static extern GCMGeometry GCM_Line(ref MbVector3D origin, ref MbVector3D direction);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Plane@@YA?AUGCM_g_record@@AEBVMbCartPoint3D@@AEBVMbVector3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Plane@@YA?AUGCM_g_record@@ABVMbCartPoint3D@@ABVMbVector3D@@@Z")]
#endif
        private static extern GCMGeometry GCM_Plane(ref MbVector3D origin, ref MbVector3D normal);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Circle@@YA?AUGCM_g_record@@AEBVMbCartPoint3D@@AEBVMbVector3D@@N@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Circle@@YA?AUGCM_g_record@@ABVMbCartPoint3D@@ABVMbVector3D@@N@Z")]
#endif
        private static extern GCMGeometry GCM_Circle(ref MbVector3D origin, ref MbVector3D normal, double radius);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_SolidLCS@@YA?AUGCM_g_record@@AEBVMbPlacement3D@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_SolidLCS@@YA?AUGCM_g_record@@ABVMbPlacement3D@@@Z")]
#endif
        private static extern GCMGeometry GCM_SolidLCS(ref MbPlacement3D position);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_AddGeom@@YA?AUMtObjectId@@PEAVMtGeomSolver@@AEBUGCM_g_record@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_AddGeom@@YA?AUMtObjectId@@PAVMtGeomSolver@@ABUGCM_g_record@@@Z")]
#endif
        private static extern GCMDescriptor GCM_AddGeom(IntPtr gSys, ref GCMGeometry g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_SubGeom@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@AEBUGCM_g_record@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_SubGeom@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@ABUGCM_g_record@@@Z")]
#endif
        private static extern GCMDescriptor GCM_SubGeom(IntPtr gSys, GCMDescriptor parent, ref GCMGeometry g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_RemoveGeom@@YAXPEAVMtGeomSolver@@UMtObjectId@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_RemoveGeom@@YAXPAVMtGeomSolver@@UMtObjectId@@@Z")]
#endif
        private static extern void GCM_RemoveGeom(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_RemoveConstraint@@YAXPEAVMtGeomSolver@@UMtObjectId@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_RemoveConstraint@@YAXPAVMtGeomSolver@@UMtObjectId@@@Z")]
#endif
        private static extern void GCM_RemoveConstraint(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_Parent@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_Parent@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@@Z")]
#endif
        private static extern GCMDescriptor GCM_Parent(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_FixRadius@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_FixRadius@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@@Z")]
#endif
        private static extern GCMDescriptor GCM_FixRadius(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_FreezeGeom@@YA_NPEAVMtGeomSolver@@UMtObjectId@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_FreezeGeom@@YA_NPAVMtGeomSolver@@UMtObjectId@@@Z")]
#endif
        private static extern bool GCM_FreezeGeom(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_FixGeom_@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_FixGeom_@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@@Z")]
#endif
        private static extern GCMDescriptor GCM_FixGeom(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_FreeGeom@@YAXPEAVMtGeomSolver@@UMtObjectId@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_FreeGeom@@YAXPAVMtGeomSolver@@UMtObjectId@@@Z")]
#endif
        private static extern void GCM_FreeGeom(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_EvaluationResult@@YA?AW4GCM_result@@PEAVMtGeomSolver@@UMtObjectId@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_EvaluationResult@@YA?AW4GCM_result@@PAVMtGeomSolver@@UMtObjectId@@@Z")]
#endif
        private static extern GCMResult GCM_EvaluationResult(IntPtr gSys, GCMDescriptor g);

#if UNITY_EDITOR_64
        [DllImport("c3d",
                   EntryPoint =
                       "?GCM_AddBinConstraint@@YA?AUMtObjectId@@PEAVMtGeomSolver@@W4GCM_c_type@@U1@2W4GCM_alignment@@W4GCM_tan_choice@@@Z")]
#else
        [DllImport("c3d", 
                   EntryPoint =
                       "?GCM_AddBinConstraint@@YA?AUMtObjectId@@PAVMtGeomSolver@@W4GCM_c_type@@U1@2W4GCM_alignment@@W4GCM_tan_choice@@@Z")]
#endif
        private static extern GCMDescriptor GCM_AddBinConstraint(IntPtr        gSys,   GCMConstraintType cType,
                                                                 GCMDescriptor geom1,  GCMDescriptor     geom2,
                                                                 GCMAlignment  aValue, GCMTanChoice      tValue);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_AddDistance@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@1NW4GCM_alignment@@@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_AddDistance@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@1NW4GCM_alignment@@@Z")]
#endif
        private static extern GCMDescriptor GCM_AddDistance(IntPtr       gSys, GCMDescriptor geom1, GCMDescriptor geom2,
                                                            double       value,
                                                            GCMAlignment aValue);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_AddAngle@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@1N@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_AddAngle@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@1N@Z")]
#endif
        private static extern GCMDescriptor GCM_AddAngle(IntPtr gSys, GCMDescriptor geom1, GCMDescriptor geom2,
                                                         double value);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_AddAngle@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@11N@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_AddAngle@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@11N@Z")]
#endif
        private static extern GCMDescriptor GCM_AddAngle(
            IntPtr        gSys,
            GCMDescriptor geom1,
            GCMDescriptor geom2,
            GCMDescriptor axis,
            double        value);


#if UNITY_EDITOR_64
        [DllImport("c3d",
                   EntryPoint = "?GCM_ChangeDrivingDimension@@YA?AW4GCM_result@@PEAVMtGeomSolver@@UMtObjectId@@N@Z")]
#else
        [DllImport("c3d", EntryPoint =
 "?GCM_ChangeDrivingDimension@@YA?AW4GCM_result@@PAVMtGeomSolver@@UMtObjectId@@N@Z")]
#endif
        private static extern GCMResult GCM_ChangeDrivingDimension(IntPtr gSys, GCMDescriptor c, double value);

#if UNITY_EDITOR_64
        [DllImport("c3d", EntryPoint = "?GCM_SetJournal@@YA_NPEAVMtGeomSolver@@PEBD@Z")]
#else
        [DllImport("c3d", EntryPoint = "?GCM_SetJournal@@YA_NPAVMtGeomSolver@@PBD@Z")]
#endif
        private static extern bool GCM_SetJournal(IntPtr gSys, string filename);


#if UNITY_EDITOR_64
        [DllImport("c3d",
                   EntryPoint = "?GCM_AddAngularPattern@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@1W4GCM_alignment@@@Z")]
#else
        [DllImport("c3d", EntryPoint =
            "?GCM_AddAngularPattern@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@1W4GCM_alignment@@@Z")]
#endif
        private static extern GCMDescriptor GCM_AddAngularPattern(
            IntPtr        gSys,
            GCMDescriptor sample,
            GCMDescriptor axis,
            GCMAlignment  align);

#if UNITY_EDITOR_64
        [DllImport("c3d",
                   EntryPoint =
                       "?GCM_AddGeomToPattern@@YA?AUMtObjectId@@PEAVMtGeomSolver@@U1@1NW4GCM_alignment@@W4GCM_scale@@@Z")]
#else
        [DllImport("c3d", EntryPoint =
            "?GCM_AddGeomToPattern@@YA?AUMtObjectId@@PAVMtGeomSolver@@U1@1NW4GCM_alignment@@W4GCM_scale@@@Z")]
#endif
        private static extern GCMDescriptor GCM_AddGeomToPattern(
            IntPtr        gSys,
            GCMDescriptor pattern,
            GCMDescriptor geom,
            double        position,
            GCMAlignment  align,
            GCMScale      scale);

        #endregion
    }
}