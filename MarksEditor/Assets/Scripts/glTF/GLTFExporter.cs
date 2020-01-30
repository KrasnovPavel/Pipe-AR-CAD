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
                
            Debug.Log($"dsadas");
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
            
            int index = 0;
            foreach (Transform childTransform in targetTransform.GetComponentInChildren<Transform>())
            {
                Debug.Log($"{index}");
                Mesh mesh;
                try{
                    mesh = childTransform.GetComponent<MeshFilter>().mesh;
                }
                catch{
                    continue;
                }
                
                FormNodeObject(currentRoot,childTransform,index);
                FormMeshDataBuffer(currentRoot, mesh, index);
                
                
                Material rendererMaterial ;
                try{
                    rendererMaterial = childTransform.GetComponent<MeshRenderer>().material;
                }
                catch{
                    index++;
                    continue;
                }

                FormMaterialObject(currentRoot,rendererMaterial,index);
                
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
                Target.transform.SetParent(markTransform, true);
                _mark currentMarkToSave = new _mark(Target.transform.localPosition.x,
                    Target.transform.localPosition.y, Target.transform.localPosition.z,
                    Target.transform.localRotation.eulerAngles.x,Target.transform.localRotation.eulerAngles.y, Target.transform.localRotation.eulerAngles.z, $"ImageTarget{currentMark.Id}",
                    "Target");
                Target.transform.SetParent(null, true);
                rootOfglTf._marksInfo._marks.Add(currentMarkToSave);
            }
        }

        
        /// <summary>Формирует объект узла и помещает его в структуру корня glTF-файла</summary>
        /// <param name="currentRoot">Объект корня glTF-файла</param>
        /// <param name="childTransform">Объект трансформа узла в исходной модели</param>
        /// <param name="index">Индекс узла</param>
        private void FormNodeObject(root currentRoot, Transform childTransform, int index )
        {
            currentRoot.nodes.Add(new node());
            currentRoot.scenes[0].nodes.Add(index);
                
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
        }
        
        /// <summary> Создает закодированный буфер из меша, объекты наследников и помещает их в структуру корня glTF-файла</summary>
        /// <param name="currentRoot">Объект корня glTF-файла</param>
        /// <param name="mesh">Объект меша узла</param>
        /// <param name="index">Индекс узла</param>
        private void  FormMeshDataBuffer(root currentRoot, Mesh mesh, int index )
        {
            
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
                currentRoot.bufferViews.Add(new bufferView(index,0,mesh.triangles.Length*2,34963));
                currentRoot.bufferViews.Add(new bufferView(index,indByteLen,mesh.vertices.Length*3*4,34962));

                currentRoot.accessors.Add(new accessor(index*2, 0,5123,triangles.Length,"SCALAR"));
                currentRoot.accessors.Add(new accessor(index*2+1, 0,5126,vertices.Length,"VEC3"));
                
                currentRoot.accessors[index*2].min = new float[1]{0};
                currentRoot.accessors[index*2].max = new float[1]{ vertices.Length - 1};
                
                if (Mathf.Infinity == minX || Mathf.Infinity == minY || Mathf.Infinity == minZ ||
                    -Mathf.Infinity == maxX || -Mathf.Infinity == maxY || -Mathf.Infinity == maxZ)
                {
                    currentRoot.accessors[index*2+1].min = new float[3]{0,0,0};
                    currentRoot.accessors[index * 2 + 1].max = new float[3] {0, 0, 0};
                }
                else
                {
                    currentRoot.accessors[index * 2 + 1].min = new float[3] {minX, minY, minZ};
                    currentRoot.accessors[index * 2 + 1].max = new float[3] {maxX, maxY, maxZ};
                }

                currentRoot.meshes.Add(new mesh());
                currentRoot.meshes[index].primitives = new primitive[1]{new primitive(index*2,index)};
                currentRoot.meshes[index].primitives[0].attributes.POSITION = index*2+1;
                
                currentRoot.buffers.Add(new buffer(byteLen,encodedBuffer));
        }

        /// <summary> Создает данные о материале меша и помещает их в корень glTF-файла</summary>
        /// <param name="currentRoot">Объект корня glTF-файла</param>
        /// <param name="rendererMaterial">Объект материала меша</param>
        /// <param name="index">Индекс узла</param>
        private void FormMaterialObject(root currentRoot, Material rendererMaterial, int index)
        {
            currentRoot.materials.Add(new material());
            currentRoot.materials[index].pbrMetallicRoughness.baseColorFactor = new[]
            {
                rendererMaterial.color.r, rendererMaterial.color.g, rendererMaterial.color.b,
                rendererMaterial.color.a
            };
        }
    }
    
   
}
