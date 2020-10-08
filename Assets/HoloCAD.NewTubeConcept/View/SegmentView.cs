// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloCAD.NewTubeConcept.Model;
using HoloCore;
using HoloCore.UI;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(LineRenderer), typeof(CapsuleCollider), typeof(SelectableObject))]
    [RequireComponent(typeof(ObjectManipulator))]
    public class SegmentView : MonoBehaviour, IMixedRealityPointerHandler, ISelectable, IMixedRealityFocusHandler
    {
        public const float Diameter = 0.05f;
        public GameObject PreviewPointPrefab;
        public Interactable VerticalButton;
        public Interactable HorizontalButton;
        public Interactable AddPointButton;
        public Transform ButtonBar;
        public Transform ConstraintBar;
        public GameObject ParallelLabel; 
        public GameObject HorizontalLabel; 
        public GameObject VerticalLabel;
        public Transform TubeView;
        
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
                                        segment.End.PropertyChanged -= SegmentOnPropertyChanged;
                                        segment.Line.PropertyChanged -= SegmentOnPropertyChanged;
                                    };

                segment.Start.PropertyChanged += SegmentOnPropertyChanged;
                segment.End.PropertyChanged += SegmentOnPropertyChanged;
                segment.Line.PropertyChanged += SegmentOnPropertyChanged;
            }
        }

        public void StartAddingPoint()
        {
            _addingPoint = AddPointButton.IsToggled;
        }

        public void OnVerticalButtonToggled()
        {
            if (VerticalButton.IsToggled) MakeVertical();
            else RemoveVertical();
        }

        public void OnHorizontalButtonToggled()
        {
            if (HorizontalButton.IsToggled) MakeHorizontal();
            else RemoveHorizontal();
        }

        public void MakeVertical()
        {
            if (!segment.IsInFlange)
            {
                var sys = GCMSystemBehaviour.System;
                RemoveHorizontal();

                _verticalConstraint = sys.MakePerpendicular(segment, GCMSystemBehaviour.HorizontalPlane);
                VerticalButton.IsToggled = true;
                if (sys.Evaluate() != GCMResult.GCM_RESULT_Ok)
                {
                    sys.RemoveConstraint(_verticalConstraint);
                    sys.Evaluate();
                    VerticalButton.IsToggled = false;
                }
            }

            ChangeConstraintLabels();
        }

        public void RemoveVertical()
        {
            if (_verticalConstraint != null)
            {
                GCMSystemBehaviour.System.RemoveConstraint(_verticalConstraint);
                GCMSystemBehaviour.System.Evaluate();
                VerticalButton.IsToggled = false;
            }

            ChangeConstraintLabels();
        }

        public void MakeHorizontal()
        {
            if (!segment.IsInFlange)
            {
                var sys = GCMSystemBehaviour.System;
                RemoveVertical();

                _horizontalConstraint = sys.MakeParallel(segment, GCMSystemBehaviour.HorizontalPlane);
                HorizontalButton.IsToggled = true;
                if (sys.Evaluate() != GCMResult.GCM_RESULT_Ok)
                {
                    sys.RemoveConstraint(_horizontalConstraint);
                    sys.Evaluate();
                    HorizontalButton.IsToggled = false;
                }
            }

            ChangeConstraintLabels();
        }

        public void RemoveHorizontal()
        {
            if (_horizontalConstraint != null)
            {
                GCMSystemBehaviour.System.RemoveConstraint(_horizontalConstraint);
                GCMSystemBehaviour.System.Evaluate();
                HorizontalButton.IsToggled = false;
            }
            ChangeConstraintLabels();
        }
        
        protected static readonly Color DefaultTubeBaseColor = new Color(1f, 1f, 0f, 0.25f);

        /// <summary> Цвет участка трубы, когда она пересекается с другим участком трубы. </summary>
        protected static readonly Color CollidingTubeBaseColor = new Color(1f, 0f, 0f, 0.25f);

        #region Unity event functions

        public void Awake()
        {
            _renderer = GetComponent<LineRenderer>();
            _collider = GetComponent<CapsuleCollider>();
            _selectable = GetComponent<SelectableObject>();
            _manipulator = GetComponent<ObjectManipulator>();
            _manipulator.OnManipulationStarted.AddListener(OnManipulationStarted);
            _manipulator.OnManipulationEnded.AddListener(OnManipulationEnded);
        }

        public void Start()
        {
            if (segment.IsInFlange)
            {
                _manipulator.ManipulationType = 0;
            }

            // ReSharper disable once PossibleNullReferenceException
            _camera = Camera.main.transform;
            _tubeViewRenderer = TubeView.GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (_addingPoint && _previewPoint != null)
            {
                _previewPoint.transform.position =
                    Vector3.Project(CameraRaycast.HitPoint - segment.Start.Origin, segment.Line.Direction)
                    + segment.Start.Origin;
            }

            if (_isDragged)
            {
                segment.Move(transform.position, transform.forward);
            }

            var right = Vector3.Cross(segment.Middle - _camera.position, segment.Line.Direction).normalized;
            if (right.y < 0) right = -right;
            
            ButtonBar.position = segment.Middle + right * 0.05f;
            ButtonBar.LookAt(_camera, right);
            ConstraintBar.position = segment.Middle - right * 0.05f;
            ConstraintBar.LookAt(_camera, -right);
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
            if (Selected)
            {
                if (_addingPoint && _isFocused) _segment.Owner.AddPoint(segment, _previewPoint.transform.position);
            }
            else
            {
                _selectable.SelectThis();
            }
        }

        public void OnFocusEnter(FocusEventData eventData)
        {
            _isFocused = true;
            if (_addingPoint && Selected)
            {
                CreatePointPreview();
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            _isFocused = false;
            Destroy(_previewPoint);
            _previewPoint = null;
        }

        public void OnManipulationStarted(ManipulationEventData eventData)
        {
            _isDragged = true;
        }

        public void OnManipulationEnded(ManipulationEventData eventData)
        {
            _isDragged = false;
        }

        #endregion

        #region ISelectable

        public bool Selected { get; set; }

        public void OnSelect()
        {
            // Do nothing
        }

        public void OnDeselect()
        {
            _addingPoint = false;
        }

        #endregion

        #region Private definitions

        private SelectableObject _selectable;
        private LineRenderer _renderer;
        private Segment _segment;
        private CapsuleCollider _collider;
        private GameObject _previewPoint;
        private bool _addingPoint;
        private ObjectManipulator _manipulator;
        private bool _isDragged;
        private bool _isFocused;
        private GCMConstraint _verticalConstraint;
        private GCMConstraint _horizontalConstraint;
        private Transform _camera;
        [CanBeNull] private MeshRenderer _tubeViewRenderer;
        protected static readonly int GridColor = Shader.PropertyToID("_GridColor");
        protected static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        
        private void ChangeConstraintLabels()
        {
            ParallelLabel.SetActive(false);
            VerticalLabel.SetActive(VerticalButton.IsToggled);
            HorizontalLabel.SetActive(HorizontalButton.IsToggled);
        }

        private void CreatePointPreview()
        {
            _previewPoint = Instantiate(PreviewPointPrefab, transform);
            _previewPoint.transform.position = segment.Middle;
        }

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
            _collider.height = _segment.LineLength;
            _collider.radius = Diameter / 2;
            _collider.center = Vector3.forward * _segment.LineLength / 2;
            
            TubeView.localPosition = Vector3.zero + Vector3.forward * segment.Start.DeltaLength;
            TubeView.up = (segment.End.Origin - segment.Start.Origin).normalized;
            TubeView.localScale = new Vector3(segment.Diameter, segment.TubeLength, segment.Diameter);
            
            Color baseColor = segment.TubeLength < 0 ? CollidingTubeBaseColor : DefaultTubeBaseColor;
            if (!(_tubeViewRenderer is null)) _tubeViewRenderer.material.SetColor(BaseColor, baseColor);
        }

        #endregion
    }
}