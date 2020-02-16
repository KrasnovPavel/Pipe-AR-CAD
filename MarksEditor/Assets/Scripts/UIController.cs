using UnityEngine;

namespace GLTFConverter
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
            ((GlTFImporterEditor) GlTFImporterEditor.Instance).ImportGLTFFile();
        }

        /// <summary>Сохраняет glTF-файл</summary>
        public void SaveGLTFFile()
        {
            GLTFExporter.Instance.ExportGLTFFile();
        }

        /// <summary>Импортирует модель с помощью PiXYZ </summary>
        public void ImportModel()
        {
            ModelImporter.Instance.ImportModel();
        }
    }
}