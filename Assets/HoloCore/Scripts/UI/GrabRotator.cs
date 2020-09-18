// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace HoloCore.UI
{
    /// <summary> Виджет вращения объектов. </summary>
    public class GrabRotator : MonoBehaviour, INotifyPropertyChanged
    {
        /// <summary> Префаб маркера, за который будет тянуть пользователь </summary>
        public GameObject GrabObjectPrefab;

        public float Scale = 1;

        public int AmountOfGrabbersEditor;
        public float RadiusEditor;

        public Transform HostTransform;

        public int AmountOfGrabbers
        {
            get => _grabbers.Count;
            set
            {
                if (value == AmountOfGrabbers) return;

                if (value > AmountOfGrabbers)
                {
                    while (AmountOfGrabbers < value)
                    {
                        AddGrabber();
                    }
                }
                else
                {
                    while (AmountOfGrabbers > value)
                    {
                        DestroyImmediate(_grabbers.Last().gameObject);
                        _grabbers.RemoveAt(_grabbers.Count - 1);
                    }
                }
#if UNITY_EDITOR
                AmountOfGrabbersEditor = value;
#endif
                SetGrabbersPosition();
            }
        }

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
                    grabber.localScale = Vector3.one * (_radius * Scale);
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

                DeltaAngle = value - _angle;
                _angle = value;
                OnPropertyChanged();
            }
        }
        
        public float DeltaAngle { get; protected set; }

        public void OnEnable()
        {
            foreach (var grabber in _grabbers)
            {
                grabber.gameObject.SetActive(true);
            }
        }

        public void OnDisable()
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
            AmountOfGrabbers = AmountOfGrabbersEditor;
            Radius = RadiusEditor;
        }

        private void Update()
        {
#if UNITY_EDITOR
            AmountOfGrabbers = AmountOfGrabbersEditor;
            Radius = RadiusEditor;
#endif

            if (_grabbedChildNumber < 0) return;

            var pos = _grabbers[_grabbedChildNumber].localPosition;
            pos.z = 0;
            float grabberAngle = Vector3.Angle(Vector3.right, pos);
            grabberAngle *= pos.y > 0 ? 1 : -1;
            Angle = grabberAngle - GrabbersAngle * _grabbedChildNumber;

            SetGrabbersPosition();

            if (HostTransform != null)
            {
                HostTransform.Rotate(transform.forward, Angle);
            }
        }

        #endregion

        #region Private definitons

        private void SetGrabbersPosition()
        {
            for (int i = 0; i < _grabbers.Count; i++)
            {
                Quaternion rot = Quaternion.AngleAxis(Angle + GrabbersAngle * i, Vector3.forward);
                var pos = Radius * (rot * Vector3.right);
                _grabbers[i].localPosition = pos;
                _grabbers[i].LookAt(transform, Vector3.Cross(transform.forward, -pos));
            }
        }

        private void AddGrabber()
        {
            var grabber = Instantiate(GrabObjectPrefab, transform);
            grabber.transform.localScale = Vector3.one * (Radius * Scale);
            var handler = grabber.GetComponent<ObjectManipulator>();
            int number = _grabbers.Count; // Для замыкания
            handler.OnManipulationStarted.AddListener(delegate { _grabbedChildNumber = number; });
            handler.OnManipulationEnded.AddListener(delegate { _grabbedChildNumber = -1; });
            _grabbers.Add(grabber.transform);
        }

        private float _angle;

        private float _radius;

        private float GrabbersAngle => 360f / AmountOfGrabbers;

        private int _grabbedChildNumber = -1;

        private List<Transform> _grabbers = new List<Transform>();

        #endregion
    }
}