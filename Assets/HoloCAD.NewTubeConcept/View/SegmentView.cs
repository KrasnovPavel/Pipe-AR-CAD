// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloCAD.NewTubeConcept.Model;
using HoloCore;
using HoloCore.UI;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace HoloCAD.NewTubeConcept.View
{
    [RequireComponent(typeof(LineRenderer), typeof(CapsuleCollider), typeof(SelectableObject))]
    public class SegmentView : MonoBehaviour, IMixedRealityPointerHandler, ISelectable, IMixedRealityFocusHandler
    {
        public const float      Diameter = 0.05f;
        public       GameObject PreviewPointPrefab;

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
            _renderer   = GetComponent<LineRenderer>();
            _collider   = GetComponent<CapsuleCollider>();
            _selectable = GetComponent<SelectableObject>();
        }

        private void Update()
        {
            if (_previewPoint != null)
            {
                _previewPoint.transform.position =
                    Vector3.Project(CameraRaycast.HitPoint - segment.Start.Origin, segment.Line.Direction)
                    + segment.Start.Origin;
            }
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
            if (_selected)
            {
                _segment.Owner.AddPoint(segment, _previewPoint.transform.position);
            }
            else
            {
                _selectable.SelectThis();
                CreatePointPreview();
            }
        }

        public void OnFocusEnter(FocusEventData eventData)
        {
            if (_selected)
            {
                CreatePointPreview();
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            Destroy(_previewPoint);
            _previewPoint = null;
        }

        #endregion

        #region ISelectable

        public void OnSelect()
        {
            _selected = true;
        }

        public void OnDeselect()
        {
            _selected = false;
        }

        #endregion

        #region Private definitions

        private SelectableObject _selectable;
        private LineRenderer     _renderer;
        private Segment          _segment;
        private CapsuleCollider  _collider;
        private bool             _selected;
        private GameObject       _previewPoint;

        private void CreatePointPreview()
        {
            _previewPoint                    = Instantiate(PreviewPointPrefab, transform);
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
            _collider.height   = _segment.Length;
            _collider.radius   = Diameter / 2;
            _collider.center   = Vector3.forward * _segment.Length / 2;
        }

        #endregion
    }
}