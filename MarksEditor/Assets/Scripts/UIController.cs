using MarksEditor.glTF;
using UnityEngine;

namespace MarksEditor
{
    /// <summary>Контроллер пользовательского интерфейса </summary>
    public class UIController : MonoBehaviour
    {
        /// <summary> Добавляет метку на сцену </summary>
        public void AddMarkOnScene()
        {
            MarksController.Instance.AddMark();
        }

        /// <summary>Загружает glTF-файл</summary>
        public void LoadGLTFFile()
        {
            glTFImporter.Instance.ImportglTFFile();
        }

        /// <summary>Сохраняет glTF-файл</summary>
        public void SaveGLTFFile()
        {
            glTFExporter.Instance.ExportglTFFile();
        }

        /// <summary>Импортирует модель с помощью PiXYZ </summary>
        public void ImportModel()
        {
            ModelImporter.Instance.ImportModel();
        }
    }
}