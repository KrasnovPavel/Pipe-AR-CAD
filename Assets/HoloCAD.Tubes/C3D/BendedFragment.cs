// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
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
            
            StartCircle = new GCMCircle(sys, Vector3.zero, -Vector3.forward, diameter / 2);
            _startPoint = new GCMPoint(sys, Vector3.zero);
            _startPoint.Freeze();
            sys.MakeConcentric(StartCircle, _startPoint);
            
            _startPlane = new GCMPlane(sys, Vector3.zero, -Vector3.forward, MainLCS);
            sys.MakeCoincident(StartCircle, _startPlane, GCMAlignment.Cooriented);
            _startPlane.Freeze();

            _axis = new GCMLine(sys, Vector3.zero, Vector3.zero, MainLCS);
            _axis.Freeze();
            
            _startRightAxis = new GCMLine(sys, Vector3.zero, Vector3.right, MainLCS);
            _startRightAxis.Freeze();
            
            // Вращаем ось погиба
            var rotLine = new GCMLine(sys, Vector3.right * bendRadius, Vector3.up, MainLCS);
            rotLine.Freeze();
            var bendAxis = new GCMLine(sys, Vector3.right * bendRadius, Vector3.up);
            _rotationPattern = sys.AddAngularPattern(rotLine, _axis, GCMAlignment.Cooriented);
            sys.AddObjectToPattern(_rotationPattern, bendAxis, Mathf.Deg2Rad * rotation, GCMAlignment.Rotated, GCMScale.GCM_RIGID);
            
            var right = new GCMLine(sys, Vector3.zero, Vector3.right);
            var rotationpattern1 = sys.AddAngularPattern(_startRightAxis, _axis, GCMAlignment.Cooriented);
            sys.AddObjectToPattern(rotationpattern1, right, Mathf.Deg2Rad * rotation, GCMAlignment.Rotated, GCMScale.GCM_RIGID);

            // Гнём трубу
            _bendPattern = sys.AddAngularPattern(StartCircle, bendAxis, GCMAlignment.Cooriented);
            sys.AddObjectToPattern(_bendPattern, EndCircle, Mathf.Deg2Rad * bendAngle, GCMAlignment.Rotated, GCMScale.GCM_RIGID);
            sys.MakeCoincident(EndCircle, EndPlane);

            var bendPatteern1 = sys.AddAngularPattern(right, bendAxis, GCMAlignment.Cooriented);
            sys.AddObjectToPattern(bendPatteern1, RightAxis, Mathf.Deg2Rad * bendAngle, GCMAlignment.Rotated, GCMScale.GCM_RIGID);
            sys.MakeCoincident(RightAxis, EndPlane);

            if (parent != null)
            {
                sys.MakeCoincident(parent.EndPlane, _startPlane, GCMAlignment.Cooriented);
                sys.MakeCoincident(parent.EndCircle, StartCircle, GCMAlignment.Cooriented);
                sys.MakeCoincident(parent.RightAxis, _startRightAxis, GCMAlignment.Cooriented);
            }

            sys.Evaluate();
        }

        // public override MbPlacement3D Placement
        // {
        //     get
        //     {
        //         var axisY = Vector3.Cross(AxisX, MainLCS.Placement.AxisZ);
        //         return MbPlacement3D.FromRightCS(MainLCS.Origin, AxisX, axisY, MainLCS.Placement.AxisZ);
        //     }
        // }

        // public float BendRadius
        // {
        //     // get => (_pivot.Origin - StartCircle.Origin).magnitude;
        //     get => (_bendCircle.Origin - StartCircle.Origin).magnitude;
        //     set
        //     {
        //         if (Math.Abs(BendRadius - value) < float.Epsilon) return;
        //
        //         // Sys.SetDistance(_pivot, _endPoint, value);
        //         // _pivot.Origin = MainLCS.Origin + AxisX * value;
        //         Sys.Evaluate();
        //         OnPropertyChanged();
        //     }
        // }

        public float Rotation
        {
            get => _rotation;
            set
            {
                if (Math.Abs(_rotation - value) < float.Epsilon) return;

                _rotation = value;
                // _pivot.Origin = Quaternion.Euler(0, 0, _rotation) * Vector3.right * BendRadius;
                Sys.Evaluate();
                OnPropertyChanged();
            }
        }

        // public float BendAngle
        // {
        //     get => Vector3.Angle(_startPoint.Origin - _bendCircle.Origin, EndCircle.Origin - _bendCircle.Origin);
        //     set
        //     {
        //         if (Math.Abs(BendAngle - value) < float.Epsilon) return;
        //
        //         // Sys.SetAngle(_startPlane, EndPlane, Mathf.Deg2Rad * value);
        //         Sys.Evaluate();
        //         OnPropertyChanged();
        //     }
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
            // _bendCircle?.Dispose();
            StartCircle?.Dispose();
            _startPlane?.Dispose();
            // _pivot?.Dispose();
            // _endPoint?.Dispose();
            // _rotationPlane?.Dispose();
            // _startPoint?.Dispose();
            base.Dispose();
        }

        public override void TestDraw(string name)
        {
            base.TestDraw(name);
        //     StartCircle.TestDraw($"{name}-StartCircle");
        //     _pivot.TestDraw($"{name}-_pivot");
        //     _bendCircle.TestDraw($"{name}-_bendCircle");
        //     _rightPoint.TestDraw($"{name}-_right");
        //     _startRightAxis.TestDraw($"{name}-_startRightAxis");
        //     _pivotAxis.TestDraw($"{name}-_pivotAxis");
        //     // _rotationCircle.TestDraw($"{name}-_rotationCircle");
        //     _axis.TestDraw($"{name}-_axis");
        }

        #region Private definitions

        // private Vector3 AxisX => (_pivot.Origin - StartCircle.Origin).normalized;
        // private Vector3 AxisX => (_bendCircle.Origin - StartCircle.Origin).normalized;

        private readonly GCMPlane _startPlane;
        public readonly GCMCircle StartCircle;

        // private readonly GCMPoint _pivot;
        // private readonly GCMLine _bendAxis;
        // private readonly GCMCircle _bendCircle;
        private readonly GCMLine _axis;
        // private readonly GCMPlane _bendPlane;
        private readonly GCMLine _startRightAxis;
        private readonly GCMLine _pivotAxis;
        // private readonly GCMPlane _zeroPlane;

        // private readonly GCMPoint _endPoint;
        // private readonly GCMPlane _rotationPlane;
        // private readonly GCMPoint _startPoint;
        private readonly GCMPattern _bendPattern;
        private readonly GCMPoint _startPoint;
        // private readonly GCMCircle _rotationCircle;
        private readonly GCMPoint _rightPoint;
        private readonly GCMPattern _rotationPattern;
        
        private float _rotation;

        #endregion
    }
}