// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloTest;
using MathExtensions;
using UnityEngine;
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace UnityC3D.Tests
{
    [HoloTestClass]
    public class ConstraintTests
    {
        public const float Epsilon = 0.0001f;

        [HoloTestCase]
        public static void CoincidentPoints()
        {
            using (var sys = new GCMSystem())
            {
                var p1 = new GCMPoint(sys, Vector3.back);
                var p2 = new GCMPoint(sys, Vector3.zero);

                sys.MakeCoincident(p1, p2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(p1.Origin, p2.Origin);
            }
        }

        [HoloTestCase]
        public static void CoincidentLines()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var l2 = new GCMLine(sys, Vector3.zero, Vector3.up);

                sys.MakeCoincident(l1, l2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(l1.Origin, l2.Origin, Epsilon);
                Assert.IsTrue(l1.Direction.IsCollinear(l2.Direction));
            }
        }

        [HoloTestCase]
        public static void CoincidentLinePoint()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var p1 = new GCMPoint(sys, Vector3.zero);

                sys.MakeCoincident(p1, l1);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(l1.Direction.IsCollinear(l1.Origin - p1.Origin));
            }
        }

        [HoloTestCase]
        public static void CoincidentPlanes()
        {
            using (var sys = new GCMSystem())
            {
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);
                var pl2 = new GCMPlane(sys, Vector3.zero, Vector3.up);

                sys.MakeCoincident(pl1, pl2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(pl1.Normal.IsCollinear(pl2.Normal));
                Assert.AreEqual(pl1.Origin.ProjectOn(pl1.Normal), pl2.Origin.ProjectOn(pl1.Normal), Epsilon);
            }
        }

        [HoloTestCase]
        public static void CoincidentPointPlane()
        {
            using (var sys = new GCMSystem())
            {
                var p1 = new GCMPoint(sys, Vector3.zero);
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);

                sys.MakeCoincident(p1, pl1);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(pl1.Origin.ProjectOn(pl1.Normal), p1.Origin.ProjectOn(pl1.Normal), Epsilon);
            }
        }

        [HoloTestCase]
        public static void CoincidentLinePlane()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.zero, Vector3.up);
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);

                sys.MakeCoincident(l1, pl1);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(l1.Direction.IsPerpendicular(pl1.Normal));
                Assert.AreEqual(GeometryUtils.DistanceLinePlane(l1, pl1), 0);
            }
        }

        [HoloTestCase]
        public static void CoincidentCircles()
        {
            using (var sys = new GCMSystem())
            {
                var c1 = new GCMCircle(sys, Vector3.back, Vector3.forward, 20);
                var c2 = new GCMCircle(sys, Vector3.zero, Vector3.up, 20);

                sys.MakeCoincident(c1, c2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(c1.Origin, c2.Origin, Epsilon);
                Assert.IsTrue(c1.Normal.IsCollinear(c2.Normal));
                Assert.AreEqual(c1.Radius, c2.Radius);

                var c3 = new GCMCircle(sys, Vector3.back, Vector3.forward, 20);
                var c4 = new GCMCircle(sys, Vector3.zero, Vector3.up, 10);
                sys.MakeCoincident(c3, c4);
                Assert.AreNotEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
            }
        }

        [HoloTestCase]
        public static void CoincidentPlaneCircle()
        {
            using (var sys = new GCMSystem())
            {
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);
                var c1 = new GCMCircle(sys, Vector3.one, Vector3.back, 20);

                sys.MakeCoincident(c1, pl1);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(pl1.Normal.IsCollinear(c1.Normal));
                Assert.AreEqual(pl1.Origin.ProjectOn(pl1.Normal), c1.Origin.ProjectOn(pl1.Normal), Epsilon);
            }
        }

        [HoloTestCase]
        public static void CoincidentLCS()
        {
            using (var sys = new GCMSystem())
            {
                var g1 = new GameObject();
                Transform t1 = GameObject.Instantiate(g1).transform;
                t1.position = Vector3.one;
                t1.rotation = Quaternion.Euler(Vector3.left);
                var g2 = new GameObject();
                Transform t2 = GameObject.Instantiate(g1).transform;
                t2.position = Vector3.one;
                t2.rotation = Quaternion.Euler(Vector3.left);

                var lcs1 = new GCM_LCS(sys, t1);
                var lcs2 = new GCM_LCS(sys, t2);

                sys.MakeCoincident(lcs1, lcs2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(lcs1.Placement, lcs2.Placement);

                GameObject.Destroy(g1);
                GameObject.Destroy(g2);
            }
        }

        [HoloTestCase]
        public static void CoincidentPointLCS()
        {
            using (var sys = new GCMSystem())
            {
                var g1 = new GameObject();
                Transform t1 = GameObject.Instantiate(g1).transform;
                t1.position = Vector3.one;
                t1.rotation = Quaternion.Euler(Vector3.left);
                var lcs1 = new GCM_LCS(sys, t1);
                var p1 = new GCMPoint(sys, Vector3.zero);

                sys.MakeCoincident(p1, lcs1);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(p1.Origin, lcs1.Origin);
                
                GameObject.Destroy(g1);
            }
        }

        [HoloTestCase]
        public static void ConcentricCircles()
        {
            using (var sys = new GCMSystem())
            {
                var circle1 = new GCMCircle(sys, Vector3.back, Vector3.forward, 20);
                var circle2 = new GCMCircle(sys, Vector3.zero, Vector3.up, 10);

                sys.MakeConcentric(circle1, circle2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(circle1.Normal.IsCollinear(circle2.Normal));
                Assert.IsTrue(circle1.Normal.IsCollinear(circle1.Origin));
                Assert.IsTrue(circle1.Normal.IsCollinear(circle2.Origin));
                Assert.AreNotEqual(circle1.Radius, circle2.Radius);
            }
        }

        [HoloTestCase]
        public static void ConcentricPointCircle()
        {
            using (var sys = new GCMSystem())
            {
                var point = new GCMPoint(sys, Vector3.left);
                var circle = new GCMCircle(sys, Vector3.zero, Vector3.down, 2);
                sys.MakeConcentric(circle, point);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(point.Origin, circle.Origin);
            }
        }

        [HoloTestCase]
        public static void ConcentricLineCircle()
        {
            using (var sys = new GCMSystem())
            {
                var line = new GCMLine(sys, Vector3.left, Vector3.one);
                var circle = new GCMCircle(sys, Vector3.zero, Vector3.down, 2);
                sys.MakeConcentric(circle, line);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(line.Direction.IsCollinear(circle.Origin));
                Assert.IsTrue(line.Direction.IsCollinear(circle.Normal));
            }
        }

        [HoloTestCase]
        public static void ParallelLines()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var l2 = new GCMLine(sys, Vector3.zero, Vector3.up);
                sys.MakeParallel(l1, l2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(l1.Direction.IsCollinear(l2.Direction));
            }
        }

        [HoloTestCase]
        public static void ParallelPlanes()
        {
            using (var sys = new GCMSystem())
            {
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);
                var pl2 = new GCMPlane(sys, Vector3.zero, Vector3.up);
                sys.MakeParallel(pl1, pl2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(pl1.Normal.IsCollinear(pl2.Normal));
            }
        }

        [HoloTestCase]
        public static void ParallelPlaneLine()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.zero, Vector3.one);
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);
                sys.MakeParallel(l1, pl1);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(pl1.Normal.IsPerpendicular(l1.Direction));
            }
        }

        [HoloTestCase]
        public static void PerpendicularLines()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var l2 = new GCMLine(sys, Vector3.zero, Vector3.up);
                sys.MakePerpendicular(l1, l2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(l1.Direction.IsPerpendicular(l2.Direction));
            }
        }

        [HoloTestCase]
        public static void PerpendicularPlanes()
        {
            using (var sys = new GCMSystem())
            {
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);
                var pl2 = new GCMPlane(sys, Vector3.zero, Vector3.up);
                sys.MakePerpendicular(pl1, pl2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(pl1.Normal.IsPerpendicular(pl2.Normal));
            }
        }

        [HoloTestCase]
        public static void PerpendicularPlaneLine()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);
                sys.MakePerpendicular(l1, pl1);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(pl1.Normal.IsCollinear(l1.Direction));
            }
        }
        
        [HoloTestCase]
        public static void DistancePoints()
        {
            using (var sys = new GCMSystem())
            {
                var p1 = new GCMPoint(sys, Vector3.back);
                var p2 = new GCMPoint(sys, Vector3.zero);

                sys.SetDistance(p1, p2, 5.6f);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual((p1.Origin - p2.Origin).magnitude, 5.6f);
            }
        }

        [HoloTestCase]
        public static void DistanceLines()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var l2 = new GCMLine(sys, Vector3.back, Vector3.forward);

                sys.SetDistance(l1, l2, 34);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistanceLines(l1, l2), 34);
                
                var l3 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var l4 = new GCMLine(sys, Vector3.one, Vector3.up);

                sys.SetDistance(l3, l4, 0.76f);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistanceLines(l3, l4), 0.76f, Epsilon);
            }
        }

        [HoloTestCase]
        public static void DistanceLinePoint()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var p1 = new GCMPoint(sys, Vector3.back);

                sys.SetDistance(p1, l1, 34);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistanceLinePoint(p1, l1), 34);
            }
        }

        [HoloTestCase]
        public static void DistancePlanes()
        {
            using (var sys = new GCMSystem())
            {
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);
                var pl2 = new GCMPlane(sys, Vector3.zero, Vector3.forward);

                sys.SetDistance(pl1, pl2, 6.4f);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistancePlanes(pl1, pl2), 6.4f);
                
                var pl3 = new GCMPlane(sys, Vector3.one, Vector3.up);
                var pl4 = new GCMPlane(sys, Vector3.zero, Vector3.forward);

                sys.SetDistance(pl3, pl4, 63.4f);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistancePlanes(pl3, pl4), 63.4f);
            }
        }

        [HoloTestCase]
        public static void DistancePointPlane()
        {
            using (var sys = new GCMSystem())
            {
                var p1 = new GCMPoint(sys, Vector3.zero);
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);

                sys.SetDistance(p1, pl1, 10);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistancePointPlane(p1, pl1), 10);
            }
        }

        [HoloTestCase]
        public static void DistanceLinePlane()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.one, Vector3.forward);
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);

                sys.SetDistance(l1, pl1, 10);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistanceLinePlane(l1, pl1), 10, Epsilon);
                
                var l2 = new GCMLine(sys, Vector3.one, Vector3.up);
                var pl2 = new GCMPlane(sys, Vector3.back, Vector3.forward);

                sys.SetDistance(l2, pl2, 14.51f);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistanceLinePlane(l2, pl2), 14.51f, Epsilon);
            }
        }

        [HoloTestCase]
        public static void ChangeDistance()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.one, Vector3.forward);
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);

                sys.SetDistance(l1, pl1, 10);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistanceLinePlane(l1, pl1), 10, Epsilon);

                sys.SetDistance(l1, pl1, 35.2f);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(GeometryUtils.DistanceLinePlane(l1, pl1), 35.2f, Epsilon);
            }
        }

        [HoloTestCase]
        public static void AngleLines()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var l2 = new GCMLine(sys, Vector3.back, Vector3.up);

                sys.SetAngle(l1, l2, Mathf.PI / 4);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(l1.Direction, l2.Direction), 45, Epsilon);
                
                var l3 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var l4 = new GCMLine(sys, Vector3.one, Vector3.up);

                sys.SetAngle(l3, l4, 2 * Mathf.PI / 3);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(l3.Direction, l4.Direction), 120, Epsilon);
            }
        }

        [HoloTestCase]
        public static void AnglePlanes()
        {
            using (var sys = new GCMSystem())
            {
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);
                var pl2 = new GCMPlane(sys, Vector3.one, Vector3.up);

                sys.SetAngle(pl1, pl2, Mathf.PI / 4);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(pl1.Normal, pl2.Normal), 45, Epsilon);
            }
        }

        [HoloTestCase]
        public static void AngleLinePlane()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var pl2 = new GCMPlane(sys, Vector3.back, Vector3.left);

                sys.SetAngle(l1, pl2, Mathf.PI / 3);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(l1.Direction, pl2.Normal), 30, Epsilon);
            }
        }

        [HoloTestCase]
        public static void AngleCircles()
        {
            using (var sys = new GCMSystem())
            {
                var c1 = new GCMCircle(sys, Vector3.back, Vector3.forward, 10);
                var c2 = new GCMCircle(sys, Vector3.one, Vector3.right, 20);

                sys.SetAngle(c1, c2, Mathf.PI / 4);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(c1.Normal, c2.Normal), 45, Epsilon);
            }
        }

        [HoloTestCase]
        public static void AngleLineCircle()
        {
            using (var sys = new GCMSystem())
            {
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);
                var c2 = new GCMCircle(sys, Vector3.one, Vector3.left, 20);

                sys.SetAngle(l1, c2, Mathf.PI / 3);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(l1.Direction, c2.Normal), 30, Epsilon);
            }
        }

        [HoloTestCase]
        public static void AngleCirclePlane()
        {
            using (var sys = new GCMSystem())
            {
                var c1 = new GCMCircle(sys, Vector3.back, Vector3.forward, 10);
                var pl2 = new GCMPlane(sys, Vector3.one, Vector3.right);

                sys.SetAngle(pl2, c1, Mathf.PI / 4);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(c1.Normal, pl2.Normal), 45, Epsilon);
            }
        }

        [HoloTestCase]
        public static void AngleLCS()
        {
            using (var sys = new GCMSystem())
            {
                var g1 = new GameObject();
                Transform t1 = GameObject.Instantiate(g1).transform;
                t1.position = Vector3.one;
                t1.rotation = Quaternion.Euler(Vector3.left);
                var g2 = new GameObject();
                Transform t2 = GameObject.Instantiate(g1).transform;
                t2.position = Vector3.zero;
                t2.rotation = Quaternion.Euler(Vector3.up);

                var lcs1 = new GCM_LCS(sys, t1);
                var lcs2 = new GCM_LCS(sys, t2);

                sys.SetAngle(lcs1, lcs2, Mathf.PI / 4);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(lcs1.Placement.AxisZ, lcs2.Placement.AxisZ), 45, Epsilon);

                GameObject.Destroy(g1);
                GameObject.Destroy(g2);
            }
        }

        [HoloTestCase]
        public static void AngleCircleLCS()
        {
            using (var sys = new GCMSystem())
            {
                var g1 = new GameObject();
                Transform t1 = GameObject.Instantiate(g1).transform;
                t1.position = Vector3.one;
                t1.rotation = Quaternion.Euler(Vector3.left);

                var lcs1 = new GCM_LCS(sys, t1);
                var c1 = new GCMCircle(sys, Vector3.back, Vector3.forward, 10);

                sys.SetAngle(c1, lcs1, Mathf.PI / 4);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(lcs1.Placement.AxisZ, c1.Normal), 45, Epsilon);

                GameObject.Destroy(g1);
            }
        }

        [HoloTestCase]
        public static void AngleLineLCS()
        {
            using (var sys = new GCMSystem())
            {
                var g1 = new GameObject();
                Transform t1 = GameObject.Instantiate(g1).transform;
                t1.position = Vector3.one;
                t1.rotation = Quaternion.Euler(Vector3.left);

                var lcs1 = new GCM_LCS(sys, t1);
                var l1 = new GCMLine(sys, Vector3.back, Vector3.forward);

                sys.SetAngle(l1, lcs1, Mathf.PI / 4);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(lcs1.Placement.AxisZ, l1.Direction), 45, Epsilon);

                GameObject.Destroy(g1);
            }
        }

        [HoloTestCase]
        public static void AnglePlaneLCS()
        {
            using (var sys = new GCMSystem())
            {
                var g1 = new GameObject();
                Transform t1 = GameObject.Instantiate(g1).transform;
                t1.position = Vector3.one;
                t1.rotation = Quaternion.Euler(Vector3.left);

                var lcs1 = new GCM_LCS(sys, t1);
                var pl2 = new GCMPlane(sys, Vector3.one, Vector3.right);

                sys.SetAngle(pl2, lcs1, Mathf.PI / 3);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(lcs1.Placement.AxisZ, pl2.Normal), 30, Epsilon);

                GameObject.Destroy(g1);
            }
        }

        [HoloTestCase]
        public static void ChangeAngle()
        {
            using (var sys = new GCMSystem())
            {
                var c1 = new GCMCircle(sys, Vector3.back, Vector3.forward, 10);
                var pl2 = new GCMPlane(sys, Vector3.one, Vector3.right);

                sys.SetAngle(pl2, c1, Mathf.PI / 4);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(c1.Normal, pl2.Normal), 45, Epsilon);

                sys.SetAngle(pl2, c1, Mathf.PI / 6);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(Vector3.Angle(c1.Normal, pl2.Normal), 30, Epsilon);
            }
        }

        [HoloTestCase]
        public static void RemoveConstraint()
        {
            using (var sys = new GCMSystem())
            {
                var pl1 = new GCMPlane(sys, Vector3.back, Vector3.forward);
                var pl2 = new GCMPlane(sys, Vector3.one, Vector3.right);

                Assert.IsFalse(pl1.Normal.IsCollinear(pl2.Normal));
                var cons = sys.MakeParallel(pl1, pl2);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(pl1.Normal.IsCollinear(pl2.Normal));
                pl1.Normal = Vector3.down;
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsTrue(pl1.Normal.IsCollinear(pl2.Normal));
                sys.RemoveConstraint(cons);
                pl1.Normal = Vector3.forward;
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.IsFalse(pl1.Normal.IsCollinear(pl2.Normal));
            }
        }
    }
}