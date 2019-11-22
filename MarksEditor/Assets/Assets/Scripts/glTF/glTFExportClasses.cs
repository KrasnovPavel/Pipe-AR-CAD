using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarksEditor.glTF
{
        
    
        /// <summary> Корневой класс glTF-файла/// </summary>
        [Serializable]
        public class root
        {
            public scene[] scenes;
            public node[] nodes;
            public mesh[] meshes;
            public buffer[] buffers;
            public bufferView[] bufferViews;
            public accessor[] accessors;
            public asset asset;
        }
    
        
        /// <summary> Класс сцены glTF-файла/// </summary>
        [Serializable]
        public class scene
        {
            public int[] nodes;
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
    
        /// <summary> Класс описание ассета glTF-файла/// </summary>
        [Serializable]
        public class asset
        {
            public string version = "2.0";
        }

        public static class glTFConvertionConsts
        {
            public const string bufferPrefix  = "data:application/octet-stream;base64,";
        }
}
