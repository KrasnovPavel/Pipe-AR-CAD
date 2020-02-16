using UnityEngine;
using UnityEngine.UI;

namespace GLTFConverter
{
    /// <summary> Класс, содержащий параметры меток </summary>
    public class MarkParamPanel : MonoBehaviour
    {
        /// <summary> Метка, к которой привязана модель </summary>
        [Tooltip("text")] public Mark Mark;


        /// <summary> Поле ввода смещения по X</summary>
        [Tooltip("text")] public InputFromPanelValidator InputX;

        /// <summary> Поле ввода смещения по Y</summary>
        [Tooltip("text")] public InputFromPanelValidator InputY;

        /// <summary> Поле ввода смещения по Z</summary>
        [Tooltip("text")] public InputFromPanelValidator InputZ;

        /// <summary> Поле ввода вращения относительно оси X</summary>
        [Tooltip("text")] public InputFromPanelValidator InputRotationX;

        /// <summary> Поле ввода вращения относительно оси Y</summary>
        [Tooltip("text")] public InputFromPanelValidator InputRotationY;

        /// <summary> Поле ввода вращения относительно оси Z</summary>
        [Tooltip("text")] public InputFromPanelValidator InputRotationZ;

        /// <summary> Текстовое поле с Id панели</summary>
        [Tooltip("text")] public Text IdText;

        /// <summary> Вектор с углами вращдения вокруг осей метки </summary>
        [Tooltip("text")] private Vector3 MarkRotation;

        /// <summary>Задает метке параметры из панели </summary>
        public void SetParamsToMarkFromInputs()
        {
            if (Mark == null) return;
            Mark.transform.position = new Vector3(InputX.Value, InputY.Value, InputZ.Value);
            MarkRotation = new Vector3(InputRotationX.Value, InputRotationY.Value, InputRotationZ.Value);

            Mark.transform.rotation = Quaternion.Euler(MarkRotation);
        }

        /// <summary>Вносит в панель параметры метки</summary>
        public void MarkTransformIntoInput()
        {
            Transform markTransform = Mark.transform;
            Vector3 markPosition = markTransform.position;
            MarkRotation = markTransform.eulerAngles;
            InputX.Value = markPosition.x;
            InputY.Value = markPosition.y;
            InputZ.Value = markPosition.z;
            InputRotationX.Value = MarkRotation.x;
            InputRotationY.Value = MarkRotation.y;
            InputRotationZ.Value = MarkRotation.z;
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