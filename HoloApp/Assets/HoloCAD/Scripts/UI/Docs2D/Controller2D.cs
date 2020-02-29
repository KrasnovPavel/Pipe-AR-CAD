// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HoloCore.IO;
using HoloCore;
using HoloCore.UI.Transparent;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

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
        public static async void OpenFile()
        {
            Transform t = Camera.main.transform;
            var pos = Quaternion.AngleAxis(t.rotation.eulerAngles.y, Vector3.up) * Vector3.forward * 1.5f + t.position;
            var rot = Quaternion.AngleAxis(t.rotation.eulerAngles.y, Vector3.up);

            (byte[] data, string path) =
                await UnityFileManager.PickAndReadBinaryFileAsync(new[] { "pdf", "jpg", "png", "txt" });

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

            go.GetComponent<Viewer2D>().Init(data, path);
            _viewers2D.Add(go.GetComponent<Viewer2D>());
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

        #endregion
    }
}