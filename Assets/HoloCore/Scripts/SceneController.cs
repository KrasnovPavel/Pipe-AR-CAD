﻿using System;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace HoloCore
{
    [RequireComponent(typeof(SpatialMappingCollider), typeof(SpatialMappingRenderer))]
    public class SceneController : Singleton<SceneController>
    {
        public bool EnableConsole = true;

        public bool EnableSpatialRenderer
        {
            get => _renderer.enabled;
            set => _renderer.enabled = value;
        }

        public bool EnableSpatialCollider
        {
            get => _collider.enabled;
            set => _collider.enabled = value;
        }

        private void Awake()
        {
            _collider = GetComponent<SpatialMappingCollider>();
            _renderer = GetComponent<SpatialMappingRenderer>();
            EnableSpatialCollider = true;
            EnableSpatialRenderer = false;
        }

        private void Update()
        {
            if (EnableConsole)
            {
                Debug.developerConsoleVisible = false;
            }
        }

        private SpatialMappingCollider _collider;
        private SpatialMappingRenderer _renderer;
    }
}