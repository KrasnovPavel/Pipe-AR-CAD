using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HoloCAD.UnityTubes;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD
{
    /// <summary> Класс трубы. </summary>
    public class Tube
    {
        /// <summary> Конструктор по умолчанию. Создает трубу по превому найденному стандарту. </summary>
        /// <exception cref="Exception"> Выбрасывается, если ни одного стандарта не было найдено. </exception>
        public Tube()
        {
            if (TubeLoader.GetStandardNames().Count == 0)
            {
                throw new Exception("No standards found.");
            }
            StandardName = TubeLoader.GetStandardNames()[0];
            CreateStartTubeFragment();
        }

        /// <summary> Создает трубу по умолчанию из указанного стандарта. </summary>
        /// <param name="standardName"> Имя стандарта. </param>
        /// <exception cref="Exception"> Выбрасывается, если стандарт с этим именем не был найден. </exception>
        public Tube(string standardName)
        {
            if (!TubeLoader.GetStandardNames().Contains(standardName))
            {
                throw new Exception($"No standard with name {standardName} found.");
            }

            StandardName = standardName;
            CreateStartTubeFragment();
        }

        /// <summary> Создает трубу с указанными параметрами из указанного стандарта. </summary>
        /// <param name="standardName"> Имя стандарта. </param>
        /// <param name="data"> Параметры трубы. </param>
        /// <exception cref="Exception">
        /// Выбрасывается, если труба с такими параметрами не была найдена в стандарте.
        /// </exception>
        public Tube(string standardName, TubeLoader.TubeData data)
        {
            if (!TubeLoader.GetStandardNames().Contains(standardName))
            {
                throw new Exception($"No standard with name {standardName} found.");
            }
            
            if (!TubeLoader.GetAvailableTubes(standardName).Contains(data))
            {
                throw new Exception($"Tube with given parameters not found in standard {standardName}.");
            }

            StandardName = standardName;
            Data = data;
            CreateStartTubeFragment();
        }
        
        /// <summary> Связан ли с этой трубой объект соединения труб. </summary>
        public bool HasTubesConnector => _tubesConnector != null;

        /// <summary> Параметры трубы, взятые из стандарта. </summary>
        public TubeLoader.TubeData Data
        {
            get => _data;
            private set
            {
                if (_data != null && _data.Equals(value)) return;
                
                _data = value;

                foreach (TubeFragment tubeFragment in _fragments)
                {
                    tubeFragment.Diameter = Data.diameter;
                }
            }
        }

        /// <summary> Наименование стандарта, по которому выполнена данная труба. </summary>
        public string StandardName
        {
            get => _standardName;
            set
            {
                if (_standardName == value || TubeLoader.GetAvailableTubes(value).Count == 0) return;
                
                _standardName = value;
                Data = TubeLoader.GetAvailableTubes(_standardName)[0];
            }
        }

        /// <summary> Участки из которых состоит эта труба. </summary>
        public ReadOnlyCollection<TubeFragment> Fragments => _fragments.AsReadOnly();
        
        /// <summary> Создает для этой трубы прямой участок. </summary>
        /// <param name="pivot"> Местоположение нового фрагмента. </param>
        public void CreateDirectTubeFragment(Transform pivot)
        {
            _fragments.Add(TubeUnityManager.CreateDirectTubeFragment(this, pivot));
            _fragments[_fragments.Count - 2].HasChild = true;
        }

        /// <summary> Создает для этой трубы участок погиба. </summary>
        /// <param name="pivot"> Местоположение нового фрагмента. </param>
        public void CreateBendedTubeFragment(Transform pivot)
        {
            _fragments.Add(TubeUnityManager.CreateBendedTubeFragment(this, pivot));
            _fragments[_fragments.Count - 2].HasChild = true;
        }

        /// <summary> Создает соединения труб. </summary>
        public void CreateTubesConnector()
        {
            if (_tubesConnector == null)
            {
                _tubesConnector = TubeUnityManager.CreateTubesConnector(this);
            }
        }

        /// <summary> Отвязывается от объекта отображения расстояния между трубами. </summary>
        public void RemoveTransformError()
        {
            _tubesConnector = null;
        }

        /// <summary> Удаляет участок трубы и все следующие за ним. </summary>
        /// <param name="fragment"> Удаляемый фрагмент. </param>
        public void OnFragmentRemoved(TubeFragment fragment)
        {
            int index = _fragments.IndexOf(fragment);
            if (index < 0) return;
            
            _fragments.RemoveRange(index, _fragments.Count - index);

            if (_fragments.Count == 0)
            {
                TubeManager.RemoveTube(this);
                if (_tubesConnector != null) _tubesConnector.RemoveThis();
            }
            else
            {
                _fragments.Last().HasChild = false;
            }
        }

        /// <summary> Выбирает больший диаметр из стандарта. </summary>
        public void SelectBiggerDiameter()
        {
            Data = TubeLoader.GetBigger(Data, StandardName);
        }

        /// <summary> Выбирает меньший диаметр из стандарта. </summary>
        public void SelectSmallerDiameter()
        {
            Data = TubeLoader.GetSmaller(Data, StandardName);
        }

        /// <summary> Переходит в режим размещения трубы. </summary>
        public void StartPlacing()
        {
            TubeUnityManager.ShowGrid(true);
            TubeManager.SelectTubeFragment(_fragments[0]);
            foreach (TubeFragment tubeFragment in _fragments)
            {
                tubeFragment.IsPlacing = true;
            }
        }

        /// <summary> Выходит из режима размещения трубы. </summary>
        public void StopPlacing()
        {
            TubeUnityManager.ShowGrid(false);
            foreach (TubeFragment tubeFragment in _fragments)
            {
                tubeFragment.IsPlacing = false;
            }
        }

        /// <summary> Завершает создание объекта соединения трубы. </summary>
        public void FinishTubesConnectorCreation()
        {
            if (!TubeUnityManager.HasActiveTubesConnector) return;

            if (HasTubesConnector)
            {
                TubeUnityManager.ActiveTubesConnector.RemoveThis();
            }
            else
            {
                _tubesConnector = TubeUnityManager.ActiveTubesConnector;
                _tubesConnector.SecondTube = this;
                TubeUnityManager.RemoveActiveTubesConnector();
            }
        }

        /// <summary> Выдает следующий фрагмент трубы. </summary>
        /// <param name="current"> Фрагмент трубы для которого надо найти следующий. </param>
        /// <returns> Следующий фрагмент трубы или null, если его нет. </returns>
        [CanBeNull] public TubeFragment GetNextFragment(TubeFragment current)
        {
            int index = _fragments.FindIndex(f => current == f);

            return (index == _fragments.Count - 1) ? null : _fragments[index + 1];
        }
        
        /// <summary> Выдает предыдущий фрагмент трубы. </summary>
        /// <param name="current"> Фрагмент трубы для которого надо найти предыдущий. </param>
        /// <returns> Предыдущий фрагмент трубы или null, если его нет. </returns>
        [CanBeNull] public TubeFragment GetPreviousFragment(TubeFragment current)
        {
            int index = _fragments.FindIndex(f => current == f);
            
            return (index == 0) ? null : _fragments[index - 1];
        }

        #region Private definitions

        private readonly List<TubeFragment> _fragments = new List<TubeFragment>();

        private string _standardName;
        private TubeLoader.TubeData _data;
        
        /// <summary> Объект соединения труб, соединяющий эту труб с другой. </summary>
        [CanBeNull] private TubesConnector _tubesConnector;
        
        /// <summary> Создает объект начального фланца для этой трубы. </summary>
        private void CreateStartTubeFragment()
        {
            _fragments.Add(TubeUnityManager.CreateStartTubeFragment(this));
        }

        #endregion
    }
}