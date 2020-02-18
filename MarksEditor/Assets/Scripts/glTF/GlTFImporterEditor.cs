using System;
using System.IO;
using UnityEngine;
using HoloCore;
using static SFB.StandaloneFileBrowser;

namespace GLTFConverter
{
    /// <summary> Класс импорта из glTF</summary>
    public class GlTFImporterEditor : GLTFImporter
    {
        /// <summary> Импортирует объект из файла в редактор </summary>
        public void ImportGLTFFile()
        {
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
        private string OpenDialog()
        {
            string[] paths = OpenFilePanel("Веберите glTF-файл", "", "gltf", false);
            if (paths.Length == 0)
            {
                return "";
            }

            return paths[0];
        }

        /// <summary> Читает файл и извлекает из него данные в формате JSON </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Строка с данными в формате JSON</returns>
        private string ReadJSONStringFromFile(string path)
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
        private root GetRootFileFromJSONString(string JSONString)
        {
            root glTFFileRoot;
            try
            {
                glTFFileRoot = JsonUtility.FromJson<root>(JSONString);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }

            return glTFFileRoot;
        }

        /// <summary> Достает объекты из корневого объекта </summary>
        /// <param name="rootOfGLTFFile">Корневой объект</param>
        private void GetGameObjectsFromImportedGLTFFile(root rootOfGLTFFile)
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
                    FormMeshesFromGLTF(currentNodeGameObject, currentNode, rootOfGLTFFile);
                    AddCollision(currentNodeGameObject);
                }
            }
        }

        /// <summary> Добавляет метки на сцену </summary>
        /// <param name="currentRoot">Корневой объект glTF-файла</param>
        private void AddMarksToSceneFromRoot(root currentRoot)
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