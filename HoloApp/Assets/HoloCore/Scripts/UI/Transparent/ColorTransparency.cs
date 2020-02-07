// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using UnityEngine;

namespace HoloCore.UI.Transparent
{
    /// <summary>
    /// Компонент для управления прозрачностью объектов с материалами имеющими свойство _Color,
    /// в том числе стандартными материалами MRTK.
    /// </summary>
    public sealed class ColorTransparency : Transparency
    {
        /// <summary> Цвет объекта. </summary>
        public Color BaseColor;
        
        /// <inheritdoc />
        public override void OnAlphaChanged()
        {
            if (_renderer != null)
            {
                _renderer.material.SetColor(ShaderColor, BaseColor * Alpha);
            }
            base.OnAlphaChanged();
        }

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
            if (_renderer != null)
            {
                BaseColor = _renderer.material.GetColor(ShaderColor);
            }
            else
            {
                BaseColor = Color.white;
            }
        }

        [CanBeNull] private MeshRenderer _renderer;
        private static readonly int ShaderColor = Shader.PropertyToID("_Color");
    }
}
