using System.ComponentModel;
using HoloCAD.NewTubeConcept.Model;
using Microsoft.MixedReality.Toolkit.Input;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(LineRenderer))]
    public class SegmentView : MonoBehaviour
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
        }

        #endregion
        
        #region Private definitions

        private LineRenderer _renderer;
        private Segment      _segment;

        private void SegmentOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalculatePosition();
        }

        private void CalculatePosition()
        {
            transform.position = _segment.Start.Origin;

            _renderer.useWorldSpace = true;
            _renderer.SetPosition(0, _segment.Start.Origin);
            _renderer.SetPosition(1, _segment.End.Origin);
            _renderer.endWidth = _renderer.startWidth = Diameter / 2;
        }

        #endregion
    }
}