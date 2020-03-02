// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HoloCAD.UnityTubes;
using UnityEngine;

namespace HoloCAD.IO
{
    public static class SceneSerialization
    {
        /// <summary> Сериализует все трубы в json формат. </summary>
        /// <param name="tubes"> Все трубы на сцене. </param>
        /// <returns> Объект в формате json. </returns>
        public static byte[] SerializeScheme(IEnumerable<Tube> tubes)
        {
            ExpTubesArray array = new ExpTubesArray();
            foreach (Tube tube in tubes)
            {
                ExpTube expTube = new ExpTube();
                array.tubes.Add(expTube);
                expTube.diameter = tube.Data.diameter * 1000;
                expTube.width = expTube.diameter * 0.1;
                expTube.standard_name = tube.StandardName;
                tube.MapFragmentsWithOutgrowth(fragment => expTube.fragments.Add(ConvertToExportFormat(fragment)));
                expTube.fragments.RemoveAll(element => element == null);
            }

            return Encoding.UTF8.GetBytes(JsonUtility.ToJson(array));
        }
        
        /// <summary> Экспорт сцены. </summary>
        /// <remarks> Для выбора файла будет вызван диалог сохранения файла. </remarks>
        /// <param name="data"> Данные о трубах. </param>
        /// <param name="marks"> Коллекция меток для привязки труб. </param>
        /// <return> Массив всех труб на сцене. </return>
        public static void DeserializeScheme(byte[] data, List<Transform> marks = null)
        {
            ExpTubesArray array = JsonUtility.FromJson<ExpTubesArray>(Encoding.UTF8.GetString(data));
            marks = marks ?? new List<Transform>();

            for (int i = 0; i < array.tubes.Count; i++)
            {
                ExpTube expTube = array.tubes[i];
                Transform parent = i < marks.Count ? marks[i] : null;
                StartTubeFragment start = parent != null ? parent.Find("TubeStart(Clone)").GetComponent<StartTubeFragment>() 
                                                         : null;

                TubeFragment lastFragment;
                if (start == null) return;

                if (parent == null || start.HasChild)
                {
                    TubeLoader.TubeData currentTubeData =
                        TubeLoader.FindTubeData((float) expTube.diameter / 1000, expTube.standard_name);
                    Tube tube = new Tube(expTube.standard_name, currentTubeData);
                    lastFragment = tube.StartFragment;  
                }
                else
                {
                    start.Owner.Data = TubeLoader.FindTubeData((float) expTube.diameter / 1000, 
                                                                expTube.standard_name);
                    lastFragment = start;
                }
                
                foreach (ExpFragment expFragment in expTube.fragments)
                {
                    switch (expFragment.type)
                    {
                        case "Direct":
                            lastFragment.AddDirectFragment();
                            break;
                        case "Bended":
                            lastFragment.AddBendFragment();
                            break;
                    }

                    lastFragment = lastFragment.Child;
                    ConvertFromExportFormat(lastFragment, expFragment);
                }
            }
        }
        
        #region Private definitions
        
        /// <summary> Задает фрагменту параметры из файла для импорта данных. </summary>
        /// <param name="fragment"> Отрезок трубы. </param>
        /// <param name="expFragment"> Фрагмент из файла импорта. </param>
        private static void ConvertFromExportFormat(TubeFragment fragment, ExpFragment expFragment)
        {
            switch (expFragment.type)
            {
                case "Direct":
                    ((DirectTubeFragment) fragment).StartLength = (float) expFragment.length / 1000;
                    break;
                case "Bended":
                    ((BendedTubeFragment) fragment).StartAngle = expFragment.bendAngle;
                    ((BendedTubeFragment) fragment).SetRadius((float)expFragment.radius / 1000);
                    ((BendedTubeFragment) fragment).RotationAngle = expFragment.rotationAngle;
                    break;
            }

            DeserializeTransform(fragment, expFragment);
        }
        
        private static ExpFragment ConvertToExportFormat(TubeFragment fragment)
        {
            ExpFragment expFragment = new ExpFragment();

            switch (fragment)
            {
                case DirectTubeFragment dtf:
                    expFragment.type = "Direct";
                    expFragment.length = dtf.Length * 1000;
                    break;
                case BendedTubeFragment btf:
                    expFragment.type = "Bended";
                    expFragment.bendAngle = btf.Angle;
                    expFragment.rotationAngle = btf.RotationAngle;
                    expFragment.radius = btf.Radius * 1000;
                    break;
                default:
                    // Фланец не добавляем, он не является частью трубы
                    return null;
            }

            expFragment.transform = SerializeTransform(fragment, fragment.Owner.StartFragment.EndPoint.transform.position);
            return expFragment;
        }
        
        
        /// <summary>
        /// Превращает положение отрезка трубы в матрицу переноса 4х4 для правосторонней системы координат.
        /// </summary>
        /// <param name="fragment"> Отрезок трубы. </param>
        /// <param name="expFragment"> Фрагмент из файла импорта. </param>
        /// <returns> Матрица переноса 4х4. </returns>
        private static void DeserializeTransform(TubeFragment fragment, ExpFragment expFragment)
        {
            Matrix4x4 tr = new Matrix4x4();
            double[] transform = expFragment.transform;
            tr[0] = (float) -transform[0];
            tr[1] = (float) -transform[1];
            tr[2] = (float) -transform[2];
            tr[3] = (float) -transform[3];
            tr[4] = (float) -transform[4];
            tr[5] = (float) -transform[5];
            tr[6] = (float) -transform[6];
            tr[7] = (float) -transform[7];
            tr[8] = (float) -transform[8];
            tr[9] = (float) -transform[9];
            tr[10] = (float) -transform[10];
            tr[11] = (float) -transform[11];
            tr[12] = (float) -transform[12] / 1000;
            tr[13] = (float) -transform[13] / 1000;
            tr[14] = (float) -transform[14] / 1000;
            tr[15] = (float) -transform[15];

            Matrix4x4 toLOS = new Matrix4x4(new Vector4(1,  0,  0, 0), 
                                            new Vector4(0,  0, 1, 0),
                                            new Vector4(0,  -1,  0, 0),
                                            new Vector4(0,  0,  0, 1));

            Matrix4x4 result = toLOS * tr;
            Vector3 scale = new Vector3
            {
                x = new Vector4(result.m00, result.m10, result.m20, result.m30).magnitude,
                y = new Vector4(result.m01, result.m11, result.m21, result.m31).magnitude,
                z = new Vector4(result.m02, result.m12, result.m22, result.m32).magnitude
            };
            fragment.transform.localScale = scale;
        }
        
        /// <summary>
        /// Превращает положение отрезка трубы в матрицу переноса 4х4 для левосторонней системы координат.
        /// </summary>
        /// <param name="fragment"> Отрезок трубы. </param>
        /// <param name="tubeOrigin"> Координаты начала трубы. </param>
        /// <returns> Матрица переноса 4х4. </returns>
        private static double[] SerializeTransform(TubeFragment fragment, Vector3 tubeOrigin)
        {
            double[] result = new double[16];
            Matrix4x4 tr = fragment.transform.localToWorldMatrix;
            Matrix4x4 toROS = new Matrix4x4(new Vector4(1,  0,  0, 0), 
                                            new Vector4(0,  0, -1, 0),
                                            new Vector4(0,  1,  0, 0),
                                            new Vector4(-tubeOrigin.x,  -tubeOrigin.z,  tubeOrigin.y, 1));

            tr = toROS * tr;
            
            result[0] = -tr[0];
            result[1] = -tr[1];
            result[2] = -tr[2];
            result[3] = -tr[3];
            result[4] = -tr[4];
            result[5] = -tr[5];
            result[6] = -tr[6];
            result[7] = -tr[7];
            result[8] = -tr[8];
            result[9] = -tr[9];
            result[10] = -tr[10];
            result[11] = -tr[11];
            result[12] = -tr[12] * 1000;
            result[13] = -tr[13] * 1000;
            result[14] = -tr[14] * 1000;
            result[15] = -tr[15];
            
            return result;
        }

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
            public List<ExpFragment> fragments = new List<ExpFragment>();

            /// <summary> Диаметр трубы. </summary>
            public double diameter;

            /// <summary> Толщина стенки трубы. </summary>
            public double width;
        
            /// <summary> Наименование стандарта. </summary>
            public string standard_name;
        }

        /// <summary> Экспортируемый объект фрагмента трубы. </summary>
        [Serializable]
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class ExpFragment
        {
            public double[] transform;

            /// <summary> Тип фрагмента: Direct или Bended. </summary>
            public string type;

            /// <summary> Длина фрагмента (только если direct). </summary>
            public double length;

            /// <summary> Радиус погиба (только если bended). </summary>
            public double radius;

            /// <summary> Угол погиба (только если bended). </summary>
            public float bendAngle;

            /// <summary> Угол поворота (только если bended) </summary>
            public float rotationAngle;
        }

        #endregion
    }
}
