// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.Model
{
    public class TubePoint : GCMPoint
    {
        [CanBeNull] public Segment Prev;
        [CanBeNull] public Segment Next;

        public Tube Owner => Next?.Owner ?? Prev?.Owner;

        public bool IsInFlange => GetFlange() != null;
        
        public float DeltaLength => BendRadius * Mathf.Tan(GetBendAngle() / 2 * Mathf.Deg2Rad);
        public bool IsInEndFlange => IsInFlange && ReferenceEquals(Next.End, this);

        public float BendRadius = 0.1f;

        public TubePoint(GCMSystem sys, Vector3 origin, GCM_LCS parent = null) :
            base(sys, origin, parent)
        {
            
        }

        public TubePoint(GCMSystem sys, Vector3 origin, Segment prev, Segment next, GCM_LCS parent = null) :
            base(sys, origin, parent)
        {
            Prev = prev;
            Next = next;
        }

        public float GetBendAngle()
        {
            if (Next == null || Prev == null) return 0;
            
            if (IsInEndFlange)
            {
                return 180 - Vector3.Angle(Next.Start.Origin - Origin, Prev.Start.Origin - Origin);
            }
            return 180 - Vector3.Angle(Next.End.Origin - Origin, Prev.Start.Origin - Origin);
        }

        [CanBeNull]
        public Flange GetFlange()
        {
            if (ReferenceEquals(Owner.StartFlange.EndPoint, this)) return Owner.StartFlange; 
            if (ReferenceEquals(Owner.EndFlange.EndPoint, this)) return Owner.EndFlange;
            return null;
        }
    }
}