using UnityC3D;

namespace HoloCAD.NewTubeConcept
{
    public static class GCMSystemExtensions
    {
        public static void MakeCoincident(this GCMSystem sys, Segment segment, GCMPlane plane)
        {
            sys.MakeCoincident(segment.Line, plane);
        }

        public static void MakeCoincident(this GCMSystem sys, Segment first, Segment second,
                                          GCMAlignment alignment = GCMAlignment.Cooriented)
        {
            if (alignment != GCMAlignment.Cooriented)
            {
                sys.MakeCoincident(first.Start, second.End);
                sys.MakeCoincident(first.End, second.Start);
            }
            else
            {
                sys.MakeCoincident(first.Start, second.Start);
                sys.MakeCoincident(first.End, second.End);
            }
        }

        public static void MakeParallel(this GCMSystem sys, Segment segment, GCMLine line,
                                             GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            sys.MakeParallel(segment.Line, line, alignment);
        }

        public static void MakePerpendicular(this GCMSystem sys, Segment segment, GCMPlane plane,
                                             GCMAlignment alignment = GCMAlignment.NoAlignment)
        {
            sys.MakePerpendicular(segment.Line, plane, alignment);
        }
    }
}