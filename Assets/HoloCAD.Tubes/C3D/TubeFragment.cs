// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.C3D
{
    /// <summary> Базовый класс для отрезков труб. </summary>
    public abstract class TubeFragment : IDisposable, INotifyPropertyChanged
    {
        /// <summary> Диаметр отрезка. </summary>
        public virtual float Diameter
        {
            get => EndCircle.Radius * 2;
            set
            {
                if (Math.Abs(Diameter - value) < float.Epsilon) return;
                EndCircle.Radius = value / 2;
                Sys.Evaluate();
                OnPropertyChanged();
            }
        }

        /// <summary> Окружность на конце отрезка. </summary>
        public readonly GCMCircle EndCircle;

        /// <summary> Плоскость канца отрезка. </summary>
        public readonly GCMPlane EndPlane;

        /// <summary> Окружность в начале отрезка. </summary>
        public GCMCircle StartCircle { get; protected set; }

        /// <summary> Плоскость начала отрезка. </summary>
        public GCMPlane StartPlane { get; protected set; }

        /// <summary> Прямая задающая ось X на конце отрезка. </summary>
        public readonly GCMLine RightAxis;

        /// <summary> Предыдущий отрезок. </summary>
        public readonly TubeFragment Parent;

        /// <summary> Событие вызываемое при удалении отрезка. </summary>
        public Action Disposed;

        /// <summary> Размещение отрезка. </summary>
        public MbPlacement3D Placement => MainLCS.Placement;

        public virtual void Dispose()
        {
            EndPlane?.Dispose();
            EndCircle?.Dispose();
            StartCircle?.Dispose();
            StartPlane?.Dispose();
            RightAxis?.Dispose();
            MainLCS?.Dispose();
            Disposed?.Invoke();
        }

        /// <summary> Функция для тестовой отрисовки отрезка на сцене в Unity. </summary>
        /// <param name="name"> Имя для отображения на сцене. </param>
        public virtual void TestDraw(string name)
        {
            StartCircle?.TestDraw($"{name}-StartCircle");
            EndCircle.TestDraw($"{name}-EndCircle");
            RightAxis.TestDraw($"{name}-RightAxis");
        }

        #region Protected definitions

        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система C3D. </param>
        /// <param name="diameter"> Диаметр отрезка. </param>
        /// <param name="parent"> Предыдущий отрезок. </param>
        /// <param name="useLCS"> Нужно ли создавать локальную систему координат. </param>
        protected TubeFragment(GCMSystem sys, float diameter, TubeFragment parent, bool useLCS = false)
        {
            Sys = sys;
            if (useLCS) MainLCS = new GCM_LCS(Sys, sys.GroundLCS.Placement, sys.GroundLCS);

            EndCircle = new GCMCircle(sys, Vector3.zero, -Vector3.forward, diameter / 2, useLCS ? MainLCS : null);
            EndPlane  = new GCMPlane(sys, Vector3.zero, -Vector3.forward, useLCS ? MainLCS : null);
            RightAxis = new GCMLine(sys, Vector3.zero, Vector3.right, useLCS ? MainLCS : null);

            sys.MakeCoincident(EndCircle, EndPlane);
            sys.MakeCoincident(RightAxis, EndPlane);

            Parent = parent;

            Sys.Evaluated += OnEvaluated;

            Sys.Evaluate();
        }

        protected virtual void OnEvaluated()
        {
            if (MainLCS != null && !MainLCS.Placement.Equals(_placement))
            {
                _placement = MainLCS.Placement;
                OnPropertyChanged(nameof(Placement));
            }
        }

        /// <summary> Система C3D. </summary>
        protected readonly GCMSystem Sys;

        /// <summary> Локальная система координат отрезка. </summary>
        protected GCM_LCS MainLCS;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private definitions

        private MbPlacement3D _placement;

        #endregion
    }

    /// <summary> Исключение, выбрасываемое при несоответствии диаметров отрезков. </summary>
    public class FragmentsNotConnectable : Exception
    {
        public FragmentsNotConnectable() : base("Fragments have different diameters")
        {
        }
    }
}