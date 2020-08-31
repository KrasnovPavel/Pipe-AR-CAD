// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
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

        protected override void UpdatePlacement()
        {
            var oldNormal = Direction;
            base.UpdatePlacement();
            if (!oldNormal.FloatEquals(Direction))
            {
                OnPropertyChanged(nameof(Direction));
            }
        }

        public override void TestDraw(string name)
        {
            base.TestDraw(name);
            var lineRenderer = DrawObject.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(1, Vector3.forward);
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

        protected override void UpdatePlacement()
        {
            var oldNormal = Normal;
            base.UpdatePlacement();
            if (!oldNormal.FloatEquals(Normal))
            {
                OnPropertyChanged(nameof(Normal));
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

        protected override void UpdatePlacement()
        {
            var oldNormal = Normal;
            base.UpdatePlacement();
            if (!oldNormal.FloatEquals(Normal))
            {
                OnPropertyChanged(nameof(Normal));
            }
        }

        public override void TestDraw(string name)
        {
            base.TestDraw(name);
            if (DrawObject == null) return;

            var lineRenderer = DrawObject.GetComponent<LineRenderer>();
            lineRenderer.endWidth = Radius / 3;
            lineRenderer.startWidth = Radius / 3;
            lineRenderer.loop = true;

            var numberOfPoints = 15;
            lineRenderer.positionCount = numberOfPoints;
            Vector3[] points = new Vector3[numberOfPoints];
            for (int i = 0; i < numberOfPoints; i++)
            {
                points[i] =  Quaternion.AngleAxis(360 / numberOfPoints * i, Vector3.forward) * Vector3.right * Radius;
            }
            lineRenderer.SetPositions(points);
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

            sys.Evaluated += UpdatePlacement;
            
            if (parent != null)
            {
                parent.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                {
                    if (args.PropertyName == nameof(Placement))
                    {
                        UpdatePlacement();
                    }
                };
            }

            _placement = GCMSys.GetPlacement(this);
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
            GCMSys.Evaluated -= UpdatePlacement;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{GetType()}-{Descriptor}";
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

        public virtual void TestDraw(string name)
        {
            if (DrawObject == null)
            {
                DrawObject = UnityEngine.Object.Instantiate(Resources.Load("DrawObject", typeof(GameObject))) as GameObject;
            }
            
            if (DrawObject == null) return;

            DrawObject.name = name;
            Placement.Apply(DrawObject.transform);
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

        protected virtual void UpdatePlacement()
        {
            var newPlacement = GCMSys.GetPlacement(this);
            var oldOrigin = Origin;
            if (!newPlacement.Equals(Placement))
            {
                _placement = newPlacement;
                OnPropertyChanged(nameof(Placement));
            }

            if (!oldOrigin.FloatEquals(Origin))
            {
                OnPropertyChanged(nameof(Origin));
            }
        }
        
        /// <summary> Родительская система координат объекта. </summary>
        public readonly GCMObject Parent;

        internal readonly GCMDescriptor Descriptor;
        protected readonly GCMSystem GCMSys;
        [CanBeNull] protected GameObject DrawObject;

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