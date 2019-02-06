using System.Collections.Generic;
using JetBrains.Annotations;
using HoloCAD.UnityTubes;

namespace HoloCAD
{
    /// <summary> Класс, контролирующий создание и управление трубами. </summary>
    public static class TubeManager
    {
        private static readonly List<Tube> AllTubes = new List<Tube>();

        /// <summary> Труба, выбранная в данный момент. </summary>
        [CanBeNull]
        public static TubeFragment SelectedTubeFragment { get; private set; }

        /// <summary> Создает новую трубу. </summary>
        public static void CreateTube()
        {
            AllTubes.Add(new Tube());
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
            AllTubes.Remove(tube);
        }
    }
}
