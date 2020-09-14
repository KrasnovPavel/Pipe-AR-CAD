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

        public event Action<Segment> SegmentAdded;

        public Tube(GCMSystem sys, Flange startFlange, Flange endFlange)
        {
            StartFlange = startFlange;
            EndFlange   = endFlange;

            var point1 = new GCMPoint(sys, StartFlange.Origin + StartFlange.Normal * BendRadius, sys.GroundLCS);
            var point2 = new GCMPoint(sys, EndFlange.Origin   + EndFlange.Normal   * BendRadius, sys.GroundLCS);

            Segments.Add(new Segment(point1, point2, null, null, this));

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
            var end   = segment.End;
            var sys   = start.GCMSys;

            var middle = new GCMPoint(sys, segment.Middle, sys.GroundLCS);
            var first  = new Segment(start, middle, segment.Parent, null, this);
            ;
            var second = new Segment(middle, end, first, segment.Child, this);

            Segments.Add(first);
            Segments.Add(second);

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

            StartFlange?.Dispose();
            EndFlange?.Dispose();
        }

        #region Private defifnitons

        private List<Vector3>       _savedPoints = new List<Vector3>();
        private List<MbPlacement3D> _savedLines  = new List<MbPlacement3D>();

        #endregion
    }
}