﻿using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <inheritdoc/>
    /// <summary>
    /// Класс, реализующий трёхмерную кнопку.
    /// </summary>
    public class Button3D : MonoBehaviour, IInputHandler, IPointerSpecificFocusable
    {
        protected TextMesh _label;
//        protected MeshRenderer _iconRenderer;
        protected ButtonState _state;

        public delegate void OnHoverEnterDel(Button3D button);

        public delegate void OnHoverExitDel(Button3D button);

        public delegate void OnPressedDel(Button3D button);

        public delegate void OnReleasedDel(Button3D button);

        public delegate void OnClickDel(Button3D button);

        /// <summary> Событие клика по кнопке. </summary>
        public OnClickDel OnClick = delegate { };
        /// <summary> Событие наведение курсора на кнопку. </summary>
        public OnHoverEnterDel OnHoverEnter = delegate { };
        /// <summary> Событие сведения курсора с кнопки. </summary>
        public OnHoverExitDel OnHoverExit = delegate { };
        /// <summary> Событие нажатия на кнопку. </summary>
        public OnPressedDel OnPressed = delegate { };
        /// <summary> Событие отпускания кнопки. </summary>
        public OnReleasedDel OnReleased = delegate { };

        /// <summary>
        /// Возможные состояния кнопки.
        /// </summary>
        public enum ButtonState
        {
            /// <summary> Кнопка выключена. </summary>
            Disabled = 10, 
            /// <summary> Кнопка включена, состояние по умолчанию. </summary>
            Enabled = 5, 
            /// <summary> Кнопка включена и на неё наведён взгляд. </summary>
            Hovered = 2, 
            /// <summary> Кнопка включена и нажата. </summary>
            Pressed = 1
        }

        /// <summary> Текущее состояние кнопки </summary>
        public virtual ButtonState State { get; protected set; }

        public string Text;

//        public Texture2D Icon;

        protected virtual void Start()
        {
            State = ButtonState.Enabled;
            _label = transform.Find("Label").GetComponent<TextMesh>();
//            _iconRenderer = transform.Find("Icon").GetComponent<MeshRenderer>();
        }

        protected virtual void Update()
        {
            _label.text = Text;
//            _iconRenderer.material.mainTexture = Icon;
        }

        public void SetEnabled(bool isEnabled)
        {
            State = isEnabled ? ButtonState.Enabled : ButtonState.Disabled;
        }
        
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
    }
}
