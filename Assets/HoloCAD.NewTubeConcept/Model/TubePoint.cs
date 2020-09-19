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
    }
}