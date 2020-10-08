// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.Model
{
    /// <summary> Фланец. </summary>
    public class Flange : IDisposable, INotifyPropertyChanged
    {
        protected const float DefaultFirstSegmentLength = 0.3f;
        
        public readonly GCMPlane Plane;
        public readonly Segment FirstSegment;

        public TubePoint EndPoint => FirstSegment.End;

        public Vector3 Origin
        {
            get => _startPoint.Origin;
            set => Move(value, Normal);
        }

        public Vector3 Normal
        {
            get => Plane.Normal;
            set => Move(Origin, value);
        }

        public float Diameter => FirstSegment.Diameter;
        
        public Tube Owner { get; private set; }
        
        public Flange(GCMSystem sys)
        {
            _sys  = sys;
            Plane = new GCMPlane(sys, Vector3.zero, Vector3.forward, sys.GroundLCS);
            _startPoint = new TubePoint(sys, Vector3.zero, sys.GroundLCS);
            _endPoint = new TubePoint(sys, Vector3.forward * DefaultFirstSegmentLength, sys.GroundLCS);
            FirstSegment = new Segment(_startPoint, _endPoint, null);
            Plane.Freeze();
            _startPoint.Freeze();
            sys.MakePerpendicular(FirstSegment, Plane);

            _startPoint.PropertyChanged += OnPropertyChanged;
            Plane.PropertyChanged += OnPropertyChanged;
            FirstSegment.PropertyChanged += OnPropertyChanged;
        }

        public void Move(Vector3 newPos, Vector3 newNorm)
        {
            Plane.Origin = newPos;
            Plane.Normal = newNorm;
            _startPoint.Origin = newPos;
            _sys.Evaluate();
        }

        public bool IsCorrect(float angleEps = 0.1f)
        {
            var actualDirection = EndPoint.Origin - Origin;
            return Vector3.Angle(actualDirection, Normal) < angleEps;
        }

        public bool Fix()
        {
            if (IsCorrect()) return false;

            EndPoint.Origin = Origin + Normal * DefaultFirstSegmentLength;
            FirstSegment.Next?.ResetLine();
            FirstSegment.Prev?.ResetLine();
            return true;
        }

        public void AddInTube(Tube tube)
        {
            if (Owner != null) RemoveFromTube();
            Owner = tube;
            FirstSegment.Owner = tube;
            OnPropertyChanged(nameof(Owner));
        }

        public void RemoveFromTube()
        {
            Owner = null;
            FirstSegment.Owner = null;
            OnPropertyChanged(nameof(Owner));
        }

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

        private GCMSystem _sys;
        private readonly TubePoint _startPoint;
        private readonly TubePoint _endPoint;

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