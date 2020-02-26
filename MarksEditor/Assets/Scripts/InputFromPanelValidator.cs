// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace GLTFConverter
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
                float.TryParse(_inputField.text,NumberStyles.Any, Ci, out float output);
                return output;
            }
            set => _inputField.text = Convert.ToString(value, Ci);
        }

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
        
        #region Private defenition

        /// <summary> Поля ввода </summary>
        private InputField _inputField;

        private static readonly CultureInfo Ci = new CultureInfo("en-US");

        /// <summary>Проверяет вводимые данные </summary>
        private void CheckInputField()
        {
            if (_inputField.text == "" || _inputField.text == ".") _inputField.text = "0";
        }

        #endregion
    }
}