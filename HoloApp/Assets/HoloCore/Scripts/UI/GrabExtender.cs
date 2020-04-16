using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCore.UI
{
    /// <summary> Виджет для изменения линейных размеров вытягиванием. </summary>
    public class GrabExtender : MonoBehaviour, INotifyPropertyChanged, ISelectable
    {
        /// <summary> Префаб виджета для вытягивания. </summary>
        public GameObject GrabberPrefab;
        
        /// <summary> Значение на которое вытянуто. </summary>
        public float Value
        {
            get => _value;
            set
            {
                if (Math.Abs(_value - value) < float.Epsilon) return;

                _value = value;
                if (_isStarted)
                {
                    _grabber.localPosition = Vector3.forward * _value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary> Масштаб виджета для вытягивания. </summary>
        public float GrabberScale = 1;

        /// <inheritdoc />
        public void OnSelect()
        {
            if (_isStarted) _grabber.gameObject.SetActive(true);
        }

        /// <inheritdoc />
        public void OnDeselect()
        {
            _grabber.gameObject.SetActive(false);
        }

        #region Unity event functions

        private void Start()
        {
            _grabber = Instantiate(GrabberPrefab, transform).transform;
            _grabber.localPosition = Vector3.forward * Value;
            _isStarted = true;
        }

        private void Update()
        {
            var pos = _grabber.localPosition.z;
            pos = pos < 0 ? 0 : pos;
            _grabber.localPosition = Vector3.forward * pos;
            _grabber.localRotation = Quaternion.identity;
            _grabber.localScale = Vector3.one * GrabberScale;
            Value = pos;
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private region

        private float _value;
        private bool _isStarted;

        private Transform _grabber;

        #endregion
    }
}