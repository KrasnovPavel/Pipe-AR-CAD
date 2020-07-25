// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloTest;
using UnityEngine;

namespace UnityC3D.Tests
{
    [HoloTestClass]
    public static class GCMSystemTests
    {
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
                Y = 2,
                Z = 3,
            };

            var b = MbVector3D.FromUnity(new Vector3(1, 2, 3));
            Assert.AreEqual(a, b);

            a = new MbVector3D
            {
                X = 2.5,
                Y = 23432.4,
                Z = -23123,
            };
            Assert.AreEqual(a.ToUnity(), new Vector3(2.5f, 23432.4f, -23123f));
        }

        [HoloTestCase]
        public static void Placement()
        {
            var g = new GameObject();
            Transform t = GameObject.Instantiate(g).transform;
            t.position = Vector3.one;
            t.rotation = Quaternion.Euler(Vector3.left);

            var p = MbPlacement3D.FromUnity(t);
            Assert.AreEqual(p.Origin, t.position);
            Assert.AreEqual(p.AxisZ,  t.forward);
            Assert.AreEqual(p.AxisX,  t.right);
            Assert.AreEqual(p.AxisY,  t.up);
            
            t.position = Vector3.zero;
            t.rotation = Quaternion.Euler(Vector3.up);
            p.Apply(t);
            Assert.AreEqual(p.Origin, t.position);
            Assert.AreEqual(p.AxisZ,  t.forward);
            Assert.AreEqual(p.AxisX,  t.right);
            Assert.AreEqual(p.AxisY,  t.up);
            
            GameObject.Destroy(g);
        }

        [HoloTestCase]
        public static void Point()
        {
            using (var sys = new GCMSystem())
            {
                var origin = new Vector3(1, 2, 3);
                var point = new GCMPoint(sys, origin);
                Assert.AreEqual(point.Placement.Origin, origin);
                
                point.Origin = Vector3.left;
                Assert.AreEqual(point.Placement.Origin, Vector3.left);
            }
        }

        [HoloTestCase]
        public static void Line()
        {
            using (var sys = new GCMSystem())
            {
                var origin = new Vector3(1, 2, 3);
                var direction = Vector3.up;
                var line = new GCMLine(sys, origin, direction);
                Assert.AreEqual(line.Placement.Origin, origin);
                Assert.AreEqual(line.Placement.AxisZ, direction);
                
                line.Direction = Vector3.back;
                Assert.AreEqual(line.Placement.Origin, origin);
                Assert.AreEqual(line.Placement.AxisZ, Vector3.back);
            }
        }

        [HoloTestCase]
        public static void AddPlane()
        {
            using (var sys = new GCMSystem())
            {
                var origin = new Vector3(1, 2, 3);
                var normal = Vector3.up;
                var p = new GCMPlane(sys, origin, normal);
                Assert.AreEqual(p.Placement.Origin, origin);
                Assert.AreEqual(p.Placement.AxisZ, normal);
            }
        }

        [HoloTestCase]
        public static void AddCircle()
        {
            using (var sys = new GCMSystem())
            {
                var origin = new Vector3(1, 2, 3);
                var normal = Vector3.up;
                float radius = 5.3f;
                var circle = new GCMCircle(sys, origin, normal, radius);
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(circle.Placement.Origin, origin);
                Assert.AreEqual(circle.Placement.AxisZ, normal);
                Assert.AreEqual(circle.Radius, radius);
                
                float newRadius = 7.56f;
                circle.Radius = newRadius;
                Assert.AreEqual(sys.Evaluate(), GCMResult.GCM_RESULT_Ok);
                Assert.AreEqual(circle.Radius, newRadius);
            }
        }

        [HoloTestCase]
        public static void AddLCS()
        {
            using (var sys = new GCMSystem())
            {
                var g = new GameObject();
                Transform t = GameObject.Instantiate(g).transform;
                t.position = Vector3.one;
                t.rotation = Quaternion.Euler(Vector3.left);

                var lcs = new GCM_LCS(sys, t);
                Assert.AreEqual(lcs.Placement.Origin, t.position);
                Assert.AreEqual(lcs.Placement.AxisZ,  t.forward);
                Assert.AreEqual(lcs.Placement.AxisY,  t.up);
                Assert.AreEqual(lcs.Placement.AxisX,  t.right);
                
                GameObject.Destroy(g);
            }
        }

        [HoloTestCase]
        public static void GroundLCS()
        {
            using (var sys = new GCMSystem())
            {
                var lcs = sys.GroundLCS;
                Assert.AreEqual(lcs.Placement.Origin, Vector3.zero);
                Assert.AreEqual(lcs.Placement.AxisZ,  -Vector3.forward);
                Assert.AreEqual(lcs.Placement.AxisY,  Vector3.up);
                Assert.AreEqual(lcs.Placement.AxisX,  Vector3.right);
            }
        }

        [HoloTestCase]
        public static void PropertyChanged()
        {
            using (var sys = new GCMSystem())
            {
                var circle = new GCMCircle(sys, Vector3.back, Vector3.forward, 34f);
                int calls = 0;
                circle.PropertyChanged += delegate { calls++; };
                sys.Evaluate();
                Assert.AreEqual(calls, 3);
            }
        }
    }
}