using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using HoloCore;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс для работы с геймпадом. </summary>
    public sealed class GamepadController : Singleton<GamepadController>, INotifyPropertyChanged
    {
        /// <summary> Время через которое будет считаться, что нажатие -- долгое. </summary>
        [Tooltip("Время через которое будет считаться, что нажатие -- долгое.")]
        public float LongPressStartTime = 0.3f;

        /// <summary> Время через которое длинное нажатие завершится. </summary>
        [Tooltip("Время через которое длинное нажатие завершится.")]
        public float LongPressFinishTime = 1f;
        
        /// <summary> Значение при котором будет считаться, что кнопка нажата. </summary>
        [Tooltip("Значение при котором будет считаться, что кнопка нажата.")]
        public float PressThreshold = 0.3f;
        
        /// <summary> Подключен ли геймпад. </summary>
        public bool IsGamepadConnected
        {
            get => _isGamepadConnected;
            set
            {
                if (_isGamepadConnected == value) return;

                _isGamepadConnected = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Перечисление всех кнопок и осей геймпада. </summary>
        public enum InputAxis 
        {
            LeftStickHorizontal,
            LeftStickVertical,
            LeftStickRight,
            LeftStickLeft,
            LeftStickUp,
            LeftStickDown,
            RightStickHorizontal,
            RightStickVertical,
            RightStickRight,
            RightStickLeft,
            RightStickUp,
            RightStickDown,
            DPADHorizontal,
            DPADVertical,
            DPADRight,
            DPADLeft,
            DPADUp,
            DPADDown,
            RightTrigger,
            LeftTrigger,
            JoystickA,
            JoystickB,
            JoystickX,
            JoystickY,
            LeftBumper,
            RightBumper,
            JoystickStart,
            LeftStickButton,
            RightStickButton
        }

        /// <summary> Подписка на событие клика по клавише. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        public static void SubscribeToClick(InputAxis button1, InputAxis? button2, Action action)
        {
            if (!Instance._clickDelegates.ContainsKey((button1, button2)))
            {
                Instance._clickCounter[(button1, button2)] = 0;
                Instance._clickDelegates[(button1, button2)] = action;
            }
            else
            {
                Instance._clickDelegates[(button1, button2)] += action;
            }
        }

        /// <summary> Отписка от события клика по клавише. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        public static void UnsubscribeFromClick(InputAxis button1, InputAxis? button2, Action action)
        {
            if (!Instance._clickDelegates.ContainsKey((button1, button2))) return;
            
            Instance._clickDelegates[(button1, button2)] -= action;
            if (Instance._clickDelegates[(button1, button2)] == null)
            {
                Instance._clickDelegates.Remove((button1, button2));
                Instance._clickCounter.Remove((button1, button2));
            }
        }

        /// <summary> Подписка на событие долгого нажатия на клавишу. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        /// <param name="start"> Действие при начале долгого нажатия. </param>
        /// <param name="finish"> Действие при завершении долгого нажатия. </param>
        /// <param name="cancel"> Действие при отмене долгого нажатия. </param>
        public static void SubscribeToLongPress(InputAxis button1, InputAxis? button2, Action start, Action finish,
            Action cancel)
        {
            
            if (!Instance._longPressDelegates.ContainsKey((button1, button2)))
            {
                Instance._longPressCounter[(button1, button2)] = 0;
                var lpa = new LongPressActions {start = start, finish = finish, cancel = cancel};
                Instance._longPressDelegates[(button1, button2)] = lpa;
            }
            else
            {
                Instance._longPressDelegates[(button1, button2)].start  += start;
                Instance._longPressDelegates[(button1, button2)].finish += finish;
                Instance._longPressDelegates[(button1, button2)].cancel += cancel;
            }
        }

        /// <summary> Отписка от события долгого нажатия на клавишу. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        /// <param name="start"> Действие при начале долгого нажатия. </param>
        /// <param name="finish"> Действие при завершении долгого нажатия. </param>
        /// <param name="cancel"> Действие при отмене долгого нажатия. </param>
        public static void UnsubscribeFromLongPress(InputAxis button1, InputAxis? button2, Action start, Action finish,
            Action cancel)
        {
            if (!Instance._longPressDelegates.ContainsKey((button1, button2))) return;
            
            Instance._longPressDelegates[(button1, button2)].start  -= start;
            Instance._longPressDelegates[(button1, button2)].finish -= finish;
            Instance._longPressDelegates[(button1, button2)].cancel -= cancel;
            if (Instance._longPressDelegates[(button1, button2)].start == null)
            {
                Instance._longPressDelegates.Remove((button1, button2));
                Instance._longPressCounter.Remove((button1, button2));
            }
        }

        /// <summary> Подписка на событие вызываемое периодически при нажатой клавише. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        public static void SubscribeToRepeatPressing(InputAxis button1, InputAxis? button2, Action action)
        {
            if (!Instance._repeatPressDelegates.ContainsKey((button1, button2)))
            {
                Instance._repeatPressCounter[(button1, button2)] = 0;
                Instance._repeatPressDelegates[(button1, button2)] = action;
            }
            else
            {
                Instance._repeatPressDelegates[(button1, button2)] += action;
            }
        }

        /// <summary> Отписка от события вызываемого периодически при нажатой клавише. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        public static void UnsubscribeFromRepeatPressing(InputAxis button1, InputAxis? button2, Action action)
        {
            if (!Instance._repeatPressDelegates.ContainsKey((button1, button2))) return;
            
            Instance._repeatPressDelegates[(button1, button2)] -= action;
            if (Instance._repeatPressDelegates[(button1, button2)] == null)
            {
                Instance._repeatPressDelegates.Remove((button1, button2));
                Instance._repeatPressCounter.Remove((button1, button2));
            }
        }

        /// <summary> Подписка на событие перемещения вдоль оси. </summary>
        /// <param name="axis"> Ось. </param>
        /// <param name="button"> Клавиша-модификатор. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        public static void SubscribeToAxis(InputAxis axis, InputAxis? button, Action<float> action)
        {
            
            if (!Instance._axisDelegates.ContainsKey((axis, button)))
            {
                Instance._axisDelegates[(axis, button)] = action;
            }
            else
            {
                Instance._axisDelegates[(axis, button)] += action;
            }
        }
        
        /// <summary> Отписка от события перемещения вдоль оси. </summary>
        /// <param name="axis"> Ось. </param>
        /// <param name="button"> Клавиша-модификатор. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        public static void UnsubscribeFromAxis(InputAxis axis, InputAxis? button, Action<float> action)
        {
            
            if (!Instance._axisDelegates.ContainsKey((axis, button))) return;
            
            Instance._axisDelegates[(axis, button)] -= action;
            if (Instance._axisDelegates[(axis, button)] == null)
            {
                Instance._axisDelegates.Remove((axis, button));
            }
        }

        /// <summary> Проверяет нажаты ли кнопки. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        public bool IsKeysPressed(InputAxis button1, InputAxis? button2 = null)
        {
            bool button1Pressed = Input.GetAxis(button1.ToString()) > PressThreshold;
            bool button2Needed = button2 != null;
            bool button2Pressed = button2Needed && Input.GetAxis(button2.ToString()) > PressThreshold;
            return button1Pressed && (!button2Needed || button2Pressed);
        }

        #region Unity event functions

        /// <summary> Функция, вызываемая в Unity через строго определённый промежуток времени. </summary>
        private void FixedUpdate()
        {
            if (!IsGamepadConnected) return;
            
            CheckClick();
            CheckLongPress();
            CheckRepeatPressing();
            CheckAxis();
        }

        #endregion

        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private Definitions

        [SerializeField]
        [Tooltip("Подключен ли геймпад.")]
        private bool _isGamepadConnected;

        private class LongPressActions
        {
            public Action start;
            public Action finish;
            public Action cancel;
        }

        private readonly Dictionary<(InputAxis, InputAxis?), float> _clickCounter = new Dictionary<(InputAxis, InputAxis?), float>();
        private readonly Dictionary<(InputAxis, InputAxis?), Action> _clickDelegates = new Dictionary<(InputAxis, InputAxis?), Action>();
        private readonly Dictionary<(InputAxis, InputAxis?), float> _longPressCounter = new Dictionary<(InputAxis, InputAxis?), float>();
        private readonly Dictionary<(InputAxis, InputAxis?), LongPressActions> _longPressDelegates = new Dictionary<(InputAxis, InputAxis?), LongPressActions>();
        private readonly Dictionary<(InputAxis, InputAxis?), float> _repeatPressCounter = new Dictionary<(InputAxis, InputAxis?), float>();
        private readonly Dictionary<(InputAxis, InputAxis?), Action> _repeatPressDelegates = new Dictionary<(InputAxis, InputAxis?), Action>();
        private readonly Dictionary<(InputAxis, InputAxis?), Action<float>> _axisDelegates = new Dictionary<(InputAxis, InputAxis?), Action<float>>();

        /// <summary> Выполняет проверку клика по клавише. </summary>
        private void CheckClick() 
        {
            foreach (var key in _clickCounter.Keys.ToArray())
            {
                if (IsKeysPressed(key.Item1, key.Item2))
                {
                    _clickCounter[key] += Time.fixedDeltaTime;
                }
                else
                {
                    if (_clickCounter[key] > 0 && _clickCounter[key] < LongPressStartTime)
                    {
                        _clickDelegates[key]?.Invoke();
                    }
                    _clickCounter[key] = 0;
                }
            }
        }

        /// <summary> Выполняет проверку длинного нажатия на клавишу. </summary>
        private void CheckLongPress() 
        {
            foreach (var key in _longPressCounter.Keys.ToArray())
            {
                if (IsKeysPressed(key.Item1, key.Item2)) // Нужные клавиши нажаты
                {
                    _longPressCounter[key] += Time.fixedDeltaTime;

                    if (_longPressCounter[key] > LongPressStartTime 
                        && _longPressCounter[key] < LongPressStartTime + Time.fixedDeltaTime)
                    {
                        _longPressDelegates[key].start?.Invoke();
                    }
                    else if (_longPressCounter[key] > LongPressFinishTime
                             && _longPressCounter[key] < LongPressFinishTime + Time.fixedDeltaTime)
                    {
                        _longPressDelegates[key].finish?.Invoke();
                    }
                }
                else
                {
                    if (_longPressCounter[key] > LongPressStartTime && _longPressCounter[key] < LongPressFinishTime)
                    {
                        _longPressDelegates[key]?.cancel?.Invoke();
                    }
                    _longPressCounter[key] = 0;
                }
            }
        }

        /// <summary> Выполняет проверку нажатия с периодическим вызовом события. </summary>
        private void CheckRepeatPressing() 
        {
            foreach (var key in _repeatPressCounter.Keys.ToArray())
            {
                if (IsKeysPressed(key.Item1, key.Item2))
                {
                    _repeatPressCounter[key] += Time.fixedDeltaTime;
                    if (_repeatPressCounter[key] > LongPressStartTime)
                    {
                        _repeatPressDelegates[key]?.Invoke();
                        _repeatPressCounter[key] = 0;
                    }
                }
                else
                {
                    _repeatPressCounter[key] = 0;
                }
            }
        }

        /// <summary> Выполняет проверку отклонения осей. </summary>
        private void CheckAxis() 
        {
            foreach (var key in _axisDelegates.Keys.ToArray())
            {
                float axisValue = Input.GetAxis(key.Item1.ToString());
                bool axisMoved = axisValue > float.Epsilon || axisValue < -float.Epsilon;
                bool buttonNeeded = key.Item2 != null;
                bool buttonPressed = buttonNeeded && Input.GetAxis(key.Item2.ToString()) > PressThreshold;
                if (axisMoved && (!buttonNeeded || buttonPressed))
                {
                    _axisDelegates[key]?.Invoke(axisValue * Time.fixedDeltaTime);
                }
            }
        }
        
        #endregion
    }
}