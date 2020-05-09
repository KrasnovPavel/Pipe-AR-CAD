// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UnityTubes
{
    /// <summary> Класс управляющий отростком трубы. </summary>
    [RequireComponent(typeof(DirectTubeFragment))]
    public sealed class Outgrowth : MonoBehaviour, INotifyPropertyChanged
    {
        /// <summary> Минимальный угол между трубой и отростком. </summary>
        public const float MinAngle = 15;
        
        /// <summary> Минимальный угол между трубой и отростком. </summary>
        public static readonly float MaxAngle = Mathf.Round(180 - MinAngle);

        /// <summary> Угол между отростком и родительской трубой </summary>
        public float Angle
        {
            get => _angle;
            private set
            {
                if (Math.Abs(_angle - value) < float.Epsilon) return;

                _angle = value;
                OnPropertyChanged();
            }
        }

        public float Position
        {
            get => transform.localPosition.z;
            private set
            {
                if (Math.Abs(transform.localPosition.z - value) < float.Epsilon) return;

                transform.localPosition = Vector3.forward * value;
                OnPropertyChanged();
            }
        }

        /// <summary> Перемещяет отросток вдоль трубы. </summary>
        /// <param name="delta"> Расстояние на которое надо переместить. </param>
        public void Move(float delta)
        {
            float maxDistance = ParentFragment.Length - Fragment.Diameter / 2;
            float minDistance = Fragment.Diameter / 2;
            bool isInside = (minDistance < Position + delta) && (Position + delta < maxDistance);
            
            if (isInside) Position += (Vector3.forward * delta).z;
        }

        /// <summary> Изменяет угол между трубой и отростком. </summary>
        /// <param name="delta"> Угол, на который надо изменить. </param>
        public void ChangeAngle(float delta)
        {
            bool isValid = (MinAngle <= Math.Round(Angle + delta)) && (Math.Round(Angle + delta) <= MaxAngle);

            if (isValid)
            {
                transform.localRotation *= Quaternion.AngleAxis(delta, Vector3.up);
                Angle = Vector3.Angle(Fragment.transform.forward, ParentFragment.transform.forward);
            }
            
        }

        /// <summary> Вращает отросток вокруг трубы. </summary>
        /// <param name="delta"> Угол, на который надо повернуть. </param>
        public void TurnAround(float delta)
        {
            transform.RotateAround(transform.position, ParentFragment.transform.forward, delta);
        }

        /// <summary> Прямой участок трубы, являющийся отростком. </summary>
        public DirectTubeFragment Fragment { get; private set; }

        /// <summary> Прямой участок трубы, являющийся родителем отростка. </summary>
        public DirectTubeFragment ParentFragment { get; private set; }
        
        /// <summary> Событие, вызываемое при изменении какого-либо свойства. </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        #region Unity event functions

        private void Start()
        {
            Fragment = GetComponent<DirectTubeFragment>();
            ParentFragment = (DirectTubeFragment) Fragment.Parent;
            Angle = 90;
        }

        #endregion

        #region Private definitions

        /// <summary> Обработчик изменения свойств. </summary>
        /// <param name="propertyName"> Имя изменившегося свойства. </param>
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private float _angle;

        #endregion
    }
}