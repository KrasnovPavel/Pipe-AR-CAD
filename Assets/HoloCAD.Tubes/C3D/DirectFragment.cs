// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.C3D
{
    /// <summary> Модель прямого отрезка трубы в системе C3D. </summary>
    public class DirectTubeFragment : TubeFragment
    {
        /// <summary> Длина отрезка. </summary>
        public float Length
        {
            get => (StartCircle.Origin - EndCircle.Origin).magnitude;
            set
            {
                if (Math.Abs(Length - value) < float.Epsilon) return;

                Sys.SetDistance(EndPlane, StartPlane, value, GCMAlignment.Cooriented);
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
        /// <param name="parent"> Предыдущий отрезок. </param>
        /// <param name="diameter"> Диаметр отрезка. </param>
        /// <param name="length"> Длина отрезка </param>
        /// <exception cref="FragmentsNotConnectable"></exception>
        public DirectTubeFragment(GCMSystem sys, TubeFragment parent, float diameter, float length)
            : base(sys, diameter, parent, true)
        {
            if (parent != null && Math.Abs(parent.Diameter - diameter) > float.Epsilon)
            {
                throw new FragmentsNotConnectable();
            }

            StartCircle = new GCMCircle(sys, Vector3.zero, Vector3.forward, diameter / 2, MainLCS);
            StartCircle.Freeze();
            _axis = new GCMLine(sys, Vector3.zero, Vector3.forward, MainLCS);
            _axis.Freeze();
            StartPlane = new GCMPlane(sys, Vector3.zero, Vector3.forward, MainLCS);
            StartPlane.Freeze();
            _startRightAxis = new GCMLine(sys, Vector3.zero, Vector3.right, MainLCS);
            _startRightAxis.Freeze();

            sys.MakeParallel(EndPlane, StartPlane, GCMAlignment.Cooriented);
            sys.MakeConcentric(EndCircle, _axis);
            sys.MakeConcentric(StartCircle, _axis);
            sys.MakeCoincident(EndCircle, EndPlane, GCMAlignment.Cooriented);
            sys.MakeCoincident(StartCircle, StartPlane, GCMAlignment.Cooriented);
            sys.MakePerpendicular(_axis, StartPlane);
            sys.MakeParallel(_startRightAxis, RightAxis);

            Length = length;

            if (parent != null)
            {
                sys.MakeCoincident(parent.EndPlane, StartPlane, GCMAlignment.Cooriented);
                sys.MakeConcentric(parent.EndCircle, StartCircle, GCMAlignment.Cooriented);
            }

            sys.Evaluate();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
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

        #endregion
    }
}