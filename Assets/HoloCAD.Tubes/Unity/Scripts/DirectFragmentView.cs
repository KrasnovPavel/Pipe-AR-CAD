// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloCAD.Tubes.C3D;
using UnityEngine;

namespace HoloCAD.Tubes.Unity
{
    [RequireComponent(typeof(LineRenderer))]
    public class DirectFragmentView : MonoBehaviour
    {
        public Transform StartObject;
        public Transform EndObject;
        
        public DirectTubeFragment TubeFragment 
        { 
            get => _tubeFragment;
            set
            {
                if (_tubeFragment != null)
                {
                    _tubeFragment.PropertyChanged -= TubeFragmentOnPropertyChanged;
                    _tubeFragment.Disposed -= TubeFragmentOnDisposed;
                }
                _tubeFragment = value;
                if (_tubeFragment == null) return;
                
                Redraw();
                _tubeFragment.PropertyChanged += TubeFragmentOnPropertyChanged;
                _tubeFragment.Disposed += TubeFragmentOnDisposed;
            }
        }

        #region Unity event functions

        private void Start()
        {
            if (_tubeFragment != null) Redraw();
        }

        #endregion

        #region Private definitions
        
        private DirectTubeFragment _tubeFragment;
        private LineRenderer _renderer;

        private void TubeFragmentOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Redraw();
        }

        private void TubeFragmentOnDisposed()
        {
            Destroy(gameObject);
        }

        private void Redraw()
        {
            // _tubeFragment.StartCircle.Placement.Apply(transform);
            EndObject.position = _tubeFragment.EndCircle.Origin;
            StartObject.localScale = Vector3.one * _tubeFragment.Diameter;
            EndObject.localScale = Vector3.one * _tubeFragment.Diameter;
            
            if (_renderer == null) _renderer = GetComponent<LineRenderer>();
            _renderer.SetPosition(0, transform.position);
            _renderer.SetPosition(1, _tubeFragment.EndCircle.Origin);
        }

        #endregion
    }
}
