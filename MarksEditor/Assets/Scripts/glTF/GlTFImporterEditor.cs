using System;
using System.IO;
using UnityEngine;
using static SFB.StandaloneFileBrowser;

namespace GLTFConverter
{
    /// <summary> Класс импорта из glTF</summary>
    public static class GLTFImporterEditor
    {
        /// <summary> Импортируемый объект </summary>
        private static GameObject Target;

        /// <summary> Импортирует объект из файла в редактор </summary>
        public static void ImportGLTFFile(GameObject target)
        {
            Target = target;
            string filepath = OpenDialog();
            if (filepath == "")
            {
                return;
            }

            string JSONString = ReadJSONStringFromFile(filepath);
            if (JSONString == "")
            {
                return;
            }

            root rootOfGLTFFile = GetRootFileFromJSONString(JSONString);
            if (rootOfGLTFFile == null)
            {
                return;
            }

            GetGameObjectsFromImportedGLTFFile(rootOfGLTFFile);
            AddMarksToSceneFromRoot(rootOfGLTFFile);
        }

        #region Private definitions

        /// <summary> Открывает диалог открытия файла </summary>
        /// <returns>Путь к файлу</returns>
        private static string OpenDialog()
        {
            string[] paths = OpenFilePanel("Выберите glTF-файл", "", "gltf", false);
            if (paths.Length == 0)
            {
                return "";
            }

            return paths[0];
        }

        /// <summary> Читает файл и извлекает из него данные в формате JSON </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Строка с данными в формате JSON</returns>
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

        /// <summary> Парсит входящую строку и извлекает из нее корневой объект для glTF-файла </summary>
        /// <param name="JSONString">Строка с данными в формате JSON</param>
        /// <returns>Корневой объект для glTF-файла</returns>
        private static root GetRootFileFromJSONString(string JSONString)
        {
            root glTFFileRoot;
            try
            {
                glTFFileRoot = JsonUtility.FromJson<root>(JSONString);
            }
            catch
            {
                return null;
            }

            return glTFFileRoot;
        }

        /// <summary> Достает объекты из корневого объекта </summary>
        /// <param name="rootOfGLTFFile">Корневой объект</param>
        private static void GetGameObjectsFromImportedGLTFFile(root rootOfGLTFFile)
        {
            int currentGameObjectIndex = 0;
            Transform parentTransform = Target.transform;
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

        /// <summary> Добавляет метки на сцену </summary>
        /// <param name="currentRoot">Корневой объект glTF-файла</param>
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