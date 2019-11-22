// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using HoloCAD.UnityTubes;
using SFB;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
    using System;
    using Windows.Storage;
    using Windows.Storage.Pickers;
#endif

namespace HoloCAD.IO
{
    /// <summary> Класс, импортирующий сцену из файла. </summary>
    public static class SceneImporter
    {
        /// <summary> Экспорт сцены. </summary>
        /// <remarks> Для выбора файла будет вызван диалог сохранения файла. </remarks>
        /// <return> Массив всех труб на сцене. </return>
        public static void Import(List<Transform> marks = null)
        {
            _marks = marks ?? new List<Transform>();
#if ENABLE_WINMD_SUPPORT
            UnityEngine.WSA.Application.InvokeOnUIThread(() => ReadFileOnHololens(), true);
#else
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                Cursor.visible = true;
                _jsonFile = ReadFileOnPC();
                DeserializeScheme();
            }, true);
#endif
        }

        #region Private definitions

        private static string _jsonFile = "";
        private static List<Transform> _marks;

#if ENABLE_WINMD_SUPPORT
        /// <summary> Запись файла на очках Hololens. Перед записью вызывает диалог сохранения файла. </summary>
        private static async void ReadFileOnHololens()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".json");
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file == null) return;
            SceneExporter.File = file;

            _jsonFile = await FileIO.ReadTextAsync(file);
            UnityEngine.WSA.Application.InvokeOnAppThread(DeserializeScheme, true);
        }    
#else

        /// <summary> Загрузка файла на компьютере. Перед загрузкой вызывает диалог выбора файла. </summary>
        /// <returns> Возвращает текст json-файла с описанием сцены. </returns>
        private static string ReadFileOnPC()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Open scheme", "", "json", false);
            if (paths.Length == 0) return null;
            SceneExporter.FilePath = paths[0];
            return System.IO.File.ReadAllText(paths[0]);
        }
#endif

        private static void DeserializeScheme()
        {
            ExpTubesArray array = JsonUtility.FromJson<ExpTubesArray>(_jsonFile);

            for (int i = 0; i < array.tubes.Count; i++)
            {
                ExpTube expTube = array.tubes[i];
                Transform parent = i < _marks.Count ? _marks[i] : null;
                StartTubeFragment start = parent != null ? parent.Find("TubeStart(Clone)").GetComponent<StartTubeFragment>() 
                                                         : null;

                TubeFragment lastFragment;
                if (parent == null || (start != null && start.HasChild))
                {
                    TubeLoader.TubeData currentTubeData =
                        TubeLoader.FindTubeData((float) expTube.diameter / 1000, expTube.standart_name);
                    Tube tube = new Tube(expTube.standart_name, currentTubeData);
                    lastFragment = tube.StartFragment;  
                }
                else
                {
                    start.Owner.Data = TubeLoader.FindTubeData((float) expTube.diameter / 1000, 
                                                                expTube.standart_name);
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
        
        /// <summary> Задает фрагменту параметры из файла для импорта данных </summary>
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
        
        #endregion
    }
}
