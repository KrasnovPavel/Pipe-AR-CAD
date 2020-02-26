// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.IO;
using UnityEngine;
using static SFB.StandaloneFileBrowser;

namespace GLTFConverter
{
    /// <summary> Класс импорта из glTF. </summary>
    public static class GLTFImporterEditor
    {
        /// <summary> Импортирует объект из файла в редактор </summary>
        public static void ImportGLTFFile(GameObject target)
        {
            _target = target;
            string filepath = OpenDialog();
            if (filepath == "") return;

            string jsonString = ReadJSONStringFromFile(filepath);
            if (jsonString == "") return;

            root rootOfGLTFFile = GetRootFileFromJSONString(jsonString);
            if (rootOfGLTFFile == null) return;

            GetGameObjectsFromImportedGLTFFile(rootOfGLTFFile);
            AddMarksToSceneFromRoot(rootOfGLTFFile);
        }

        #region Private definitions
        
        /// <summary> Импортируемый объект. </summary>
        private static GameObject _target;

        /// <summary> Открывает диалог открытия файла. </summary>
        /// <returns> Путь к файлу. </returns>
        private static string OpenDialog()
        {
            string[] paths = OpenFilePanel("Выберите glTF-файл", "", "gltf", false);
            return paths.Length == 0 ? "" : paths[0];
        }

        /// <summary> Читает файл и извлекает из него данные в формате JSON. </summary>
        /// <param name="path"> Путь к файлу. </param>
        /// <returns> Строка с данными в формате JSON. </returns>
        private static string ReadJSONStringFromFile(string path)
        {
            string jsonString;
            StreamReader streamReader = new StreamReader(path);

            try
            {
                jsonString = streamReader.ReadToEnd();
            }
            catch
            {
                return "";
            }

            streamReader.Close();
            return jsonString;
        }

        /// <summary> Парсит входящую строку и извлекает из нее корневой объект для glTF-файла. </summary>
        /// <param name="jsonString"> Строка с данными в формате JSON. </param>
        /// <returns> Корневой объект для glTF-файла. </returns>
        private static root GetRootFileFromJSONString(string jsonString)
        {
            root gltfFileRoot;
            try
            {
                gltfFileRoot = JsonUtility.FromJson<root>(jsonString);
            }
            catch
            {
                return null;
            }

            return gltfFileRoot;
        }

        /// <summary> Достает объекты из корневого объекта. </summary>
        /// <param name="rootOfGLTFFile">Корневой объект. </param>
        private static void GetGameObjectsFromImportedGLTFFile(root rootOfGLTFFile)
        {
            Transform parentTransform = _target.transform;
            foreach (scene currentScene in rootOfGLTFFile.scenes)
            {
                foreach (int currentSceneNodeIndex in currentScene.nodes)
                {
                    node currentNode = rootOfGLTFFile.nodes[currentSceneNodeIndex];
                    GameObject currentNodeGameObject = new GameObject(currentNode.name);
                    currentNodeGameObject.transform.parent = parentTransform;
                    GLTFImporter.FormMeshesFromGLTF(currentNodeGameObject, currentNode, rootOfGLTFFile);
                    GLTFImporter.AddCollision(currentNodeGameObject);
                }
            }
        }

        /// <summary> Добавляет метки на сцену. </summary>
        /// <param name="currentRoot"> Корневой объект glTF-файла. </param>
        private static void AddMarksToSceneFromRoot(root currentRoot)
        {
            MarksController.Instance.DeleteAllMarks();
            foreach (_mark currentMarkFromGLTF in currentRoot._marksInfo._marks)
            {
                MarksController.Instance.AddMark(currentMarkFromGLTF.Position, currentMarkFromGLTF.Rotation);
            }
        }

        #endregion
    }
}