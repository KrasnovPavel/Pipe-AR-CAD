// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace HoloCore.UI
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary> Класс, реализующий трёхмерную кнопку. </summary>
    [ExecuteInEditMode]
    public class Button3D : MonoBehaviour, IMixedRealityInputHandler, IMixedRealityFocusHandler
    {
        /// <summary> Отображаемый на кнопке текст. </summary>
        [CanBeNull] public string Text;

        /// <summary> Отображаемая на кнопке иконка. </summary>
        [CanBeNull] public Texture Icon;

        /// <summary> Материал для иконки. </summary>
        [CanBeNull] public Material IconMaterial;

        /// <summary> Заполнитель на случай если иконка не задана. </summary>
        [CanBeNull] public Texture EmptyIcon; 
        
        /// <summary> Объект, на который выводится текст. </summary>
        [CanBeNull] protected TextMesh Label;

        /// <summary> Объект, на который выводится иконка. </summary>
        [CanBeNull] protected MeshRenderer IconRenderer;

        /// <summary> Объект, который отрисовывает кнопку. </summary>
        [CanBeNull] protected MeshRenderer ButtonRenderer;

        public delegate void OnHoverEnterHandler(object button, EventArgs args);

        public delegate void OnHoverExitHandler(object button, EventArgs args);

        public delegate void OnPressedHandler(object button, EventArgs args);

        public delegate void OnReleasedHandler(object button, EventArgs args);

        public delegate void OnClickHandler(object button, EventArgs args);

        /// <summary> Событие клика по кнопке. </summary>
        public OnClickHandler OnClick;

        /// <summary> Событие наведение курсора на кнопку. </summary>
        public OnHoverEnterHandler OnHoverEnter;

        /// <summary> Событие сведения курсора с кнопки. </summary>
        public OnHoverExitHandler OnHoverExit;

        /// <summary> Событие нажатия на кнопку. </summary>
        public OnPressedHandler OnPressed;

        /// <summary> Событие отпускания кнопки. </summary>
        public OnReleasedHandler OnReleased;

        /// <summary> Возможные состояния кнопки. </summary>
        public enum ButtonState
        {
            /// <summary> Кнопка выключена. </summary>
            Disabled = 20,

            /// <summary> Кнопка включена, состояние по умолчанию. </summary>
            Enabled = 5,

            /// <summary> Кнопка включена и на неё наведён взгляд. </summary>
            Hovered = 2,

            /// <summary> Кнопка включена и нажата. </summary>
            Pressed = 1
        }

        /// <summary> Текущее состояние кнопки </summary>
        public virtual ButtonState State { get; protected set; }

        /// <summary> Функция, включающая или выключающая кнопку. </summary>
        /// <param name="isEnabled"> Новое состояние кнопки. </param>
        public void SetEnabled(bool isEnabled)
        {
            if (isEnabled && State != ButtonState.Disabled) return;
            
            State = isEnabled ? ButtonState.Enabled : ButtonState.Disabled;
            _forceDisable = !isEnabled;
        }

        /// <summary> Виртуальное нажатие на кнопку. </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Press()
        {
            switch (State)
            {
                case ButtonState.Disabled:
                    break;
                case ButtonState.Enabled:
                case ButtonState.Hovered:
                    State = ButtonState.Pressed;
                    OnPressed?.Invoke(this, null);
                    break;
                case ButtonState.Pressed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Виртуальное отпускание кнопки. </summary>
        /// <param name="setHover"> Если true, то кнопка перейдет в состояние Hovered, иначе в Enabled. </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Release(bool setHover = true)
        {
            
            switch (State)
            {
                case ButtonState.Disabled:
                    break;
                case ButtonState.Enabled:
                    break;
                case ButtonState.Hovered:
                    break;
                case ButtonState.Pressed:
                    State = setHover ? ButtonState.Hovered : ButtonState.Enabled;
                    OnReleased?.Invoke(this, null);
                    OnClick?.Invoke(this, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region HoloToolKit event functions

        /// <summary> Обработчик нажатия на кнопку </summary>
        /// <param name="eventData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void OnInputDown(InputEventData eventData)
        {
            Press();
        }

        /// <summary> Обработчик отпускания кнопки. </summary>
        /// <param name="eventData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void OnInputUp(InputEventData eventData)
        {
            Release(true);
        }

        /// <summary> Обработчик наведения курсора на кнопку. </summary>
        /// <param name="eventData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void OnFocusEnter(FocusEventData eventData)
        {
            switch (State)
            {
                case ButtonState.Hovered:
                    break;
                case ButtonState.Pressed:
                    break;
                case ButtonState.Disabled:
                    break;
                case ButtonState.Enabled:
                    State = ButtonState.Hovered;
                    OnHoverEnter?.Invoke(this, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Обработчик сведения курсора с кнопки. </summary>
        /// <param name="eventData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void OnFocusExit(FocusEventData eventData)
        {
            switch (State)
            {
                case ButtonState.Hovered:
                    State = ButtonState.Enabled;
                    OnHoverExit?.Invoke(this, null);
                    break;
                case ButtonState.Pressed:
                    State = ButtonState.Enabled;
                    OnHoverExit?.Invoke(this, null);
                    OnReleased?.Invoke(this, null);
                    break;
                case ButtonState.Disabled:
                    break;
                case ButtonState.Enabled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Unity event functions

        /// <summary> Функция, инициализирующая объект в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        protected virtual void Start()
        {
            ButtonRenderer = GetComponent<MeshRenderer>();

            try
            {
                Label = transform.Find("Label").GetComponent<TextMesh>();
            }
            catch (UnassignedReferenceException){}
            catch (NullReferenceException){}

            try
            {
                IconRenderer = transform.Find("Icon").GetComponent<MeshRenderer>();
            }
            catch (UnassignedReferenceException){}
            catch (NullReferenceException){}

            State = _forceDisable ? ButtonState.Disabled : ButtonState.Enabled;
            SetVisual();
        }

        /// <summary> Функция, выполняющаяся в Unity каждый кадр. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Update()</c>.
        /// </remarks>
        protected virtual void Update()
        {
            SetVisual();
        }
        
        /// <summary> Функция, выполняющаяся при включении объекта в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnEnable()</c>.
        /// </remarks>
        protected virtual void OnEnable()
        {
            if (_forceDisable) return;
            State = ButtonState.Enabled;
        }

        /// <summary> Функция, выполняющаяся при выключении объекта в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnDisable()</c>.
        /// </remarks>
        protected virtual void OnDisable()
        {
            State = ButtonState.Disabled;
        }

        #endregion
        
        #region Private definitions

        /// <summary> <c>true</c>, если кнопка была выключена через <see cref="SetEnabled"/>. </summary>
        private bool _forceDisable;

        private Texture _iconTexture;

        private void SetVisual()
        {
            if (Label != null) Label.text = Text;
            if (IconRenderer != null)
            {
                Texture tmpTexture = Icon != null ? Icon : EmptyIcon;
                if (tmpTexture == _iconTexture) return;

                _iconTexture = tmpTexture;
                Material temp = new Material(IconMaterial)
                {
                    mainTexture = _iconTexture
                };
                IconRenderer.sharedMaterial = temp;
            }
        }

        #endregion
    }
}