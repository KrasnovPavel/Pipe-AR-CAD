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
        public readonly GCMPlane Plane;
        public readonly GCMLine  Axis;
        public readonly GCMPoint Point;

        public Vector3 Origin
        {
            get => Point.Origin;
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
            Axis  = new GCMLine(sys, Vector3.zero, Vector3.forward, sys.GroundLCS);
            Point = new GCMPoint(sys, Vector3.zero, sys.GroundLCS);
            Plane.Freeze();
            Point.Freeze();
            sys.MakeCoincident(Point, Axis);
            sys.MakePerpendicular(Axis, Plane);

            Point.PropertyChanged += OnPropertyChanged;
            Plane.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, Point) && e.PropertyName == nameof(Point.Origin))
            {
                OnPropertyChanged(nameof(Origin));   
            }

            if (ReferenceEquals(sender, Plane) && e.PropertyName == nameof(Plane.Origin))
            {
                OnPropertyChanged(nameof(Normal));   
            }
        }

        public void Move(Vector3 newPos, Vector3 newNorm)
        {
            Plane.Origin = newPos;
            Plane.Normal = newNorm;
            Point.Origin = newPos;
            _sys.Evaluate();
        }

        public void Dispose()
        {
            Point.PropertyChanged -= OnPropertyChanged;
            Plane.PropertyChanged -= OnPropertyChanged;
            
            Plane?.Dispose();
            Axis?.Dispose();
            Point?.Dispose();
        }

        #region Private definitions

        private GCMSystem _sys;

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