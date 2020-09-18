// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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
        public TubeView Owner;
        
        public TubePoint Point
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

        private TubePoint _point;
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
            MbPlacement3D? lastPlacePrev = _point.Prev?.Line.Placement;
            MbPlacement3D? lastPlaceNext = _point.Next?.Line.Placement;
            
            _point.Origin = transform.position;
            _point.Prev?.ResetLine();
            _point.Next?.ResetLine();

            var res = Point.GCMSys.Evaluate(); 
            if (res != GCMResult.GCM_RESULT_Ok)
            {
                Debug.LogWarning(res);
                _point.Origin = lastPos;
                if (_point.Prev != null && lastPlacePrev != null)
                {
                    _point.Prev.Line.Placement = lastPlacePrev.Value; 
                }
                if (_point.Next != null && lastPlaceNext != null)
                {
                    _point.Next.Line.Placement = lastPlaceNext.Value; 
                }
                Point.GCMSys.Evaluate();
            }
            Owner.tube.FixErrors();
        }

        #endregion
    }
}