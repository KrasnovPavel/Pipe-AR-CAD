using HoloCore.UI.Transparent;
using UnityEngine;

namespace HoloCAD.UI.Docs2D
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
