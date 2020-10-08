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
    public class Segment : IDisposable, INotifyPropertyChanged
    {
        public readonly TubePoint Start;
        public readonly TubePoint End;
        public readonly GCMLine   Line;
        public          Tube      Owner;

        public float LineLength => (End.Origin - Start.Origin).magnitude;

        public float TubeLength => LineLength - End.DeltaLength - Start.DeltaLength; 

        public Vector3 Middle => (Start.Origin + End.Origin) / 2;

        public event Action<Segment> Disposed;

        [CanBeNull] public Segment Next => End.Next;

        [CanBeNull] public Segment Prev => Start.Prev;

        public GCMSystem Sys => Start.GCMSys;

        public bool IsInFlange => Owner.StartFlange.FirstSegment == this || Owner.EndFlange.FirstSegment == this;

        public float Diameter
        {
            get => _diameter;
            set
            {
                if (Mathf.Abs(_diameter - value) < float.Epsilon) return;
                _diameter = value;
                OnPropertyChanged();
            }
        }

        public Segment(TubePoint start, TubePoint end, Tube owner)
        {
            Start = start;
            End   = end;
            Owner = owner;

            Start.Next = this;
            End.Prev   = this;

            Line = new GCMLine(Sys, start.Origin, Start.Origin - End.Origin, Start.Parent);
            Sys.MakeCoincident(Start, Line);
            Sys.MakeCoincident(End, Line);

            Diameter = 0.05f;

            Start.PropertyChanged += PointOnPropertyChanged;
            End.PropertyChanged += PointOnPropertyChanged;
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

        public void Move(Vector3 newOrigin, Vector3 newDirection)
        {
            var length = LineLength;
            Start.Origin = newOrigin;
            Line.Origin = newOrigin;
            Line.Direction = newDirection;
            End.Origin = Start.Origin + newDirection * length;
            Start.Prev?.ResetLine();
            End.Next?.ResetLine();
            Sys.Evaluate();
            Owner.FixErrors();
        }

        public void ResetLine()
        {
            Line.Origin    = Start.Origin;
            Line.Direction = (End.Origin - Start.Origin).normalized;
        }

        public void Dispose()
        {
            Line?.Dispose();
            Disposed?.Invoke(this);
            Start.PropertyChanged -= PointOnPropertyChanged;
            End.PropertyChanged -= PointOnPropertyChanged;
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



        private void PointOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(LineLength));
        }

        #endregion
    }
}