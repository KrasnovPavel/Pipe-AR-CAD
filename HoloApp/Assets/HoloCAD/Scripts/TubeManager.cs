// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using HoloCAD.UnityTubes;

namespace HoloCAD
{
    /// <summary> Класс, контролирующий создание и управление трубами. </summary>
    public static class TubeManager
    {
        /// <summary> Труба, выбранная в данный момент. </summary>
        [CanBeNull] public static TubeFragment SelectedTubeFragment { get; private set; }

        /// <summary> Массив всех труб находящихся на сцене. </summary>
        [NotNull] public static ReadOnlyCollection<Tube> AllTubes => _allTubes.AsReadOnly();

        /// <summary> Создает новую трубу. </summary>
        public static void CreateTube(Tube basedOn = null)
        {
            _allTubes.Add(basedOn == null ? new Tube() : new Tube(basedOn.StandardName, basedOn.Data));
        }

        /// <summary> Переключает состояние выбора участка трубы <paramref name="selectedTubeFragment"/>. </summary>
        /// <param name="selectedTubeFragment"> Участок трубы, состояние которого надо переключить. </param>
        public static void ToggleTubeSelection(TubeFragment selectedTubeFragment)
        {
            if (selectedTubeFragment.IsSelected)
            {
                DeselectTubeFragments();
            }
            else
            {
                SelectTubeFragment(selectedTubeFragment);
            }
        }

        /// <summary> Выбирает участок трубы <paramref name="selectedTubeFragment"/>. </summary>
        /// <param name="selectedTubeFragment"> Участок трубы, который надо выбрать. </param>
        public static void SelectTubeFragment([NotNull] TubeFragment selectedTubeFragment)
        {
            if (SelectedTubeFragment != null)
            {
                SelectedTubeFragment.IsSelected = false;
            }
            SelectedTubeFragment = selectedTubeFragment;
            SelectedTubeFragment.IsSelected = true;
        }

        /// <summary> Снимает выбор со всех участков труб. </summary>
        public static void DeselectTubeFragments()
        {
            if (SelectedTubeFragment == null) return;
            
            SelectedTubeFragment.IsSelected = false;
            SelectedTubeFragment = null;
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
            SchemeExporter.Export(AllTubes);
        }
        
        /// <summary> Выбирает следующий фрагмент трубы. </summary>
        public static void SelectNext()
        {
            if (SelectedTubeFragment == null) return;

            TubeFragment nextFragment = SelectedTubeFragment.GetNextTubeFragment();

            if (nextFragment == null) return;
            
            SelectTubeFragment(nextFragment);
        }
        
        /// <summary> Выбирает следующий фрагмент трубы. </summary>
        public static void SelectPrevious()
        {
            if (SelectedTubeFragment == null) return;

            TubeFragment previousFragment = SelectedTubeFragment.GetPreviousTubeFragment();

            if (previousFragment == null) return;
            
            SelectTubeFragment(previousFragment);
        }

        #region Private definitions

        // ReSharper disable once InconsistentNaming
        private static readonly List<Tube> _allTubes = new List<Tube>();

        #endregion
    }
}
