// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using UnityEngine;

namespace HoloCore.UI.Transparent
{
    /// <summary> Компонент для управления прозрачностью SimpleButton. </summary>
    public sealed class SimpleButtonTransparency : Transparency
    {
        /// <inheritdoc />
        public override void OnAlphaChanged()
        {
            if (_renderer != null) _renderer.material.SetFloat(ShaderAlpha, Alpha);
            base.OnAlphaChanged();
        }

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        [CanBeNull] private MeshRenderer _renderer;
        private static readonly int ShaderAlpha = Shader.PropertyToID("_Alpha");
    }
}
