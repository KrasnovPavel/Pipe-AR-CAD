using System.ComponentModel;
using HoloCAD.NewTubeConcept.Model;
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
            _point1   = transform.Find("point1");
            _point2   = transform.Find("point2");
            _renderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if (firstPointMoving)  MovePoint(segment.Start, _point1.position);
            if (secondPointMoving) MovePoint(segment.End,   _point2.position);
        }
        
        public void StartPointManipulationStarted()
        {
            firstPointMoving = true;
        }

        public void StartPointManipulationEnded()
        {
            firstPointMoving = false;
        }

        public void EndPointManipulationStarted()
        {
            secondPointMoving = true;
        }

        public void EndPointManipulationEnded()
        {
            secondPointMoving = false;
        }

        #endregion

        #region Private definitions

        private bool firstPointMoving;
        private bool secondPointMoving;

        private LineRenderer _renderer;
        private Transform    _point1;
        private Transform    _point2;
        private Segment      _segment;

        private GCMSystem _sys => segment.Start.GCMSys;

        private void SegmentOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalculatePosition();
        }

        private void CalculatePosition()
        {
            transform.position = _segment.Start.Origin;

            _point1.localPosition = Vector3.zero;
            _point1.localScale    = Vector3.one * Diameter;
            _point2.position      = _segment.End.Origin;
            _point2.localScale    = Vector3.one * Diameter;

            _renderer.useWorldSpace = true;
            _renderer.SetPosition(0, _segment.Start.Origin);
            _renderer.SetPosition(1, _segment.End.Origin);
            _renderer.endWidth = _renderer.startWidth = Diameter / 2;
        }

        private void MovePoint(GCMPoint point, Vector3 pos)
        {
            var lastPos = point.Origin;
            point.Origin = pos;

            if (_sys.Evaluate() != GCMResult.GCM_RESULT_Ok || !segment.Owner.IsCorrect())
            {
                point.Origin = lastPos;
                _sys.Evaluate();
            }
        }

        #endregion
    }
}