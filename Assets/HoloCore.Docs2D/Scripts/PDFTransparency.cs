// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCore.UI.Transparent;
using UnityEngine;

namespace HoloCore.Docs2D
{
    /// <summary> Компонент для управления прозрачностью обозревателя PDF. </summary>
    [RequireComponent(typeof(Viewer2D))]
    public class PDFTransparency : Transparency
    {
        /// <inheritdoc />
        public override void OnAlphaChanged()
        {
            GetComponent<ViewerPDF>().PageMaterial.color = Color.white * Alpha;
            base.OnAlphaChanged();
        }
    }
}
