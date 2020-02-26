// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityEngine;
using UnityEngine.UI;

namespace GLTFConverter
{
    /// <summary> Класс, содержащий параметры меток. </summary>
    public class MarkParamPanel : MonoBehaviour
    {
        /// <summary> Метка, к которой привязана модель. </summary>
        [Tooltip("Метка, к которой привязана модель.")] 
        public Mark Mark;

        /// <summary> Поле ввода смещения по X. </summary>
        [Tooltip("Поле ввода смещения по X.")] 
        public InputFromPanelValidator InputX;

        /// <summary> Поле ввода смещения по Y. </summary>
        [Tooltip("оле ввода смещения по Y.")] 
        public InputFromPanelValidator InputY;

        /// <summary> Поле ввода смещения по Z. </summary>
        [Tooltip("Поле ввода смещения по Z.")] 
        public InputFromPanelValidator InputZ;

        /// <summary> Поле ввода вращения относительно оси X. </summary>
        [Tooltip("Поле ввода вращения относительно оси X.")] 
        public InputFromPanelValidator InputRotationX;

        /// <summary> Поле ввода вращения относительно оси Y. </summary>
        [Tooltip("Поле ввода вращения относительно оси Y.")] 
        public InputFromPanelValidator InputRotationY;

        /// <summary> Поле ввода вращения относительно оси Z. </summary>
        [Tooltip("Поле ввода вращения относительно оси Z.")] 
        public InputFromPanelValidator InputRotationZ;

        /// <summary> Текстовое поле с Id панели. </summary>
        [Tooltip("Текстовое поле с Id панели.")] 
        public Text IdText;

        /// <summary> Задает метке параметры из панели. </summary>
        public void SetParamsToMarkFromInputs()
        {
            if (Mark == null) return;
            Mark.transform.position = new Vector3(InputX.Value, InputY.Value, InputZ.Value);
            _markRotation = new Vector3(InputRotationX.Value, InputRotationY.Value, InputRotationZ.Value);

            Mark.transform.rotation = Quaternion.Euler(_markRotation);
        }

        /// <summary> Вносит в панель параметры метки. </summary>
        public void MarkTransformIntoInput()
        {
            Transform markTransform = Mark.transform;
            Vector3 markPosition = markTransform.position;
            _markRotation = markTransform.eulerAngles;
            InputX.Value = markPosition.x;
            InputY.Value = markPosition.y;
            InputZ.Value = markPosition.z;
            InputRotationX.Value = _markRotation.x;
            InputRotationY.Value = _markRotation.y;
            InputRotationZ.Value = _markRotation.z;
        }

        /// <summary> Удаляет привязанную метку. </summary>
        public void DeleteMark()
        {
            MarksController.Instance.DeleteMark(Mark.Id);
        }

        /// <summary> Выбирает привязанную метку. </summary>
        public void SelectMark()
        {
            MarksController.Instance.SelectMark(Mark.Id);
        }

        #region Private definitions

        /// <summary> Вектор с углами вращдения вокруг осей метки. </summary>
        private Vector3 _markRotation;

        #endregion
    }
}