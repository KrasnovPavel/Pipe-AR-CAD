// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HoloCore;
using HoloCore.UI.Transparent;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using SFB;
using UnityEngine;
using UnityEngine.Windows;

#if ENABLE_WINMD_SUPPORT
    using System;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.Storage.Streams;
#endif

namespace HoloCAD.UI.Docs2D
{
    /// <summary> Класс контролирующий работу обозревателей 2D документов. </summary>
    public class Controller2D : Singleton<Controller2D>
    {
        /// <summary> Префаб обозревателя PDF. </summary>
        public GameObject ViewerPDFPrefab;

        /// <summary> Префаб обозревателя изображений. </summary>
        public GameObject ViewerImagePrefab;

        /// <summary> Префаб обозревателя текста. </summary>
        public GameObject ViewerTextPrefab;
        
        /// <summary> Коллекция обозревателей. </summary>
        public static ReadOnlyCollection<Viewer2D> Viewers2D => _viewers2D.ToReadOnlyCollection();
        
        /// <summary> Открывает файл, используя окно выбора файла и создаёт новый обозреватель. </summary>
        public static void OpenFile()
        {
#if ENABLE_WINMD_SUPPORT
            UnityEngine.WSA.Application.InvokeOnUIThread(() => ReadFileOnHololens(), true);
#else
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                Cursor.visible = true;
                ReadFileOnPC();
            }, true);
#endif
        }

        /// <summary> Скрыть все обозреватели. </summary>
        public static void HideAll()
        {
            foreach (var viewer2D in _viewers2D) viewer2D.Hide();
        }

        /// <summary> Показать все обозреватели. </summary>
        public static void ShowAll()
        {
            foreach (var viewer2D in _viewers2D) viewer2D.Show();
        }

        /// <summary> Устанавливает прозрачность всем 2D-документам. </summary>
        /// <param name="alpha"> Прозрачность. </param>
        public static void SetTransparency(float alpha)
        {
            foreach (var viewer in _viewers2D)
            {
                viewer.GetComponent<Transparency>().Alpha = alpha;
            }
        }
        
        /// <summary> Функция управления прозрачностью 2D-документов с помощью слайдера из MRTK. </summary>
        /// <param name="data"> Данные слайдера. </param>
        public void OnAlphaSliderUpdated(SliderEventData data)
        {
            SetTransparency(data.NewValue + 0.2f);
        }

        /// <summary> Функция, обрабатывающая удаление обозреветеля. </summary>
        /// <param name="viewer"> Удаляемый обозреватель. </param>
        public static void ViewerDestroyed(Viewer2D viewer)
        {
            _viewers2D.Remove(viewer);
        }

        #region Private definitions

        private static List<Viewer2D> _viewers2D = new List<Viewer2D>();

        /// <summary> Функция, вызываемая после чтения файла. Создаёт соответствующий объект обозревателя. </summary>
        /// <param name="byteArray"> Содержимое файла. </param>
        /// <param name="path"> Путь к файлу. </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void OnFileOpened(byte[] byteArray, string path)
        {
            Transform t = Camera.main.transform;
            var pos = Quaternion.AngleAxis(t.rotation.eulerAngles.y, Vector3.up) * Vector3.forward * 1.5f + t.position;
            var rot = Quaternion.AngleAxis(t.rotation.eulerAngles.y, Vector3.up);

            GameObject go;
            
            switch (path.Split('.').Last().ToLower())
            {
                case "pdf":
                    go = Instantiate(Instance.ViewerPDFPrefab, pos, rot);
                    break;
                case "jpg": // fallthrough
                case "png":
                    go = Instantiate(Instance.ViewerImagePrefab, pos, rot);
                    break;
                case "txt":
                    go = Instantiate(Instance.ViewerTextPrefab, pos, rot);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(path), "Недопустимый формат файла.");
            }

            go.GetComponent<Viewer2D>().Init(byteArray, path);
            _viewers2D.Add(go.GetComponent<Viewer2D>());
        }
        
#if ENABLE_WINMD_SUPPORT
        /// <summary> Чтение файла на очках Hololens. Перед чтением вызывает диалог открытия файла. </summary>
        private static async void ReadFileOnHololens()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".pdf");
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".txt");
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file == null) return;
            var b = await FileIO.ReadBufferAsync(file);
            DataReader dataReader = DataReader.FromBuffer(b);
            byte[] bytes = new byte[b.Length];
            dataReader.ReadBytes(bytes);
            UnityEngine.WSA.Application.InvokeOnAppThread(() => OnFileOpened(bytes, file.Path), true);
        }    
#endif
        
        /// <summary> Чтение файла на PC. Перед чтением вызывает диалог открытия файла. </summary>
        private static void ReadFileOnPC()
        {
            var filter = new ExtensionFilter("2D files", new [] { "pdf", "jpg", "png", "txt" });
            var paths = StandaloneFileBrowser.OpenFilePanel("Open scheme", "", new []{filter}, false);
            byte[] byteArray = File.ReadAllBytes(paths[0]);
            OnFileOpened(byteArray, paths[0]);
        }
        
        #endregion
    }
}
