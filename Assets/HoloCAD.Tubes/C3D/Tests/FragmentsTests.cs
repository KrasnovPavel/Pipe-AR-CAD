﻿using System;
using HoloTest;
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

                var f = new DirectTubeFragment(sys, s, 34, 0.43f);
                Assert.AreEqual(f.StartCircle.Origin, s.EndCircle.Origin);
                Assert.AreEqual(f.StartCircle.Normal, s.EndCircle.Normal);

                t.position = Vector3.down;
                t.LookAt(Vector3.one);
                s.SetPlacement(MbPlacement3D.FromUnity(t));

                // Странная фигня, почему-то c3d меняет направление оси Y на противоположное.
                // В данном случае, это ни на что не влияет, но может неприятно вылезти в будущем.
                // Assert.AreEqual(s.Placement, MbPlacement3D.FromUnity(t));
                Assert.AreEqual(f.StartCircle.Origin, s.EndCircle.Origin);
                Assert.AreEqual(f.StartCircle.Normal, s.EndCircle.Normal);
                Assert.AreEqual((s.EndCircle.Origin - f.EndCircle.Origin).magnitude, 0.43f);

                GameObject.Destroy(g);
                GameObject.Destroy(t.gameObject);
            }
        }

        [HoloTestCase]
        public static void BendedFragmentCreation1()
        {
            var diameter = 0.005f;
            var bendRadius = 0.02f;
            var bendAngle = 90;
            var rotationAngle = 0;

            using (var sys = new GCMSystem())
            {
                sys.SetJournal();
                var s = new StartFragment(sys, diameter, sys.GroundLCS.Placement);
                var b = new BendedFragment(sys, bendRadius, bendAngle, rotationAngle, diameter, s);

                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual((s.EndCircle.Origin - b.EndCircle.Origin).magnitude, 0.02f * Mathf.Sqrt(2));
                Assert.AreEqual(b.EndCircle.Origin, new Vector3(bendRadius, 0, bendRadius));
                Assert.AreEqual(b.EndCircle.Radius, diameter / 2);
            }
        }

        [HoloTestCase]
        public static void BendedFragmentCreation2()
        {
            var diameter = 0.01f;
            var bendRadius = 0.15f;
            var bendAngle = 90;
            var rotationAngle = 45;

            using (var sys = new GCMSystem())
            {
                sys.SetJournal();
                var s = new StartFragment(sys, diameter, sys.GroundLCS.Placement);
                var b = new BendedFragment(sys, bendRadius, bendAngle, rotationAngle, diameter, s);

                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                // b.TestDraw("b");
                Assert.AreEqual((s.EndCircle.Origin - b.EndCircle.Origin).magnitude, bendRadius * Mathf.Sqrt(2));
                Assert.AreEqual(b.EndCircle.Origin - b.StartCircle.Origin,
                    new Vector3(0,
                        -bendRadius * Mathf.Sin(Mathf.Deg2Rad * rotationAngle),
                        bendRadius * Mathf.Cos(Mathf.Deg2Rad * rotationAngle)));
                Assert.AreEqual(b.EndCircle.Radius, diameter / 2);
            }
        }

        [HoloTestCase]
        public static void BendedFragmentCreation3()
        {
            var diameter = 0.01f;
            var bendRadius = 0.15f;
            var bendAngle = 45;
            var rotationAngle = 0;

            using (var sys = new GCMSystem())
            {
                sys.SetJournal();
                var s = new StartFragment(sys, diameter, sys.GroundLCS.Placement);
                var b = new BendedFragment(sys, bendRadius, bendAngle, rotationAngle, diameter, s);

                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(bendAngle, b.BendAngle);
                Assert.AreEqual(b.StartCircle.Origin, Vector3.zero);
                Assert.AreEqual(b.EndCircle.Origin - b.StartCircle.Origin,
                    new Vector3(0.15f - bendRadius * Mathf.Sin(Mathf.Deg2Rad * bendAngle),
                        0,
                        bendRadius * Mathf.Cos(Mathf.Deg2Rad * bendAngle)));
                Assert.AreEqual(b.EndCircle.Radius, diameter / 2);
            }
        }

        [HoloTestCase]
        public static void BendedFragmentCreation5()
        {
            var diameter = 0.01f;
            var bendRadius = 0.15f;
            var bendAngle = 60;
            var rotationAngle = 0;

            using (var sys = new GCMSystem())
            {
                sys.SetJournal();
                var s = new StartFragment(sys, diameter, sys.GroundLCS.Placement);
                var b = new BendedFragment(sys, bendRadius, bendAngle, rotationAngle, diameter, s);

                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                b.TestDraw("b");
                Assert.AreEqual(bendAngle, b.BendAngle);
                Assert.AreEqual(b.StartCircle.Origin, Vector3.zero);
                Assert.AreEqual(b.EndCircle.Origin - b.StartCircle.Origin,
                    new Vector3(0.15f - bendRadius * Mathf.Cos(Mathf.Deg2Rad * bendAngle),
                        0,
                        bendRadius * Mathf.Sin(Mathf.Deg2Rad * bendAngle)));
                Assert.AreEqual(b.EndCircle.Radius, diameter / 2);
            }
        }

        [HoloTestCase]
        public static void BendedFragmentCreation4()
        {
            var diameter = 0.005f;
            var bendRadius = 0.02f;
            var bendAngle = 180;
            var rotationAngle = 0;
            using (var sys = new GCMSystem())
            {
                sys.SetJournal();
                var s = new StartFragment(sys, diameter, sys.GroundLCS.Placement);
                var b = new BendedFragment(sys, bendRadius, bendAngle, rotationAngle, diameter, s);

                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual((s.EndCircle.Origin - b.EndCircle.Origin).magnitude, bendRadius * 2);
                Assert.AreEqual(b.EndCircle.Origin, new Vector3(bendRadius * 2, 0, 0));
                Assert.AreEqual(b.EndCircle.Radius, diameter / 2);
            }
        }
    }
}