// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityC3D;

namespace HoloCAD.NewTubeConcept.Model
{
    public static class SegmentConstraints
    {
        public static GCMConstraint MakeCoincident(this GCMSystem sys, Segment segment, GCMPlane plane)
        {
            return sys.MakeCoincident(segment.Line, plane);
        }

        public static GCMConstraint MakeParallel(this GCMSystem sys, Segment segment, GCMLine line,
                                             GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return sys.MakeParallel(segment.Line, line, alignment);
        }

        public static GCMConstraint MakeParallel(this GCMSystem sys, Segment segment, GCMPlane plane,
                                        GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return sys.MakeParallel(segment.Line, plane, alignment);
        }

        public static GCMConstraint MakePerpendicular(this GCMSystem sys, Segment segment, GCMPlane plane,
                                             GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            return sys.MakePerpendicular(segment.Line, plane, alignment);
        }
    }
}