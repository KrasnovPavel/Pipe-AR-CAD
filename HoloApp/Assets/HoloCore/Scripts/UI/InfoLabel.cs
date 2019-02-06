using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCore.UI
{
    /// <summary> Виджет всплывающих сообщений. </summary>
    public class InfoLabel : MonoBehaviour
    {
        // ReSharper disable once NotNullMemberIsNotInitialized
        [NotNull] private GameObject _background;
        // ReSharper disable once NotNullMemberIsNotInitialized
        [NotNull] private TextMesh _label;
        private readonly Vector3 _backgroundScale = new Vector3(200, 83, 100);
        private readonly Vector3 _backgroundPosition = new Vector3(-200, 83, 0);
        private float _animationState;
        private State _state = State.Closed;
        private float _showTime;
        private bool _isChanging;
        [CanBeNull] private string _changingText;

        /// <summary> Множитель скорости появления/исчезания сообщений. </summary>
        [Tooltip("Множитель скорости появления/исчезания сообщений.")]
        public float AnimationSpeed = 3f;

        private enum State
        {
            Closed,
            Opening,
            Opened,
            Closing
        }

        /// <summary> Функция, инициализирующая объект в Unity.  </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        protected virtual void Start()
        {
            _background = transform.Find("Background").gameObject;
            _label = _background.transform.Find("Label").GetComponent<TextMesh>();
            _background.transform.localScale = Vector3.zero;
        }

        /// <summary> Функция, выполняющаяся в Unity каждый кадр.  </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Update()</c>.
        /// </remarks>
        protected virtual void Update()
        {
            switch (_state)
            {
                case State.Closed:
                    break;
                case State.Opening:
                    _animationState = Mathf.Lerp(_animationState, 1, AnimationSpeed * Time.deltaTime);
                    _background.transform.localScale = _animationState * _backgroundScale;
                    _background.transform.localPosition = _animationState * _backgroundPosition;
                    if (_animationState >= 1)
                    {
                        _state = State.Opened;
                    }
                    break;
                case State.Opened:
                    _showTime -= Time.deltaTime;
                    if (_showTime <= 0)
                    {
                        _state = State.Closing;
                    }
                    break;
                case State.Closing:
                    _animationState = Mathf.Lerp(_animationState, 0, AnimationSpeed * Time.deltaTime);
                    _background.transform.localScale = _animationState * _backgroundScale;
                    _background.transform.localPosition = _animationState * _backgroundPosition;
                    if (_animationState > 0.01) break;

                    if (_isChanging)
                    {
                        _state = State.Opening;
                        _label.text = _changingText;
                    }
                    else
                    {
                        _state = State.Closed;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Показывает сообщение с текстом <paramref name="text"/> на время <paramref name="time"/> секунд.
        /// Если уже показывается какое-то сообщение, то оно сначала будет закрыто.
        /// </summary>
        /// <param name="text">Текст сообщения.</param>
        /// <param name="time">Время показа сообщения, если не указано,
        /// то сообщение показывается до прихода следующего.</param>
        public virtual void ShowMessage(string text, float time = float.MaxValue)
        {
            _showTime = time;
            if (_state == State.Opened || _state == State.Opening)
            {
                _isChanging = true;
                _changingText = text;
                _state = State.Closing;
            }
            else
            {
                _state = State.Opening;
                _label.text = text;
            }
        }

        /// <summary> Сворачивает виджет окна сообщений. </summary>
        public virtual void HideMessageWindow()
        {
            _state = State.Closing;
        }
    }
}