// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace HoloCore.UI.Transparent
{
    /// <summary> Компонент для управления прозрачностью TextMeshPro. </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public sealed class TextMeshProTransparency : Transparency
    {
        /// <inheritdoc />
        public override void OnAlphaChanged()
        {
            if (_mesh != null)
            {
                Color textColor = _mesh.color;
                textColor.a = Alpha;
                _mesh.color = textColor;
            }
        
            base.OnAlphaChanged();
        }

        private void Start()
        {
            _mesh = GetComponent<TextMeshPro>();
        }

        [CanBeNull] private TextMeshPro _mesh;
    }
}
