using System;
using System.Collections.Generic;
using System.Linq;
using HoloCore;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс для работы с геймпадом. </summary>
    public sealed class GamepadController : Singleton<GamepadController>
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

        /// <summary> Значение до которого не считается отклонение по оси. </summary>
        [Tooltip("Значение до которого не считается отклонение по оси.")]
        public float AxisDead = 0.1f;

        /// <summary> Событие вызываемое при подключении/отключении геймпада. </summary>
        public static event Action<bool> GamepadConnectionChanged; 
        
        /// <summary> Подключен ли геймпад. </summary>
        public static bool IsGamepadConnected
        {
            get => _isGamepadConnected;
            set
            {
                if (_isGamepadConnected == value) return;

                _isGamepadConnected = value;
                GamepadConnectionChanged?.Invoke(_isGamepadConnected);
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
            JoystickBack,
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
            Axis a = new Axis{Main=button1, Mod=button2};
            if (!Instance._clickDelegates.ContainsKey(a))
            {
                Instance._clickCounter[a] = 0;
                Instance._clickDelegates[a] = action;
            }
            else
            {
                Instance._clickDelegates[a] += action;
            }
        }

        /// <summary> Отписка от события клика по клавише. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        public static void UnsubscribeFromClick(InputAxis button1, InputAxis? button2, Action action)
        {
            Axis a = new Axis{Main=button1, Mod=button2};
            if (!Instance._clickDelegates.ContainsKey(a)) return;
            
            Instance._clickDelegates[a] -= action;
            if (Instance._clickDelegates[a] == null)
            {
                Instance._clickDelegates.Remove(a);
                Instance._clickCounter.Remove(a);
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
            Axis a = new Axis{Main=button1, Mod=button2};
            if (!Instance._longPressDelegates.ContainsKey(a))
            {
                Instance._longPressCounter[a] = 0;
                var lpa = new LongPressActions {start = start, finish = finish, cancel = cancel};
                Instance._longPressDelegates[a] = lpa;
            }
            else
            {
                Instance._longPressDelegates[a].start  += start;
                Instance._longPressDelegates[a].finish += finish;
                Instance._longPressDelegates[a].cancel += cancel;
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
            Axis a = new Axis{Main=button1, Mod=button2};
            if (!Instance._longPressDelegates.ContainsKey(a)) return;
            
            Instance._longPressDelegates[a].start  -= start;
            Instance._longPressDelegates[a].finish -= finish;
            Instance._longPressDelegates[a].cancel -= cancel;
            if (Instance._longPressDelegates[a].start == null)
            {
                Instance._longPressDelegates.Remove(a);
                Instance._longPressCounter.Remove(a);
            }
        }

        /// <summary> Подписка на событие вызываемое периодически при нажатой клавише. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        public static void SubscribeToRepeatPressing(InputAxis button1, InputAxis? button2, Action action)
        {
            Axis a = new Axis{Main=button1, Mod=button2};
            if (!Instance._repeatPressDelegates.ContainsKey(a))
            {
                Instance._repeatPressCounter[a] = 0;
                Instance._repeatPressDelegates[a] = action;
            }
            else
            {
                Instance._repeatPressDelegates[a] += action;
            }
        }

        /// <summary> Отписка от события вызываемого периодически при нажатой клавише. </summary>
        /// <param name="button1"> Первая клавиша. </param>
        /// <param name="button2"> Вторая клавиша. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        public static void UnsubscribeFromRepeatPressing(InputAxis button1, InputAxis? button2, Action action)
        {
            Axis a = new Axis{Main=button1, Mod=button2};
            if (!Instance._repeatPressDelegates.ContainsKey(a)) return;
            
            Instance._repeatPressDelegates[a] -= action;
            if (Instance._repeatPressDelegates[a] == null)
            {
                Instance._repeatPressDelegates.Remove(a);
                Instance._repeatPressCounter.Remove(a);
            }
        }

        /// <summary> Подписка на событие перемещения вдоль оси. </summary>
        /// <param name="axis"> Ось. </param>
        /// <param name="button"> Клавиша-модификатор. Null если нужна только ось. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        /// <param name="factor"> Множитель. </param>
        public static void SubscribeToAxis(InputAxis axis, InputAxis? button, Action<float> action, float factor = 1)
        {
            Axis a = new Axis{Main=axis, Mod=button, Factor = factor};
            if (!Instance._axisDelegates.ContainsKey(a))
            {
                Instance._axisDelegates[a] = action;
            }
            else
            {
                Instance._axisDelegates[a] += action;
            }
        }
        
        /// <summary> Отписка от события перемещения вдоль оси. </summary>
        /// <param name="axis"> Ось. </param>
        /// <param name="button"> Клавиша-модификатор. Null если нужна только одна. </param>
        /// <param name="action"> Действие выполняемое при наступлении события. </param>
        /// <param name="factor"> Множитель. </param>
        public static void UnsubscribeFromAxis(InputAxis axis, InputAxis? button, Action<float> action, float factor = 1)
        {
            Axis a = new Axis{Main=axis, Mod=button, Factor = factor};
            if (!Instance._axisDelegates.ContainsKey(a)) return;
            
            Instance._axisDelegates[a] -= action;
            if (Instance._axisDelegates[a] == null)
            {
                Instance._axisDelegates.Remove(a);
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
            IsGamepadConnected = Input.GetJoystickNames().Contains("Xbox Controller 1");
            if (!IsGamepadConnected) return;
            
            CheckClick();
            CheckLongPress();
            CheckRepeatPressing();
            CheckAxis();
        }

        #endregion
        
        #region Private Definitions

        private static bool _isGamepadConnected;

        private class LongPressActions
        {
            public Action start;
            public Action finish;
            public Action cancel;
        }

        private struct Axis
        {
            public InputAxis Main;
            public InputAxis? Mod;
            public float Factor;
        }

        private readonly Dictionary<Axis, float> _clickCounter = new Dictionary<Axis, float>();
        private readonly Dictionary<Axis, Action> _clickDelegates = new Dictionary<Axis, Action>();
        private readonly Dictionary<Axis, float> _longPressCounter = new Dictionary<Axis, float>();
        private readonly Dictionary<Axis, LongPressActions> _longPressDelegates = new Dictionary<Axis, LongPressActions>();
        private readonly Dictionary<Axis, float> _repeatPressCounter = new Dictionary<Axis, float>();
        private readonly Dictionary<Axis, Action> _repeatPressDelegates = new Dictionary<Axis, Action>();
        private readonly Dictionary<Axis, Action<float>> _axisDelegates = new Dictionary<Axis, Action<float>>();

        /// <summary> Выполняет проверку клика по клавише. </summary>
        private void CheckClick()
        {
            foreach (var key in _clickCounter.Keys.ToArray())
            {
                if (!IsCombinationOverlaps(key.Main, key.Mod) && IsKeysPressed(key.Main, key.Mod))
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
                if (!IsCombinationOverlaps(key.Main, key.Mod) && IsKeysPressed(key.Main, key.Mod)) // Нужные клавиши нажаты
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
                if (!IsCombinationOverlaps(key.Main, key.Mod) && IsKeysPressed(key.Main, key.Mod))
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
                if (!IsCombinationOverlaps(key.Main, key.Mod) && IsAxisMoved(key.Main, key.Mod))
                {
                    _axisDelegates[key]?.Invoke(Input.GetAxis(key.Main.ToString()) * Time.fixedDeltaTime * key.Factor);
                }
            }
        }

        /// <summary> Проверяет перекрывает ли другая комбинация клавиш, нажатую. </summary>
        /// <remarks>
        /// Допустим есть комбинация LeftStick+RightTrigger,
        /// если она нажата, то события для LeftStick не должны приходить.
        /// </remarks>
        /// <param name="main"> Клавиша которую надо проверить. </param>
        /// <param name="mod"> Клавиша-модификатор. </param>
        private bool IsCombinationOverlaps(InputAxis main, InputAxis? mod)
        {
            if (mod != null) return false;
            
            bool result = false;
            Axis twoKeys = _clickCounter.Keys.ToArray().FirstOrDefault((a => (a.Main == main && a.Mod != null) 
                                                                        || (a.Mod != null && a.Mod == main)));
            result |= !twoKeys.Equals(default(Axis)) && IsKeysPressed(twoKeys.Main, twoKeys.Mod);
            twoKeys = _longPressCounter.Keys.ToArray().FirstOrDefault((a => (a.Main == main && a.Mod != null) 
                                                                             || (a.Mod != null && a.Mod == main)));
            result |= !twoKeys.Equals(default(Axis)) && IsKeysPressed(twoKeys.Main, twoKeys.Mod);
            twoKeys = _repeatPressCounter.Keys.ToArray().FirstOrDefault((a => (a.Main == main && a.Mod != null) 
                                                                               || (a.Mod != null && a.Mod == main)));
            result |= !twoKeys.Equals(default(Axis)) && IsKeysPressed(twoKeys.Main, twoKeys.Mod);
            twoKeys = _axisDelegates.Keys.ToArray().FirstOrDefault((a => (a.Main == main && a.Mod != null) 
                                                                          || (a.Mod != null && a.Mod == main)));
            result |= !twoKeys.Equals(default(Axis)) && IsAxisMoved(twoKeys.Main, twoKeys.Mod);
           
            return result;
        }

        /// <summary> Проверяет отклонён ли стик геймпада по заданной оси. </summary>
        /// <param name="axis"> Ось. </param>
        /// <param name="button"> Клавиша-модификатор. Null если нужна только ось. </param>
        private bool IsAxisMoved(InputAxis axis, InputAxis? button)
        {
            float axisValue = Input.GetAxis(axis.ToString());
            bool axisMoved = axisValue > AxisDead || axisValue < -AxisDead;
            bool buttonNeeded = button != null;
            bool buttonPressed = buttonNeeded && Input.GetAxis(button.ToString()) > PressThreshold;
            return axisMoved && (!buttonNeeded || buttonPressed);
        }
        
        #endregion
    }
}