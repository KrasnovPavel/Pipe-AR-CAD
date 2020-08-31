// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using Microsoft.MixedReality.Toolkit;
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
            StartCircle = new GCMCircle(sys, Vector3.zero, -Vector3.forward, diameter / 2);
            _startPoint = new GCMPoint(sys, Vector3.zero);
            sys.MakeConcentric(StartCircle, _startPoint);
            
            _startPlane = new GCMPlane(sys, Vector3.zero, -Vector3.forward);
            sys.MakeCoincident(StartCircle, _startPlane, GCMAlignment.Cooriented);
            
            _axis = new GCMLine(sys, Vector3.zero, Vector3.zero);
            sys.MakeCoincident(_startPoint, _axis);
            sys.MakePerpendicular(_axis, _startPlane);

            // _rotationCircle = new GCMCircle(sys, Vector3.zero, -Vector3.forward, bendRadius);
            // sys.MakeConcentric(StartCircle, _rotationCircle, GCMAlignment.Cooriented);
            
            // sys.SetDistance(_startPoint, _rightPoint, bendRadius);
            // sys.MakeCoincident(_rightPoint, _startPlane);
            
            _startRightAxis = new GCMLine(sys, Vector3.zero, Vector3.right);
            _rightPoint = new GCMPoint(sys, Vector3.right * bendRadius);
            sys.MakeCoincident(_startRightAxis, _startPlane);
            sys.MakeCoincident(_rightPoint, _startRightAxis);
            sys.MakeCoincident(_startPoint, _startRightAxis);
            
            _pivot = new GCMPoint(sys, Vector3.right * bendRadius);
            sys.SetDistance(_startPoint, _pivot, bendRadius);
            sys.MakeCoincident(_pivot, _startPlane);
            // sys.MakeCoincident(_pivot, RightAxis);
            _pivotAxis = new GCMLine(sys, Vector3.zero, Vector3.right);
            sys.MakeCoincident(_pivot, _pivotAxis);
            sys.MakeCoincident(_startPoint, _pivotAxis);
            
            sys.SetAngle(_startRightAxis, _pivotAxis, _axis, rotation * Mathf.Deg2Rad);

            _bendCircle = new GCMCircle(sys, Vector3.right * bendRadius, Vector3.up, bendRadius);
            _bendPlane = new GCMPlane(sys, Vector3.right * bendRadius, Vector3.up);
            sys.MakeCoincident(_bendCircle, _bendPlane);
            sys.MakeCoincident(_axis, _bendPlane);
            sys.MakeConcentric(_bendCircle, _pivot);
            
            // _rotationPattern = sys.AddAngularPattern(_rightPoint, _rotationCircle, GCMAlignment.Cooriented);
            // sys.AddObjectToPattern(_rotationPattern, _bendCircle, Mathf.Deg2Rad * rotation, GCMAlignment.Rotated,
            //     GCMScale.GCM_RIGID);
            
            _bendPattern = sys.AddAngularPattern(StartCircle, _bendCircle, GCMAlignment.AlignWithAxialGeom);
            sys.AddObjectToPattern(_bendPattern, EndCircle, Mathf.Deg2Rad * bendAngle, GCMAlignment.Rotated, GCMScale.GCM_RIGID);
            

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

        public float BendRadius
        {
            // get => (_pivot.Origin - StartCircle.Origin).magnitude;
            get => (_bendCircle.Origin - StartCircle.Origin).magnitude;
            set
            {
                if (Math.Abs(BendRadius - value) < float.Epsilon) return;

                // Sys.SetDistance(_pivot, _endPoint, value);
                // _pivot.Origin = MainLCS.Origin + AxisX * value;
                Sys.Evaluate();
                OnPropertyChanged();
            }
        }

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

        public float BendAngle
        {
            get => Vector3.Angle(_startPoint.Origin - _bendCircle.Origin, EndCircle.Origin - _bendCircle.Origin);
            set
            {
                if (Math.Abs(BendAngle - value) < float.Epsilon) return;

                // Sys.SetAngle(_startPlane, EndPlane, Mathf.Deg2Rad * value);
                Sys.Evaluate();
                OnPropertyChanged();
            }
        }

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
            StartCircle.TestDraw($"{name}-StartCircle");
            _pivot.TestDraw($"{name}-_pivot");
            _bendCircle.TestDraw($"{name}-_bendCircle");
            _rightPoint.TestDraw($"{name}-_right");
            _startRightAxis.TestDraw($"{name}-_startRightAxis");
            _pivotAxis.TestDraw($"{name}-_pivotAxis");
            // _rotationCircle.TestDraw($"{name}-_rotationCircle");
            _axis.TestDraw($"{name}-_axis");
        }

        #region Private definitions

        // private Vector3 AxisX => (_pivot.Origin - StartCircle.Origin).normalized;
        private Vector3 AxisX => (_bendCircle.Origin - StartCircle.Origin).normalized;

        private readonly GCMPlane _startPlane;
        public readonly GCMCircle StartCircle;

        private readonly GCMPoint _pivot;
        // private readonly GCMLine _bendAxis;
        private readonly GCMCircle _bendCircle;
        private readonly GCMLine _axis;
        private readonly GCMPlane _bendPlane;
        private readonly GCMLine _startRightAxis;
        private readonly GCMLine _pivotAxis;

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