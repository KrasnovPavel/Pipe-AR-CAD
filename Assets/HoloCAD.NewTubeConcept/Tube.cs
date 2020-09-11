using System;
using System.Collections.Generic;
using System.Linq;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept
{
    public class Tube : IDisposable
    {
        public readonly GCMPlane StartPlane;
        public readonly Segment StartSegment;

        public readonly GCMPlane EndPlane;
        public readonly Segment EndSegment;

        public const float BendRadius = 0.1f;

        public readonly List<Segment> Segments = new List<Segment>();
        
        public event Action<Segment> SegmentAdded; 

        public Tube(GCMSystem sys, Vector3 startPoint, Vector3 startNormal, Vector3 endPoint, Vector3 endNormal,
                    GCM_LCS parentLCS)
        {
            var point1 = new GCMPoint(sys, startPoint, sys.GroundLCS);
            point1.Freeze();
            var point2 = new GCMPoint(sys, startPoint + startNormal * BendRadius, sys.GroundLCS);
            var point3 = new GCMPoint(sys, endPoint + endNormal * BendRadius, sys.GroundLCS);
            var point4 = new GCMPoint(sys, endPoint, sys.GroundLCS);
            point4.Freeze();
            

            StartSegment = new Segment(point1, point2, null, null);
            Segments.Add(new Segment(point2, point3, StartSegment, null));
            EndSegment = new Segment(point3, point4, Segments[0], null);
            
            StartPlane = new GCMPlane(sys, startPoint, startNormal, parentLCS);
            StartPlane.Freeze();
            EndPlane = new GCMPlane(sys, endPoint, endNormal, parentLCS);
            EndPlane.Freeze();

            sys.MakePerpendicular(StartSegment, StartPlane, GCMAlignment.Cooriented);
            sys.MakePerpendicular(EndSegment, EndPlane, GCMAlignment.Cooriented);

            sys.Evaluate();
        }

        // public void TestDraw(GameObject drawObject, GameObject pointPrefab)
        // {
        //     var lineRenderer = drawObject.GetComponent<LineRenderer>();
        //
        //     var points = new List<Vector3> {StartSegment.Start.Origin, StartSegment.End.Origin};
        //     points.AddRange(Segments.Select(s => s.End.Origin));
        //     points.Add(EndSegment.End.Origin);
        //     lineRenderer.positionCount = points.Count;
        //     lineRenderer.SetPositions(points.ToArray());
        //
        //     foreach (var point in points)
        //     {
        //         GameObject.Instantiate(pointPrefab, point, Quaternion.identity, drawObject.transform);
        //     }
        // }

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
            var first = new Segment(start, middle, segment.Parent, null);;
            var second = new Segment(middle, end, first, segment.Child);
            
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

            StartPlane?.Dispose();
            EndPlane?.Dispose();
        }
    }
}