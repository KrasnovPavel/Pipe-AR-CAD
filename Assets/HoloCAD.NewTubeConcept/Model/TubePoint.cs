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

        [CanBeNull]
        public Flange GetFlange()
        {
            if (ReferenceEquals(Owner.StartFlange.EndPoint, this)) return Owner.StartFlange; 
            if (ReferenceEquals(Owner.EndFlange.EndPoint, this)) return Owner.EndFlange;
            return null;
        }
    }
}