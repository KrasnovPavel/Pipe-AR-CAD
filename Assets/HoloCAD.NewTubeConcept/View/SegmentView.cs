using System.ComponentModel;
using HoloCAD.NewTubeConcept.Model;
using Microsoft.MixedReality.Toolkit.Input;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(LineRenderer), typeof(CapsuleCollider))]
    public class SegmentView : MonoBehaviour, IMixedRealityPointerHandler
    {
        public const float Diameter = 0.05f;

        public Segment segment
        {
            get => _segment;
            set
            {
                _segment = value;

                CalculatePosition();

                segment.Disposed += delegate
                {
                    Destroy(gameObject);
                    segment.Start.PropertyChanged -= SegmentOnPropertyChanged;
                    segment.End.PropertyChanged   -= SegmentOnPropertyChanged;
                };

                segment.Start.PropertyChanged += SegmentOnPropertyChanged;
                segment.End.PropertyChanged   += SegmentOnPropertyChanged;
            }
        }

        #region Unity event functions

        public void Awake()
        {
            _renderer = GetComponent<LineRenderer>();
            _collider = GetComponent<CapsuleCollider>();
        }

        #endregion

        #region MRTK event functions

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            _segment.Owner.AddPoint(segment);
        }

        #endregion
        
        #region Private definitions

        private LineRenderer _renderer;
        private Segment      _segment;
        private CapsuleCollider _collider;

        private void SegmentOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalculatePosition();
        }

        private void CalculatePosition()
        {
            transform.position = _segment.Start.Origin;
            transform.LookAt(_segment.End.Origin);

            _renderer.useWorldSpace = true;
            _renderer.SetPosition(0, _segment.Start.Origin);
            _renderer.SetPosition(1, _segment.End.Origin);
            _renderer.endWidth = _renderer.startWidth = Diameter / 2;
            _collider.height = _segment.Length;
            _collider.radius = Diameter / 2;
            _collider.center = Vector3.forward * _segment.Length / 2;
        }

        #endregion
    }
}