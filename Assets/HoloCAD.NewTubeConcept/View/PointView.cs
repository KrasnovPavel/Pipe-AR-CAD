using System;
using System.ComponentModel;
using HoloCAD.NewTubeConcept.Model;
using Microsoft.MixedReality.Toolkit.UI;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(SphereCollider), typeof(ObjectManipulator))]
    public class PointView : MonoBehaviour
    {
        public Segment PrevSegment;
        public Segment NextSegment;
        
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
            var lastPlacePrev = PrevSegment.Line.Placement;
            var lastPlaceNext = NextSegment.Line.Placement;
            
            _point.Origin = transform.position;
            PrevSegment.ResetLine();
            NextSegment.ResetLine();

            var res = Point.GCMSys.Evaluate(); 
            if (res != GCMResult.GCM_RESULT_Ok)
            {
                Debug.LogWarning(res);
                _point.Origin = lastPos;
                PrevSegment.Line.Placement = lastPlacePrev;
                NextSegment.Line.Placement = lastPlaceNext;
                Point.GCMSys.Evaluate();
            }
        }

        #endregion
    }
}