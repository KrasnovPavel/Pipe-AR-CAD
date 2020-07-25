using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityC3D
{
    // TODO: Вызывать OnPropertyChanged только тогда, когда свойство действительно изменилось.
    
    public class GCMPoint : GCMObject
    {
        public GCMPoint(GCMSystem sys, Vector3 origin)
            : base(sys, sys.AddPoint(origin))
        {
        }
    }

    public class GCMLine : GCMObject
    {
        public GCMLine(GCMSystem sys, Vector3 origin, Vector3 direction)
            : base(sys, sys.AddLine(origin, direction))
        {
            sys.Evaluated += delegate { OnPropertyChanged(nameof(Direction)); };
        }

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

    public class GCMPlane : GCMObject
    {
        public GCMPlane(GCMSystem sys, Vector3 origin, Vector3 normal)
            : base(sys, sys.AddPlane(origin, normal))
        {
            sys.Evaluated += delegate { OnPropertyChanged(nameof(Normal)); };
        }

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

    public class GCMCircle : GCMObject
    {
        public GCMCircle(GCMSystem sys, Vector3 origin, Vector3 normal, float radius)
            : base(sys, sys.AddCircle(origin, normal, radius))
        {
            RadiusConstraint = sys.CreateRadiusConstraint(this);
            Radius = radius;
            sys.Evaluated += delegate { OnPropertyChanged(nameof(Normal)); };
        }
        
        public float Radius
        {
            get => GCMSys.GetRadius(this);
            set
            {
                GCMSys.SetRadius(this, value);
                OnPropertyChanged();
            }
        }

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

    // ReSharper disable once InconsistentNaming
    public class GCM_LCS : GCMObject
    {
        public GCM_LCS(GCMSystem sys, Transform placement)
            : base(sys, sys.AddLCS(placement))
        {
        }
        
        internal GCM_LCS(GCMSystem sys, GCMDescriptor desc) : base(sys, desc)
        {
        }
    }

    public abstract class GCMObject : INotifyPropertyChanged
    {
        internal GCMObject(GCMSystem sys, GCMDescriptor desc)
        {
            GCMSys = sys;
            Descriptor = desc;

            sys.Evaluated += delegate { OnPropertyChanged(nameof(Placement)); };
            sys.Evaluated += delegate { OnPropertyChanged(nameof(Origin)); };
        }

        public Vector3 Origin
        {
            get => Placement.Origin;
            set
            {
                var p = Placement;
                p.Origin = value;
                Placement = p;
                OnPropertyChanged();
            }
        }

        public MbPlacement3D Placement
        {
            get => GCMSys.GetPlacement(this);
            set
            {
                GCMSys.SetPlacement(this, value);
                OnPropertyChanged();
            }
        }

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
    }
}