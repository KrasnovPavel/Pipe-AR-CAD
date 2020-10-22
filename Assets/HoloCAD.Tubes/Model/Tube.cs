// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.Model
{
    /// <summary> Труба. </summary>
    public class Tube : IDisposable, INotifyPropertyChanged
    {
        /// <summary> Начальный фланец. </summary>
        public readonly Flange StartFlange;

        /// <summary> Конечный фланец. </summary>
        public readonly Flange EndFlange;

        /// <summary> Точки погиба. </summary>
        public readonly List<TubePoint> Points = new List<TubePoint>();

        /// <summary> Прямые отрезки. </summary>
        public readonly List<Segment> Segments = new List<Segment>();

        /// <summary> Данные о трубе из стандарта. </summary>
        public TubeLoader.TubeData TubeData
        {
            get => _tubeData;
            set
            {
                if (Equals(value, _tubeData)) return;
                _tubeData = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Diameter));
                OnPropertyChanged(nameof(FirstBendRadius));
                OnPropertyChanged(nameof(SecondBendRadius));
                foreach (var point in Points)
                {
                    point.RecalculateDiameter();
                }
                StartFlange.EndPoint.RecalculateDiameter();
                EndFlange.EndPoint.RecalculateDiameter();
            }
        }

        /// <summary> Диаметр трубы. </summary>
        public float Diameter => TubeData.diameter;

        /// <summary> Первый допустимый радиус погиба. </summary>
        public float FirstBendRadius => TubeData.first_radius;
        
        /// <summary> Второй допустимый радиус погиба. </summary>
        public float SecondBendRadius => TubeData.second_radius;

        /// <summary> Конструктор трубы. </summary>
        /// <param name="sys"> Система ограничений. </param>
        /// <param name="startFlange"> Начальный фланец. </param>
        /// <param name="endFlange"> Конечный фланец. </param>
        /// <param name="tubeData"> Данные о трубе из стандарта. </param>
        public Tube(GCMSystem sys, Flange startFlange, Flange endFlange, TubeLoader.TubeData tubeData)
        {
            StartFlange = startFlange;
            EndFlange   = endFlange;
            StartFlange.AddInTube(this);
            EndFlange.AddInTube(this);

            var middle = new Segment(StartFlange.EndPoint, EndFlange.EndPoint, this);

            StartFlange.EndPoint.Next = middle;
            EndFlange.EndPoint.Next   = EndFlange.FirstSegment;
            EndFlange.EndPoint.Prev   = middle;

            Segments.Add(middle);
            TubeData = tubeData;

            sys.Evaluate();
        }

        /// <summary> Система ограничений. </summary>
        public GCMSystem Sys => StartFlange.Plane.GCMSys;

        /// <inheritdoc />
        public void Dispose()
        {
            EndFlange.EndPoint.Prev = EndFlange.FirstSegment;
            foreach (var segment in Segments) segment?.Dispose();
            foreach (var point in Points) point?.Dispose();
            Disposed?.Invoke();
        }

        /// <summary> Событие добавления нового прямого отрезка. </summary>
        public event Action<Segment> SegmentAdded;

        /// <summary> Событие добавления новой точки. </summary>
        public event Action<TubePoint> PointAdded;

        public event Action Disposed;

        /// <summary> Исправляет ошибки трубы. </summary>
        public void FixErrors()
        {
            // TODO: Добавить исправление некорректных погибов.
            var hasErrors = StartFlange.Fix();
            hasErrors = hasErrors || EndFlange.Fix();
            if (hasErrors) Sys.Evaluate();
        }

        /// <summary> Добавляет новую точку в середине переданного отрезка. </summary>
        /// <param name="segment"> Отрезок на котором надо создать точку. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Труба не содержит переданного отрезка. </exception>
        public void AddPoint(Segment segment)
        {
            AddPoint(segment, segment.Middle);
        }

        /// <summary> Добавляет новую точку на переданном отрезке в указанный координатах. </summary>
        /// <param name="segment"> Отрезок на котором надо создать точку. </param>
        /// <param name="pos"> Координаты создаваемой точки. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Труба не содержит переданного отрезка. </exception>
        public void AddPoint(Segment segment, Vector3 pos)
        {
            // TODO: ГОВНО!!! ПЕРЕДЕЛАТЬ!!!
            if (segment == StartFlange.FirstSegment)
            {
                var point = segment.End.Origin;
                AddPoint(segment.Next);
                if (segment.Next?.Next == null) return;
                segment.End.Origin           = (StartFlange.Origin + pos) / 2;
                segment.Next.End.Origin      = pos;
                segment.Next.Next.End.Origin = point;
                segment.Next.ResetLine();
                segment.Next.Next?.ResetLine();
                Sys.Evaluate();
                return;
            }

            if (segment == EndFlange.FirstSegment)
            {
                var point = segment.End.Origin;
                AddPoint(segment.End.Prev);
                if (segment.End.Prev?.Prev == null) return;
                segment.End.Origin                 = (EndFlange.Origin + pos) / 2;
                segment.End.Prev.Start.Origin      = pos;
                segment.End.Prev.Prev.Start.Origin = point;
                segment.End.Prev.ResetLine();
                segment.End.Prev.Prev?.ResetLine();
                Sys.Evaluate();
                return;
            }
            // Дальше, вроде, не говно.

            if (!Segments.Contains(segment))
                throw new ArgumentOutOfRangeException(nameof(segment), segment, "Tube does not contains this segment");

            Segments.Remove(segment);

            var start = segment.Start;
            var end   = segment.End;

            var middle = new TubePoint(Sys, pos, null, Sys.GroundLCS);
            var first  = new Segment(start,  middle, this);
            var second = new Segment(middle, end,    this);

            start.Next  = first;
            middle.Prev = first;
            middle.Next = second;

            Segments.Add(first);
            Segments.Add(second);
            Points.Add(middle);

            PointAdded?.Invoke(middle);
            SegmentAdded?.Invoke(first);
            SegmentAdded?.Invoke(second);
            
            middle.RecalculateDiameter();

            Segments.Remove(segment);
            segment.Dispose();
            Sys.Evaluate();
        }

        /// <summary> Удаляет переданную точку. </summary>
        /// <param name="point"> Удаляемая точка. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Труба не содержит переданной точки. </exception>
        public void RemovePoint(TubePoint point)
        {
            if (ReferenceEquals(StartFlange.EndPoint,  point)
                || ReferenceEquals(EndFlange.EndPoint, point))
                throw new ArgumentOutOfRangeException(nameof(point), point, "Can't remove this point");

            if (!Points.Contains(point))
                throw new ArgumentOutOfRangeException(nameof(point), point, "Tube does not contains this point");

            var prevSegment = point.Prev;
            var nextSegment = point.Next;
            var prevPoint   = prevSegment?.Start;
            var nextPoint   = nextSegment?.End;

            point.Dispose();
            prevSegment?.Dispose();
            nextSegment?.Dispose();
            Points.Remove(point);
            Segments.Remove(prevSegment);
            Segments.Remove(nextSegment);

            var newSegment = new Segment(prevPoint, nextPoint, this);
            Segments.Add(newSegment);
            SegmentAdded?.Invoke(newSegment);
            Sys.Evaluate();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private definitions
        
        private TubeLoader.TubeData _tubeData;

        #endregion
    }
}
