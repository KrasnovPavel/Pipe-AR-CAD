// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using MathExtensions;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.C3D
{
    public class BendedFragment : TubeFragment
    {
        public BendedFragment(GCMSystem sys, float bendRadius, float bendAngle, float rotation, float diameter,
            TubeFragment parent)
            : base(sys, diameter, parent)
        {
            MainLCS = new GCM_LCS(sys, sys.GroundLCS.Placement);
            
            StartCircle = new GCMCircle(sys, Vector3.zero, Vector3.forward, diameter / 2, MainLCS);
            StartCircle.Freeze();
            
            StartPlane = new GCMPlane(sys, Vector3.zero, Vector3.forward, MainLCS);
            StartPlane.Freeze();

            _axis = new GCMLine(sys, Vector3.zero, Vector3.forward, MainLCS);
            _axis.Freeze();
            
            _startRightAxis = new GCMLine(sys, Vector3.zero, Vector3.right, MainLCS); //-V3127
            _startRightAxis.Freeze();
            
            // Вращаем ось погиба
            _zeroBendAxis = new GCMLine(sys, Vector3.right * bendRadius, Vector3.up, MainLCS);
            _zeroBendAxis.Freeze();
            _bendAxis = new GCMLine(sys, Vector3.right * bendRadius, Vector3.up);
            _rotationPattern = sys.CreateAngularPattern(_zeroBendAxis, _axis, GCMAlignment.Cooriented);
            _rotationPattern.AddObject(_bendAxis, Mathf.Deg2Rad * rotation, GCMAlignment.Rotated, GCMScale.GCM_RIGID);

            // Гнём трубу
            _bendPattern = sys.CreateAngularPattern(StartCircle, _bendAxis, GCMAlignment.Cooriented);
            _bendPattern.AddObject(EndCircle, -Mathf.Deg2Rad * bendAngle, GCMAlignment.Rotated, GCMScale.GCM_RIGID);
            sys.MakeCoincident(EndCircle, EndPlane);

            // Устанавливаем положение оси X на конце отрезка
            _pivotAxis = new GCMLine(sys, Vector3.zero, Vector3.right);
            _rightAxisRotationPattern = sys.CreateAngularPattern(_startRightAxis, _axis, GCMAlignment.Cooriented);
            _rightAxisRotationPattern.AddObject(_pivotAxis, Mathf.Deg2Rad * rotation, GCMAlignment.Rotated, GCMScale.GCM_RIGID);
            _rightAxisBendPattern = sys.CreateAngularPattern(_pivotAxis, _bendAxis, GCMAlignment.Cooriented);
            _rightAxisBendPattern.AddObject(RightAxis, -Mathf.Deg2Rad * bendAngle, GCMAlignment.Rotated, GCMScale.GCM_RIGID);
            sys.MakeCoincident(RightAxis, EndPlane);

            if (parent != null)
            {
                sys.MakeCoincident(parent.EndPlane, StartPlane, GCMAlignment.Cooriented);
                sys.MakeCoincident(parent.EndCircle, StartCircle, GCMAlignment.Cooriented);
                sys.MakeCoincident(parent.RightAxis, _startRightAxis, GCMAlignment.Cooriented);
            }

            sys.Evaluate();
        }

        public float BendRadius
        {
            get => Geometry.DistancePointLine(StartCircle.Origin, _bendAxis.Origin, _bendAxis.Direction);
            // set
            // {
            //     if (Math.Abs(BendRadius - value) < float.Epsilon) return;
            //     // Sys.SetDistance(_pivot, _endPoint, value);
            //     // _pivot.Origin = MainLCS.Origin + AxisX * value;
            //     Sys.Evaluate();
            //     OnPropertyChanged();
            // }
        }

        public float Rotation
        {
            get
            {
                var x   = Vector3.Project(EndCircle.Origin, Placement.AxisX);
                var y   = Vector3.Project(EndCircle.Origin, Placement.AxisY);
                var vec = new Vector2(x.magnitude, y.magnitude);

                vec.x *= Vector3.Angle(x, Placement.AxisX) > 90 ? -1 : 1;
                vec.y *= Vector3.Angle(y, Placement.AxisY) > 90 ? -1 : 1; 
                var res = Vector2.Angle(vec, Vector2.right);
                res *= vec.y > 0 ? -1 : 1;
                return res;
            }
        //     // set
        //     // {
        //     //     if (Math.Abs(_rotation - value) < float.Epsilon) return;
        //     //
        //     //     _rotation = value;
        //     //     // _pivot.Origin = Quaternion.Euler(0, 0, _rotation) * Vector3.right * BendRadius;
        //     //     Sys.Evaluate();
        //     //     OnPropertyChanged();
        //     // }
        }

        public float BendAngle;
        // {
        //     get => Vector3.Angle(_startPoint.Origin - _bendCircle.Origin, EndCircle.Origin - _bendCircle.Origin);
        //     // set
        //     // {
        //     //     if (Math.Abs(BendAngle - value) < float.Epsilon) return;
        //     //
        //     //     // Sys.SetAngle(_startPlane, EndPlane, Mathf.Deg2Rad * value);
        //     //     Sys.Evaluate();
        //     //     OnPropertyChanged();
        //     // }
        // }

        /// <inheritdoc />
        public override float Diameter
        {
            get => EndCircle.Radius * 2;
            set
            {
                if (Math.Abs(Diameter - value) < float.Epsilon) return;

                if (Parent != null && Math.Abs(Parent.Diameter - value) > float.Epsilon)
                {
                    throw new FragmentsNotConnectable();
                }

                EndCircle.Radius = value / 2;
                StartCircle.Radius = value / 2;
                Sys.Evaluate();
                OnPropertyChanged();
            }
        }

        public override void Dispose()
        {
            _rotationPattern?.Dispose();
            _bendPattern?.Dispose();
            _rightAxisRotationPattern?.Dispose();
            _rightAxisBendPattern?.Dispose();
            
            base.Dispose();
            
            _axis?.Dispose();
            _startRightAxis?.Dispose();
            _zeroBendAxis?.Dispose();
            _bendAxis?.Dispose();
            _pivotAxis?.Dispose();
        }

        public override void TestDraw(string name)
        {
            base.TestDraw(name);
            _axis.TestDraw($"{name}-_axis");
            _startRightAxis.TestDraw($"{name}-_startRightAxis");
            _bendAxis.TestDraw($"{name}-_bendAxis");
        }

        #region Private definitions

        // private readonly GCMPoint   _pivot;
        private readonly GCMLine    _axis;
        private readonly GCMLine    _startRightAxis;
        private readonly GCMLine    _zeroBendAxis;
        private readonly GCMLine    _bendAxis;
        private readonly GCMLine    _pivotAxis;
        private readonly GCMPattern _bendPattern;
        private readonly GCMPattern _rotationPattern;
        private readonly GCMPattern _rightAxisRotationPattern;
        private readonly GCMPattern _rightAxisBendPattern;

        #endregion
    }
}