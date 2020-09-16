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
        public readonly TubePoint Start;
        public readonly TubePoint End;
        public readonly GCMLine  Line;
        public Tube     Owner;

        public float Length => (End.Origin - Start.Origin).magnitude;

        public Vector3 Middle => (Start.Origin + End.Origin) / 2;
        
        public event Action<Segment> Disposed;

        [CanBeNull] public Segment Next => End.Next;
        // {
        //     get => _next;
        //     set
        //     {
        //         if (_next == value) return;
        //
        //         if (_next != null && value == null) _next.Prev = null;
        //         
        //         _next = value;
        //         if (value != null) value.Prev = this;
        //     }
        // }

        [CanBeNull] public Segment Prev => Start.Prev;
        // {
        //     get => _prev;
        //     set
        //     {
        //         if (_prev == value) return;
        //
        //         if (_prev != null && value == null) _prev.Next = null;
        //         
        //         _prev = value;
        //         if (value != null) value.Next = this;
        //     }
        // }

        public Segment(TubePoint start, TubePoint end, Tube owner)
        {
            Start      = start;
            End        = end;
            Owner      = owner;

            var sys = start.GCMSys;
            Line = new GCMLine(sys, start.Origin, Start.Origin - End.Origin, Start.Parent);
            sys.MakeCoincident(Start, Line);
            sys.MakeCoincident(End, Line);
        }

        public float GetMinimalLength(float bendRadius)
        {
            float childLength  = 0;
            float parentLength = 0;

            if (Next != null)
            {
                float angle = Vector3.Angle(Next.End.Origin - Next.Start.Origin, End.Origin - Start.Origin);
                childLength = bendRadius * Mathf.Tan(angle / 2);
            }

            if (Prev != null)
            {
                float angle = Vector3.Angle(Prev.End.Origin - Prev.Start.Origin, End.Origin - Start.Origin);
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
            Line?.Dispose();
            Disposed?.Invoke(this);
        }

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