// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.C3D
{
    /// <summary> Модель прямого отрезка трубы в системе C3D. </summary>
    public class DirectFragment : TubeFragment
    {
        /// <summary> Длина отрезка. </summary>
        public float Length
        {
            get => (StartCircle.Origin - EndCircle.Origin).magnitude;
            set
            {
                if (Math.Abs(Length - value) < float.Epsilon) return;

                // Sys.SetDistance(EndPlane, StartPlane, value, GCMAlignment.Cooriented);
                _lengthPattern.ChangeValue(EndCircle, value);
                
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

                EndCircle.Radius   = value / 2;
                StartCircle.Radius = value / 2;
                Sys.Evaluate();
                OnPropertyChanged();
            }
        }

        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система C3D. </param>
        /// <param name="diameter"> Диаметр отрезка. </param>
        /// <param name="length"> Длина отрезка </param>
        /// <param name="parent"> Предыдущий отрезок. </param>
        /// <exception cref="FragmentsNotConnectable"></exception>
        public DirectFragment(GCMSystem sys, float diameter, float length, TubeFragment parent)
            : base(sys, diameter, parent)
        {
            if (parent != null && Math.Abs(parent.Diameter - diameter) > float.Epsilon)
            {
                throw new FragmentsNotConnectable();
            }

            // MainLCS = new GCM_LCS(Sys, sys.GroundLCS.Placement, sys.GroundLCS);

            StartCircle = new GCMCircle(sys, Vector3.zero, Vector3.forward, diameter / 2);
            var startPoint = new GCMPoint(sys, Vector3.zero);
            sys.MakeConcentric(StartCircle, startPoint);
            
            var startPlane = new GCMPlane(sys, Vector3.zero, Vector3.forward);
            _axis = new GCMLine(sys, Vector3.zero, Vector3.forward);
            sys.MakeCoincident(StartCircle, startPlane, GCMAlignment.Cooriented);
            sys.MakePerpendicular(_axis, startPlane, GCMAlignment.Cooriented);
            
            _startRightAxis = new GCMLine(sys, Vector3.zero, Vector3.right);

            _lengthPattern = sys.CreateLinearPattern(StartCircle, _axis, GCMAlignment.AlignWithAxialGeom);
            _lengthPattern.AddObject(EndCircle, length, GCMAlignment.Cooriented, GCMScale.GCM_RIGID);

            var endPoint = new GCMPoint(sys, Vector3.forward * length);
            sys.MakeConcentric(EndCircle, endPoint);
            sys.MakeCoincident(endPoint, RightAxis);
            sys.MakeParallel(RightAxis, _startRightAxis, GCMAlignment.Cooriented);
            
            if (parent != null)
            {
                sys.MakeConcentric(parent.EndCircle, startPoint);
                sys.SetAngle(parent.EndCircle, StartCircle, 0);
                sys.MakeCoincident(parent.RightAxis, _startRightAxis, GCMAlignment.Cooriented);
            }

            sys.Evaluate();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _lengthPattern?.Dispose();
            _axis?.Dispose();
            StartPlane?.Dispose();
            StartCircle?.Dispose();
            _startRightAxis?.Dispose();
            base.Dispose();
        }

        public override void TestDraw(string name)
        {
            base.TestDraw(name);
            _axis.TestDraw($"{name}-_axis");
        }

        #region Private definitions

        private readonly GCMLine       _axis;
        private readonly GCMLine       _startRightAxis;
        private          GCMConstraint _lengthConstraint;
        private readonly GCMPattern    _lengthPattern;

        #endregion
    }
}