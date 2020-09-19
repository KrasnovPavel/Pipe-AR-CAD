﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using HoloCAD.NewTubeConcept.Model;
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
            var startFlangeSegment = tube.StartFlange.FirstSegment;
            var endFlangeSegment = tube.EndFlange.FirstSegment;
            OnSegmentAdded(startFlangeSegment);
            OnSegmentAdded(endFlangeSegment);

            foreach (var segment in tube.Segments)
            {
                OnSegmentAdded(segment);
            }

            tube.PointAdded += OnPointAdded;
            OnPointAdded(startFlangeSegment.End);
            OnPointAdded(endFlangeSegment.End);

            foreach (var point in tube.Points)
            {
                OnPointAdded(point);
            }
        }

        private void OnPointAdded(TubePoint point)
        {
            var go = Instantiate(TubePrefabsContainer.Instance.PointPrefab, transform);
            var p = go.GetComponent<PointView>(); 
            p.Point = point;
            p.Owner = this;
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