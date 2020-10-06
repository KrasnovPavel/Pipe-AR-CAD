// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.Model
{
    public class Tube : IDisposable
    {
        public const float BendRadius = 0.1f;
        public readonly Flange EndFlange;
        public readonly List<TubePoint> Points = new List<TubePoint>();

        public readonly List<Segment> Segments = new List<Segment>();
        public readonly Flange StartFlange;

        public Tube(GCMSystem sys, Flange startFlange, Flange endFlange)
        {
            StartFlange = startFlange;
            EndFlange = endFlange;

            StartFlange.FirstSegment.Owner = this;
            EndFlange.FirstSegment.Owner = this;

            var middle = new Segment(StartFlange.EndPoint,
                                     EndFlange.EndPoint,
                                     this);

            StartFlange.EndPoint.Next = middle;
            EndFlange.EndPoint.Next = EndFlange.FirstSegment;
            EndFlange.EndPoint.Prev = middle;

            Segments.Add(middle);

            sys.Evaluate();
        }

        public GCMSystem Sys => StartFlange.Plane.GCMSys;

        public void Dispose()
        {
            Segments[0].Start?.Dispose();
            foreach (var point in Segments.Select(s => s.End)) point?.Dispose();

            foreach (var segment in Segments) segment?.Dispose();

            foreach (var point in Points) point?.Dispose();

            StartFlange?.Dispose();
            EndFlange?.Dispose();
        }

        public event Action<Segment> SegmentAdded;

        public event Action<TubePoint> PointAdded;

        public void FixErrors()
        {
            var hasErrors = StartFlange.Fix();
            hasErrors = hasErrors || EndFlange.Fix();
            if (hasErrors) Sys.Evaluate();
        }

        public bool IsCorrect()
        {
            return Segments.All(s => s.GetMinimalLength(BendRadius) < s.Length);
        }

        public void AddPoint(Segment segment)
        {
            AddPoint(segment, segment.Middle);
        }

        public void AddPoint(Segment segment, Vector3 pos)
        {
            // TODO: ГОВНО!!! ПЕРЕДЕЛАТЬ!!!
            if (segment == StartFlange.FirstSegment)
            {
                var point = segment.End.Origin;
                AddPoint(segment.Next);
                if (segment.Next?.Next == null) return;
                segment.End.Origin = (StartFlange.Origin + pos) / 2;
                segment.Next.End.Origin = pos;
                segment.Next.Next.End.Origin = point;
                segment.Next.ResetLine();
                segment.Next.Next?.ResetLine();
                Sys.Evaluate();
                return;
            }

            if (segment == EndFlange.FirstSegment)
            {
                var point = segment.End.Origin;
                AddPoint(segment.End.Prev);
                if (segment.End.Prev?.Prev == null) return;
                segment.End.Origin = (EndFlange.Origin + pos) / 2;
                segment.End.Prev.Start.Origin = pos;
                segment.End.Prev.Prev.Start.Origin = point;
                segment.End.Prev.ResetLine();
                segment.End.Prev.Prev?.ResetLine();
                Sys.Evaluate();
                return;
            }
            // Дальше, вроде, не говно.

            if (!Segments.Contains(segment))
                throw new ArgumentOutOfRangeException(nameof(segment), segment, "Tube does not contains this segment");

            Segments.Remove(segment);

            var start = segment.Start;
            var end = segment.End;

            var middle = new TubePoint(Sys, pos, Sys.GroundLCS);
            var first = new Segment(start, middle, this);
            var second = new Segment(middle, end, this);

            start.Next = first;
            middle.Prev = first;
            middle.Next = second;

            Segments.Add(first);
            Segments.Add(second);
            Points.Add(middle);

            PointAdded?.Invoke(middle);
            SegmentAdded?.Invoke(first);
            SegmentAdded?.Invoke(second);

            Segments.Remove(segment);
            segment.Dispose();
            Sys.Evaluate();
        }

        public void RemovePoint(TubePoint point)
        {
            if (ReferenceEquals(StartFlange.EndPoint, point)
                || ReferenceEquals(EndFlange.EndPoint, point))
                throw new ArgumentOutOfRangeException(nameof(point), point, "Can't remove this point");

            if (!Points.Contains(point))
                throw new ArgumentOutOfRangeException(nameof(point), point, "Tube does not contains this point");

            var prevSegment = point.Prev;
            var nextSegment = point.Next;
            var prevPoint = prevSegment?.Start;
            var nextPoint = nextSegment?.End;

            point.Dispose();
            prevSegment?.Dispose();
            nextSegment?.Dispose();
            Points.Remove(point);
            Segments.Remove(prevSegment);
            Segments.Remove(nextSegment);

            var newSegment = new Segment(prevPoint, nextPoint, this);
            Segments.Add(newSegment);
            SegmentAdded?.Invoke(newSegment);
            Sys.Evaluate();
        }
    }
}