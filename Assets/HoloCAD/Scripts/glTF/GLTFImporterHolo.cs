// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.IO;
using HoloCAD;
using UnityEngine;
using System.Collections.Generic;
#if ENABLE_WINMD_SUPPORT
    using Windows.Storage;
    using Windows.Storage.Pickers;
#else
using static SFB.StandaloneFileBrowser;

#endif

namespace GLTFConverter
{
    /// <summary> Класс импорта из glTF</summary>
    public static class GLTFImporterHolo
    {
        /// <summary> Импортирует объект из файла в редактор </summary>
        public static void ImportGLTFFile(GameObject target)
        {
            _target = target;
            UnityEngine.WSA.Application.InvokeOnUIThread(PickJson, false);
        }

        #region Private definitions

        /// <summary> Корневой объект на сцене </summary>
        private static GameObject _target;
        private static string _filePath;
        private static string _jsonText;
        private static root _currentRoot;

#pragma warning disable 1998
        private static async void PickJson()
#pragma warning restore 1998
        {
#if ENABLE_WINMD_SUPPORT
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".gltf");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file == null) return;
            _jsonText = await FileIO.ReadTextAsync(file);
            if (_jsonText.Length == 0) return;
            UnityEngine.WSA.Application.InvokeOnAppThread(ReadJsonAndBuildTargetWithMarks, true);
#elif UNITY_2018
            string[] paths = OpenFilePanel("Выберите glTF-файл", "", "gltf", false);
            if (paths.Length == 0)
            {
                return;
            }

            string path = paths[0];
            StreamReader streamReader = new StreamReader(path);

            try
            {
                _jsonText = streamReader.ReadToEnd();
            }
            catch (IOException e)
            {
                Debug.LogError(e);
                return;
            }

            streamReader.Close();

            UnityEngine.WSA.Application.InvokeOnAppThread(ReadJsonAndBuildTargetWithMarks, true);
#endif
        }

        private static void ReadJsonAndBuildTargetWithMarks()
        {
            try
            {
                _currentRoot = JsonUtility.FromJson<root>(_jsonText);
            }
            catch
            {
                return;
            }

            int currentGameObjectIndex = 0;
            Transform parentTransform = _target.transform.GetChild(0);
            foreach (scene currentScene in _currentRoot.scenes)
            {
                GameObject currentSceneGameObject = new GameObject(Convert.ToString(currentGameObjectIndex++));
                currentSceneGameObject.transform.parent = parentTransform;
                currentSceneGameObject.transform.localPosition = Vector3.zero;
                currentSceneGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                foreach (int currentSceneNodeIndex in currentScene.nodes)
                {
                    node currentNode = _currentRoot.nodes[currentSceneNodeIndex];
                    GameObject currentNodeGameObject = new GameObject(currentNode.name);
                    currentNodeGameObject.transform.parent = currentSceneGameObject.transform;
                    GLTFImporter.FormMeshesFromGLTF(currentNodeGameObject, currentNode, _currentRoot);
                    GLTFImporter.AddCollision(currentNodeGameObject);
                }
            }

            AddMarksToSceneFromRoot();
            _target.GetComponent<MarksTarget>().Model = _target;
        }


        private static void AddMarksToSceneFromRoot()
        {
            MarksTarget currentTarget = _target.GetComponent<MarksTarget>();
            currentTarget.Marks = new List<Mark>();
            currentTarget.PositionsOfMarks = new List<Vector3>();
            currentTarget.RotationsOfMarks = new List<Vector3>();
            foreach (_mark currentMarkFromGLTF in _currentRoot._marksInfo._marks)
            {
                currentTarget.Marks.Add(GameObject.Find(currentMarkFromGLTF.name).GetComponent<Mark>());
                currentTarget.PositionsOfMarks.Add(currentMarkFromGLTF.Position);
                Quaternion quaternion = currentMarkFromGLTF.Rotation;
                currentTarget.RotationsOfMarks.Add(quaternion.eulerAngles);
            }
        }

        #endregion
    }
}