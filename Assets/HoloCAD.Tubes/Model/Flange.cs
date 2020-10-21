// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.Model
{
    /// <summary> Фланец. </summary>
    public class Flange : IDisposable, INotifyPropertyChanged
    {
        /// <summary> Длина первого отрезка по умолчанию. </summary>
        protected const float DefaultFirstSegmentLength = 0.3f;

        /// <summary> Плоскость фланца. </summary>
        public readonly GCMPlane Plane;

        /// <summary> Отрезок исходящий из фланца. </summary>
        public readonly Segment FirstSegment;

        /// <summary> Конечная точка фланца. </summary>
        public TubePoint EndPoint => FirstSegment.End;

        /// <summary> Координаты фланца. </summary>
        public Vector3 Origin
        {
            get => _startPoint.Origin;
            set => Move(value, Normal);
        }

        /// <summary> Нормаль к плоскости фланца. </summary>
        public Vector3 Normal
        {
            get => Plane.Normal;
            set => Move(Origin, value);
        }

        public TubeLoader.TubeData TubeData
        {
            get => Owner != null ? Owner.TubeData : _tubeData;
            set
            {
                if (Owner != null)
                {
                    Owner.TubeData = value;
                } 
                else if (!Equals(_tubeData, value))
                {
                    _tubeData = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Diameter));
                }
            }
        }

        /// <summary> Диаметр фланца. </summary>
        public float Diameter => TubeData.diameter;

        /// <summary> Труба - хозяин фланца. </summary>
        [CanBeNull] public Tube Owner { get; private set; }

        /// <summary> Конструктор фланца. </summary>
        /// <param name="sys"> Система ограничений. </param>
        public Flange(GCMSystem sys)
        {
            _tubeData    = TubeLoader.GetAvailableTubes(TubeLoader.GetStandardNames().First()).First();
            _sys         = sys;
            Plane        = new GCMPlane(sys, Vector3.zero, Vector3.forward, sys.GroundLCS);
            _startPoint  = new TubePoint(sys, Vector3.zero,                                sys.GroundLCS);
            _endPoint    = new TubePoint(sys, Vector3.forward * DefaultFirstSegmentLength, sys.GroundLCS);
            FirstSegment = new Segment(_startPoint, _endPoint, null);
            Plane.Freeze();
            _startPoint.Freeze();
            sys.MakePerpendicular(FirstSegment, Plane);

            _startPoint.PropertyChanged  += OnPropertyChanged;
            Plane.PropertyChanged        += OnPropertyChanged;
            FirstSegment.PropertyChanged += OnPropertyChanged;
        }

        /// <summary> Перемещяет фланец по новым координатам. </summary>
        /// <param name="newPos"> Новые координаты. </param>
        /// <param name="newNorm"> Новая нормаль. </param>
        public void Move(Vector3 newPos, Vector3 newNorm)
        {
            Plane.Origin       = newPos;
            Plane.Normal       = newNorm;
            _startPoint.Origin = newPos;
            _sys.Evaluate();
        }

        /// <summary> Корректен ли фланец. </summary>
        /// <remarks> Проверяет, что отрезок исходящий из фланца сонаправлен нормали.  </remarks>
        /// <param name="angleEps"> Точность. </param>
        /// <returns></returns>
        public bool IsCorrect(float angleEps = 0.1f)
        {
            var actualDirection = EndPoint.Origin - Origin;
            return Vector3.Angle(actualDirection, Normal) < angleEps;
        }

        /// <summary> Исправляет ошибочный фланец. </summary>
        /// <returns></returns>
        public bool Fix()
        {
            if (IsCorrect()) return false;

            EndPoint.Origin = Origin + Normal * DefaultFirstSegmentLength;
            FirstSegment.Next?.ResetLine();
            FirstSegment.Prev?.ResetLine();
            return true;
        }

        /// <summary> Добавляет фланец в трубу. </summary>
        /// <param name="tube"> Труба, в которую добавляется фланец. </param>
        public void AddInTube(Tube tube)
        {
            if (Owner != null) RemoveFromTube();
            Owner              = tube;
            FirstSegment.Owner = tube;
            OnPropertyChanged(nameof(Owner));
            Owner.PropertyChanged += OnPropertyChanged;
        }

        /// <summary> Удаляет фланец из трубы. </summary>
        public void RemoveFromTube()
        {
            Owner              = null;
            FirstSegment.Owner = null;
            OnPropertyChanged(nameof(Owner));
            Owner.PropertyChanged -= OnPropertyChanged;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_startPoint != null)
            {
                _startPoint.PropertyChanged -= OnPropertyChanged;
                _startPoint.Dispose();
            }

            if (Plane != null)
            {
                Plane.PropertyChanged -= OnPropertyChanged;
                Plane.Dispose();
            }

            if (FirstSegment != null)
            {
                FirstSegment.PropertyChanged -= OnPropertyChanged;
                FirstSegment.Dispose();
            }

            _endPoint?.Dispose();
        }

        #region Private definitions

        private          GCMSystem _sys;
        private readonly TubePoint _startPoint;
        private readonly TubePoint _endPoint;

        private TubeLoader.TubeData _tubeData;

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, _startPoint) && e.PropertyName == nameof(_startPoint.Origin))
            {
                OnPropertyChanged(nameof(Origin));
            }

            if (ReferenceEquals(sender, Plane) && e.PropertyName == nameof(Plane.Origin))
            {
                OnPropertyChanged(nameof(Normal));
            }

            if (ReferenceEquals(sender, FirstSegment) && e.PropertyName == nameof(FirstSegment.Diameter))
            {
                OnPropertyChanged(nameof(Diameter));
            }

            if (ReferenceEquals(sender, Owner) && e.PropertyName == nameof(Tube.TubeData))
            {
                OnPropertyChanged(nameof(TubeData));
                OnPropertyChanged(nameof(Diameter));
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
