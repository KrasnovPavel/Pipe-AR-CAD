// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;

namespace UnityC3D
{
    public class GCMPattern : IDisposable
    {
        internal GCMPattern(GCMSystem sys, GCMDescriptor descriptor, GCMObject sample, GCMObject axialObject)
        {
            _sys        = sys;
            Descriptor  = descriptor;
            Sample      = sample;
            AxialObject = axialObject;
        }

        public void AddObject(GCMObject obj, float value, GCMAlignment alignment, GCMScale scale)
        {
            Constraints.Add(_sys.AddObjectToPattern(this, obj, value, alignment, scale));
        }

        public void RemoveObject(GCMObject obj)
        {
            var co = Constraints.Find(c => c.Obj3.Equals(obj));
            if (co == null) return;

            Constraints.Remove(co);
            _sys.RemoveConstraint(co);
        }

        public void ChangeValue(GCMObject obj, float newValue)
        {
            var co = Constraints.Find(c => c.Obj3.Equals(obj));
            if (co == null) return;
            
            _sys.ChangeDrivingDimensions(co, newValue);
        }

        public void Dispose()
        {
            _sys.RemovePattern(this);
        }

        internal readonly GCMDescriptor       Descriptor;
        internal readonly GCMObject           Sample;
        internal readonly GCMObject           AxialObject;
        internal readonly List<GCMConstraint> Constraints = new List<GCMConstraint>();

        #region Private definitions

        private readonly GCMSystem _sys;

        #endregion
    }
}