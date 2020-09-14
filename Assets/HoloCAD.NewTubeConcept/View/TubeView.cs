using System.Collections.Generic;
using HoloCAD.NewTubeConcept.Model;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.View
{
    public class TubeView : MonoBehaviour
    {
        public FlangeView StartFlangeView;
        public FlangeView EndFlangeView;
        public List<SegmentView> SegmentViews = new List<SegmentView>();

        public Tube tube;

        private void Start()
        {
            tube.SegmentAdded += OnSegmentAdded;
            OnSegmentAdded(tube.StartFlange.FirstSegment);
            OnSegmentAdded(tube.EndFlange.FirstSegment);

            foreach (var segment in tube.Segments)
            {
                OnSegmentAdded(segment);
            }

            tube.PointAdded += OnPointAdded;
            OnPointAdded(tube.StartFlange.FirstSegment.End);
            OnPointAdded(tube.EndFlange.FirstSegment.End);

            foreach (var point in tube.Points)
            {
                OnPointAdded(point);
            }
        }

        private void OnPointAdded(GCMPoint point)
        {
            var go = Instantiate(TubePrefabsContainer.Instance.PointPrefab, transform);
            go.GetComponent<PointView>().Point = point;
        }

        private void OnSegmentAdded(Segment segment)
        {
            var go = Instantiate(TubePrefabsContainer.Instance.SegmentPrefab, transform);
            var sv = go.GetComponent<SegmentView>();
            sv.segment = segment;
            SegmentViews.Add(sv);
        }
    }
}