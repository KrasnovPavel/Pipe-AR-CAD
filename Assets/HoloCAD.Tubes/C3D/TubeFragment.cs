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

        /// <summary> Предыдущий отрезок. </summary>
        public readonly TubeFragment Parent;

        /// <summary> Событие вызываемое при удалении отрезка. </summary>
        public Action Disposed;

        /// <summary> Размещение отрезка. </summary>
        public MbPlacement3D Placement
        {
            get => MainLCS.Placement;
            set
            {
                MainLCS.Placement = value;
                Sys.Evaluate();
            }
        }

        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система C3D. </param>
        /// <param name="diameter"> Диаметр отрезка. </param>
        /// <param name="parent"> Предыдущий отрезок. </param>
        protected TubeFragment(GCMSystem sys, float diameter, TubeFragment parent)
        {
            Sys = sys;
            MainLCS = new GCM_LCS(Sys,
                MbPlacement3D.FromLeftCS(Vector3.zero, Vector3.right, Vector3.up, Vector3.forward), sys.GroundLCS);
            EndCircle = new GCMCircle(sys, Vector3.zero, -Vector3.forward, diameter / 2, MainLCS);
            EndPlane = new GCMPlane(sys, Vector3.zero, -Vector3.forward, MainLCS);
            Parent = parent;

            MainLCS.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(GCM_LCS.Placement)) OnPropertyChanged(nameof(Placement));
            };

            Sys.Evaluate();
        }

        public virtual void Dispose()
        {
            EndPlane?.Dispose();
            EndCircle?.Dispose();
            MainLCS?.Dispose();
            Disposed?.Invoke();
        }

        #region Protecteed definitions

        /// <summary> Система C3D. </summary>
        protected readonly GCMSystem Sys;
        
        /// <summary> Локальная система координат отрезка. </summary>
        protected readonly GCM_LCS MainLCS;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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