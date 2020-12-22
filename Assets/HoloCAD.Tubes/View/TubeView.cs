// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using HoloCAD.Tubes.Model;
using UnityEngine;

namespace HoloCAD.Tubes.View
{
    /// <summary> Компонент, отвечающий за отображение всей трубы. </summary>
    public class TubeView : MonoBehaviour
    {
        /// <summary> Виджет начального фланца. </summary>
        public FlangeView StartFlangeView;

        /// <summary> Виджет конечного фланца. </summary>
        public FlangeView EndFlangeView;

        /// <summary> Список виджетов отрезков трубы. </summary>
        public List<SegmentView> SegmentViews = new List<SegmentView>();

        /// <summary> Список виджетов точек трубы. </summary>
        public List<PointView> PointViews = new List<PointView>();

        // ReSharper disable once InconsistentNaming
        /// <summary> Модель трубы. </summary>
        public Tube tube;

        #region Unity Event functions

        private void Start()
        {
            tube.SegmentAdded += OnSegmentAdded;
            var startFlangeSegment = tube.StartFlange.FirstSegment;
            var endFlangeSegment   = tube.EndFlange.FirstSegment;
            OnSegmentAdded(startFlangeSegment);
            OnSegmentAdded(endFlangeSegment);

            foreach (var segment in tube.Segments)
            {
                OnSegmentAdded(segment);
            }

            tube.PointAdded += OnPointAdded;
            tube.Disposed += delegate { Destroy(gameObject); };
            OnPointAdded(startFlangeSegment.End);
            OnPointAdded(endFlangeSegment.End);

            foreach (var point in tube.Points)
            {
                OnPointAdded(point);
            }
            
            TubeViewsManager.TubeViews.Add(this);
        }

        private void OnDestroy()
        {
            TubeViewsManager.TubeViews.Remove(this);
        }

        #endregion

        #region Private definitions

        /// <summary> Создаёт новый виджет для отображения точки. </summary>
        /// <param name="point"> Модель точки. </param>
        private void OnPointAdded(TubePoint point)
        {
            var go = Instantiate(TubePrefabsContainer.Instance.PointPrefab, transform);
            var p  = go.GetComponent<PointView>();
            p.Point = point;
            p.Owner = this;
            PointViews.Add(p);
            p.Point.Disposed += delegate { PointViews.Remove(p); };
        }

        /// <summary> Создаёт новый виджет для отображения отрезка. </summary>
        /// <param name="segment"> Модель отрезка. </param>
        private void OnSegmentAdded(Segment segment)
        {
            var go = Instantiate(TubePrefabsContainer.Instance.SegmentPrefab, transform);
            var sv = go.GetComponent<SegmentView>();
            sv.segment = segment;
            sv.Owner = this;
            SegmentViews.Add(sv);
            sv.segment.Disposed += delegate { SegmentViews.Remove(sv); };
        }

        #endregion
    }
}
