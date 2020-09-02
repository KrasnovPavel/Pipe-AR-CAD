// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using HoloTest;
using MathExtensions;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.C3D.Tests
{
    [HoloTestClass]
    public class FragmentsTests
    {
        [HoloTestCase]
        public static void DirectFragmentCreation()
        {
            using (var sys = new GCMSystem())
            {
                var f = new DirectTubeFragment(sys, null, 34, 0.43f);
                Assert.AreEqual(f.Diameter, 34);
                Assert.AreEqual(f.Length, 0.43f);
                Assert.IsNull(f.Parent);
                Assert.AreEqual((f.StartCircle.Origin - f.EndCircle.Origin).magnitude, 0.43f);

                var f1 = new DirectTubeFragment(sys, f, 34, 2.4f);
                Assert.AreEqual(f.Diameter, 34);
                Assert.AreEqual((f1.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 2.4f);
                Assert.AreEqual((f.EndCircle.Origin - f1.EndCircle.Origin).magnitude, 2.4f);
                Assert.AreEqual((f.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 2.4f + 0.43f);
                Assert.AreEqual(f1.Parent, f);

                // ReSharper disable once ObjectCreationAsStatement
                // ReSharper disable once AccessToDisposedClosure
                Assert.ThrowsException<FragmentsNotConnectable>(() => new DirectTubeFragment(sys, f, 2, 2));
            }
        }

        [HoloTestCase]
        public static void DirectFragmentChanging()
        {
            using (var sys = new GCMSystem())
            {
                var f = new DirectTubeFragment(sys, null, 34, 0.43f);
                f.Diameter = 0.53f;
                Assert.AreEqual(f.Diameter, 0.53f);
                f.Length = 2.31f;
                Assert.AreEqual(f.Length, 2.31f);

                var f1 = new DirectTubeFragment(sys, f, 0.53f, 2.4f);
                Assert.ThrowsException<FragmentsNotConnectable>(() => f1.Diameter = 3.2f);

                f1.Length = 0.12f;
                Assert.AreEqual(f1.Length, 0.12f);
                Assert.AreEqual((f1.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 0.12f);
                Assert.AreEqual((f1.EndCircle.Origin - f.EndCircle.Origin).magnitude, 0.12f);
                Assert.AreEqual((f.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 2.31f + 0.12f);

                f.Length = 0.36f;
                Assert.AreEqual(f.Length, 0.36f);
                Assert.AreEqual((f.StartCircle.Origin - f.EndCircle.Origin).magnitude, 0.36f);
                Assert.AreEqual((f1.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 0.12f);
                Assert.AreEqual((f.EndCircle.Origin - f1.EndCircle.Origin).magnitude, 0.12f);
                Assert.AreEqual((f.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 0.36f + 0.12f);
            }
        }

        [HoloTestCase]
        public static void StartFragment()
        {
            using (var sys = new GCMSystem())
            {
                sys.SetJournal();
                var g = new GameObject();
                var t = GameObject.Instantiate(g).transform;

                var s = new StartFragment(sys, 34, MbPlacement3D.FromUnity(t));
                Assert.AreEqual(s.EndCircle.Origin, t.position);
                Assert.AreEqual(s.EndCircle.Normal, t.forward);
                Assert.AreEqual(s.Placement, MbPlacement3D.FromUnity(t));

                var f = new DirectTubeFragment(sys, s, 34, 0.43f);
                Assert.AreEqual(f.StartCircle.Origin, s.EndCircle.Origin);
                Assert.AreEqual(f.StartCircle.Normal, s.EndCircle.Normal);

                t.position = Vector3.down;
                t.LookAt(Vector3.one);
                s.SetPlacement(MbPlacement3D.FromUnity(t));

                Assert.AreEqual(s.Placement, MbPlacement3D.FromUnity(t));
                Assert.AreEqual(f.StartCircle.Origin, s.EndCircle.Origin);
                Assert.AreEqual(f.StartCircle.Normal, s.EndCircle.Normal);
                Assert.AreEqual((s.EndCircle.Origin - f.EndCircle.Origin).magnitude, 0.43f);

                GameObject.Destroy(g);
                GameObject.Destroy(t.gameObject);
            }
        }
        
        private static Vector3 GetBendedEndPoint(float bendRadius, float bendAngle, float rotationAngle)
        {
            return new
                Vector3(
                        bendRadius * (1 - Mathf.Cos(Mathf.Deg2Rad * bendAngle)) *
                        Mathf.Cos(Mathf.Deg2Rad * rotationAngle),
                        -bendRadius * (1 - Mathf.Cos(Mathf.Deg2Rad * bendAngle)) *
                        Mathf.Sin(Mathf.Deg2Rad * rotationAngle),
                        -bendRadius * Mathf.Sin(Mathf.Deg2Rad * bendAngle));
        }

        private struct AngleData
        {
            public float Bend;
            public float Rotation;

            public override string ToString()
            {
                return $"[bend {Bend:f1}, rotation {Rotation:f1}]";
            }
        }

        [HoloTestGenerator]
        public static IEnumerable<Action> BendedFragmentCreation()
        {
            var diameter   = 0.01f;
            var bendRadius = 0.15f;

            var angles = new[]
            {
                new AngleData {Bend = 90f, Rotation  = 0f},
                new AngleData {Bend = 30f, Rotation  = 0f},
                new AngleData {Bend = 45f, Rotation  = 0f},
                new AngleData {Bend = 180f, Rotation = 0f},
                new AngleData {Bend = 130f, Rotation = 0f},
                new AngleData {Bend = 90f, Rotation  = 45f},
                new AngleData {Bend = 30f, Rotation  = 45f},
                new AngleData {Bend = 45f, Rotation  = 45f},
                new AngleData {Bend = 180f, Rotation = 45f},
                new AngleData {Bend = 130f, Rotation = 45f},
                new AngleData {Bend = 90f, Rotation  = 90f},
                new AngleData {Bend = 30f, Rotation  = 90f},
                new AngleData {Bend = 45f, Rotation  = 90f},
                new AngleData {Bend = 180f, Rotation = 90f},
                new AngleData {Bend = 130f, Rotation = 90f},
                new AngleData {Bend = 90f, Rotation  = 60f},
                new AngleData {Bend = 30f, Rotation  = 60f},
                new AngleData {Bend = 45f, Rotation  = 60f},
                new AngleData {Bend = 180f, Rotation = 60f},
                new AngleData {Bend = 130f, Rotation = 60f},
                new AngleData {Bend = 90f, Rotation  = 120f},
                new AngleData {Bend = 30f, Rotation  = 120f},
                new AngleData {Bend = 45f, Rotation  = 120f},
                new AngleData {Bend = 180f, Rotation = 120f},
                new AngleData {Bend = 130f, Rotation = 120f},
                new AngleData {Bend = 90f, Rotation  = 180f},
                new AngleData {Bend = 30f, Rotation  = 180f},
                new AngleData {Bend = 45f, Rotation  = 180f},
                new AngleData {Bend = 180f, Rotation = 180f},
                new AngleData {Bend = 130f, Rotation = 180f},
                new AngleData {Bend = 90f, Rotation  = 200f},
                new AngleData {Bend = 30f, Rotation  = 200f},
                new AngleData {Bend = 45f, Rotation  = 200f},
                new AngleData {Bend = 180f, Rotation = 200f},
                new AngleData {Bend = 130f, Rotation = 200f},
                new AngleData {Bend = 90f, Rotation  = 270f},
                new AngleData {Bend = 30f, Rotation  = 270f},
                new AngleData {Bend = 45f, Rotation  = 270f},
                new AngleData {Bend = 180f, Rotation = 270f},
                new AngleData {Bend = 130f, Rotation = 270f},
                new AngleData {Bend = 90f, Rotation  = 300f},
                new AngleData {Bend = 30f, Rotation  = 300f},
                new AngleData {Bend = 45f, Rotation  = 300f},
                new AngleData {Bend = 180f, Rotation = 300f},
                new AngleData {Bend = 130f, Rotation = 300f},
                new AngleData {Bend = 90f, Rotation  = 360f},
                new AngleData {Bend = 30f, Rotation  = 360f},
                new AngleData {Bend = 45f, Rotation  = 360f},
                new AngleData {Bend = 180f, Rotation = 360f},
                new AngleData {Bend = 130f, Rotation = 360f},
                new AngleData {Bend = 90f, Rotation  = 450f},
                new AngleData {Bend = 30f, Rotation  = 450f},
                new AngleData {Bend = 45f, Rotation  = 450f},
                new AngleData {Bend = 180f, Rotation = 450f},
                new AngleData {Bend = 130f, Rotation = 450f},
            };

            foreach (var angle in angles)
            {
                yield return delegate
                             {
                                 using (var sys = new GCMSystem())
                                 {
                                     sys.SetJournal($"Journals\\BendedFragmentCreation{angle.ToString()}");
                                     var s = new StartFragment(sys, diameter, sys.GroundLCS.Placement);
                                     var b = new BendedFragment(sys, bendRadius, angle.Bend, angle.Rotation, diameter,
                                                                s);

                                     Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok, angle.ToString());
                                     Assert.AreEqual(b.EndCircle.Origin - b.StartCircle.Origin,
                                                     GetBendedEndPoint(bendRadius, angle.Bend, angle.Rotation),
                                                     Assert.Epsilon,
                                                     angle + " // EndCircle pos");

                                     Assert.AreEqual(
                                                     Geometry.DistancePointLine(b.EndCircle.Origin, b.RightAxis.Origin,
                                                                                    b.RightAxis.Direction),
                                                     0f,
                                                     angle + " // RightLine origin");
                                     Assert.AreEqual(b.RightAxis.Direction,
                                                     -(b.EndCircle.Origin -
                                                       Quaternion.AngleAxis(-angle.Rotation, b.StartCircle.Normal) *
                                                       Vector3.right * bendRadius).normalized,
                                                     Assert.Epsilon,
                                                     angle + " // Right Line direction");

                                     Assert.AreEqual(b.EndCircle.Radius, diameter / 2, Assert.Epsilon,
                                                     angle.ToString());
                                 }
                             };
            }
        }
    }
}