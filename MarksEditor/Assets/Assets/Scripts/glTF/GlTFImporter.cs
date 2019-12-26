using System;
using System.IO;
using Boo.Lang;
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
            string JSONString  = ReadJSONStringFromFile(filepath);
            root rootOfglTFFile = GetRootFileFromJSONString(JSONString);
            GetGameObjectsFromImportedglTFFile(rootOfglTFFile);
        }

        /// <summary> Открывает диалог открытия файла </summary>
        /// <returns>Путь к файлу</returns>
        /// <exception cref="Exception"> Файл не выбран</exception>
        private string OpenDialog()
        {
            string[] paths = OpenFilePanel("Веберите glTF-файл","","gltf",false);
            if (paths.Length == 0)
            {
                throw new Exception("Файл не выбран!");
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
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
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
                Debug.Log(e);
                throw;
            }

            return glTFFileRoot;
        }
        
        /// <summary> Достает объекты из корневого объекта </summary>
        /// <param name="rootOfglTFFile">Корневой объект</param>
        private void GetGameObjectsFromImportedglTFFile(root rootOfglTFFile)
        {
            int currentGameObjectIndex = 0;
            Transform parentTransform = MarkPlaceController.Instance.Target.transform;
            foreach (scene currentScene in rootOfglTFFile.scenes)
            {
                GameObject currentSceneGameObject = new GameObject(Convert.ToString(currentGameObjectIndex++));
                currentSceneGameObject.transform.parent = parentTransform;
                
                foreach (int currentSceneNodeIndex in currentScene.nodes)
                {
                    node currentNode = rootOfglTFFile.nodes[currentSceneNodeIndex];
                    GameObject currentNodeGameObject = new GameObject(currentNode.name);
                    currentNodeGameObject.transform.parent = currentSceneGameObject.transform;
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
                currentNodeGameObject.transform.localRotation = Quaternion.Euler((new Quaternion(currentNode.rotation[0],
                currentNode.rotation[1], currentNode.rotation[2], currentNode.rotation[3])).eulerAngles);
            currentNodeGameObject.transform.localPosition = new Vector3(currentNode.translation[0],currentNode.translation[1],currentNode.translation[2]);
            currentNodeGameObject.transform.localScale = new Vector3(currentNode.scale[0],currentNode.scale[1],currentNode.scale[2]);
            mesh currentMesh = rootOfglTFFile.meshes[currentNode.mesh];
            foreach (primitive currentMeshPrimitive in currentMesh.primitives)
            {
                GameObject currentPrimitiveGameObject = new GameObject();
                currentPrimitiveGameObject.transform.parent = currentNodeGameObject.transform;
                currentPrimitiveGameObject.transform.localScale  = new Vector3(1,1,1);
                currentPrimitiveGameObject.transform.localPosition  = new Vector3(0,0,0);
                currentPrimitiveGameObject.transform.localRotation  = Quaternion.Euler(0,0,0);
                MeshFilter currentMeshFilter = currentPrimitiveGameObject.AddComponent<MeshFilter>();
                
                MeshRenderer currentMeshRenderer = currentPrimitiveGameObject.AddComponent<MeshRenderer>();
                currentMeshRenderer.material = new Material(Shader.Find("VR/SpatialMapping/Wireframe"));
                currentMeshRenderer.material.color = Color.white;
                
                Mesh currentUnityMesh = new Mesh();
                
                accessor positionAccessor =  rootOfglTFFile.accessors[currentMeshPrimitive.attributes.POSITION];
                accessor indicesAccessor = rootOfglTFFile.accessors[currentMeshPrimitive.indices]; //TODO: нормали + материалы

                bufferView positionBufferView = rootOfglTFFile.bufferViews[positionAccessor.bufferView];
                bufferView indicesBufferView = rootOfglTFFile.bufferViews[indicesAccessor.bufferView];

                buffer positionBuffer = rootOfglTFFile.buffers[positionBufferView.buffer];
                buffer indicesBuffer = rootOfglTFFile.buffers[indicesBufferView.buffer];

                string positionBufferWithoutPrefix = positionBuffer.uri.Substring(glTFConvertionConsts.bufferPrefix.Length);
                string indicesBufferWithoutPrefix = indicesBuffer.uri.Substring(glTFConvertionConsts.bufferPrefix.Length);

                byte[] positionBytes = Convert.FromBase64String(positionBufferWithoutPrefix);
                byte[] indicesBytes = Convert.FromBase64String(indicesBufferWithoutPrefix);

                MemoryStream positionMemoryStream = new MemoryStream(positionBytes, positionBufferView.byteOffset, positionBufferView.byteLength);
                MemoryStream indicesMemoryStream = new MemoryStream(indicesBytes,indicesBufferView.byteOffset,indicesBufferView.byteLength);
                
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
                    indices[indexIndex] = BitConverter.ToUInt16(triangleIndicesBytes,0);
                }
                
                indicesMemoryStream.Close();
                currentUnityMesh.triangles = indices;
                currentMeshFilter.mesh = currentUnityMesh;
            }
        }

        /// <summary> Добавляет коллизию мешам внутри входного узла </summary>
        /// <param name="currentNodeGameObject"> Входной узел</param>
        private void AddCollision(GameObject currentNodeGameObject)
        {
            foreach (MeshFilter meshFilterChild in currentNodeGameObject.transform.GetComponentsInChildren<MeshFilter>())
            {
                GameObject newGameObject = new GameObject(meshFilterChild.mesh.name);
                newGameObject.layer = 30;
                Rigidbody rigidbodyOfNewGameObject = newGameObject.AddComponent<Rigidbody>();
                rigidbodyOfNewGameObject.isKinematic = true;
                MeshCollider meshCollider = newGameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilterChild.mesh;
                newGameObject.transform.SetParent(meshFilterChild.transform);
                newGameObject.transform.localPosition = Vector3.zero;
            }
        }
    }
    
    
}


