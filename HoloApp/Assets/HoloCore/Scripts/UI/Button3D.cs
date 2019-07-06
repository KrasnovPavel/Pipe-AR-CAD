// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using HoloToolkit.Unity.InputModule;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCore.UI
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary> Класс, реализующий трёхмерную кнопку. </summary>
    [ExecuteInEditMode]
    public class Button3D : MonoBehaviour, IInputHandler, IPointerSpecificFocusable
    {
        /// <summary> Объект, на который выводится текст. </summary>
        [CanBeNull] protected TextMesh Label;

        /// <summary> Объект, на который выводится иконка. </summary>
        [CanBeNull] protected MeshRenderer IconRenderer;

        /// <summary> Объект, который отрисовывает кнопку. </summary>
        [CanBeNull] protected MeshRenderer ButtonRenderer;

        public delegate void OnHoverEnterDel(Button3D button);

        public delegate void OnHoverExitDel(Button3D button);

        public delegate void OnPressedDel(Button3D button);

        public delegate void OnReleasedDel(Button3D button);

        public delegate void OnClickDel(Button3D button);

        /// <summary> Событие клика по кнопке. </summary>
        [NotNull] public OnClickDel OnClick = delegate { };

        /// <summary> Событие наведение курсора на кнопку. </summary>
        [NotNull] public OnHoverEnterDel OnHoverEnter = delegate { };

        /// <summary> Событие сведения курсора с кнопки. </summary>
        [NotNull] public OnHoverExitDel OnHoverExit = delegate { };

        /// <summary> Событие нажатия на кнопку. </summary>
        [NotNull] public OnPressedDel OnPressed = delegate { };

        /// <summary> Событие отпускания кнопки. </summary>
        [NotNull] public OnReleasedDel OnReleased = delegate { };

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

        /// <summary> Отображаемый на кнопке текст. </summary>
        [CanBeNull] public string Text;

        /// <summary> Отображаемая на кнопке иконка. </summary>
        [CanBeNull] public Material Icon;

        /// <summary> Материал-заполнитель для отсутствующей иконки. </summary>
        [CanBeNull] public Material EmptyIcon;

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
                    OnPressed(this);
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
                    OnReleased(this);
                    OnClick(this);
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
            switch (State)
            {
                case ButtonState.Hovered:
                    State = ButtonState.Pressed;
                    OnPressed(this);
                    break;
                case ButtonState.Pressed:
                    break;
                case ButtonState.Disabled:
                    break;
                case ButtonState.Enabled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Обработчик отпускания кнопки. </summary>
        /// <param name="eventData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void OnInputUp(InputEventData eventData)
        {
            switch (State)
            {
                case ButtonState.Hovered:
                    break;
                case ButtonState.Pressed:
                    State = ButtonState.Hovered;
                    OnReleased(this);
                    OnClick(this);
                    break;
                case ButtonState.Disabled:
                    break;
                case ButtonState.Enabled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Обработчик наведения курсора на кнопку. </summary>
        /// <param name="eventData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void OnFocusEnter(PointerSpecificEventData eventData)
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
                    OnHoverEnter(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Обработчик сведения курсора с кнопки. </summary>
        /// <param name="eventData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void OnFocusExit(PointerSpecificEventData eventData)
        {
            switch (State)
            {
                case ButtonState.Hovered:
                    State = ButtonState.Enabled;
                    OnHoverExit(this);
                    break;
                case ButtonState.Pressed:
                    State = ButtonState.Enabled;
                    OnHoverExit(this);
                    OnReleased(this);
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
#if UNITY_EDITOR
            SetVisual();
#endif
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

        private void SetVisual()
        {
            if (Label != null) Label.text = Text;
            if (IconRenderer != null) IconRenderer.sharedMaterial = Icon != null ? Icon : EmptyIcon;
        }

        #endregion
    }
}