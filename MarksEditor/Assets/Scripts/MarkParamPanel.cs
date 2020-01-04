using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace MarksEditor
{
    
    /// <summary> Класс, содержащий параметры меток </summary>
    public class MarkParamPanel : MonoBehaviour
    {  
        CultureInfo ci = new CultureInfo("en-US");
        
        /// <summary> Метка, к которой привязана модель </summary>
        public MarkOnScene Mark;
        
        
        /// <summary> Поле ввода смещения по X</summary>
        public InputField InputX;
        
        /// <summary> Поле ввода смещения по Y</summary>
        public InputField InputY;
        
        /// <summary> Поле ввода смещения по Z</summary>
        public InputField InputZ;
        
        /// <summary> Поле ввода вращения относительно оси X</summary>
        public InputField InputRotationX;
        
        /// <summary> Поле ввода вращения относительно оси Y</summary>
        public InputField InputRotationY;
        
        /// <summary> Поле ввода вращения относительно оси Z</summary>
        public InputField InputRotationZ;
        
        /// <summary> Текстовое поле с Id панели</summary>
        public Text IdText;
    
        /// <summary>Задает метке параметры из панели </summary>
        public void SetParamsToMarkFromInputs()
        {
            if (Mark == null) return;
            Mark.transform.position = new Vector3(float.Parse(InputX.text,ci), float.Parse(InputY.text,ci),
                float.Parse(InputZ.text,ci));
            Mark.transform.rotation = Quaternion.Euler(float.Parse(InputRotationX.text,ci), 
                float.Parse(InputRotationY.text,ci),float.Parse(InputRotationZ.text,ci));
        }

        /// <summary>Вносит в панель параметры метки</summary>
        public void MarkTransformIntoInput()
        {
            Transform markTransform = Mark.transform;
            Vector3 markPosition = markTransform.position;
            Vector3 markRotation = markTransform.eulerAngles;
            InputX.text = Convert.ToString(markPosition.x, ci);
            InputY.text = Convert.ToString(markPosition.y, ci);
            InputZ.text = Convert.ToString(markPosition.z, ci);
            InputRotationX.text = Convert.ToString(markRotation.x, ci);
            InputRotationY.text = Convert.ToString(markRotation.y, ci);
            InputRotationZ.text = Convert.ToString(markRotation.z, ci);
        }
    
        private void Awake()
        {
            InputX.text = "0";
            InputY.text = "0";
            InputZ.text = "0";
            InputRotationX.text = "0";
            InputRotationY.text = "0";
            InputRotationZ.text = "0";
        }
    
        /// <summary> Удаляет привязанную метку </summary>
        public void DeleteMark()
        {
            MarksController.Instance.DeleteMark(Mark.Id);
        }

        /// <summary> Выбирает привязанную метку </summary>
        public void SelectMark()
        {
            MarksController.Instance.SelectMark(Mark.Id);
        }
    }
}
