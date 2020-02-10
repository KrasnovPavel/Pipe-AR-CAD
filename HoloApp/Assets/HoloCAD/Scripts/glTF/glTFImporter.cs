using System;
using System.IO;
using HoloCore;
using UnityEngine;
using System.Collections.Generic;
using glTFConverter;
#if ENABLE_WINMD_SUPPORT
    using Windows.Storage;
    using Windows.Storage.Pickers;
#else
using static SFB.StandaloneFileBrowser;

#endif

namespace HoloCAD.glTF
{
    /// <summary> Класс импорта из glTF</summary>
    public class glTFImporter : Singleton<glTFImporter>
    {
        /// <summary> Корневой объект на сцене </summary>
        public GameObject Target;

        /// <summary> Шейдер для отображения модели </summary>
        public Shader ShaderToImport;

        /// <summary> Импортирует объект из файла в редактор </summary>
        public void ImportglTFFile()
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(PickJson, false);
        }

        #region Private definitions

        private string filePath;
        private string jsonText;
        private root currentRoot;

        private async void PickJson()
        {
#if ENABLE_WINMD_SUPPORT
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".gltf");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file==null) return;
            jsonText = await FileIO.ReadTextAsync(file);
            if (jsonText.Length == 0)  return;
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
                jsonText = streamReader.ReadToEnd();
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

        private void ReadJsonAndBuildTargetWithMarks()
        {
            try
            {
                currentRoot = JsonUtility.FromJson<root>(jsonText);
            }
            catch
            {
                return;
            }

            int currentGameObjectIndex = 0;
            Transform parentTransform = Target.transform.GetChild(0);
            foreach (scene currentScene in currentRoot.scenes)
            {
                GameObject currentSceneGameObject = new GameObject(Convert.ToString(currentGameObjectIndex++));
                currentSceneGameObject.transform.parent = parentTransform;
                currentSceneGameObject.transform.localPosition = Vector3.zero;
                currentSceneGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                foreach (int currentSceneNodeIndex in currentScene.nodes)
                {
                    node currentNode = currentRoot.nodes[currentSceneNodeIndex];
                    GameObject currentNodeGameObject = new GameObject(currentNode.name);
                    currentNodeGameObject.transform.parent = currentSceneGameObject.transform;
                    FormMeshesFromglTF(currentNodeGameObject, currentNode, currentRoot);
                    AddCollision(currentNodeGameObject);
                }
            }

            AddMarksToSceneFromRoot();
            Target.GetComponent<MarksTarget>().Model = Target;
        }

        /// <summary> Читает меши из входного узла</summary>
        /// <param name="currentNodeGameObject">GameObject в который добавляется меш отдельным объектом</param>
        /// <param name="currentNode">Входной узел</param>
        /// <param name="rootOfglTFFile">Корневой объект</param>
        private void FormMeshesFromglTF(GameObject currentNodeGameObject, node currentNode, root rootOfglTFFile)
        {
            SetTransformToNode(currentNode, currentNodeGameObject);
            mesh currentMesh = rootOfglTFFile.meshes[currentNode.mesh];
            foreach (primitive currentMeshPrimitive in currentMesh.primitives)
            {
                GameObject currentPrimitiveGameObject = CreateGameObjectOfPrimitive(currentNodeGameObject);

                SetMaterial(rootOfglTFFile.materials[currentMeshPrimitive.material],
                    currentPrimitiveGameObject.AddComponent<MeshRenderer>());

                CreateMesh(rootOfglTFFile, currentMeshPrimitive, currentPrimitiveGameObject.AddComponent<MeshFilter>());
            }
        }

        /// <summary> Добавляет коллизию мешам внутри входного узла </summary>
        /// <param name="currentNodeGameObject"> Входной узел</param>
        private void AddCollision(GameObject currentNodeGameObject)
        {
            MeshFilter[] meshFilters = currentNodeGameObject.transform.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter meshFilterChild in meshFilters)
            {
                GameObject newGameObject = new GameObject(meshFilterChild.mesh.name);
                newGameObject.layer = 30;

                Rigidbody rigidbodyOfNewGameObject = newGameObject.AddComponent<Rigidbody>();

                rigidbodyOfNewGameObject.isKinematic = true;

                MeshCollider meshCollider = newGameObject.AddComponent<MeshCollider>();

                meshCollider.sharedMesh = meshFilterChild.mesh;

                newGameObject.transform.SetParent(meshFilterChild.transform);

                newGameObject.transform.localScale = Vector3.one;
                newGameObject.transform.localPosition = Vector3.zero;
                newGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void AddMarksToSceneFromRoot()
        {
            MarksTarget currentTarget = Target.GetComponent<MarksTarget>();
            currentTarget.Marks = new List<Mark>();
            currentTarget.PositionsOfMarks = new List<Vector3>();
            currentTarget.RotationsOfMarks = new List<Vector3>();
            foreach (_mark currentMarkFromglTF in currentRoot._marksInfo._marks)
            {
                currentTarget.Marks.Add(GameObject.Find(currentMarkFromglTF.name).GetComponent<Mark>());
                currentTarget.PositionsOfMarks.Add(new Vector3(currentMarkFromglTF.x, currentMarkFromglTF.y,
                    currentMarkFromglTF.z));
                Quaternion quaternion = Quaternion.Euler(currentMarkFromglTF.rotationX, currentMarkFromglTF.rotationY,
                    currentMarkFromglTF.rotationZ);
                currentTarget.RotationsOfMarks.Add(quaternion.eulerAngles);
            }
        }

        /// <summary> Вносит данные о позиции, масштабе и повороте объекта из объекта узла </summary>
        /// <param name="currentNode">Объект узла glTF</param>
        /// <param name="currentNodeGameObject">Объект, в который заносятся данные</param>
        private void SetTransformToNode(node currentNode, GameObject currentNodeGameObject)
        {
            currentNodeGameObject.transform.localRotation = currentNode.FormRotation();
            currentNodeGameObject.transform.localPosition = currentNode.FormPosition();
            currentNodeGameObject.transform.localScale = currentNode.FormScale();
        }

        /// <summary>Устанавливает материал мешу из glTF-файла </summary>
        /// <param name="currentExportedMaterial">Объект материала из glTF-файла</param>
        /// <param name="currentMeshRenderer">Рендерер меша</param>
        private void SetMaterial(material currentExportedMaterial, MeshRenderer currentMeshRenderer)
        {
            currentMeshRenderer.material = new Material(Shader.Find("Standard"));
            currentMeshRenderer.material.SetFloat("_Glossiness",
                currentExportedMaterial.pbrMetallicRoughness.roughnessFactor);
            currentMeshRenderer.material.SetFloat("_Metallic",
                currentExportedMaterial.pbrMetallicRoughness.metallicFactor);
            currentMeshRenderer.material.color = currentExportedMaterial.FormUnityColor();
        }

        /// <summary> Создает меш из данных glTF-файла </summary>
        /// <param name="rootOfglTFFile">Коренной объект glTF-файла</param>
        /// <param name="currentMeshPrimitive">Объект примитива glTF-файла</param>
        /// <param name="currentMeshFilter">Меш фильтр объекта</param>
        private void CreateMesh(root rootOfglTFFile, primitive currentMeshPrimitive, MeshFilter currentMeshFilter)
        {
            Mesh currentUnityMesh = new Mesh();

            accessor positionAccessor = rootOfglTFFile.accessors[currentMeshPrimitive.attributes.POSITION];
            accessor indicesAccessor = rootOfglTFFile.accessors[currentMeshPrimitive.indices]; //TODO: нормали 

            bufferView positionBufferView = rootOfglTFFile.bufferViews[positionAccessor.bufferView];
            bufferView indicesBufferView = rootOfglTFFile.bufferViews[indicesAccessor.bufferView];

            buffer positionBuffer = rootOfglTFFile.buffers[positionBufferView.buffer];
            buffer indicesBuffer = rootOfglTFFile.buffers[indicesBufferView.buffer];

            string positionBufferWithoutPrefix = positionBuffer.uri.Substring(glTFConvertionConsts.bufferPrefix.Length);
            string indicesBufferWithoutPrefix = indicesBuffer.uri.Substring(glTFConvertionConsts.bufferPrefix.Length);

            byte[] positionBytes = Convert.FromBase64String(positionBufferWithoutPrefix);
            byte[] indicesBytes = Convert.FromBase64String(indicesBufferWithoutPrefix);

            MemoryStream positionMemoryStream = new MemoryStream(positionBytes, positionBufferView.byteOffset,
                positionBufferView.byteLength);
            MemoryStream indicesMemoryStream =
                new MemoryStream(indicesBytes, indicesBufferView.byteOffset, indicesBufferView.byteLength);

            Vector3[] vertices = new Vector3[positionAccessor.count];

            for (int vector3Index = 0; vector3Index < positionAccessor.count; vector3Index++)
            {
                Vector3 currentVector3 = new Vector3();
                byte[] coordinateFloatBytes = new byte[12];
                positionMemoryStream.Read(coordinateFloatBytes, 0, 12);
                currentVector3.x = BitConverter.ToSingle(coordinateFloatBytes, 0);
                currentVector3.y = BitConverter.ToSingle(coordinateFloatBytes, 4);
                currentVector3.z = BitConverter.ToSingle(coordinateFloatBytes, 8);
                vertices[vector3Index] = currentVector3;
            }

#if ENABLE_WINMD_SUPPORT
            positionMemoryStream.Dispose();
#else
            positionMemoryStream.Close();
#endif
            currentUnityMesh.vertices = vertices;
            int[] indices = new int[indicesAccessor.count];
            for (int indexIndex = 0; indexIndex < indicesAccessor.count; indexIndex++)
            {
                byte[] triangleIndicesBytes = new byte[2];
                indicesMemoryStream.Read(triangleIndicesBytes, 0, 2);
                indices[indexIndex] = BitConverter.ToUInt16(triangleIndicesBytes, 0);
            }

#if ENABLE_WINMD_SUPPORT
                indicesMemoryStream.Dispose();
#else
            indicesMemoryStream.Close();
#endif
            currentUnityMesh.triangles = indices;
            currentMeshFilter.mesh = currentUnityMesh;
        }


        /// <summary> Создает объект примитива </summary>
        /// <param name="currentNodeGameObject"> Объект узла</param>
        /// <returns>Объект примитива</returns>
        private GameObject CreateGameObjectOfPrimitive(GameObject currentNodeGameObject)
        {
            GameObject currentPrimitiveGameObject = new GameObject();
            currentPrimitiveGameObject.transform.parent = currentNodeGameObject.transform;
            currentPrimitiveGameObject.transform.localScale = Vector3.one;
            currentPrimitiveGameObject.transform.localPosition = Vector3.zero;
            currentPrimitiveGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return currentNodeGameObject;
        }

        #endregion
    }
}