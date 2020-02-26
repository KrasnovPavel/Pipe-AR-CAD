// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static SFB.StandaloneFileBrowser;

namespace GLTFConverter
{
    /// <summary> Класс экспорта модели в glTF. </summary>
    public static class GLTFExporter
    {
        /// <summary> Экспортирует объект в glTF формат. </summary>
        public static void ExportGLTFFile(GameObject target)
        {
            _target = target;
            string filePath = SaveDialog();
            if (filePath.Length == 0)
            {
                return;
            }

            root rootOfGLTF = FormGLTFRoot();

            AddMarksData(rootOfGLTF);
            string gltfString = FormGLTFString(rootOfGLTF);
            WriteGLTFStringIntoFile(filePath, gltfString);
        }

        #region Private definitions

        /// <summary> Экспортируемый объект. </summary>
        private static GameObject _target;

        /// <summary> Записывает данные в файл по указанному пути. </summary>
        /// <param name="filePath"> Путь к файлу. </param>
        /// <param name="glTfString"> Данные. </param>
        private static void WriteGLTFStringIntoFile(string filePath, string glTfString)
        {
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(glTfString);
            streamWriter.Close();
        }

        /// <summary> Записывает glTF-объект в строку. </summary>
        /// <param name="currentRoot"> glTF-объект. </param>
        /// <returns> Строка-содержимое glTF-файла. </returns>
        private static string FormGLTFString(root currentRoot)
        {
            return JsonUtility.ToJson(currentRoot);
        }

        /// <summary> Открывает диалог сохранения файла. </summary>
        /// <returns> Путь к указанному файлу. </returns>
        private static string SaveDialog()
        {
            string filePath = SaveFilePanel("Сохраните файл!", "", "undefined", "glTF");
            return filePath;
        }

        /// <summary> Создает glTF-объект из указанного объекта. </summary>
        /// <returns> Созданный glTF-объект. </returns>
        private static root FormGLTFRoot()
        {
            root currentRoot = new root {scenes = new List<scene>(new [] {new scene()})};
            Transform targetTransform = _target.transform;

            int index = 0;
            foreach (Transform childTransform in targetTransform.GetComponentInChildren<Transform>())
            {
                var meshFilter = childTransform.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    continue;
                }

                var mesh = meshFilter.mesh;
                if (mesh == null)
                {
                    continue;
                }

                FormNodeObject(currentRoot, childTransform, index);
                FormMeshDataBuffer(currentRoot, mesh, index);

                var rendererMaterial = childTransform.GetComponent<MeshRenderer>().material;
                if (rendererMaterial == null)
                {
                    index++;
                    continue;
                }

                FormMaterialObject(currentRoot, rendererMaterial, index);
                index++;
            }

            return currentRoot;
        }

        /// <summary> Добавляет данные о метках в корневой объект glTF-файла. </summary>
        /// <param name="rootOfGLTF"> Корневой объект. </param>
        private static void AddMarksData(root rootOfGLTF)
        {
            rootOfGLTF._marksInfo = new _marksInfo();
            int marksCount = MarksController.Instance.AllMarks.Count;
            rootOfGLTF._marksInfo.marksCount = marksCount;
            rootOfGLTF._marksInfo._marks = new List<_mark>();
            foreach (Mark currentMark in MarksController.Instance.AllMarks)
            {
                Transform markTransform = currentMark.transform;
                _target.transform.SetParent(markTransform, true);
                Vector3 targetTransformLocalPosition = _target.transform.localPosition;
                Vector3 targetTransformLocalRotation = _target.transform.localRotation.eulerAngles;
                _mark currentMarkToSave = new _mark(targetTransformLocalPosition,
                                                    targetTransformLocalRotation,
                                                    $"ImageTarget{currentMark.Id}",
                                                    "Target");
                _target.transform.SetParent(null, true);
                rootOfGLTF._marksInfo._marks.Add(currentMarkToSave);
            }
        }


        /// <summary> Формирует объект узла и помещает его в структуру корня glTF-файла. </summary>
        /// <param name="currentRoot"> Объект корня glTF-файла. </param>
        /// <param name="childTransform"> Объект трансформа узла в исходной модели. </param>
        /// <param name="index"> Индекс узла. </param>
        private static void FormNodeObject(root currentRoot, Transform childTransform, int index)
        {
            currentRoot.nodes.Add(new node());
            currentRoot.scenes[0].nodes.Add(index);

            currentRoot.scenes[0].nodes[index] = index;
            currentRoot.nodes[index].name = childTransform.gameObject.name;
            currentRoot.nodes[index].mesh = index;
            Transform objTransform = childTransform.transform;
            currentRoot.nodes[index].Position = objTransform.localPosition;
            currentRoot.nodes[index].Scale = objTransform.localScale;
            currentRoot.nodes[index].Rotation = objTransform.localRotation;
        }

        /// <summary>
        /// Создает закодированный буфер и объекты аксессоров из меша и помещает их в структуру корня glTF-файла.
        /// </summary>
        /// <param name="currentRoot"> Объект корня glTF-файла. </param>
        /// <param name="mesh"> Объект меша узла. </param>
        /// <param name="index"> Индекс узла. </param>
        private static void FormMeshDataBuffer(root currentRoot, Mesh mesh, int index)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            int indByteLen = triangles.Length * 2;
            indByteLen = (indByteLen % 4 != 0) ? indByteLen + (4 - indByteLen % 4) : indByteLen;
            int vertByteLen = 3 * 4 * vertices.Length;
            vertByteLen = (vertByteLen % 4 != 0) ? vertByteLen + (4 - vertByteLen % 4) : vertByteLen;
            int byteLen = indByteLen + vertByteLen;
            byte[] bytes = new byte[byteLen];
            int currentByte = 0;

            float minX = Mathf.Infinity;
            float minY = Mathf.Infinity;
            float minZ = Mathf.Infinity;
            float maxX = -Mathf.Infinity;
            float maxY = -Mathf.Infinity;
            float maxZ = -Mathf.Infinity;

            foreach (int ind in triangles)
            {
                byte[] bytesOfInd = BitConverter.GetBytes((ushort) ind);
                foreach (byte b in bytesOfInd)
                {
                    bytes[currentByte++] = b;
                }
            }

            currentByte += indByteLen - triangles.Length * 2;

            foreach (Vector3 vec in vertices)
            {
                minX = Mathf.Min(minX, vec.x);
                maxX = Mathf.Max(maxX, vec.x);
                float coordinate = vec.x;
                byte[] bytesOfVec = BitConverter.GetBytes(coordinate);
                foreach (byte b in bytesOfVec)
                {
                    bytes[currentByte++] = b;
                }

                minY = Mathf.Min(minY, vec.y);
                maxY = Mathf.Max(maxY, vec.y);
                coordinate = vec.y;
                bytesOfVec = BitConverter.GetBytes(coordinate);
                foreach (byte b in bytesOfVec)
                {
                    bytes[currentByte++] = b;
                }

                minZ = Mathf.Min(minZ, vec.z);
                maxZ = Mathf.Max(maxZ, vec.z);
                coordinate = vec.z;
                bytesOfVec = BitConverter.GetBytes(coordinate);
                foreach (byte b in bytesOfVec)
                {
                    bytes[currentByte++] = b;
                }
            }

            string encodedBuffer = Convert.ToBase64String(bytes);
            currentRoot.bufferViews.Add(new bufferView(index, 0, mesh.triangles.Length * 2, 34963));
            currentRoot.bufferViews.Add(new bufferView(index, indByteLen, mesh.vertices.Length * 3 * 4, 34962));

            currentRoot.accessors.Add(new accessor(index * 2, 0, 5123, triangles.Length, "SCALAR"));
            currentRoot.accessors.Add(new accessor(index * 2 + 1, 0, 5126, vertices.Length, "VEC3"));

            currentRoot.accessors[index * 2].min = new [] {0f};
            currentRoot.accessors[index * 2].max = new float[] {vertices.Length - 1};

            if (float.IsPositiveInfinity(minX) || float.IsPositiveInfinity(minY) || float.IsPositiveInfinity(minZ) ||
                float.IsNegativeInfinity(maxX) || float.IsNegativeInfinity(maxY) || float.IsNegativeInfinity(maxZ))
            {
                currentRoot.accessors[index * 2 + 1].min = new [] {0f, 0f, 0f};
                currentRoot.accessors[index * 2 + 1].max = new [] {0f, 0f, 0f};
            }
            else
            {
                currentRoot.accessors[index * 2 + 1].min = new []{minX, minY, minZ};
                currentRoot.accessors[index * 2 + 1].max = new [] {maxX, maxY, maxZ};
            }

            currentRoot.meshes.Add(new mesh());
            currentRoot.meshes[index].primitives = new [] {new primitive(index * 2, index)};
            currentRoot.meshes[index].primitives[0].attributes.POSITION = index * 2 + 1;

            currentRoot.buffers.Add(new buffer(byteLen, encodedBuffer));
        }

        /// <summary> Создает данные о материале меша и помещает их в корень glTF-файла</summary>
        /// <param name="currentRoot"> Объект корня glTF-файла. </param>
        /// <param name="rendererMaterial"> Объект материала меша. </param>
        /// <param name="index"> Индекс узла. </param>
        private static void FormMaterialObject(root currentRoot, Material rendererMaterial, int index)
        {
            currentRoot.materials.Add(new material());
            currentRoot.materials[index].Color = rendererMaterial.color;
        }

        #endregion
    }
}