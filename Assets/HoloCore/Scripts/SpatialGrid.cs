// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityEngine;
using UnityEngine.XR.WSA;

namespace HoloCore
{
    /// <summary> Компонент для управления отображением сетки сканирования. </summary>
    [RequireComponent(typeof(SpatialMappingCollider), typeof(SpatialMappingRenderer))]
    public class SpatialGrid : Singleton<SpatialGrid>
    {
        private void Start()
        {
            enabled = false;
            _mapCollider = gameObject.GetComponent<SpatialMappingCollider>();
            _mapRenderer = gameObject.GetComponent<SpatialMappingRenderer>();
        }
        
        private void OnEnable()
        {
            Instance._mapCollider.enabled = true;
            Instance._mapRenderer.enabled = true;
        }

        private void OnDisable()
        {
            Instance._mapCollider.enabled = true;
            Instance._mapRenderer.enabled = true;
        }
        
        private SpatialMappingCollider _mapCollider;
        private SpatialMappingRenderer _mapRenderer;
    }
}