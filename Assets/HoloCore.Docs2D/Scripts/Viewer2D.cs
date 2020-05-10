// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.IO;
using HoloCore.UI;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TMPro; 
using UnityEngine;

namespace HoloCore.Docs2D
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

        /// <summary> Кнопка для возвращения в вертикальное положение. </summary>
        [Tooltip("Кнопка для возвращения в вертикальное положение.")]
        [CanBeNull] public Button3D VerticalButton;

        /// <summary> Кнопка для фиксации документа перед лицом пользователя. </summary>
        [Tooltip("Кнопка для фиксации документа перед лицом пользователя.")]
        [CanBeNull] public Button3D LockButton;
        
        /// <summary> Заглавие. </summary>
        [Tooltip("Заглавие.")]
        [CanBeNull] public TextMeshPro Label;

        /// <summary> Холст. </summary>
        [Tooltip("Холст.")]
        // ReSharper disable once NotNullMemberIsNotInitialized
        // Если холста нет, то всё совсем плохо и можно падать
        [NotNull] public GameObject Canvas;

        /// <summary> Масштаб холста в закреплённом состоянии. </summary>
        [Tooltip("Масштаб холста в закреплённом состоянии.")]
        public float LockScale = 0.3f;
        
        /// <summary> MRTK-cкрипт, управляющий фиксацией документа перед лицом пользователя. </summary>
        [CanBeNull] protected Orbital OrbitalSolver;

        /// <summary> MRTK-скрипт, определяющий клик по объекту. </summary>
        [CanBeNull] protected Interactable ClickHandler;
        
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
                if (VerticalButton != null) VerticalButton.gameObject.SetActive(!IsHided);
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
        public virtual void Close()
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
            OrbitalSolver = GetComponent<Orbital>();
            ClickHandler = GetComponent<Interactable>();
            
            if (CloseButton != null)    CloseButton.OnClick    += delegate { Close(); };
            if (HideButton != null)     HideButton.OnClick     += delegate { ToggleHiding(); };
            if (VerticalButton != null) VerticalButton.OnClick += delegate { ReturnToVertical(); };
            if (LockButton != null)     LockButton.OnClick += delegate
            {
                transform.localScale = Vector3.one * LockScale;
                if (OrbitalSolver != null) OrbitalSolver.enabled = true;
                if (ClickHandler != null)  ClickHandler.enabled = true;
            };
            if (OrbitalSolver != null) OrbitalSolver.enabled = false;
            if (ClickHandler != null)  ClickHandler.enabled = false;
        }
        
        /// <summary> Функция, вызывающаяся при уничтожении объекта в Unity. </summary>
        protected virtual void OnDestroy()
        {
            Controller2D.ViewerDestroyed(this);
        }

        #endregion

        #region Private definitions

        private bool _isHided;

        /// <summary> Возвращает обозреватель в вертикальное положение. </summary>
        private void ReturnToVertical()
        {
            Vector3 angles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(0, angles.y, 0));
        }

        #endregion
    }
}
