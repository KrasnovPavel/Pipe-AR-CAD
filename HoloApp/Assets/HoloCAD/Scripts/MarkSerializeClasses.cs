// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Linq;

namespace HoloCAD
{
    [Serializable]
    public struct SerializedMarks
    {
        public SerializedMark[] AllMarks;

        public SerializedMarks(SerializedMark[] allMarks)
        {
            AllMarks = allMarks.ToArray();
        }
    }
    
    [Serializable]
    public struct SerializedMark
    {
        public float X;
        public float Y;
        public float Z;
        public float RotationX;
        public float RotationY;
        public float RotationZ;
        public string Name;
        public string DrawObjectName;

        public SerializedMark(float x, float y, float z, float rotationX, float rotationY, float rotationZ, string name, string drawObjectName)
        {
            X = x;
            Y = y;
            Z = z;
            RotationX = rotationX;
            RotationY = rotationY;
            RotationZ = rotationZ;
            Name = name;
            DrawObjectName = drawObjectName;
        }
    }
}