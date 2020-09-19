// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using UnityEngine;

namespace HoloCore
{
    public class CameraRaycast : Singleton<CameraRaycast>
    {
        public float RaycastDistance = 10f;

        public static Vector3 HitPoint { get; private set; }

        private void Awake()
        {
            // ReSharper disable once PossibleNullReferenceException
            _camera = Camera.main.transform;
        }

        private void Update()
        {
            RaycastHit hit;            
            if (Physics.Raycast(_camera.position, _camera.forward, out hit, RaycastDistance))
            {
                HitPoint = hit.point;
            }
            else
            {
                HitPoint = _camera.forward * RaycastDistance;
            }
        }

        private Transform _camera;
    }
}