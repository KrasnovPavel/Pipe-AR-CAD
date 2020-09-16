﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MathExtensions;
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
            var actualDirection = FirstSegment.End.Origin - FirstSegment.Start.Origin;
            return Vector3.Angle(actualDirection, Normal) < angleEps;
        }

        public bool Fix()
        {
            if (IsCorrect()) return false;

            FirstSegment.End.Origin = FirstSegment.Start.Origin + Normal * DefaultFirstSegmentLength;
            FirstSegment.Next?.ResetLine();
            FirstSegment.Prev?.ResetLine();
            return true;
        }

        public void Dispose()
        {
            _startPoint.PropertyChanged -= OnPropertyChanged;
            Plane.PropertyChanged -= OnPropertyChanged;
            
            Plane?.Dispose();
            FirstSegment?.Dispose();
            _startPoint?.Dispose();
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