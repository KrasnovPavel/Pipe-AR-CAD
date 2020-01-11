// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.IO;
using HoloCore.UI;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace HoloCAD.UI.Docs2D
{
    /// <summary> Класс для отображения 2D документов. </summary>
    public class Viewer2D : MonoBehaviour
    {
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
        
        /// <summary> Имя открытого файла. </summary>
        public string FileName { get; protected set; }

        /// <summary> Свернуто ли окно? </summary>
        public virtual bool IsHided
        {
            get => _isHided;
            protected set
            {
                _isHided = value;
                Canvas.SetActive(!IsHided);
                GetComponent<BoxCollider>().enabled = !IsHided;
                if (HideButton != null) HideButton.Text = IsHided ? "Развернуть" : "Свернуть";
            }
        }

        /// <summary> Свернуть окно. </summary>
        public virtual void Hide()
        {
            IsHided = true;
        }

        /// <summary> Развернуть окно. </summary>
        public virtual void Show()
        {
            IsHided = false;
        }

        /// <summary> Меняет состояние свёрнутости окна. </summary>
        public virtual void ToggleHiding()
        {
            IsHided = !IsHided;
        }

        /// <summary> Закрывает окно. </summary>
        public void Close()
        {
            Destroy(gameObject);
        }
        
        /// <summary> Инициализирует обозреватель новыми значениями. </summary>
        /// <param name="byteArray"></param>
        /// <param name="path"></param>
        public virtual void Init(byte[] byteArray, string path)
        {
            FileName = Path.GetFileName(path);
            SetText();
        }
        
        /// <summary> Устанавливает пропорции холста в зависимости от размера страницы. </summary>
        protected void ResizeCanvas(Vector2 size)
        {
            if (size.x > size.y)
            {
                Canvas.transform.localScale = new Vector3(1, size.y / size.x, 1);
            }
            else
            {
                Canvas.transform.localScale = new Vector3(size.x / size.y, 1, 1);
            }
        }
        
        /// <summary> Устанавливает заголовок окна. </summary>
        protected virtual void SetText()
        {
            if (Label != null) Label.text = $"{FileName}";
        }

        #region Unity event functions

        /// <summary> Функция, выполняющаяся после инициализизации обозревателя в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Awake()</c>.
        /// </remarks>
        protected virtual void Awake()
        {
            if (CloseButton != null) CloseButton.OnClick += delegate { Close(); };
            if (HideButton != null)  HideButton.OnClick  += delegate { ToggleHiding(); };
        }
        
        /// <summary> Функция, вызывающаяся при уничтожении объекта в Unity. </summary>
        protected virtual void OnDestroy()
        {
            Controller2D.ViewerDestroyed(this);
        }

        #endregion

        #region Private definitions

        private bool _isHided;

        #endregion
    }
}
