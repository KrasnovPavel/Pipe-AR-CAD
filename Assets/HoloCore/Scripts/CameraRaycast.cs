// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using UnityEngine;

namespace HoloCore
{
    public class CameraRaycast : Singleton<CameraRaycast>
    {
        public float RaycastDistance = 10f;

        public static Vector3 HitPoint { get; private set; }
        
        [CanBeNull] public static Transform HitObject { get; private set; } 

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
                HitObject = hit.transform;
            }
            else
            {
                HitPoint = _camera.forward * RaycastDistance;
                HitObject = null;
            }
        }

        private Transform _camera;
    }
}