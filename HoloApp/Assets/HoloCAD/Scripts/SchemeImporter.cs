using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using HoloCAD.UnityTubes;
using SFB;

#if ENABLE_WINMD_SUPPORT
    using Windows.Storage;
    using Windows.Storage.Pickers;
#endif
namespace HoloCAD
{

    /// <summary> Класс, импортирующий схему из файла. </summary>
    public static class SchemeImporter
    {
        /// <summary> Экспорт схемы. </summary>
        /// <remarks> Для выбора файла будет вызван диалог сохранения файла. </remarks>
        /// <param name="tubes"> Массив всех труб на сцене. </param>
        public static IEnumerable<Tube> Import()
        {
            IEnumerable<Tube> tubes = new List<Tube>();
            string data = "";
#if ENABLE_WINMD_SUPPORT
            UnityEngine.WSA.Application.InvokeOnUIThread(() => ReadFileOnHololens(data), true);
            
#else
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                Cursor.visible = true;
                data = ReadFileOnPC();
            }, true);
#endif
            tubes = DeserializeScheme(data);
            return tubes;
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
        private class ExpTube
        {
            // ReSharper disable once InconsistentNaming
            /// <summary> Массив экспортируемых фрагментов трубы. </summary>
            public List<ExpFragment> fragments = new List<ExpFragment>();

            // ReSharper disable once InconsistentNaming
            /// <summary> Диаметр трубы. </summary>
            public double diameter;

            // ReSharper disable once InconsistentNaming
            /// <summary> Толщина стенки трубы. </summary>
            public double width;

            // ReSharper disable once InconsistentNaming
            /// <summary> Наименование стандарта. </summary>
            public string standart_name;
        }

        /// <summary> Экспортируемый объект фрагмента трубы. </summary>
        [Serializable]
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        private class ExpFragment
        {
            public double[] transform;

            // ReSharper disable once InconsistentNaming
            /// <summary> Тип фрагмента: Direct или Bended. </summary>
            public string type;

            // ReSharper disable once InconsistentNaming
            /// <summary> Длина фрагмента (только если direct). </summary>
            public double length;

            // ReSharper disable once InconsistentNaming
            /// <summary> Радиус погиба (только если bended). </summary>
            public double radius;

            // ReSharper disable once InconsistentNaming
            /// <summary> Угол погиба (только если bended). </summary>
            public float bendAngle;
        }
#if ENABLE_WINMD_SUPPORT
        /// <summary> Запись файла на очках Hololens. Перед записью вызывает диалог сохранения файла. </summary>
        /// <param name="data"> Данные, которые будут записаны в файл. </param>
        private static async void ReadFileOnHololens(string data)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.DefaultFileExtension = ".json";
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("HoloCAD json", new List<string>() { ".json" });
            StorageFile file = await savePicker.PickSaveFileAsync();

            await FileIO.WriteTextAsync(file, data);
        }    
#endif

        /// <summary> Загрузка файла на компьютере. Перед загрузки вызывает диалог загрузки файла. </summary>
        /// <returns> Возвращает текст json-файла с описанием сцены </returns>
        private static string ReadFileOnPC()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Open scheme", "", "json", false);
            if (paths.Length == 0) return null;
            string data = System.IO.File.ReadAllText(paths[0]);
            return data;
        }

        private static IEnumerable<Tube> DeserializeScheme(string data)
        {
            List<Tube> tubes = new List<Tube>();
            ExpTubesArray array = JsonUtility.FromJson<ExpTubesArray>(data);
            foreach (ExpTube expTube in array.tubes)
            {
                TubeLoader.TubeData currentTubeData =
                    TubeLoader.FindTubeData((float) expTube.diameter  / 1000, expTube.standart_name);
                Tube tube = new Tube(expTube.standart_name, currentTubeData);
                TubeUnityManager.CreateStartTubeFragment(tube);
                TubeFragment lastFragment = tube.StartFragment;
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
                
                tubes.Add(tube);
            }

            return tubes;
        }
        
        /// <summary>
        /// Задает фрагменту праметры из файла для импорта данных
        /// </summary>
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
            Transform transformOfFragment = fragment.transform;
            Vector3 scale;
            scale.x = new Vector4(result.m00, result.m10, result.m20, result.m30).magnitude;
            scale.y = new Vector4(result.m01, result.m11, result.m21, result.m31).magnitude;
            scale.z = new Vector4(result.m02, result.m12, result.m22, result.m32).magnitude;
            transformOfFragment.localScale = scale;
            if (expFragment.type == "Bended")
            {
                BendedTubeFragment tubeFragment = (BendedTubeFragment) fragment;
                tubeFragment.RotationAngle = Vector3.Angle(tubeFragment.Parent.EndPoint.transform.right,
                    transformOfFragment.transform.right);
            }
        }





        #endregion
    }
}