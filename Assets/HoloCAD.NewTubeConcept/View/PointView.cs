// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using HoloCAD.NewTubeConcept.Model;
using HoloCore.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityC3D;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(SelectableObject))]
    public class PointView : MonoBehaviour, ISelectable
    {
        public TubeView Owner;

        public GameObject ToolBar;
        public bool Selected { get; set; }

        public GameObject BendView;

        public TubePoint Point
        {
            get => _point;
            set
            {
                if (ReferenceEquals(_point, value)) return;

                _point             = value;
                transform.position = _point.Origin;
                _point.Disposed += delegate
                {
                    Destroy(gameObject);
                    _point.PropertyChanged -= PointOnPropertyChanged;
                };

                _point.PropertyChanged += PointOnPropertyChanged;
            }
        }

        public void ManipulationStarted(ManipulationEventData eventData)
        {
            _isManipulationStarted = true;
        }

        public void ManipulationEnded()
        {
            _isManipulationStarted = false;
        }

        public void RemoveThisPoint()
        {
            Point.Owner.RemovePoint(Point);
        }

        #region Event functions

        private void Awake()
        {
            // ReSharper disable once PossibleNullReferenceException
            _camera     = Camera.main.transform;
            name += GetHashCode().ToString();
            
            GCMSystemBehaviour.System.Evaluated += delegate { _redrawPended = true; };
        }

        private void Start()
        {
            var data = new TubeLoader.TubeData();
            data.diameter = 0.05f;
            data.first_radius = data.second_radius = 0.1f;
            var angle = 180 - Vector3.Angle(Point.Next.End.Origin - Point.Origin, Point.Prev.Start.Origin - Point.Origin); 
            BendView.GetComponent<MeshFilter>().mesh = MeshFactory.GetMeshes(data)[0];
            var renderer = BendView.GetComponent<MeshRenderer>();
            renderer.material.SetFloat(ShaderDiameter, Point.Next.Diameter);
            renderer.material.SetFloat(ShaderBendRadius, 0.1f);
            renderer.material.SetFloat(ShaderAngle, angle / 180f * (float) Math.PI);
            _redrawPended = true;
        }

        private void Update()
        {
            if (_isManipulationStarted) MovePoint();

            ToolBar.transform.LookAt(_camera);
        }

        private void LateUpdate()
        {
            if (_redrawPended)
            {
                var prev = Point.Prev.Start.Origin;
                var next = Point.Next.End.Origin;
                if (Point.IsInEndFlange) next = Point.Next.Start.Origin;
                var forward = (next - Point.Origin).normalized;
                var backward = (prev - Point.Origin).normalized;
                BendView.GetComponent<MeshRenderer>().material.SetFloat(ShaderAngle, Point.GetBendAngle() * Mathf.Deg2Rad);
                BendView.transform.position = Point.Origin + backward * Point.DeltaLength;
                BendView.transform.LookAt(Point.Origin, Vector3.Cross(Vector3.ProjectOnPlane(forward, -backward), -backward));
                _redrawPended = false;
            }
        }

        public void OnSelect()
        {
            if (!Point.IsInFlange)
            {
                ToolBar.SetActive(true);
            }
        }

        public void OnDeselect()
        {
            if (!Point.IsInFlange)
            {
                ToolBar.SetActive(false);
            }
        }

        #endregion

        #region Private definitions

        private TubePoint _point;
        private bool      _isManipulationStarted;
        private Transform _camera;
        private IMixedRealityPointer _draggingPointer;
        private bool _redrawPended;
        private static readonly int ShaderDiameter = Shader.PropertyToID("_Diameter");
        private static readonly int ShaderBendRadius = Shader.PropertyToID("_BendRadius");
        private static readonly int ShaderAngle = Shader.PropertyToID("_Angle");

        private void PointOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GCMPoint.Origin))
            {
                transform.position = _point.Origin;
            }
        }

        private void MovePoint()
        {
            var            lastPos= Point.Origin;
            MbPlacement3D? lastPlacePrev = Point.Prev?.Line.Placement;
            MbPlacement3D? lastPlaceNext = Point.Next?.Line.Placement;
            
            if (Point.IsInFlange)
            {
                var flange = Point.GetFlange();
                var projection = Vector3.Project(transform.position - flange.Origin, flange.Normal);
                if (Vector3.Angle(projection, flange.Normal) > 90)
                {
                    transform.position = lastPos;
                    return;
                }
                
                Point.Origin = flange.Origin + projection;
                Point.Prev?.ResetLine();
                Point.Next?.ResetLine();
                Point.GCMSys.Evaluate();
                transform.position = Point.Origin;
                return;
            }
            
            Point.Origin = transform.position;
            Point.Prev?.ResetLine();
            Point.Next?.ResetLine();

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