using System;
using System.Linq;
using HoloCAD.NewTubeConcept.Model;
using HoloCAD.NewTubeConcept.View;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.Tests
{
    [RequireComponent(typeof(LineRenderer))]
    public class TubeTester : MonoBehaviour
    {
        public Vector3 StartOrigin;
        public Vector3 StartNormal;
        public Vector3 EndOrigin;
        public Vector3 EndNormal;

        public GameObject SegmentPrefab;

        private Tube tube;
        private GCMSystem _system = new GCMSystem();
        
        private void Start()
        {
            _system.SetJournal(name);
            //tube = new Tube(_system, StartOrigin, StartNormal, EndOrigin, EndNormal, _system.GroundLCS);
            tube.SegmentAdded += OnSegmentAdded;
            foreach (var segment in tube.Segments)
            {
                OnSegmentAdded(segment);
            }
        }

        private void OnSegmentAdded(Segment segment)
        {
            var go = Instantiate(SegmentPrefab);
            go.GetComponent<SegmentView>().segment = segment;
        }

        public void AddButton()
        {
            tube.AddPoint(tube.Segments.First());
            
            var vert = new GCMLine(_system, Vector3.zero, Vector3.up, _system.GroundLCS);
            vert.Freeze();

            _system.MakeParallel(tube.Segments.First(), vert);
            _system.Evaluate();
        }

        private void OnDestroy()
        {
            _system.Dispose();
        }
    }
}