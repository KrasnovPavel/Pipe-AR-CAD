using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.Model
{
    public class Segment: IDisposable, INotifyPropertyChanged
    {
        public readonly GCMPoint Start;
        public readonly GCMPoint End;
        public readonly GCMLine  Line;
        public Tube     Owner;

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

        public Segment(GCMPoint start, GCMPoint end, Segment parent, Segment child, Tube owner)
        {
            Start      = start;
            End        = end;
            Child      = child;
            Owner      = owner;
            Parent     = parent;

            var sys = start.GCMSys;
            Line = new GCMLine(sys, start.Origin, Start.Origin - End.Origin, Start.Parent);
            sys.MakeCoincident(Start, Line);
            sys.MakeCoincident(End, Line);
        }

        public float GetMinimalLength(float bendRadius)
        {
            float childLength  = 0;
            float parentLength = 0;

            if (Child != null)
            {
                float angle = Vector3.Angle(Child.End.Origin - Child.Start.Origin, End.Origin - Start.Origin);
                childLength = bendRadius * Mathf.Tan(angle / 2);
            }

            if (Parent != null)
            {
                float angle = Vector3.Angle(Parent.End.Origin - Parent.Start.Origin, End.Origin - Start.Origin);
                parentLength = bendRadius * Mathf.Tan(angle / 2);
            }

            return childLength + parentLength;
        }

        public void ResetLine()
        {
            Line.Origin = Start.Origin;
            Line.Direction = (End.Origin - Start.Origin).normalized;
        }

        public void Dispose()
        {
            if (Child?.Parent == this) Child.Parent = null;
            else                       _child       = null;
            
            if (Parent?.Child == this) Parent.Child = null;
            else                       _parent      = null;
            
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