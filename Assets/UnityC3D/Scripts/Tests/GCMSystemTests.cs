// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloTest;
using UnityEngine;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace UnityC3D.Tests
{
    [HoloTestClass]
    public static class GCMSystemTests
    {
        public const float Epsilon = 0.0001f;

        [HoloTestCase]
        public static void CreateRemove()
        {
            var a = new GCMSystem();
            a.Clear();
            a.Dispose();
        }

        [HoloTestCase]
        public static void Vector()
        {
            var a = new MbVector3D
            {
                X = 1,
                Y = 3,
                Z = 2,
            };

            // Проверка на преобразование левосторонней СК в правостороннюю
            var b = MbVector3D.FromUnity(new Vector3(1, 2, 3));
            Assert.AreEqual(a, b);

            a = new MbVector3D
            {
                X = 2.5,
                Y = 23432.4,
                Z = -23123,
            };
            Assert.AreEqual(a.ToUnity(), new Vector3(2.5f, -23123f, 23432.4f));
        }

        [HoloTestCase]
        public static void Point()
        {
            using (var sys = new GCMSystem())
            {
                var origin = new Vector3(1, 2, 3);
                var point  = new GCMPoint(sys, origin);
                Assert.AreEqual(point.Origin, origin);

                point.Origin = Vector3.left;
                Assert.AreEqual(point.Origin, Vector3.left);
            }
        }

        [HoloTestCase]
        public static void Line()
        {
            using (var sys = new GCMSystem())
            {
                var origin    = new Vector3(1, 2, 3);
                var direction = Vector3.up;
                var line      = new GCMLine(sys, origin, direction);
                Assert.AreEqual(line.Origin, origin);
                Assert.AreEqual(line.Direction, direction);

                line.Direction = Vector3.back;
                Assert.AreEqual(line.Origin, origin);
                Assert.AreEqual(line.Direction, Vector3.back);
            }
        }

        [HoloTestCase]
        public static void AddPlane()
        {
            using (var sys = new GCMSystem())
            {
                var origin = new Vector3(1, 2, 3);
                var normal = Vector3.up;
                var p      = new GCMPlane(sys, origin, normal);
                Assert.AreEqual(p.Origin, origin);
                Assert.AreEqual(p.Normal, normal);
            }
        }

        [HoloTestCase]
        public static void AddCircle()
        {
            using (var sys = new GCMSystem())
            {
                var   origin = new Vector3(1, 2, 3);
                var   normal = Vector3.up;
                float radius = 5.3f;
                var   circle = new GCMCircle(sys, origin, normal, radius);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(circle.Origin, origin);
                Assert.AreEqual(circle.Normal, normal);
                Assert.AreEqual(circle.Radius, radius);

                float newRadius = 7.56f;
                circle.Radius = newRadius;
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(circle.Radius, newRadius);

                circle.Origin = -Vector3.one;
                circle.Normal = Vector3.one;
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(circle.Origin, -Vector3.one);
                Assert.AreEqual(circle.Normal, Vector3.one.normalized);
            }
        }

        [HoloTestCase]
        public static void AddLCS()
        {
            using (var sys = new GCMSystem())
            {
                var       g = new GameObject();
                Transform t = GameObject.Instantiate(g).transform;
                t.position = Vector3.one;
                t.rotation = Quaternion.Euler(Vector3.left);

                var lcs = new GCM_LCS(sys, t);
                Assert.AreEqual(lcs.Placement.Origin, t.position);
                Assert.AreEqual(lcs.Placement.AxisZ, t.forward);
                Assert.AreEqual(lcs.Placement.AxisY, t.up);
                Assert.AreEqual(lcs.Placement.AxisX, t.right);

                GameObject.Destroy(g);
                GameObject.Destroy(t.gameObject);
            }
        }

        [HoloTestCase]
        public static void GroundLCS()
        {
            using (var sys = new GCMSystem())
            {
                var lcs = sys.GroundLCS;
                Assert.AreEqual(lcs.Placement.Origin, Vector3.zero);
                Assert.AreEqual(lcs.Placement.AxisZ, Vector3.forward);
                Assert.AreEqual(lcs.Placement.AxisY, Vector3.up);
                Assert.AreEqual(lcs.Placement.AxisX, Vector3.right);
            }
        }

        [HoloTestCase]
        public static void Parent()
        {
            using (var sys = new GCMSystem())
            {
                var       g = new GameObject();
                Transform t = GameObject.Instantiate(g).transform;
                t.position = Vector3.one;

                var lcs = new GCM_LCS(sys, t);
                Assert.IsNull(lcs.Parent);

                var p = new GCMPoint(sys, Vector3.down, lcs);
                Assert.AreEqual(lcs.Descriptor, sys.GetParent(p));
                Assert.AreEqual(p.Parent, lcs);
                Assert.AreEqual(p.Origin, lcs.Origin + Vector3.down);

                GameObject.Destroy(g);
                GameObject.Destroy(t.gameObject);
            }
        }

        [HoloTestCase]
        public static void FreezeFree()
        {
            using (var sys = new GCMSystem())
            {
                var       g = new GameObject();
                Transform t = GameObject.Instantiate(g).transform;
                t.position = Vector3.one;

                var lcs = new GCM_LCS(sys, t);
                Assert.IsNull(lcs.Parent);

                var p = new GCMPoint(sys, Vector3.down, lcs);
                Assert.AreEqual(p.Parent, lcs);
                Assert.AreEqual(p.Origin, lcs.Origin + Vector3.down);

                p.Freeze();
                lcs.Origin = Vector3.zero;
                Assert.AreEqual(p.Origin, lcs.Origin + Vector3.down);
                p.Free();

                GameObject.Destroy(g);
                GameObject.Destroy(t.gameObject);
            }
        }
    }
}