using System.Collections.Generic;
using HoloCAD.NewTubeConcept.Model;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.View
{
    public class TubeView : MonoBehaviour
    {
        public FlangeView        StartFlangeView;
        public FlangeView        EndFlangeView;
        public List<SegmentView> SegmentViews;

        public Tube tube;
        
        
    }
}