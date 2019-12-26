using UnityEngine;
using UnityEngine.UI;

namespace MarksEditor
{
    public class InputFromPanel : MonoBehaviour
    {
        public MarkOnScene Mark { get; set; }
        public MarkParamPanel MarkParamPanel { get; set; }
    
        
        
        /// <summary> Проверяет вводимые данные </summary>
        public void OnEndEdit()
        {
            InputField inputField = GetComponent<InputField>();
            if (inputField.text == "" || inputField.text == ".") inputField.text = "0";
        }
    
        /// <summary> Меняет параметры метки, к которой привязана панель </summary>
        private void ChangeMarkParams()
        {
            MarkParamPanel.SetParamsToMarkFromInputs();
        }

    }
}
