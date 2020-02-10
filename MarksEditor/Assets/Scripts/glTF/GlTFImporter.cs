using System;
using System.IO;
using HoloCore;
using UnityEngine;
using static SFB.StandaloneFileBrowser;

namespace MarksEditor.glTF
{
    /// <summary> Класс импорта из glTF</summary>
    public class GlTFImporter : Singleton<GlTFImporter>
    {
        /// <summary> Корневой объект на сцене </summary>
        public GameObject Target;

        /// <summary> Импортирует объект из файла в редактор </summary>
        public void ImportglTFFile()
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

            root rootOfglTFFile = GetRootFileFromJSONString(JSONString);
            if (rootOfglTFFile == null)
            {
                return;
            }

            GetGameObjectsFromImportedglTFFile(rootOfglTFFile);
        }

        /// <summary> Открывает диалог открытия файла </summary>
        /// <returns>Путь к файлу</returns>
        /// <exception cref="Exception"> Файл не выбран</exception>
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
            catch (IOException e)
            {
                Debug.LogError(e);
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
        /// <param name="rootOfglTFFile">Корневой объект</param>
        private void GetGameObjectsFromImportedglTFFile(root rootOfglTFFile)
        {
            int currentGameObjectIndex = 0;
            Transform parentTransform = Target.transform;
            foreach (scene currentScene in rootOfglTFFile.scenes)
            {
                foreach (int currentSceneNodeIndex in currentScene.nodes)
                {
                    node currentNode = rootOfglTFFile.nodes[currentSceneNodeIndex];
                    GameObject currentNodeGameObject = new GameObject(currentNode.name);
                    currentNodeGameObject.transform.parent = parentTransform;
                    FormMeshesFromglTF(currentNodeGameObject, currentNode, rootOfglTFFile);
                    AddCollision(currentNodeGameObject);
                }
            }
        }

        /// <summary> Читает меши из входного узла</summary>
        /// <param name="currentNodeGameObject">GameObject в который добавляется меш отдельным объктом</param>
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
                newGameObject.transform.localPosition = Vector3.zero;
                newGameObject.transform.localScale = Vector3.one;
                newGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        /// <summary> Вносит данные о позиции, масштабу и повороту объекта из объекта узла </summary>
        /// <param name="currentNode">Объект узла glTF</param>
        /// <param name="currentNodeGameObject">Объект, в который заносятся данные</param>
        private void SetTransformToNode(node currentNode, GameObject currentNodeGameObject)
        {
            float[] nodeRotation = currentNode.rotation;
            float[] nodeTranslation = currentNode.translation;
            float[] nodeScale = currentNode.scale;
            currentNodeGameObject.transform.localRotation = new Quaternion(nodeRotation[0],
                nodeRotation[1], nodeRotation[2], nodeRotation[3]);
            currentNodeGameObject.transform.localPosition =
                new Vector3(nodeTranslation[0], nodeTranslation[1], nodeTranslation[2]);
            currentNodeGameObject.transform.localScale = new Vector3(nodeScale[0], nodeScale[1], nodeScale[2]);
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
            currentMeshRenderer.material.color = new Color(
                currentExportedMaterial.pbrMetallicRoughness.baseColorFactor[0],
                currentExportedMaterial.pbrMetallicRoughness.baseColorFactor[1],
                currentExportedMaterial.pbrMetallicRoughness.baseColorFactor[2],
                currentExportedMaterial.pbrMetallicRoughness.baseColorFactor[3]);
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

            positionMemoryStream.Close();
            currentUnityMesh.vertices = vertices;
            int[] indices = new int[indicesAccessor.count];
            for (int indexIndex = 0; indexIndex < indicesAccessor.count; indexIndex++)
            {
                byte[] triangleIndicesBytes = new byte[2];
                indicesMemoryStream.Read(triangleIndicesBytes, 0, 2);
                indices[indexIndex] = BitConverter.ToUInt16(triangleIndicesBytes, 0);
            }

            indicesMemoryStream.Close();
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
    }
}