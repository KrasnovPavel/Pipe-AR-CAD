// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCore.UI;
using JetBrains.Annotations;
using Paroxe.PdfRenderer;
using UnityEngine;

namespace HoloCAD.UI.Docs2D
{
    /// <summary> Обозреватель PDF файлов. </summary>
    public class ViewerPDF : Viewer2D
    {
        /// <summary> Кнопка перехода на следующую страницу. </summary>
        [Tooltip("Кнопка перехода на следующую страницу.")]
        [CanBeNull] public Button3D NextPageButton;
        
        /// <summary> Кнопка перехода на предыдущую страницу. </summary>
        [Tooltip("Кнопка перехода на предыдущую страницу.")]
        [CanBeNull] public Button3D PreviousPageButton;

        /// <summary> Шейдер для отрисовки документа. </summary>
        [Tooltip("Шейдер для отрисовки документа.")]
        [CanBeNull] public Shader CanvasShader;
        
        /// <summary> Открытый документ. </summary>
        [CanBeNull] public PDFDocument Document { get; protected set; }

        /// <inheritdoc />
        public override void Init(byte[] byteArray, string path)
        {
            base.Init(byteArray, path);
            
            Document = new PDFDocument(byteArray);
            GoToPage(0);
        }

        /// <inheritdoc />
        public override void ToggleHiding()
        {
            base.ToggleHiding();
            if (PreviousPageButton != null) PreviousPageButton.gameObject.SetActive(!IsHided);
            if (NextPageButton != null) NextPageButton.gameObject.SetActive(!IsHided);
        }

        /// <summary> Переходит к следующей странице документа, если возможно. </summary>
        public void NextPage()
        {
            if (Document != null && _currentPageNumber < Document.GetPageCount() - 1)
            {
                GoToPage(_currentPageNumber + 1);
            }
        }

        /// <summary> Переходит к следующей странице документа, если возможно. </summary>
        public void PreviousPage()
        {
            if (Document != null && _currentPageNumber > 0)
            {
                GoToPage(_currentPageNumber - 1);
            }
        }
        
        /// <summary> Переходит к указанному номеру страницы, если возможно. </summary>
        /// <param name="index"> Номер страницы к которой нужно перейти. </param>
        public void GoToPage(int index)
        {
            if (Document != null
                && index >= 0 && index < Document.GetPageCount()
                && index != _currentPageNumber)
            {
                _currentPageNumber = index;
                PDFPage page = Document.GetPage(index);
                _material.mainTexture = Document.Renderer.RenderPageToTexture(page, (int) page.GetPageSize().x * 2, 
                                                                                   (int) page.GetPageSize().y * 2);
                ResizeCanvas(page.GetPageSize());
                CheckPageButtons();
                SetText();
            }
        }

        #region Unity event functions

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
            
            if (NextPageButton != null)     NextPageButton.OnClick     += delegate { NextPage(); };
            if (PreviousPageButton != null) PreviousPageButton.OnClick += delegate { PreviousPage(); };
        }

        #endregion

        #region Private definitions

        private int _currentPageNumber = -1;
        private Material _material;

        private void CheckPageButtons()
        {
            if (PreviousPageButton != null) PreviousPageButton.SetEnabled(Document != null && _currentPageNumber > 0);
            if (NextPageButton != null) NextPageButton.SetEnabled(Document != null && _currentPageNumber < Document.GetPageCount() - 1);
        }

        private void SetText()
        {
            if (Document == null && Label != null)
            {
                Label.text = "Документ не открыт";
            } 
            else if (Document != null && Label != null)
            {
                Label.text = $"({_currentPageNumber + 1}/{Document.GetPageCount()}) {FileName}";
            }
        }

        #endregion
        
    }
}
