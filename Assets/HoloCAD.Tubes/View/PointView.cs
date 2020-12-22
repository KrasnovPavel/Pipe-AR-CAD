// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloCAD.Tubes.Model;
using HoloCore.UI;
using Microsoft.MixedReality.Toolkit.Input;
using UnityC3D;
using UnityEngine;

namespace HoloCAD.Tubes.View
{
    /// <summary> Виджет точки. </summary>
    [RequireComponent(typeof(SelectableObject))]
    public class PointView : MonoBehaviour, ISelectable
    {
        /// <summary> Труба - хозяин точки. </summary>
        public TubeView Owner;

        /// <summary> Панель кнопок. </summary>
        public GameObject ToolBar;

        /// <summary> Объект, отображающий погиб. </summary>
        public GameObject BendView;

        public Transform Sphere;

        /// <summary> Производится ли перемещение точки пользователем. </summary>
        public bool IsManipulationStarted { get; set; }

        public float MinimalPointSize;

        /// <summary> Модель точки. </summary>
        public TubePoint Point
        {
            get => _point;
            set
            {
                if (ReferenceEquals(_point, value)) return;

                if (_point != null)
                {
                    _point.Disposed        -= PointOnDisposed;
                    _point.PropertyChanged -= PointOnPropertyChanged;
                }
                
                _point                 =  value;
                transform.position     =  _point.Origin;
                _point.Disposed        += PointOnDisposed;
                _point.PropertyChanged += PointOnPropertyChanged;
            }
        }

        /// <summary> Удаление этой точки. </summary>
        public void RemoveThisPoint()
        {
            Point.Owner.RemovePoint(Point);
        }

        public void ChangeRadius()
        {
            Point.UseSecondRadius = !Point.UseSecondRadius;
        }

        #region Event functions

        private void Awake()
        {
            // ReSharper disable once PossibleNullReferenceException
            _camera =  Camera.main.transform;
            name    += GetHashCode().ToString();

            GCMSystemBehaviour.System.Evaluated += delegate { _redrawRequested = true; };
        }

        private void Start()
        {
            _mesh            = BendView.GetComponent<MeshFilter>();
            _meshRenderer    = BendView.GetComponent<MeshRenderer>();
            _redrawRequested = true;
        }

        private void Update()
        {
            if (IsManipulationStarted) Point.Move(transform.position);

            ToolBar.transform.LookAt(_camera);
        }

        private void LateUpdate()
        {
            if (_redrawRequested)
            {
                Redraw();
                _redrawRequested = false;
            }
        }

        private void OnDestroy()
        {
            _point.Disposed        -= PointOnDisposed;
            _point.PropertyChanged -= PointOnPropertyChanged;
        }

        #endregion

        #region ISelectable

        /// <inheritdoc />
        public bool Selected { get; set; }

        /// <inheritdoc />
        public void OnSelect()
        {
            if (!Point.IsInFlange)
            {
                ToolBar.SetActive(true);
            }
        }

        /// <inheritdoc />
        public void OnDeselect()
        {
            if (!Point.IsInFlange)
            {
                ToolBar.SetActive(false);
            }
        }

        #endregion

        #region Private definitions

        private                 TubePoint            _point;
        private                 Transform            _camera;
        private                 IMixedRealityPointer _draggingPointer;
        private                 bool                 _redrawRequested;
        private                 MeshFilter           _mesh;
        private                 MeshRenderer         _meshRenderer;
        private static readonly int                  ShaderDiameter   = Shader.PropertyToID("_Diameter");
        private static readonly int                  ShaderBendRadius = Shader.PropertyToID("_BendRadius");
        private static readonly int                  ShaderAngle      = Shader.PropertyToID("_Angle");
        private static readonly int                  ShaderGridColor  = Shader.PropertyToID("_GridColor");

        private void PointOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GCMPoint.Origin))
            {
                transform.position = _point.Origin;
            }

            _redrawRequested = true;
        }

        private void Redraw()
        {
            if (Point.Owner == null || Point.Prev == null || Point.Next == null)
            {
                Destroy(gameObject);
                return;
            }
            
            var prev                      = Point.Prev.Start.Origin;
            var next                      = (Point.IsInEndFlange) ? Point.Next.Start.Origin : Point.Next.End.Origin;
            var forward                   = (next - Point.Origin).normalized;
            var backward                  = (prev - Point.Origin).normalized;
            _mesh.mesh = MeshFactory.GetMeshes(_point.Owner.TubeData)[Point.UseSecondRadius ? 1 : 0];
            _meshRenderer.material.SetFloat(ShaderAngle,      Point.GetBendAngle() * Mathf.Deg2Rad);
            _meshRenderer.material.SetFloat(ShaderDiameter,   Point.Diameter);
            _meshRenderer.material.SetFloat(ShaderBendRadius, Point.BendRadius);
            _meshRenderer.material.SetColor(ShaderGridColor, Color.green);
            BendView.transform.position = Point.Origin + backward * Point.DeltaLength;
            BendView.transform.LookAt(Point.Origin,
                                      Vector3.Cross(Vector3.ProjectOnPlane(forward, -backward), -backward));

            Sphere.localScale = Vector3.one * Mathf.Max(Point.Diameter, MinimalPointSize);
        }

        private void PointOnDisposed()
        {
            Destroy(gameObject);
        }

        #endregion
    }
}
