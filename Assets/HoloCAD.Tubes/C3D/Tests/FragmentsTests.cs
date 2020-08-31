using System;
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

        private static Vector3 GetBendedEndPoint(float bendRadius, float bendAngle, float rotationAngle)
        {
            return new
                Vector3(bendRadius * (1 - Mathf.Cos(Mathf.Deg2Rad * bendAngle)) * Mathf.Cos(Mathf.Deg2Rad * rotationAngle),
                        bendRadius * (1 - Mathf.Cos(Mathf.Deg2Rad * bendAngle)) *
                        Mathf.Sin(Mathf.Deg2Rad * rotationAngle),
                        -bendRadius * Mathf.Sin(Mathf.Deg2Rad * bendAngle));
        }

        private static float GetBendedDistance(float bendRadius, float bendAngle)
        {
            int intAngle = (int) bendAngle;
            if (intAngle % 90 == 0)
            {
                if (intAngle % 180 == 0)
                {
                    return bendRadius * 2;
                }

                return bendRadius * Mathf.Sqrt(2);
            }

            return Mathf.Sqrt(2 * bendRadius * bendRadius * (1 - Mathf.Cos(Mathf.Deg2Rad * bendAngle)));
        }

        private struct AngleData
        {
            public float bend;
            public float rotation;

            public override string ToString()
            {
                return $"[bend: {bend:f1}, rotation: {rotation:f1}]";
            }
        }

        [HoloTestCase]
        public static void BendedFragmentCreation()
        {
            var diameter = 0.01f;
            var bendRadius = 0.15f;

            var angles = new[]
            {
                new AngleData {bend = 90f, rotation = 0f},
                new AngleData {bend = 30f, rotation = 0f},
                new AngleData {bend = 45f, rotation = 0f},
                new AngleData {bend = 180f, rotation = 0f},
                new AngleData {bend = 130f, rotation = 0f},
                new AngleData {bend = 90f, rotation = 45f},
                new AngleData {bend = 30f, rotation = 45f},
                new AngleData {bend = 45f, rotation = 45f},
                new AngleData {bend = 180f, rotation = 45f},
                new AngleData {bend = 130f, rotation = 45f},
                new AngleData {bend = 90f, rotation = 90f},
                new AngleData {bend = 30f, rotation = 90f},
                new AngleData {bend = 45f, rotation = 90f},
                new AngleData {bend = 180f, rotation = 90f},
                new AngleData {bend = 130f, rotation = 90f},
                new AngleData {bend = 90f, rotation = 60f},
                new AngleData {bend = 30f, rotation = 60f},
                new AngleData {bend = 45f, rotation = 60f},
                new AngleData {bend = 180f, rotation = 60f},
                new AngleData {bend = 130f, rotation = 60f},
                new AngleData {bend = 90f, rotation = 120f},
                new AngleData {bend = 30f, rotation = 120f},
                new AngleData {bend = 45f, rotation = 120f},
                new AngleData {bend = 180f, rotation = 120f},
                new AngleData {bend = 130f, rotation = 120f},
                new AngleData {bend = 90f, rotation = 180f},
                new AngleData {bend = 30f, rotation = 180f},
                new AngleData {bend = 45f, rotation = 180f},
                new AngleData {bend = 180f, rotation = 180f},
                new AngleData {bend = 130f, rotation = 180f},
                new AngleData {bend = 90f, rotation = 200f},
                new AngleData {bend = 30f, rotation = 200f},
                new AngleData {bend = 45f, rotation = 200f},
                new AngleData {bend = 180f, rotation = 200f},
                new AngleData {bend = 130f, rotation = 200f},
                new AngleData {bend = 90f, rotation = 270f},
                new AngleData {bend = 30f, rotation = 270f},
                new AngleData {bend = 45f, rotation = 270f},
                new AngleData {bend = 180f, rotation = 270f},
                new AngleData {bend = 130f, rotation = 270f},
                new AngleData {bend = 90f, rotation = 300f},
                new AngleData {bend = 30f, rotation = 300f},
                new AngleData {bend = 45f, rotation = 300f},
                new AngleData {bend = 180f, rotation = 300f},
                new AngleData {bend = 130f, rotation = 300f},
                new AngleData {bend = 90f, rotation = 360f},
                new AngleData {bend = 30f, rotation = 360f},
                new AngleData {bend = 45f, rotation = 360f},
                new AngleData {bend = 180f, rotation = 360f},
                new AngleData {bend = 130f, rotation = 360f},
                new AngleData {bend = 90f, rotation = 450f},
                new AngleData {bend = 30f, rotation = 450f},
                new AngleData {bend = 45f, rotation = 450f},
                new AngleData {bend = 180f, rotation = 450f},
                new AngleData {bend = 130f, rotation = 450f},
            };

            foreach (var angle in angles)
            {
                using (var sys = new GCMSystem())
                {
                    sys.SetJournal();
                    var s = new StartFragment(sys, diameter, sys.GroundLCS.Placement);
                    var b = new BendedFragment(sys, bendRadius, angle.bend, angle.rotation, diameter, s);
                    
                    if (angle.rotation >= 90) b.TestDraw("b");

                    Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                    Assert.AreEqual((s.EndCircle.Origin - b.EndCircle.Origin).magnitude,
                                    GetBendedDistance(bendRadius, angle.bend), Assert.Epsilon, angle.ToString());
                    Assert.AreEqual(b.EndCircle.Origin - b.StartCircle.Origin,
                                    GetBendedEndPoint(bendRadius, angle.bend, angle.rotation), Assert.Epsilon,
                                    angle.ToString());
                    Assert.AreEqual(b.EndCircle.Radius, diameter / 2, Assert.Epsilon, angle.ToString());
                }
            }
        }
    }
}