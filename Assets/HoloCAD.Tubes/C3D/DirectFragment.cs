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
            get => (_startCircle.Origin - EndCircle.Origin).magnitude;
            set
            {
                if (Math.Abs(Length - value) < float.Epsilon) return;

                Sys.SetDistance(EndPlane, _startPlane, value, GCMAlignment.Cooriented);
                Sys.Evaluate();
                OnPropertyChanged();
            }
        }

        /// <summary> Диаметр отрезка трубы. </summary>
        /// <exception cref="FragmentsNotConnectable"></exception>
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
                _startCircle.Radius = value / 2;
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
            : base(sys, diameter, parent)
        {
            if (parent != null && Math.Abs(parent.Diameter - diameter) > float.Epsilon)
            {
                throw new FragmentsNotConnectable();
            }

            _startCircle = new GCMCircle(sys, Vector3.zero, -Vector3.forward, diameter / 2, MainLCS);
            _startCircle.Freeze();
            _axis = new GCMLine(sys, Vector3.zero, -Vector3.forward, MainLCS);
            _axis.Freeze();
            _startPlane = new GCMPlane(sys, Vector3.zero, -Vector3.forward, MainLCS);
            _startPlane.Freeze();
            
            sys.MakeParallel(EndPlane, _startPlane, GCMAlignment.Cooriented);
            sys.MakeConcentric(EndCircle, _axis);
            sys.MakeCoincident(EndCircle, EndPlane, GCMAlignment.Cooriented);

            Length = length;
            
            if (parent != null)
            {
                sys.MakeCoincident(parent.EndPlane, _startPlane, GCMAlignment.Cooriented);
                sys.MakeConcentric(parent.EndCircle, _startCircle, GCMAlignment.Cooriented);
            }

            sys.Evaluate();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _axis?.Dispose();
            _startPlane?.Dispose();
            _startCircle?.Dispose();
            base.Dispose();
        }

        #region Private definitions

        private readonly GCMLine _axis;
        private readonly GCMPlane _startPlane;
        private readonly GCMCircle _startCircle;
        private GCMConstraint _lengthConstraint;
        
        #endregion
    }
}