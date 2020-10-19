// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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

        /// <summary> Список отрезков трубы. </summary>
        public List<SegmentView> SegmentViews = new List<SegmentView>();

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
            OnPointAdded(startFlangeSegment.End);
            OnPointAdded(endFlangeSegment.End);

            foreach (var point in tube.Points)
            {
                OnPointAdded(point);
            }
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
        }

        /// <summary> Создаёт новый виджет для отображения отрезка. </summary>
        /// <param name="segment"> Модель отрезка. </param>
        private void OnSegmentAdded(Segment segment)
        {
            var go = Instantiate(TubePrefabsContainer.Instance.SegmentPrefab, transform);
            var sv = go.GetComponent<SegmentView>();
            sv.segment = segment;
            SegmentViews.Add(sv);
        }

        #endregion
    }
}
