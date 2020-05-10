// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace HoloCore.UI
{
    /// <summary> Виджет вращения объектов. </summary>
    public class GrabRotator : MonoBehaviour, INotifyPropertyChanged, ISelectable
    {
        /// <summary> Префаб маркера, за который будет тянуть пользователь </summary>
        public GameObject GrabObjectPrefab;

        /// <summary> Радиус окружности на которой будут находиться маркеры. </summary>
        public float Radius
        {
            get => _radius;
            set
            {
                if (Math.Abs(_radius - value) < float.Epsilon) return;

                _radius = value;
                SetGrabbersPosition();
                foreach (var grabber in _grabbers)
                {
                    grabber.localScale = Vector3.one * _radius;
                }
            }
        }

        /// <summary> Угол поворота. </summary>
        public float Angle
        {
            get => _angle;
            protected set
            {
                if (Math.Abs(_angle - value) < float.Epsilon) return;

                _angle = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public void OnSelect()
        {
            foreach (var grabber in _grabbers)
            {
                grabber.gameObject.SetActive(true);
            }
        }

        /// <inheritdoc />
        public void OnDeselect()
        {
            foreach (var grabber in _grabbers)
            {
                grabber.gameObject.SetActive(false);
            }

            _grabbedChildNumber = -1;
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Unity event functions

        private void Start()
        {
            for (int i = 0; i < _amountOfGrabbers; i++)
            {
                var grabber = Instantiate(GrabObjectPrefab, transform);
                Quaternion q = Quaternion.AngleAxis(DeltaAngle * i, Vector3.forward);
                grabber.transform.localPosition = Radius * (q * Vector3.right);
                grabber.transform.localScale = Vector3.one * Radius;
                var handler = grabber.GetComponent<ManipulationHandler>();
                int number = i; // Для замыкания
                handler.OnManipulationStarted.AddListener(delegate { _grabbedChildNumber = number; });
                handler.OnManipulationEnded.AddListener(delegate { _grabbedChildNumber = -1; });
                _grabbers.Add(grabber.transform);
            }
        }

        private void Update()
        {
            if (_grabbedChildNumber < 0) return;
            
            var pos = _grabbers[_grabbedChildNumber].localPosition;
            float grabberAngle = Vector3.Angle(Vector3.right, pos);
            grabberAngle *= pos.y > 0 ? 1 : -1;
            Quaternion q = Quaternion.AngleAxis(grabberAngle, Vector3.forward);
            pos = Radius * (q * Vector3.right);
            pos.z = 0;
            _grabbers[_grabbedChildNumber].localPosition = pos;
            Angle = grabberAngle - DeltaAngle * _grabbedChildNumber;

            SetGrabbersPosition();
        }

        #endregion

        #region Private definitons

        private void SetGrabbersPosition()
        {
            for (int i = 0; i < _grabbers.Count; i++)
            {
                if (i == _grabbedChildNumber) continue;
                
                Quaternion rot = Quaternion.AngleAxis(Angle + DeltaAngle * i, Vector3.forward);
                _grabbers[i].localPosition = Radius * (rot * Vector3.right);
            }
        }

        private float _angle;

        private float _radius = 3;

        private float DeltaAngle => 360f / _amountOfGrabbers;

        private int _amountOfGrabbers = 4;
        
        private int _grabbedChildNumber = -1;

        private List<Transform> _grabbers = new List<Transform>();

        #endregion
    }
}