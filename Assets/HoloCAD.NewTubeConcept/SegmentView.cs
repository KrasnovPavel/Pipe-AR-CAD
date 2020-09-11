using System;
using System.ComponentModel;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept
{
    public class SegmentView : MonoBehaviour
    {
        public const float Diameter = 0.05f;

        public Segment segment
        {
            get => _segment;
            set
            {
                _segment = value;

                _point1.localScale = Vector3.one * Diameter * 2;
                _point2.localScale = Vector3.one * Diameter * 2;
                CalculatePosition();
                
                segment.Disposed += delegate { Destroy(gameObject); };
                segment.Start.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                                                 {
                                                     if (args.PropertyName == nameof(segment.Start.Origin))
                                                     {
                                                         CalculatePosition();
                                                     }
                                                 };
                segment.End.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                                                 {
                                                     if (args.PropertyName == nameof(segment.End.Origin))
                                                     {
                                                         CalculatePosition();
                                                     }
                                                 };
            }
        }

        private void CalculatePosition()
        {
            transform.position = _segment.Start.Origin;
            _tube.localPosition = Vector3.zero;
            _point1.localPosition = Vector3.zero;
            _point2.position = _segment.End.Origin;
            _tube.localScale = new Vector3(Diameter, Diameter, _segment.Length / 2);
            
            _tube.LookAt(_point2);
        }

        public void Awake()
        {
            _tube = transform.Find("tube");
            _point1 = transform.Find("point1");
            _point2 = transform.Find("point2");
        }

        private void Update()
        {
            var sys = segment.Start.GCMSys;
            if (firstPointMoving)
            {
                var lastPos = segment.Start.Origin; 
                segment.Start.Origin = _point1.position;
                if (sys.Evaluate() != GCMResult.GCM_RESULT_Ok)
                {
                    segment.Start.Origin = lastPos;
                    sys.Evaluate();
                }
            }
            if (secondPointMoving)
            {
                var lastPos = segment.End.Origin; 
                segment.End.Origin = _point2.position;
                if (sys.Evaluate() != GCMResult.GCM_RESULT_Ok)
                {
                    segment.End.Origin = lastPos;
                    sys.Evaluate();
                }
            }
        }

        public void ManipulationStartedStart()
        {
            firstPointMoving = true;
        }

        public void ManipulationEndedStart()
        {
            firstPointMoving = false;
        }

        public void ManipulationStartedEnd()
        {
            secondPointMoving = true;
        }

        public void ManipulationEndedEnd()
        {
            secondPointMoving = false;
        }

        private bool firstPointMoving;
        private bool secondPointMoving;

        private Transform _tube;
        private Transform _point1;
        private Transform _point2;
        private Segment _segment;

    }
}