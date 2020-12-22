// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HoloCAD.Tubes.Model
{
    public static class TubesManager
    {
        public static List<Tube> AllTubes = new List<Tube>();
        
        
        /// <summary> Экспортирует все трубы в json формат. </summary>
        /// <returns> Объект в формате json. </returns>
        public static byte[] Serialize()
        {
            ExpTubesArray array = new ExpTubesArray();
            foreach (Tube tube in AllTubes)
            {
                ExpTube expTube = new ExpTube();
                array.tubes.Add(expTube);
                expTube.diameter      = tube.Diameter    * 1000;
                expTube.width         = expTube.diameter * 0.1;
                expTube.standard_name = tube.TubeData.standard_name;
                expTube.points.Add(new ExpVector(tube.StartFlange.Origin, 0));
                expTube.points.AddRange(tube.Points.Select(p => new ExpVector(p.Origin, p.BendRadius)));
                expTube.points.Add(new ExpVector(tube.EndFlange.Origin, 0));
            }
    
            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(array));
        }
        
        #region Private definitions
    
        /// <summary> Коренной объект Json. </summary>
        [Serializable]
        private class ExpTubesArray
        {
            // ReSharper disable once InconsistentNaming
            /// <summary> Массив экспортируемых труб. </summary>
            public List<ExpTube> tubes = new List<ExpTube>();
        }
    
        /// <summary> Экспортируемый объект трубы. </summary>
        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class ExpTube
        {
            /// <summary> Массив экспортируемых фрагментов трубы. </summary>
            public List<ExpVector> points = new List<ExpVector>();
    
            /// <summary> Диаметр трубы. </summary>
            public double diameter;
    
            /// <summary> Толщина стенки трубы. </summary>
            // ReSharper disable once NotAccessedField.Local
            public double width;
        
            /// <summary> Наименование стандарта. </summary>
            public string standard_name;
        }
    
        /// <summary> Экспортируемый объект фрагмента трубы. </summary>
        [Serializable]
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class ExpVector
        {
            public ExpVector(Vector3 vec, float bendRadius)
            {
                x           = vec.x;
                y           = vec.z;
                z           = vec.y;
                bend_radius = bendRadius;
            }
            
            public double x;
            public double y;
            public double z;
            public double bend_radius;
        }
    
        #endregion
    }
}
