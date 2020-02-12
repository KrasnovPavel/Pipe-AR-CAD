// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using UnityEngine;

namespace HoloCore.UI.Transparent
{
    /// <summary> Компонент для управления прозрачностью TextMesh. </summary>
    [RequireComponent(typeof(TextMesh))]
    public sealed class TextMeshTransparency : Transparency
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
            _mesh = GetComponent<TextMesh>();
        }

        [CanBeNull] private TextMesh _mesh;
    }
}
