using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace MarksEditor
{
    /// <summary> Класс, занимающийся парсингом и проверкой вводимых данных в строку ввода </summary>
    public class InputFromPanelValidator : MonoBehaviour
    {
        /// <summary> Значение из поля ввода</summary>
        public float Value
        {
            get
            {
                CheckInputField();
                float output;
                float.TryParse(_inputField.text,NumberStyles.Any, ci, out output);
                return output;
            }
            set => _inputField.text = Convert.ToString(value, ci);
        }

        #region Private defenition

        /// <summary> Поля ввода </summary>
        private InputField _inputField;

        private static CultureInfo ci = new CultureInfo("en-US");


        /// <summary>Проверяет вводимые данные </summary>
        private void CheckInputField()
        {
            if (_inputField.text == "" || _inputField.text == ".") _inputField.text = "0";
        }

        #endregion


        #region Unity events

        private void Awake()
        {
            _inputField = GetComponent<InputField>();
            _inputField.text = "0";
        }

        public void OnEndEdit()
        {
            CheckInputField();
        }

        #endregion
    }
}