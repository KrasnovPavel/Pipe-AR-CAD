// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Text;
using HoloCore.UI;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace HoloCAD.UI.Docs2D
{
    /// <summary> Обозреватель текстовых файлов. </summary>
    public class ViewerText : Viewer2D
    {
        /// <summary> Кнопка перехода на следующую страницу. </summary>
        [Tooltip("Кнопка перехода на следующую страницу.")]
        [CanBeNull] public Button3D NextPageButton;
        
        /// <summary> Кнопка перехода на предыдущую страницу. </summary>
        [Tooltip("Кнопка перехода на предыдущую страницу.")]
        [CanBeNull] public Button3D PreviousPageButton;
        
        /// <inheritdoc />
        public override bool IsHided
        {
            get => base.IsHided;
            protected set
            {
                base.IsHided = value;
                if (PreviousPageButton != null) PreviousPageButton.gameObject.SetActive(!IsHided);
                if (NextPageButton != null) NextPageButton.gameObject.SetActive(!IsHided);
            } 
        }
        
        /// <inheritdoc />
        public override void Init(byte[] byteArray, string path)
        {
            base.Init(byteArray, path);
            _textCanvas.text = Encoding.UTF8.GetString(byteArray);
            GoToPage(1);
        }
        
        /// <summary> Переходит к следующей странице документа, если возможно. </summary>
        public void NextPage()
        {
            _textCanvas.pageToDisplay++;
            SetText();
            CheckPageButtons();
        }

        /// <summary> Переходит к следующей странице документа, если возможно. </summary>
        public void PreviousPage()
        {
            if (_textCanvas.pageToDisplay > 1)
            {
                _textCanvas.pageToDisplay--;
                SetText();
                CheckPageButtons();
            }
        }
        
        /// <summary> Переходит к указанному номеру страницы, если возможно. </summary>
        /// <param name="index"> Номер страницы к которой нужно перейти. </param>
        public void GoToPage(int index)
        {
            if (index >= 1)
            {
                _textCanvas.pageToDisplay = index;
                SetText();
                CheckPageButtons();
            }
        }

        /// <inheritdoc />
        protected override void SetText()
        {
            if (Label == null) return;
            
            if (string.IsNullOrWhiteSpace(_textCanvas.text))
            {
                Label.text = "Документ не открыт";
            } 
            else
            {
                Label.text = $"{FileName} Страница №{_textCanvas.pageToDisplay}";
            }
        }

        #region Unity event functions

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            
            _textCanvas = Canvas.GetComponent<TextMeshPro>();
            if (NextPageButton != null)     NextPageButton.OnClick     += delegate { NextPage(); };
            if (PreviousPageButton != null) PreviousPageButton.OnClick += delegate { PreviousPage(); };
        }

        #endregion

        #region Private defnitions

        private TextMeshPro _textCanvas;
        
        private void CheckPageButtons()
        {
            if (PreviousPageButton != null) PreviousPageButton.SetEnabled(_textCanvas.pageToDisplay > 1);
        }

        #endregion
    }
}
