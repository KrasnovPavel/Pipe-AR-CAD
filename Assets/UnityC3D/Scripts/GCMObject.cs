// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MathExtensions;
using UnityEngine;

namespace UnityC3D
{
    /// <summary> Точка в системе C3D. </summary>
    public class GCMPoint : GCMObject
    {
        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система геометрических ограничений. </param>
        /// <param name="origin"> Координаты точки. </param>
        /// <param name="parent"> Родительская система координат. </param>
        public GCMPoint(GCMSystem sys, Vector3 origin, GCM_LCS parent = null)
            : base(sys, sys.AddPoint(origin, parent?.Descriptor), parent)
        {
        }
    }

    /// <summary> Линия в системе C3D. </summary>
    public class GCMLine : GCMObject
    {
        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система геометрических ограничений. </param>
        /// <param name="origin"> Точка на прямой. </param>
        /// <param name="direction"> Направляющий вектор прямой. </param>
        /// <param name="parent"> Родительская система координат. </param>
        public GCMLine(GCMSystem sys, Vector3 origin, Vector3 direction, GCM_LCS parent = null)
            : base(sys, sys.AddLine(origin, direction, parent?.Descriptor), parent)
        {
            sys.Evaluated += delegate { OnPropertyChanged(nameof(Direction)); };
        }

        /// <summary> Направляющий вектор прямой. </summary>
        public Vector3 Direction
        {
            get => Placement.AxisZ;
            set
            {
                var p = Placement;
                p.AxisZ = value;
                Placement = p;
                OnPropertyChanged();
            }
        }
    }

    /// <summary> Плоскость в системе C3D. </summary>
    public class GCMPlane : GCMObject
    {
        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система геометрических ограничений. </param>
        /// <param name="origin"> Точка на плоскости. </param>
        /// <param name="normal"> Нормаль к плоскости. </param>
        /// <param name="parent"> Родительская система координат. </param>
        public GCMPlane(GCMSystem sys, Vector3 origin, Vector3 normal, GCM_LCS parent = null)
            : base(sys, sys.AddPlane(origin, normal, parent?.Descriptor), parent)
        {
            sys.Evaluated += delegate { OnPropertyChanged(nameof(Normal)); };
        }

        /// <summary> Нормаль к плоскости. </summary>
        public Vector3 Normal
        {
            get => Placement.AxisZ;
            set
            {
                var p = Placement;
                p.AxisZ = value;
                Placement = p;
                OnPropertyChanged();
            }
        }
    }

    /// <summary> Окружность в системе C3D. </summary>
    public class GCMCircle : GCMObject
    {
        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система геометрических ограничений. </param>
        /// <param name="origin"> Центр окружности. </param>
        /// <param name="normal"> Нормаль к плоскости окружности. </param>
        /// <param name="radius"> Разиус окружности. </param>
        /// <param name="parent"> Родительская система координат. </param>
        public GCMCircle(GCMSystem sys, Vector3 origin, Vector3 normal, float radius, GCM_LCS parent = null)
            : base(sys, sys.AddCircle(origin, normal, radius, parent?.Descriptor), parent)
        {
            RadiusConstraint = sys.CreateRadiusConstraint(this);
            Radius = radius;
            sys.Evaluated += delegate { OnPropertyChanged(nameof(Normal)); };
        }
        
        /// <summary> Радиус окружности. </summary>
        public float Radius
        {
            get => GCMSys.GetRadius(this);
            set
            {
                GCMSys.SetRadius(this, value);
                OnPropertyChanged();
            }
        }

        /// <summary> Нормаль к плоскости окружности. </summary>
        public Vector3 Normal
        {
            get => Placement.AxisZ;
            set
            {
                var p = Placement;
                p.AxisZ = value;
                Placement = p;
                OnPropertyChanged();
            }
        }

        internal readonly GCMDescriptor RadiusConstraint;
    }

    /// <summary> Локальная система координат. </summary>
    // ReSharper disable once InconsistentNaming
    public class GCM_LCS : GCMObject
    {
        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система геометрических ограничений. </param>
        /// <param name="placement"> Размещение объекта в Unity. </param>
        /// <param name="parent"> Родительская система координат. </param>
        public GCM_LCS(GCMSystem sys, Transform placement, GCM_LCS parent = null)
            : base(sys, sys.AddLCS(placement, parent?.Descriptor), parent)
        {
        }
        
        /// <summary> Конструктор. </summary>
        /// <param name="sys"> Система геометрических ограничений. </param>
        /// <param name="placement"> Размещение объекта в C3D. </param>
        /// <param name="parent"> Родительская система координат. </param>
        public GCM_LCS(GCMSystem sys, MbPlacement3D placement, GCM_LCS parent = null)
            : base(sys, sys.AddLCS(placement, parent?.Descriptor), parent)
        {
        }
        
        internal GCM_LCS(GCMSystem sys, GCMDescriptor desc) : base(sys, desc)
        {
        }
    }
    
    /// <summary> Абстрактрный геометрический объект в C3D. </summary>
    public abstract class GCMObject : INotifyPropertyChanged, IDisposable
    {
        internal GCMObject(GCMSystem sys, GCMDescriptor desc, GCM_LCS parent = null)
        {
            GCMSys = sys;
            Descriptor = desc;
            Parent = parent;

            sys.Evaluated += delegate { Placement = GCMSys.GetPlacement(this); };
            
            if (parent != null)
            {
                parent.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                {
                    if (args.PropertyName == nameof(Placement))
                    {
                        Placement = GCMSys.GetPlacement(this);
                    }
                };
            }

            Placement = GCMSys.GetPlacement(this);
        }

        /// <summary> Координаты объекта. </summary>
        public Vector3 Origin
        {
            get => Placement.Origin;
            set
            {
                if (Origin.FloatEquals(value)) return;
                
                var p = Placement;
                p.Origin = value;
                Placement = p;
                OnPropertyChanged();
            }
        }

        /// <summary> Расположение объекта. </summary>
        public MbPlacement3D Placement
        {
            get => _placement;
            set
            {
                if (value.Equals(_placement)) return;

                _placement = value;
                GCMSys.SetPlacement(this, _placement);
                OnPropertyChanged();
            }
        }
        
        public void Dispose()
        {
            GCMSys.Remove(this);
        }

        /// <summary> Заморозить объект. </summary>
        public void Freeze()
        {
            GCMSys.Freeze(this);
        }

        /// <summary> Убрать заморозку объекта. </summary>
        public void Free()
        {
            GCMSys.Free(this);
        }

        protected bool Equals(GCMObject other)
        {
            return Descriptor.Equals(other.Descriptor) && Equals(GCMSys, other.GCMSys);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GCMObject) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Descriptor.GetHashCode() * 397) ^ (GCMSys != null ? GCMSys.GetHashCode() : 0);
            }
        }
        
        /// <summary> Родительская система координат объекта. </summary>
        public readonly GCMObject Parent;

        internal readonly GCMDescriptor Descriptor;
        protected readonly GCMSystem GCMSys;

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
}