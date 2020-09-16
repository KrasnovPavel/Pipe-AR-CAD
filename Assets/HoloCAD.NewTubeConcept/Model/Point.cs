using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.Model
{
    public class TubePoint : GCMPoint
    {
        [CanBeNull] public Segment Prev;
        [CanBeNull] public Segment Next;

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