using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using HoloCAD.UnityTubes;

namespace HoloCAD
{
    /// <summary>
    /// Класс, контролирующий создание и управление трубами.
    /// </summary>
    public static class TubeManager
    {
        private static readonly List<BaseTube> AllTubes = new List<BaseTube>();

        /// <value> Труба, выбранная в данный момент. </value>
        [CanBeNull]
        public static BaseTube SelectedTube { get; private set; }

        /// <summary>
        /// Создает объект трубы типа <paramref name="type"/>
        /// с параметрами <paramref name="data"/> из стандарта <paramref name="standardName"/>.
        /// Устанавливает ему родителя <paramref name="pivot"/>
        /// </summary>
        /// <param name="pivot"> Родитель создаваемого объекта в Unity/</param>
        /// <param name="standardName">Имя стандарта по которому выполняется погиб</param>
        /// <param name="data">Параметры трубы</param>
        /// <param name="type">Тип трубы</param>
        public static void CreateTube(Transform pivot, TubeLoader.TubeData data, string standardName, TubeFactory.TubeType type)
        {
            AllTubes.Add(TubeFactory.Instance.CreateTube(pivot, data, standardName, type).GetComponent<BaseTube>());
        }

        /// <summary>
        /// Добавляет объект трубы <paramref name="newTube"/> в список труб.
        /// </summary>
        /// <param name="newTube"> Труба которую надо добавить. </param>
        public static void AddTube(BaseTube newTube)
        {
            AllTubes.Add(newTube);
        }

        /// <summary>
        /// Переключает состояние выбора трубы <paramref name="selectedTube"/>.
        /// </summary>
        /// <param name="selectedTube"> Труба состояние которой надо переключить. </param>
        public static void ToggleTubeSelection(BaseTube selectedTube)
        {
            if (selectedTube.IsSelected)
            {
                DeselectTubes();
            }
            else
            {
                SelectTube(selectedTube);
            }
        }

        /// <summary>
        /// Выбирает трубу <paramref name="selectedTube"/>. 
        /// </summary>
        /// <param name="selectedTube"> Труба которую надо выбрать. </param>
        public static void SelectTube(BaseTube selectedTube)
        {
            ClearNulls();
            SelectedTube = selectedTube;

            foreach (BaseTube tube in AllTubes)
            {
                tube.IsSelected = tube == SelectedTube;
            }
        }

        /// <summary>
        /// Снимает выбор со всех труб.
        /// </summary>
        public static void DeselectTubes()
        {
            ClearNulls();
            if (SelectedTube == null) return;
            
            SelectedTube.IsSelected = false;
            SelectedTube = null;
        }

        private static void ClearNulls()
        {
            AllTubes.RemoveAll(arg => arg == null);
        }
    }
}
