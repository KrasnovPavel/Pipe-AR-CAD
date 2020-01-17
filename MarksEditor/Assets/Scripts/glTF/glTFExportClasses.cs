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
        public List<scene> scenes;
        public List<node> nodes;
        public List<mesh> meshes;
        public List<buffer> buffers;
        public List<bufferView> bufferViews;
        public List<accessor> accessors;
        public _marksInfo _marksInfo;
        public List<material> materials;
        public asset asset;
    }


    /// <summary> Класс сцены glTF-файла/// </summary>
    [Serializable]
    public class scene
    {
        public List<int> nodes;
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
        public attribute attributes;
        public int indices;
        public int material;
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
    }

    /// <summary> Класс-описание частей буфера glTF-файла/// </summary>
    [Serializable]
    public class bufferView
    {
        public int buffer;
        public int byteOffset;
        public int byteLength;
        public int target; //вершины 34962 индексы вершин 34963
    }

    /// <summary> Класс-описание наследника glTF-файла/// </summary>
    [Serializable]
    public class accessor
    {
        public int bufferView;
        public int byteOffset;
        public int componentType;
        public int count;
        public string type;
        public float[] max;
        public float[] min;
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
        public List<_mark> _marks;
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
        public pbrMetallicRoughness_material pbrMetallicRoughness;
    }
    
    /// <summary> Параметр материала для glTF-файла </summary>
    [Serializable]
    public class pbrMetallicRoughness_material
    {
        public float[] baseColorFactor  = {1.000f, 0.766f, 0.336f, 1.0f};
        public float metallicFactor = 0.5f;
        public float roughnessFactor = 0.5f;
    }
}
