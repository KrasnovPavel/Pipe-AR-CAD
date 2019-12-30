using System.Linq;
using HoloCore.UI;
using JetBrains.Annotations;
using Paroxe.PdfRenderer;
using SFB;
using TMPro;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
    using System;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.Storage.Streams;
#endif

namespace Docs2D
{
    /// <summary> Класс для отображения 2D документов. </summary>
    public class Viewer2D : MonoBehaviour
    {
        /// <summary> Кнопка перехода на следующую страницу. </summary>
        [Tooltip("Кнопка перехода на следующую страницу.")]
        [CanBeNull] public Button3D NextPageButton;
        
        /// <summary> Кнопка перехода на предыдущую страницу. </summary>
        [Tooltip("Кнопка перехода на предыдущую страницу.")]
        [CanBeNull] public Button3D PreviousPageButton;
        
        /// <summary> Кнопка открытия документа. </summary>
        [Tooltip("Кнопка открытия документа.")]
        [CanBeNull] public Button3D OpenButton;
        
        /// <summary> Кнопка закрытия. </summary>
        [Tooltip("Кнопка закрытия.")]
        [CanBeNull] public Button3D CloseButton;

        /// <summary> Кнопка сворачивания. </summary>
        [Tooltip("Кнопка сворачивания.")]
        [CanBeNull] public Button3D HideButton;
        
        /// <summary> Заглавие. </summary>
        [Tooltip("Заглавие.")]
        [CanBeNull] public TextMeshPro Label;

        /// <summary> Холст. </summary>
        [Tooltip("Холст.")]
        [NotNull] public GameObject Canvas;

        /// <summary> Шейдер для отрисовки документа. </summary>
        [Tooltip("Шейдер для отрисовки документа.")]
        [CanBeNull] public Shader CanvasShader;
        
        /// <summary> Открытый документ. </summary>
        [CanBeNull] public PDFDocument Document { get; protected set; }
        
        /// <summary> Имя открытого файла. </summary>
        public string FileName { get; protected set; }

        /// <summary> Свернуто ли окно? </summary>
        public bool IsHided { get; protected set; }

        /// <summary> Вызывает диалог открытия файла и читает выбранный файл. </summary>
        public void OpenFile()
        {
#if ENABLE_WINMD_SUPPORT
            UnityEngine.WSA.Application.InvokeOnUIThread(() => ReadFileOnHololens(), true);
#else
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                Cursor.visible = true;
                Document = new PDFDocument(ReadFileOnPC());
                _currentPageNumber = -1;
                GoToPage(0);
            }, true);
#endif
        }

        /// <summary> Сворачивает окно. </summary>
        public void Hide()
        {
            IsHided = !IsHided;
            Canvas.SetActive(!IsHided);
            GetComponent<BoxCollider>().enabled = !IsHided;
            if (PreviousPageButton != null) PreviousPageButton.gameObject.SetActive(!IsHided);
            if (NextPageButton != null) NextPageButton.gameObject.SetActive(!IsHided);
            if (OpenButton != null) OpenButton.gameObject.SetActive(!IsHided);
            if (HideButton != null) HideButton.Text = IsHided ? "Развернуть" : "Свернуть";
            if (Label != null) Label.transform.localPosition = IsHided ? new Vector3(0.065f, 0.64f, 0) 
                                                                       : new Vector3(-0.075f, 0.64f, 0);
        }

        /// <summary> Закрывает окно. </summary>
        public void Close()
        {
            Destroy(gameObject);
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
                SetCanvasSize();
                CheckPageButtons();
                SetText();
            }
        }

        #region Unity event functions

        private void Start()
        {
            if (CanvasShader == null)
            {
                CanvasShader = Shader.Find("Mixed Reality Toolkit/Standard");
            }
            Canvas.GetComponent<MeshRenderer>().sharedMaterial = new Material(CanvasShader);
            _material = Canvas.GetComponent<MeshRenderer>().sharedMaterial;

            if (OpenButton != null)         OpenButton.OnClick         += delegate { OpenFile(); };
            if (CloseButton != null)        CloseButton.OnClick        += delegate { Close(); };
            if (HideButton != null)         HideButton.OnClick         += delegate { Hide(); };
            if (NextPageButton != null)     NextPageButton.OnClick     += delegate { NextPage(); };
            if (PreviousPageButton != null) PreviousPageButton.OnClick += delegate { PreviousPage(); };
            
            OpenFile();
        }

        #endregion
        
        #region Private definitions

        /// <summary> Устанавливает пропорции холста в зависимости от размера страницы. </summary>
        private void SetCanvasSize()
        {
            if (Document == null) return;
            
            Vector2 size = Document.GetPageSize(_currentPageNumber);
            if (size.x > size.y)
            {
                Canvas.transform.localScale = new Vector3(1, size.y / size.x, 1);
            }
            else
            {
                Canvas.transform.localScale = new Vector3(size.x / size.y, 1, 1);
            }
        }

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
            } else if (Document != null && Label != null)
            {
                Label.text = $"({_currentPageNumber + 1}/{Document.GetPageCount()}) {FileName}";
            }
        }

        private Material _material;
        private int _currentPageNumber = -1;
        
#if ENABLE_WINMD_SUPPORT
        /// <summary> Чтение файла на очках Hololens. Перед чтением вызывает диалог открытия файла. </summary>
        private async void ReadFileOnHololens()
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.FileTypeFilter.Add(".pdf");
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file == null) return;
                var b = await FileIO.ReadBufferAsync(file);
                FileName = file.Name;
                DataReader dataReader = DataReader.FromBuffer(b);
                byte[] bytes = new byte[b.Length];
                dataReader.ReadBytes(bytes);
                UnityEngine.WSA.Application.InvokeOnAppThread(() => {
                    _currentPageNumber = -1;
                    Document = new PDFDocument(bytes);
                    GoToPage(0);
                }, true);
        }    
#endif
    
        /// <summary> Чтение файла на PC. Перед чтением вызывает диалог открытия файла. </summary>
        private string ReadFileOnPC()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Open scheme", "", "pdf", false);
            FileName = paths[0].Split('/').Last();
            return paths.Length == 0 ? null : paths[0];
        }

        #endregion
    }
}