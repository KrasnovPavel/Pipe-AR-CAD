// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HoloCAD.IO;
using HoloCAD.UI;
using HoloCAD.UnityTubes;
using HoloCore.UI;
using JetBrains.Annotations;

namespace HoloCAD
{
    /// <summary> Класс трубы. </summary>
    public sealed class Tube : INotifyPropertyChanged
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
            set
            {
                if (_data != null && _data.Equals(value)) return;
                
                _data = value;

                MapFragmentsWithOutgrowth((fragment => fragment.Diameter = Data.diameter));
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
        
        /// <summary> Фланец. </summary>
        public StartTubeFragment StartFragment { get; private set; }

        /// <summary> Участки из которых состоит эта труба (без учета отростков). </summary>
        public ReadOnlyCollection<TubeFragment> Fragments
        {
            get
            {
                List<TubeFragment> fragments = new List<TubeFragment>();
                MapFragments((fragment => fragments.Add(fragment)));
                return fragments.AsReadOnly();
            }
        }
        
        /// <summary> Создает соединение труб. </summary>
        public void CreateTubesConnector()
        {
            if (_tubesConnector == null)
            {
                _tubesConnector = TubeUnityManager.CreateTubesConnector(this);
                OnPropertyChanged(nameof(HasTubesConnector));
            }
        }

        /// <summary> Отвязывается от соединения труб. </summary>
        public void RemoveTubeConnection()
        {
            _tubesConnector = null;
            OnPropertyChanged(nameof(HasTubesConnector));
        }

        /// <summary> Удаляет участок трубы и все следующие за ним. </summary>
        /// <param name="fragment"> Удаляемый фрагмент. </param>
        public void OnFragmentRemoved(TubeFragment fragment)
        {
            if (fragment == StartFragment)
            {
                TubeManager.RemoveTube(this);
                if (_tubesConnector != null) _tubesConnector.RemoveThis();
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
            StartFragment.GetComponent<SelectableObject>()?.SelectThis();
            MapFragmentsWithOutgrowth((fragment => fragment.IsPlacing = true));
        }

        /// <summary> Выходит из режима размещения трубы. </summary>
        public void StopPlacing()
        {
            TubeUnityManager.ShowGrid(false);
            MapFragmentsWithOutgrowth((fragment => fragment.IsPlacing = false));
        }

        /// <summary> Проходит по всем участкам трубы(без отростков) и вызывает переданную функцию. </summary>
        /// <param name="function"> Функция, которая будет вызвана для каждого участка.</param>
        /// <param name="firstFragment">
        /// Участок, с которого будет начат обход. Если он равен null, то обход начинается с фланца.
        /// </param>
        public void MapFragments(Action<TubeFragment> function, TubeFragment firstFragment = null)
        {
            TubeFragment current = (firstFragment != null) ? firstFragment : StartFragment;
            while (current != null)
            {
                function.Invoke(current);
                current = current.Child;
            }
        }

        /// <summary> Проходит по всем участкам трубы(включая отростки) и вызывает переданную функцию. </summary>
        /// <param name="function"> Функция, которая будет вызвана для каждого участка.</param>
        /// <param name="firstFragment">
        /// Участок, с которого будет начат обход. Если он равен null, то обход начинается с фланца.
        /// </param>
        public void MapFragmentsWithOutgrowth(Action<TubeFragment> function, TubeFragment firstFragment = null)
        {
            TubeFragment current = (firstFragment != null) ? firstFragment : StartFragment;
            while (current != null)
            {
                function.Invoke(current);
                DirectTubeFragment currentDirect = current as DirectTubeFragment;
                if (currentDirect != null)
                {
                    foreach (var outgrowth in currentDirect.Outgrowths)
                    {
                        MapFragmentsWithOutgrowth(function, outgrowth);
                    }
                }
                current = current.Child;
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
                OnPropertyChanged(nameof(HasTubesConnector));
            }
        }
        
        /// <summary> Событие, вызываемое при изменении какого-либо свойства. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private definitions

        private string _standardName;
        private TubeLoader.TubeData _data;
        
        /// <summary> Объект соединения труб, соединяющий эту труб с другой. </summary>
        [CanBeNull] private TubesConnector _tubesConnector;
        
        /// <summary> Создает объект начального фланца для этой трубы. </summary>
        private void CreateStartTubeFragment()
        {
            StartFragment = TubeUnityManager.CreateStartTubeFragment(this);
        }
        
        /// <summary> Обработчик изменения свойств. </summary>
        /// <param name="propertyName"> Имя изменившегося свойства. </param>
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}