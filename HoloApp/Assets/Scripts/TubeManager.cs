using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD
{
    /// <summary>
    /// Класс контролирующий создание и управление трубами.
    /// </summary>
    public static class TubeManager
    {
        private static readonly List<BaseTube> AllTubes = new List<BaseTube>();

        [CanBeNull] private static BaseTube _selectedTube;

        /// <value> Труба выбранная в данный момент. </value>
        [CanBeNull]
        public static BaseTube SelectedTube
        {
            get { return _selectedTube; }
        }

        /// <summary>
        /// Создает объект погиба диаметра: <paramref name="diameter"/>, если <paramref name="isBended"/> == <c>true</c>,
        /// или объект прямой трубы диаметра: <paramref name="diameter"/>, если <paramref name="isBended"/> == <c>false</c>.
        /// Устанавливает ему родителя <paramref name="pivot"/>
        /// </summary>
        /// <param name="pivot"> Родитель создаваемого объекта в Unity/</param>
        /// <param name="diameter"> Диаметр трубы. </param>
        /// <param name="isBended"> Флаг, какую создавать трубу: прямую или погиб. </param>
        public static void CreateTube(Transform pivot, float diameter, bool isBended)
        {
            AllTubes.Add(TubeFactory.Instance.CreateTube(pivot, diameter, isBended).GetComponent<BaseTube>());
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
            _selectedTube = selectedTube;

            foreach (BaseTube tube in AllTubes)
            {
                tube.IsSelected = tube == _selectedTube;
            }
        }

        /// <summary>
        /// Снимает выбор со всех труб.
        /// </summary>
        public static void DeselectTubes()
        {
            ClearNulls();
            if (_selectedTube == null) return;
            
            _selectedTube.IsSelected = false;
            _selectedTube = null;
        }

        private static void ClearNulls()
        {
            AllTubes.RemoveAll(arg => arg == null);
        }
    }
}
