using System;
using System.ComponentModel;
using Microsoft.MixedReality.Toolkit.UI;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(SphereCollider), typeof(ObjectManipulator))]
    public class PointView : MonoBehaviour
    {
        public GCMPoint Point
        {
            get => _point;
            set
            {
                if (ReferenceEquals(_point, value)) return;

                _point = value;
                transform.position = _point.Origin;
                _point.Disposed += delegate
                                    {
                                        Destroy(gameObject);
                                        _point.PropertyChanged -= PointOnPropertyChanged;
                                    };

                _point.PropertyChanged += PointOnPropertyChanged;
            }
        }

        public void ManipulationStarted()
        {
            _isManipulationStarted = true;
        }

        public void ManipulationEnded()
        {
            _isManipulationStarted = false;
        }

        #region Unity event functions

        private void Update()
        {
            if (_isManipulationStarted) MovePoint();
        }

        #endregion


        #region Private definitions

        private GCMPoint _point;
        private bool _isManipulationStarted; 

        private void PointOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GCMPoint.Origin))
            {
                transform.position = _point.Origin;
            }
        }     
        
        private void MovePoint()
        {
            var lastPos = _point.Origin;
            _point.Origin = transform.position;

            var res = Point.GCMSys.Evaluate(); 
            if (res != GCMResult.GCM_RESULT_Ok)
            {
                Debug.LogWarning(res);
                _point.Origin = lastPos;
                Point.GCMSys.Evaluate();
            }
        }

        #endregion
    }
}