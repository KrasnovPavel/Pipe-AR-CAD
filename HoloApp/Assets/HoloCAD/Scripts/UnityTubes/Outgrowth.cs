// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityEngine;

namespace HoloCAD.UnityTubes
{
    /// <summary> Класс управляющий отростком трубы. </summary>
    [RequireComponent(typeof(DirectTubeFragment))]
    public sealed class Outgrowth : MonoBehaviour
    {
        /// <summary> Минимальный угол между трубой и отростком. </summary>
        public const float MinAngle = 15;
        
        /// <summary> Минимальный угол между трубой и отростком. </summary>
        public static readonly float MaxAngle = Mathf.Round(180 - MinAngle);

        /// <summary> Шаг перемещения отростка вдоль трубы при нажатии на кнопку. </summary>
        public const float Step = 0.05f;
        
        /// <summary> Угол между отростком и родительской трубой </summary>
        public float Angle { get; private set; }

        /// <summary> Перемещяет отросток вдоль трубы. </summary>
        /// <param name="delta"> Расстояние на которое надо переместить. </param>
        public void Move(float delta)
        {
            Vector3 pos = transform.localPosition;
            float maxDistance = ParentFragment.Length - Fragment.Diameter / 2;
            float minDistance = Fragment.Diameter / 2;
            bool isInside = (minDistance < pos.z + delta) && (pos.z + delta < maxDistance);
            
            if (isInside) transform.localPosition += Vector3.forward * delta;
        }

        /// <summary> Изменяет угол между трубой и отростком. </summary>
        /// <param name="delta"> Угол, на который надо изменить. </param>
        public void ChangeAngle(float delta)
        {
            bool isValid = (MinAngle <= Math.Round(Angle + delta)) 
                           && (Math.Round(Angle + delta) <= MaxAngle);

            if (isValid) transform.localRotation *= Quaternion.AngleAxis(delta, Vector3.up);
            
            Angle = Vector3.Angle(Fragment.transform.forward, ParentFragment.transform.forward);
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

        #region Unity event functions

        private void Start()
        {
            Fragment = GetComponent<DirectTubeFragment>();
            ParentFragment = (DirectTubeFragment) Fragment.Parent;
        }

        #endregion
    }
}