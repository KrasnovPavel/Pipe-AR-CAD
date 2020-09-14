using System;
using System.Collections.Generic;
using System.Linq;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.Model
{
    public class Tube : IDisposable
    {
        public readonly Flange StartFlange;
        public readonly Flange EndFlange;

        public const float BendRadius = 0.1f;

        public readonly List<Segment> Segments = new List<Segment>();
        public readonly List<GCMPoint> Points = new List<GCMPoint>();

        public event Action<Segment> SegmentAdded;
        public event Action<GCMPoint> PointAdded;

        public Tube(GCMSystem sys, Flange startFlange, Flange endFlange)
        {
            StartFlange = startFlange;
            EndFlange = endFlange;

            StartFlange.FirstSegment.Owner = this;
            EndFlange.FirstSegment.Owner = this;

            Segments.Add(new Segment(StartFlange.FirstSegment.End,
                                     EndFlange.FirstSegment.End,
                                     StartFlange.FirstSegment,
                                     EndFlange.FirstSegment,
                                     this));

            sys.Evaluate();
        }

        public bool IsCorrect()
        {
            return Segments.All(s => s.GetMinimalLength(BendRadius) < s.Length);
        }

        public void AddPoint(Segment segment)
        {
            if (!Segments.Contains(segment))
            {
                throw new ArgumentOutOfRangeException(nameof(segment), segment, "Tube does not contains this segment");
            }

            Segments.Remove(segment);

            var start = segment.Start;
            var end = segment.End;
            var sys = start.GCMSys;

            var middle = new GCMPoint(sys, segment.Middle, sys.GroundLCS);
            var first = new Segment(start, middle, segment.Parent, null, this);
            var second = new Segment(middle, end, first, segment.Child, this);

            Segments.Add(first);
            Segments.Add(second);
            Points.Add(middle);

            PointAdded?.Invoke(middle);
            SegmentAdded?.Invoke(first);
            SegmentAdded?.Invoke(second);

            segment.Dispose();
            sys.Evaluate();
        }

        public void Dispose()
        {
            Segments[0].Start?.Dispose();
            foreach (var point in Segments.Select(s => s.End))
            {
                point?.Dispose();
            }

            foreach (var segment in Segments)
            {
                segment?.Dispose();
            }

            foreach (var point in Points)
            {
                point?.Dispose();
            }

            StartFlange?.Dispose();
            EndFlange?.Dispose();
        }

        #region Private defifnitons

        private List<Vector3> _savedPoints = new List<Vector3>();
        private List<MbPlacement3D> _savedLines = new List<MbPlacement3D>();

        #endregion
    }
}