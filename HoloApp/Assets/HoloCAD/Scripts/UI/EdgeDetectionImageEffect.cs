using UnityEngine;

namespace HoloCAD.UI
{
    // TODO: Сменить имя и документацию на более универсальный вариант. 
    /// <summary> Класс, добавляющий эффект подсветки граней на этапе постобработки. </summary>
    [ExecuteInEditMode]
    public class EdgeDetectionImageEffect : MonoBehaviour 
    {
        /// <summary> Материал реализующий подсветку граней. </summary>
        [Tooltip("Материал реализующий подсветку граней.")]
        public Material Material;

        private void OnRenderImage(RenderTexture src, RenderTexture dest) {
            Graphics.Blit(src, dest, Material);
        }
    }
}