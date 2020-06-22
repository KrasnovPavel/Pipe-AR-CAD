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
        private void Awake()
        {
            _mapCollider = GetComponent<SpatialMappingCollider>();
            _mapRenderer = GetComponent<SpatialMappingRenderer>();
            enabled = false;
        }
        
        private void OnEnable()
        {
            _mapCollider.enabled = true;
            _mapRenderer.enabled = true;
        }

        private void OnDisable()
        {
            _mapCollider.enabled = false;
            _mapRenderer.enabled = false;
        }
        
        private SpatialMappingCollider _mapCollider;
        private SpatialMappingRenderer _mapRenderer;
    }
}