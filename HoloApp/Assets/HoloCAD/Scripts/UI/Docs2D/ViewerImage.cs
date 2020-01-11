// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI.Docs2D
{
    /// <summary> Обозреватель изображений. </summary>
    public class ViewerImage : Viewer2D
    {
        /// <summary> Шейдер для отрисовки документа. </summary>
        [Tooltip("Шейдер для отрисовки документа.")]
        [CanBeNull] public Shader CanvasShader;

        /// <inheritdoc />
        public override void Init(byte[] byteArray, string path)
        {
            base.Init(byteArray, path);
            Texture2D sample = new Texture2D(2, 2);
            sample.LoadImage(byteArray);
            _material.mainTexture = sample;
            ResizeCanvas(new Vector2(sample.texelSize.y, sample.texelSize.x));
        }

        #region Unity event functioins

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            
            if (CanvasShader == null)
            {
                CanvasShader = Shader.Find("Mixed Reality Toolkit/Standard");
            }
            Canvas.GetComponent<MeshRenderer>().sharedMaterial = new Material(CanvasShader);
            _material = Canvas.GetComponent<MeshRenderer>().sharedMaterial;
        }

        #endregion

        #region Private definitions

        private Material _material;

        #endregion
    }
}