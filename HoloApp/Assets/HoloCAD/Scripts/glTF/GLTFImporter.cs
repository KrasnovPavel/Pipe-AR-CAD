using System;
using System.IO;
using HoloCore;
using UnityEngine;

namespace GLTFConverter
{
    /// <summary> Класс импорта из glTF</summary>
    public class GLTFImporter : Singleton<GLTFImporter>
    {
        /// <summary> Корневой объект на сцене </summary>
        public GameObject Target;

        #region Protected definitions

        /// <summary> Добавляет коллизию мешам внутри входного узла </summary>
        /// <param name="currentNodeGameObject"> Входной узел</param>
        protected void AddCollision(GameObject currentNodeGameObject)
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
        protected void SetTransformToNode(node currentNode, GameObject currentNodeGameObject)
        {
            currentNodeGameObject.transform.localRotation = currentNode.Rotation;
            currentNodeGameObject.transform.localPosition = currentNode.Position;
            currentNodeGameObject.transform.localScale = currentNode.Scale;
        }

        /// <summary>Устанавливает материал мешу из glTF-файла </summary>
        /// <param name="currentExportedMaterial">Объект материала из glTF-файла</param>
        /// <param name="currentMeshRenderer">Рендерер меша</param>
        protected void SetMaterial(material currentExportedMaterial, MeshRenderer currentMeshRenderer)
        {
            currentMeshRenderer.material = new Material(Shader.Find("Standard"));
            currentMeshRenderer.material.SetFloat("_Glossiness",
                currentExportedMaterial.pbrMetallicRoughness.roughnessFactor);
            currentMeshRenderer.material.SetFloat("_Metallic",
                currentExportedMaterial.pbrMetallicRoughness.metallicFactor);
            currentMeshRenderer.material.color = currentExportedMaterial.Color;
        }

        /// <summary> Создает меш из данных glTF-файла </summary>
        /// <param name="rootOfglTFFile">Коренной объект glTF-файла</param>
        /// <param name="currentMeshPrimitive">Объект примитива glTF-файла</param>
        /// <param name="currentMeshFilter">Меш фильтр объекта</param>
        protected void CreateMesh(root rootOfglTFFile, primitive currentMeshPrimitive, MeshFilter currentMeshFilter)
        {
            Mesh currentUnityMesh = new Mesh();

            accessor positionAccessor = rootOfglTFFile.accessors[currentMeshPrimitive.attributes.POSITION];
            accessor indicesAccessor = rootOfglTFFile.accessors[currentMeshPrimitive.indices];

            bufferView positionBufferView = rootOfglTFFile.bufferViews[positionAccessor.bufferView];
            bufferView indicesBufferView = rootOfglTFFile.bufferViews[indicesAccessor.bufferView];

            buffer positionBuffer = rootOfglTFFile.buffers[positionBufferView.buffer];
            buffer indicesBuffer = rootOfglTFFile.buffers[indicesBufferView.buffer];

            string positionBufferWithoutPrefix = positionBuffer.uri.Substring(GLTFConvertionConsts.bufferPrefix.Length);
            string indicesBufferWithoutPrefix = indicesBuffer.uri.Substring(GLTFConvertionConsts.bufferPrefix.Length);

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
        protected GameObject CreateGameObjectOfPrimitive(GameObject currentNodeGameObject)
        {
            GameObject currentPrimitiveGameObject = new GameObject();
            currentPrimitiveGameObject.transform.parent = currentNodeGameObject.transform;
            currentPrimitiveGameObject.transform.localScale = Vector3.one;
            currentPrimitiveGameObject.transform.localPosition = Vector3.zero;
            currentPrimitiveGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return currentNodeGameObject;
        }


        /// <summary> Читает меши из входного узла</summary>
        /// <param name="currentNodeGameObject">GameObject в который добавляется меш отдельным объектом</param>
        /// <param name="currentNode">Входной узел</param>
        /// <param name="rootOfGLTFFile">Корневой объект</param>
        protected void FormMeshesFromGLTF(GameObject currentNodeGameObject, node currentNode, root rootOfGLTFFile)
        {
            SetTransformToNode(currentNode, currentNodeGameObject);
            mesh currentMesh = rootOfGLTFFile.meshes[currentNode.mesh];
            foreach (primitive currentMeshPrimitive in currentMesh.primitives)
            {
                GameObject currentPrimitiveGameObject = CreateGameObjectOfPrimitive(currentNodeGameObject);

                SetMaterial(rootOfGLTFFile.materials[currentMeshPrimitive.material],
                    currentPrimitiveGameObject.AddComponent<MeshRenderer>());

                CreateMesh(rootOfGLTFFile, currentMeshPrimitive, currentPrimitiveGameObject.AddComponent<MeshFilter>());
            }
        }

        #endregion
    }
}