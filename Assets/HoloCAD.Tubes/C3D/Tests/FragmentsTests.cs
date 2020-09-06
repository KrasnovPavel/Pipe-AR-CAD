// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using HoloTest;
using MathExtensions;
using UnityC3D;
using UnityEngine;
using Random = UnityEngine.Random;

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
                sys.SetJournal();
                var f = new DirectFragment(sys, 0.34f, 0.43f, null);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(f.Diameter, 0.34f);
                Assert.AreEqual(f.Length, 0.43f);
                Assert.IsNull(f.Parent);
                Assert.AreEqual((f.StartCircle.Origin - f.EndCircle.Origin).magnitude, 0.43f);

                var f1 = new DirectFragment(sys, 0.34f, 2.4f, f);

                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(f.Diameter, 0.34f);
                Assert.AreEqual(f1.StartCircle.Origin, f.EndCircle.Origin);
                Assert.AreEqual((f1.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 2.4f);
                Assert.AreEqual((f.EndCircle.Origin - f1.EndCircle.Origin).magnitude, 2.4f);

                Assert.AreEqual(f.EndCircle.Normal, f1.StartCircle.Normal);

                Assert.AreEqual((f.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 2.4f + 0.43f);
                Assert.AreEqual(f1.Parent, f);

                // ReSharper disable once ObjectCreationAsStatement
                // ReSharper disable once AccessToDisposedClosure
                Assert.ThrowsException<FragmentsNotConnectable>(() => new DirectFragment(sys, 2, 2, f));
            }
        }

        [HoloTestCase]
        public static void DirectFragmentChanging()
        {
            using (var sys = new GCMSystem())
            {
                sys.SetJournal();
                var f = new DirectFragment(sys, 34, 0.43f, null);
                f.Diameter = 0.53f;
                Assert.AreEqual(f.Diameter, 0.53f);
                f.Length = 2.31f;
                Assert.AreEqual(f.Length, 2.31f);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);

                var f1 = new DirectFragment(sys, 0.53f, 2.4f, f);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(f1.Length, 2.4f);
                Assert.AreEqual(f1.StartCircle.Origin, f.EndCircle.Origin);
                Assert.AreEqual((f1.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 2.4f);
                Assert.AreEqual((f1.EndCircle.Origin - f.EndCircle.Origin).magnitude, 2.4f);
                Assert.AreEqual((f.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 2.31f + 2.4f);

                Assert.ThrowsException<FragmentsNotConnectable>(() => f1.Diameter = 3.2f);

                f1.Length = 0.12f;
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(f1.Length, 0.12f);
                Assert.AreEqual(f1.StartCircle.Origin, f.EndCircle.Origin);
                Assert.AreEqual((f1.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 0.12f);
                Assert.AreEqual((f1.EndCircle.Origin - f.EndCircle.Origin).magnitude, 0.12f);
                Assert.AreEqual((f.StartCircle.Origin - f1.EndCircle.Origin).magnitude, 2.31f + 0.12f);

                f.Length = 0.36f;
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(f.Length, 0.36f);
                Assert.AreEqual(f1.StartCircle.Origin, f.EndCircle.Origin);
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

                var f = new DirectFragment(sys, 34, 0.43f, s);
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
            return new Vector3(
                               bendRadius * (1 - Mathf.Cos(Mathf.Deg2Rad * bendAngle)) *
                               Mathf.Cos(Mathf.Deg2Rad * rotationAngle),
                               -bendRadius * (1 - Mathf.Cos(Mathf.Deg2Rad * bendAngle)) *
                               Mathf.Sin(Mathf.Deg2Rad * rotationAngle),
                               bendRadius * Mathf.Sin(Mathf.Deg2Rad * bendAngle));
        }

        private struct AngleData
        {
            public float bend;
            public float rotation;

            public override string ToString()
            {
                return $"[bend {bend:f1}, rotation {rotation:f1}]";
            }
        }

        private static AngleData[] angles =
        {
            new AngleData {bend = 90f, rotation  = 0f},
            new AngleData {bend = 30f, rotation  = 0f},
            new AngleData {bend = 45f, rotation  = 0f},
            new AngleData {bend = 180f, rotation = 0f},
            new AngleData {bend = 130f, rotation = 0f},
            new AngleData {bend = 90f, rotation  = 45f},
            new AngleData {bend = 30f, rotation  = 45f},
            new AngleData {bend = 45f, rotation  = 45f},
            new AngleData {bend = 180f, rotation = 45f},
            new AngleData {bend = 130f, rotation = 45f},
            new AngleData {bend = 90f, rotation  = 90f},
            new AngleData {bend = 30f, rotation  = 90f},
            new AngleData {bend = 45f, rotation  = 90f},
            new AngleData {bend = 180f, rotation = 90f},
            new AngleData {bend = 130f, rotation = 90f},
            new AngleData {bend = 90f, rotation  = 60f},
            new AngleData {bend = 30f, rotation  = 60f},
            new AngleData {bend = 45f, rotation  = 60f},
            new AngleData {bend = 180f, rotation = 60f},
            new AngleData {bend = 130f, rotation = 60f},
            new AngleData {bend = 90f, rotation  = 120f},
            new AngleData {bend = 30f, rotation  = 120f},
            new AngleData {bend = 45f, rotation  = 120f},
            new AngleData {bend = 180f, rotation = 120f},
            new AngleData {bend = 130f, rotation = 120f},
            new AngleData {bend = 90f, rotation  = 180f},
            new AngleData {bend = 30f, rotation  = 180f},
            new AngleData {bend = 45f, rotation  = 180f},
            new AngleData {bend = 180f, rotation = 180f},
            new AngleData {bend = 130f, rotation = 180f},
            new AngleData {bend = 90f, rotation  = 200f},
            new AngleData {bend = 30f, rotation  = 200f},
            new AngleData {bend = 45f, rotation  = 200f},
            new AngleData {bend = 180f, rotation = 200f},
            new AngleData {bend = 130f, rotation = 200f},
            new AngleData {bend = 90f, rotation  = 270f},
            new AngleData {bend = 30f, rotation  = 270f},
            new AngleData {bend = 45f, rotation  = 270f},
            new AngleData {bend = 180f, rotation = 270f},
            new AngleData {bend = 130f, rotation = 270f},
            new AngleData {bend = 90f, rotation  = 300f},
            new AngleData {bend = 30f, rotation  = 300f},
            new AngleData {bend = 45f, rotation  = 300f},
            new AngleData {bend = 180f, rotation = 300f},
            new AngleData {bend = 130f, rotation = 300f},
            new AngleData {bend = 90f, rotation  = 360f},
            new AngleData {bend = 30f, rotation  = 360f},
            new AngleData {bend = 45f, rotation  = 360f},
            new AngleData {bend = 180f, rotation = 360f},
            new AngleData {bend = 130f, rotation = 360f},
            new AngleData {bend = 90f, rotation  = 450f},
            new AngleData {bend = 30f, rotation  = 450f},
            new AngleData {bend = 45f, rotation  = 450f},
            new AngleData {bend = 180f, rotation = 450f},
            new AngleData {bend = 130f, rotation = 450f},
        };

        [HoloTestGenerator]
        public static IEnumerable<Action> BendedFragmentCreation()
        {
            var diameter   = 0.01f;
            var bendRadius = 0.15f;

            foreach (var angle in angles)
            {
                yield return delegate
                {
                    using (var sys = new GCMSystem())
                    {
                        sys.SetJournal($"Journals\\BendedFragmentCreation{angle.ToString()}");
                        var s = new StartFragment(sys, diameter, sys.GroundLCS.Placement);
                        var b = new BendedFragment(sys, bendRadius, angle.bend, angle.rotation, diameter, s);

                        Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok, angle.ToString());

                        Assert.AreEqual(b.BendRadius, bendRadius, Assert.Epsilon, "// Bend radius");
                        Assert.AreEqual(Geometry.NormalizeAngle(b.Rotation), Geometry.NormalizeAngle(angle.rotation),
                                        Assert.Epsilon, "// Rotation");
                        Assert.AreEqual(b.BendAngle, angle.bend, Assert.Epsilon, "// Bend angle");

                        Assert.AreEqual(b.EndCircle.Origin - b.StartCircle.Origin,
                                        GetBendedEndPoint(bendRadius, angle.bend, angle.rotation),
                                        Assert.Epsilon,
                                        angle + " // EndCircle pos");

                        Assert.AreEqual(Geometry.DistancePointLine(b.EndCircle.Origin, b.RightAxis.Origin,
                                                                   b.RightAxis.Direction),
                                        0f,
                                        angle + " // RightLine origin");
                        Assert.AreEqual(b.RightAxis.Direction,
                                        -(b.EndCircle.Origin -
                                          Quaternion.AngleAxis(-angle.rotation, b.StartCircle.Normal) *
                                          Vector3.right * bendRadius).normalized,
                                        Assert.Epsilon,
                                        angle + " // Right Line direction");

                        Assert.AreEqual(b.EndCircle.Radius, diameter / 2, Assert.Epsilon,
                                        angle.ToString());
                    }
                };
            }
        }

        [HoloTestCase]
        public static void SimpleTube()
        {
            var diameter      = 0.1f;
            var bendRadius    = 0.2f;
            var bendAngle     = 60;
            var rotationAngle = 30;

            using (var sys = new GCMSystem())
            {
                sys.SetJournal();
                var s  = new StartFragment(sys, diameter, sys.GroundLCS.Placement);
                var d1 = new DirectFragment(sys, diameter, 0.5f, s);
                var b  = new BendedFragment(sys, bendRadius, bendAngle, rotationAngle, diameter, d1);
                var d2 = new DirectFragment(sys, diameter, 1f, b);

                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);

                Assert.AreEqual(d1.EndCircle.Origin, Vector3.forward * 0.5f, "d1");
                Assert.AreEqual(b.EndCircle.Origin,
                                GetBendedEndPoint(bendRadius, bendAngle, rotationAngle) + d1.EndCircle.Origin, "b");
                Assert.AreEqual(d2.EndCircle.Origin, b.EndCircle.Origin + b.EndCircle.Normal * 1f,
                                Assert.Epsilon, "d2");
            }
        }

        private class FragmentData
        {
            public float diameter;
            public float bendRadius;
            public float length;
            public float bend;
            public float rotation;
        }

        private static List<FragmentData> GetTestData(TubeLoader.TubeData tubeData)
        {
            int numberOfFragments = angles.Length + angles.Length / 2 + 1;
            var result            = new List<FragmentData>(numberOfFragments);

            for (int i = 0, j = 0; i < numberOfFragments; i++)
            {
                result.Add(new FragmentData());
                result[i].diameter   = tubeData.diameter;
                result[i].bendRadius = tubeData.first_radius;
                result[i].length     = tubeData.diameter + tubeData.first_radius;
                if (i % 3 != 0)
                {
                    result[i].bend     = angles[j].bend;
                    result[i].rotation = angles[j].rotation;
                    j++;
                }
            }

            return result;
        }

        [HoloTestCase]
        public static void ComplexTube()
        {
            var tubeData = TubeLoader.GetAvailableTubes(TubeLoader.GetStandardNames()[0])[10];
            var data     = GetTestData(tubeData);
            using (var sys = new GCMSystem())
            {
                sys.SetJournal();
                var          s      = new StartFragment(sys, data[0].diameter, sys.GroundLCS.Placement);
                TubeFragment parent = s;
                for (int i = 0; i < data.Count; i++)
                {
                    TubeFragment current;
                    if (i % 3 == 0)
                    {
                        current = new DirectFragment(sys, data[i].diameter, data[i].length, parent);
                    }
                    else
                    {
                        current = new BendedFragment(sys, data[i].bendRadius, data[i].bend, data[i].rotation,
                                                     data[i].diameter, parent);
                    }

                    Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok, $"// number of fragments: {i}");
                    parent = current;
                }
            }
        }
    }
}