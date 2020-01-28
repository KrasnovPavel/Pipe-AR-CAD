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

        /// <summary> Вектор с углами вращдения вокруг осей метки </summary>
        private Vector3 MarkRotation;
    
        /// <summary>Задает метке параметры из панели </summary>
        public void SetParamsToMarkFromInputs()
        {
            if (Mark == null) return;
            Mark.transform.position = new Vector3(float.Parse(InputX.text,ci), float.Parse(InputY.text,ci),
                float.Parse(InputZ.text,ci));
            MarkRotation = new Vector3(float.Parse(InputRotationX.text,ci), 
                float.Parse(InputRotationY.text,ci),float.Parse(InputRotationZ.text,ci));
            
            Mark.transform.rotation = Quaternion.Euler(MarkRotation);
        }

        /// <summary>Вносит в панель параметры метки</summary>
        public void MarkTransformIntoInput()
        {
            Transform markTransform = Mark.transform;
            Vector3 markPosition = markTransform.position;
//            if (!(Math.Abs(Quaternion.Euler(MarkRotation).x - markTransform.rotation.x) < Quaternion.kEpsilon &&
//                  Math.Abs(Quaternion.Euler(MarkRotation).y - markTransform.rotation.y) < Quaternion.kEpsilon &&
//                  Math.Abs(Quaternion.Euler(MarkRotation).z - markTransform.rotation.z) < Quaternion.kEpsilon &&
//                  Math.Abs(Quaternion.Euler(MarkRotation).w - markTransform.rotation.w) < Quaternion.kEpsilon))
//            {
//                Debug.Log($"{Quaternion.Euler(MarkRotation).x},{Quaternion.Euler(MarkRotation).y},{Quaternion.Euler(MarkRotation).z},{Quaternion.Euler(MarkRotation).w} !!!!! {markTransform.rotation.x},{markTransform.rotation.y},{markTransform.rotation.z},{markTransform.rotation.w}");
//                MarkRotation = markTransform.eulerAngles;
//            }
            MarkRotation = markTransform.eulerAngles;
            InputX.text = Convert.ToString(markPosition.x, ci);
            InputY.text = Convert.ToString(markPosition.y, ci);
            InputZ.text = Convert.ToString(markPosition.z, ci);
            InputRotationX.text = Convert.ToString(MarkRotation.x, ci);
            InputRotationY.text = Convert.ToString(MarkRotation.y, ci);
            InputRotationZ.text = Convert.ToString(MarkRotation.z, ci);
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
