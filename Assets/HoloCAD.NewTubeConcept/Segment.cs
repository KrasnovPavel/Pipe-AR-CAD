using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;
using Vuforia;

namespace HoloCAD.NewTubeConcept
{
    public class Segment: IDisposable, INotifyPropertyChanged
    {
        public readonly GCMPoint Start;
        public readonly GCMPoint End;
        public readonly GCMLine Line;

        public float Length => (End.Origin - Start.Origin).magnitude;

        public Vector3 Middle => (Start.Origin + End.Origin) / 2;

        public event Action<Segment> Disposed;

        [CanBeNull]
        public Segment Child
        {
            get => _child;
            set
            {
                if (_child == value) return;

                if (_child != null && value == null) _child.Parent = null;
                
                _child = value;
                if (value != null) value.Parent = this;
            }
        }

        [CanBeNull]
        public Segment Parent
        {
            get => _parent;
            set
            {
                if (_parent == value) return;

                if (_parent != null && value == null) _parent.Child = null;
                
                _parent = value;
                if (value != null) value.Child = this;
            }
        }

        public Segment(GCMPoint start, GCMPoint end, Segment parent, Segment child)
        {
            Start = start;
            End = end;
            Child = child;
            Parent = parent;

            var sys = start.GCMSys;
            Line = new GCMLine(sys, start.Origin, End.Origin - Start.Origin, Start.Parent);
            sys.MakeCoincident(Start, Line);
            sys.MakeCoincident(End, Line);
        }

        public void Dispose()
        {
            Line?.Dispose();
            Disposed?.Invoke(this);
        }
        
        private Segment _child;
        private Segment _parent;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}