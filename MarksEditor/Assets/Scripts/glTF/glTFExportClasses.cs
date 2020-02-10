using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace MarksEditor.glTF
{
    /// <summary> Класс с константами для экспорта</summary>
    public static class glTFConvertionConsts
    {
        public const string bufferPrefix = "data:application/octet-stream;base64,";
    }

    /// <summary> Корневой класс glTF-файла </summary>
    [Serializable]
    public class root
    {
        public List<scene> scenes = new List<scene>();
        public List<node> nodes = new List<node>();
        public List<mesh> meshes = new List<mesh>();
        public List<buffer> buffers = new List<buffer>();
        public List<bufferView> bufferViews = new List<bufferView>();
        public List<accessor> accessors = new List<accessor>();
        public _marksInfo _marksInfo = new _marksInfo();
        public List<material> materials = new List<material>();
        public asset asset;
    }


    /// <summary> Класс сцены glTF-файла/// </summary>
    [Serializable]
    public class scene
    {
        public List<int> nodes = new List<int>();
    }

    /// <summary> Класс узла glTF-файла/// </summary>
    [Serializable]
    public class node
    {
        public int mesh;
        public string name;
        public float[] rotation;
        public float[] scale;
        public float[] translation;
    }

    /// <summary> Класс меша glTF-файла/// </summary>
    [Serializable]
    public class mesh
    {
        public primitive[] primitives;
    }

    /// <summary> Класс примитивов меша glTF-файла/// </summary>
    [Serializable]
    public class primitive
    {
        public attribute attributes = new attribute();
        public int indices;
        public int material;

        public primitive(int indices, int material)
        {
            this.indices = indices;
            this.material = material;
        }
    }

    /// <summary> Класс атрибутов примитивов меша glTF-файла/// </summary>
    [Serializable]
    public class attribute
    {
        public int POSITION;

        //        public int NORMAL;
        //        public int TANGENT;
        //        public int TEXCOORD_0;
        //        public int TEXCOORD_1;
        //        public int COLOR_0;
        //        public int JOINTS_0;
        //        public int WEIGHTS_0;
    }

    /// <summary> Класс буфера с данными о меше glTF-файла/// </summary>
    [Serializable]
    public class buffer
    {
        public string uri = "data:application/octet-stream;base64,";
        public int byteLength;


        public buffer(int byteLength, string encodedBuffer)
        {
            this.byteLength = byteLength;
            uri += encodedBuffer;
        }
    }

    /// <summary> Класс-описание частей буфера glTF-файла/// </summary>
    [Serializable]
    public class bufferView
    {
        public int buffer;
        public int byteOffset;
        public int byteLength;
        public int target; //вершины 34962 индексы вершин 34963

        public bufferView(int buffer, int byteOffset, int byteLength, int target)
        {
            this.buffer = buffer;
            this.byteOffset = byteOffset;
            this.byteLength = byteLength;
            this.target = target;
        }
    }

    /// <summary> Класс-описание аксессор glTF-файла</summary>
    [Serializable]
    public class accessor
    {
        public int bufferView;
        public int byteOffset;
        public int componentType;
        public int count;
        public string type;
        public float[] max = {0f, 0f, 0f};
        public float[] min = {0f, 0f, 0f};

        public accessor(int bufferView, int byteOffset, int componentType, int count, string type)
        {
            this.bufferView = bufferView;
            this.byteOffset = byteOffset;
            this.componentType = componentType;
            this.count = count;
            this.type = type;
        }
    }

    /// <summary> Класс описание ассета glTF-файла /// </summary>
    [Serializable]
    public class asset
    {
        public string version = "2.0";
    }


    /// <summary> Класс с парметрами всех меток </summary>
    [Serializable]
    public class _marksInfo
    {
        public int marksCount;
        public List<_mark> _marks = new List<_mark>();
    }


    /// <summary>Класс с параметрами меток </summary>
    [Serializable]
    public class _mark
    {
        public string target;
        public string name;
        public float x;
        public float y;
        public float z;
        public float rotationX;
        public float rotationY;
        public float rotationZ;

        public _mark(float x, float y, float z, float rotationX, float rotationY, float rotationZ, string name,
            string drawObjectName)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.name = name;
            this.target = drawObjectName;
        }
    }

    /// <summary> Описание материала для glTF-файла </summary>
    [Serializable]
    public class material
    {
        public pbrMetallicRoughness_material pbrMetallicRoughness = new pbrMetallicRoughness_material();
    }

    /// <summary> Параметр материала для glTF-файла </summary>
    [Serializable]
    public class pbrMetallicRoughness_material
    {
        public float[] baseColorFactor = {0f, 0f, 0f, 0f};
        public float metallicFactor = 0.5f;
        public float roughnessFactor = 0.5f;
    }
}