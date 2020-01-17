using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using HoloCore;
using UnityEngine;

using static SFB.StandaloneFileBrowser;


namespace MarksEditor.glTF
{
  
    
    /// <summary> Класс экспорта модели в glTF</summary>
    public class GLTFExporter : Singleton<GLTFExporter>
    {
        /// <summary> Экспортируемый объект </summary>
        public GameObject Target;
        
        /// <summary> Экспортирует объект в glTF формат </summary>
        public void ExportglTFFile()
        {
            string filePath = SaveDialog();
            root rootOfglTF = FormglTFRoot();
            AddMarksData(rootOfglTF);
            string glTFString = FormglTFString(rootOfglTF);
            WriteglTFStringIntoFile(filePath,glTFString);
        }

        

        /// <summary> Записывает данные в файл по указанному пути </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="glTfString">Данные</param>
        private void WriteglTFStringIntoFile(string filePath, string glTfString)
        {
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(glTfString);
            streamWriter.Close();
        }

        /// <summary> Записывает glTF-объект в строку </summary>
        /// <param name="currentRoot">glTF-объект</param>
        /// <returns>Строка-содержимое glTF-файла</returns>
        private string FormglTFString(root currentRoot)
        {
            DataContractJsonSerializer dc = new DataContractJsonSerializer(typeof(root));
            
            string gltfString = JsonUtility.ToJson(currentRoot);
            return gltfString;
        }
        
        /// <summary> Открывает диалог сохранения файла </summary>
        /// <returns> Путь к указанному файлу </returns>
        private string SaveDialog()
        {
            string filePath = SaveFilePanel("Сохраните файл!","","undefined","glTF");
            if (filePath.Length == 0)
            {
                throw new Exception("Файл не выбран!");
            }
            
            return filePath;
        }

        /// <summary>Создает glTF-объект из указанного объекта </summary>
        /// <returns>Созданный glTF-объект</returns>
        private root FormglTFRoot()
        {
            root currentRoot = new root();
            currentRoot.scenes = new List<scene>(new scene[]{new scene()});
            Transform targetTransform = Target.transform;
            currentRoot.nodes = new List<node>();
            
            currentRoot.scenes[0].nodes = new List<int>();
            currentRoot.meshes = new List<mesh>();
            currentRoot.buffers = new List<buffer>();
            
            currentRoot.bufferViews = new List<bufferView>();
            currentRoot.accessors =new List<accessor>();
            currentRoot.materials = new List<material>();
            
            int index = 0;
            foreach (Transform childTransform in targetTransform.GetComponentInChildren<Transform>())
            {
                Debug.Log(index);
                Mesh mesh;
                try{
                    mesh = childTransform.GetComponent<MeshFilter>().mesh;
                }
                catch{
                    continue;
                }
                currentRoot.nodes.Add(new node());
                currentRoot.scenes[0].nodes.Add(index);
                Vector3[] vertices = mesh.vertices;
                int[] triangles = mesh.triangles;
                int indByteLen = triangles.Length*2;
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
                    foreach(byte b in bytesOfInd)
                    {
                        bytes[currentByte++] = b;
                    }
                }
                
                currentByte +=indByteLen - triangles.Length * 2;
                
                int n = 0;
            
                foreach (Vector3 vec in vertices)
                {
                    minX = Mathf.Min(minX, vec.x);
                    maxX = Mathf.Max(maxX, vec.x);
                    float coordinate = vec.x;
                    byte[] bytesOfVec = BitConverter.GetBytes(coordinate);
                    foreach(byte b in bytesOfVec)
                    {
                        bytes[currentByte++] = b;
                    }
                
                    minY = Mathf.Min(minY, vec.y);
                    maxY = Mathf.Max(maxY, vec.y);
                    coordinate = vec.y;
                    bytesOfVec = BitConverter.GetBytes(coordinate);
                    foreach(byte b in bytesOfVec)
                    {
                        bytes[currentByte++] = b;
                    }
            
                    minZ = Mathf.Min(minZ, vec.z);
                    maxZ = Mathf.Max(maxZ, vec.z);
                    coordinate = vec.z;
                    bytesOfVec = BitConverter.GetBytes(coordinate);
                    foreach(byte b in bytesOfVec)
                    {
                        bytes[currentByte++] = b;
                    }
                }
                string encodedBuffer = Convert.ToBase64String(bytes);
                currentRoot.scenes[0].nodes[index] = index;
                currentRoot.nodes[index].name = childTransform.gameObject.name;
                currentRoot.nodes[index].mesh = index;
                Transform objTransform = childTransform.transform;
                currentRoot.nodes[index].translation = new float[3]{
                    objTransform.localPosition.x,
                    objTransform.localPosition.y,
                    objTransform.localPosition.z
                };
                currentRoot.nodes[index].scale = new float[3]{
                    objTransform.localScale.x,
                    objTransform.localScale.y,
                    objTransform.localScale.z
                };
                currentRoot.nodes[index].rotation = new float[4]
                {
                    objTransform.localRotation.x,
                    objTransform.localRotation.y,
                    objTransform.localRotation.z,
                    objTransform.localRotation.w
                };
                
                currentRoot.meshes.Add(new mesh());
                currentRoot.meshes[index].primitives = new primitive[1]{new primitive()};
                currentRoot.meshes[index].primitives[0].attributes = new attribute();
                currentRoot.meshes[index].primitives[0].attributes.POSITION = index*2+1;
                currentRoot.meshes[index].primitives[0].indices = index*2;
                currentRoot.meshes[index].primitives[0].material = index;
                currentRoot.buffers.Add(new buffer());
                currentRoot.buffers[index].uri += encodedBuffer;
                currentRoot.buffers[index].byteLength = byteLen;
                currentRoot.bufferViews.Add(new bufferView());
                currentRoot.bufferViews.Add(new bufferView());
                currentRoot.bufferViews[index*2].byteLength = triangles.Length*2;
                currentRoot.bufferViews[index*2].buffer = index;
                currentRoot.bufferViews[index*2].target = 34963;
                currentRoot.bufferViews[index*2].byteOffset = 0;
                currentRoot.bufferViews[index*2+1].byteLength = vertices.Length*3*4;
                currentRoot.bufferViews[index*2+1].buffer = index;
                currentRoot.bufferViews[index*2+1].target = 34962;
                currentRoot.bufferViews[index*2+1].byteOffset = indByteLen;
                currentRoot.accessors.Add(new accessor());
                currentRoot.accessors.Add(new accessor());
                currentRoot.accessors[index*2].bufferView = index*2;
                currentRoot.accessors[index*2].byteOffset = 0;
                currentRoot.accessors[index*2].count = triangles.Length;
                currentRoot.accessors[index*2].componentType = 5123;
                currentRoot.accessors[index*2].type = "SCALAR";
                currentRoot.accessors[index*2].min = new float[1]{0};
                currentRoot.accessors[index*2].max = new float[1]{vertices.Length - 1};
                currentRoot.accessors[index*2+1].bufferView = index*2+1;
                currentRoot.accessors[index*2+1].byteOffset = 0;
                currentRoot.accessors[index*2+1].count = vertices.Length;
                currentRoot.accessors[index*2+1].componentType = 5126;
                currentRoot.accessors[index*2+1].type = "VEC3";
                if (Mathf.Infinity == minX || Mathf.Infinity == minY || Mathf.Infinity == minZ ||
                    -Mathf.Infinity == maxX || -Mathf.Infinity == maxY || -Mathf.Infinity == maxZ)
                {
                    currentRoot.accessors[index*2+1].min = new float[3]{0,0,0};
                    currentRoot.accessors[index * 2 + 1].max = new float[3] {0, 0, 0};
                }
                currentRoot.accessors[index*2+1].min = new float[3]{minX,minY,minZ};
                currentRoot.accessors[index * 2 + 1].max = new float[3] {maxX, maxY, maxZ};
                currentRoot.materials.Add(new material());
                Material rendererMaterial ;
                try{
                    rendererMaterial = childTransform.GetComponent<MeshRenderer>().material;
                }
                catch{
                    continue;
                }
                currentRoot.materials[index].pbrMetallicRoughness = new pbrMetallicRoughness_material();
                currentRoot.materials[index].pbrMetallicRoughness.baseColorFactor = new[]
                {
                    rendererMaterial.color.r, rendererMaterial.color.g, rendererMaterial.color.b,
                    rendererMaterial.color.a
                };
                index++;
                
            }
            
            Debug.Log($"{index}, {currentRoot.nodes.Count}");

            return currentRoot;
        }
        
        /// <summary> Добавляет данные о метках в корневой объект glTF-файла </summary>
        /// <param name="rootOfglTf">Корневой объект</param>
        private void AddMarksData(root rootOfglTf)
        {
            rootOfglTf._marksInfo = new _marksInfo();
            int marksCount = MarksController.Instance.AllMarks.Count;
            rootOfglTf._marksInfo.marksCount = marksCount;
            rootOfglTf._marksInfo._marks = new List<_mark>();
            foreach (MarkOnScene currentMark in MarksController.Instance.AllMarks)
            {
                Transform markTransform = currentMark.transform;
                _mark currentMarkToSave = new _mark(markTransform.position.x,
                    markTransform.position.y, markTransform.position.z,
                    markTransform.eulerAngles.x,markTransform.eulerAngles.y, markTransform.eulerAngles.z, $"ImageTarget{currentMark.Id}",
                    "Target");
                rootOfglTf._marksInfo._marks.Add(currentMarkToSave);
            }
        }


    }
}
