// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.Model
{
    /// <summary> Отрезок трубы. </summary>
    public class Segment : IDisposable, INotifyPropertyChanged
    {
        /// <summary> Начальная точка. </summary>
        public readonly TubePoint Start;

        /// <summary> Конечная точка. </summary>
        public readonly TubePoint End;

        /// <summary> Линия. </summary>
        public readonly GCMLine Line;

        /// <summary> Труба - хозяин отрезка. </summary>
        public Tube Owner
        {
            get => _owner;
            set
            {
                if (_owner != null)
                {
                    Owner.PropertyChanged -= OwnerOnPropertyChanged;
                }
                _owner = value;
                if (_owner != null)
                {
                    _owner.PropertyChanged += OwnerOnPropertyChanged;
                }
            }
        }

        /// <summary> Длина линии. </summary>
        /// <remarks> ВНИМАНИЕ!!! Не соответствует длине прямого участка трубы. </remarks>
        public float LineLength => (End.Origin - Start.Origin).magnitude;

        /// <summary> Длина прямого участка трубы. </summary>
        public float TubeLength => LineLength - End.DeltaLength - Start.DeltaLength;

        /// <summary> Координаты середины отрезка. </summary>
        public Vector3 Middle => (Start.Origin + End.Origin) / 2;

        /// <summary> Событие, вызываемое при удалении отрезка. </summary>
        public event Action<Segment> Disposed;

        /// <summary> Следующий отрезок. </summary>
        [CanBeNull]
        public Segment Next => End.Next;

        /// <summary> Предыдущий отрезок. </summary>
        [CanBeNull]
        public Segment Prev => Start.Prev;

        /// <summary> Система ограничений. </summary>
        public GCMSystem Sys => Start.GCMSys;

        /// <summary> Находится ли отрезок во фланце. </summary>
        public bool IsInFlange => ReferenceEquals(Owner.StartFlange.FirstSegment, this) ||
                                  ReferenceEquals(Owner.EndFlange.FirstSegment,   this);

        /// <summary> Диаметр отрезка трубы. </summary>
        public float Diameter => Owner.Diameter;

        /// <summary> Конструктор отрезка. </summary>
        /// <param name="start"> Начальная точка. </param>
        /// <param name="end"> Конечная точка. </param>
        /// <param name="owner"> Труба - хозяин отрезка. </param>
        public Segment(TubePoint start, TubePoint end, Tube owner)
        {
            Start = start;
            End   = end;
            Owner = owner;

            Start.Next = this;
            End.Prev   = this;

            Line = new GCMLine(Sys, start.Origin, Start.Origin - End.Origin, Start.Parent);
            Sys.MakeCoincident(Start, Line);
            Sys.MakeCoincident(End,   Line);

            Start.PropertyChanged += PointOnPropertyChanged;
            End.PropertyChanged   += PointOnPropertyChanged;
        }

        /// <summary> Перемещает отрезок в новые координаты. </summary>
        /// <param name="newOrigin"> Координаты начала отрезка. </param>
        /// <param name="newDirection"> Направление отрезка. </param>
        public void Move(Vector3 newOrigin, Vector3 newDirection)
        {
            var length = LineLength;
            Start.Origin   = newOrigin;
            Line.Origin    = newOrigin;
            Line.Direction = newDirection;
            End.Origin     = Start.Origin + newDirection * length;
            Start.Prev?.ResetLine();
            End.Next?.ResetLine();
            Sys.Evaluate();
            Owner.FixErrors();
        }

        /// <summary> Пересчитывает положение линии исходя из координат точек. </summary>
        public void ResetLine()
        {
            Line.Origin    = Start.Origin;
            Line.Direction = (End.Origin - Start.Origin).normalized;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Line?.Dispose();
            Disposed?.Invoke(this);
            Start.PropertyChanged -= PointOnPropertyChanged;
            End.PropertyChanged   -= PointOnPropertyChanged;
            if (Owner != null) Owner.PropertyChanged -= OwnerOnPropertyChanged;
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

        private float _diameter;
        private Tube  _owner;

        private void PointOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(LineLength));
        }

        private void OwnerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Owner.Diameter))
            {
                OnPropertyChanged(nameof(Diameter));
            }
        }

        #endregion
    }
}
