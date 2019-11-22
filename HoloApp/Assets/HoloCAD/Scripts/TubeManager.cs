// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using System.Collections.ObjectModel;
using HoloCAD.IO;
using JetBrains.Annotations;
using HoloCAD.UnityTubes;

namespace HoloCAD
{
    /// <summary> Класс, контролирующий создание и управление трубами. </summary>
    public static class TubeManager
    {
        /// <summary> Массив всех труб находящихся на сцене. </summary>
        [NotNull] public static ReadOnlyCollection<Tube> AllTubes => _allTubes.AsReadOnly();

        /// <summary> Создает новую трубу. </summary>
        /// <returns> Новая труба. </returns>
        public static Tube CreateTube(Tube basedOn = null)
        {
            Tube t = basedOn == null ? new Tube() : new Tube(basedOn.StandardName, basedOn.Data);
            _allTubes.Add(t);
            return t;
        }

        /// <summary> Удаляет трубу из списка. </summary>
        /// <param name="tube"> Удаляемая труба. </param>
        public static void RemoveTube(Tube tube)
        {
            _allTubes.Remove(tube);
        }
        
        /// <summary> Сохраняет сцену в выбираемый пользователем файл. </summary>
        public static void SaveScene()
        {
            SceneExporter.Export(AllTubes, false);
        }

        public static void SaveSceneAs()
        {
            SceneExporter.Export(AllTubes, true);
        }
        
        /// <summary> Загрузить сцену из выбираемого пользователем файла. </summary>
        public static void LoadScene()
        {
            SceneImporter.Import(TubeUnityManager.Instance.StartTubeMarks);
        }
        
        #region Private definitions

        // ReSharper disable once InconsistentNaming
        private static readonly List<Tube> _allTubes = new List<Tube>();

        #endregion
    }
}
