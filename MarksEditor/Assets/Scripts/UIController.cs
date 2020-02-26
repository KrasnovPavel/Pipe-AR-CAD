// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityEngine;

namespace GLTFConverter
{
    /// <summary> Контроллер пользовательского интерфейса. </summary>
    public class UIController : MonoBehaviour
    {
        /// <summary> Целевой объект </summary>
        [Tooltip("Целевой объект")]
        public GameObject Target;

        /// <summary> Добавляет метку на сцену. </summary>
        public void AddMarkOnScene()
        {
            MarksController.Instance.AddMark();
        }

        /// <summary> Загружает glTF-файл. </summary>
        public void LoadGLTFFile()
        {
            GLTFImporterEditor.ImportGLTFFile(Target);
        }

        /// <summary> Сохраняет glTF-файл. </summary>
        public void SaveGLTFFile()
        {
            GLTFExporter.ExportGLTFFile(Target);
        }

        /// <summary> Импортирует модель с помощью PiXYZ. </summary>
        public void ImportModel()
        {
            ModelImporter.Instance.ImportModel();
        }
    }
}