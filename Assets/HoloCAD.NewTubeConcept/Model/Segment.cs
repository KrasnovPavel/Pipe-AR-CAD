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

        [CanBeNull] public Segment Prev => Start.Prev;

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