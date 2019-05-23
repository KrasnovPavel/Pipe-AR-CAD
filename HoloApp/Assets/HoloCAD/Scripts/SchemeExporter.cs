// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using HoloCAD.UnityTubes;
#if ENABLE_WINMD_SUPPORT
    using Windows.Storage;
    using Windows.Storage.Pickers;
#endif

namespace HoloCAD
{
    /// <summary> Класс, экспортирующий схему в файл. </summary>
    public static class SchemeExporter
    {
        /// <summary> Экспорт схемы. </summary>
        /// <param name="tubes"> Массив всех труб на сцене. </param>
        public static void Export(IEnumerable<Tube> tubes)
        {
#if ENABLE_WINMD_SUPPORT
            string data = SerializeScheme(tubes);
            UnityEngine.WSA.Application.InvokeOnUIThread(() => WriteFileInHololens(data), true);
#endif
        }

        #region Private definitioins

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
        private class ExpTube
        {
            // ReSharper disable once InconsistentNaming
            /// <summary> Массив экспортируемых фрагментов трубы. </summary>
            public List<ExpFragment> fragments = new List<ExpFragment>();

            /// <summary> Диаметр трубы. </summary>
            public double diameter;

            /// <summary> Толщина стенки трубы. </summary>
            public double width;
        }

        /// <summary> Экспортируемый объект фрагмента трубы. </summary>
        [Serializable]
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
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
            public int bendAngle;
        }

        // ReSharper disable once UnusedMember.Local
        private static string SerializeScheme(IEnumerable<Tube> tubes)
        {
            ExpTubesArray array = new ExpTubesArray();
            foreach (Tube tube in tubes)
            {
                ExpTube expTube = new ExpTube();
                array.tubes.Add(expTube);
                expTube.diameter = tube.Data.diameter * 1000;
                expTube.width = expTube.diameter * 0.1;

                for (int i = 1; i < tube.Fragments.Count; i++)
                {
                    ExpFragment fragment = new ExpFragment();

                    switch (tube.Fragments[i])
                    {
                        case DirectTubeFragment dtf:
                            fragment.type = "Direct";
                            fragment.length = dtf.Length * 1000;
                            break;
                        case BendedTubeFragment btf:
                            fragment.type = "Bended";
                            fragment.bendAngle = btf.Angle;
                            fragment.radius = btf.Radius * 1000;
                            break;
                        default:
                            continue;
                    }

                    fragment.transform = SerializeTransform(tube.Fragments[i], tube.Fragments[1].transform.position);

                    expTube.fragments.Add(fragment);
                }
            }

            return JsonUtility.ToJson(array);
        }

        private static double[] SerializeTransform(TubeFragment fragment, Vector3 tubeOrigin)
        {
            double[] result = new double[16];
            

            Matrix4x4 tr = fragment.transform.localToWorldMatrix;

            Matrix4x4 toROS = new Matrix4x4(new Vector4(1,  0,  0, 0), 
                                            new Vector4(0,  0, -1, 0),
                                            new Vector4(0, -1,  0, 0),
                                            new Vector4(-tubeOrigin.x,  tubeOrigin.z,  tubeOrigin.y, 1));

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
        
#if ENABLE_WINMD_SUPPORT
        private static async void WriteFileInHololens(string data)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.DefaultFileExtension = ".json";
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("HoloCAD json", new List<string>() { ".json" });
            StorageFile file = await savePicker.PickSaveFileAsync();

            await FileIO.WriteTextAsync(file, data);
        }    
#endif
        
        #endregion
    }
}