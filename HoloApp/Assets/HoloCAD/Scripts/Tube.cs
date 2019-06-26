// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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

                MapFragments((fragment => fragment.Diameter = Data.diameter));
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
        
        public StartTubeFragment StartFragment { get; private set; }

        /// <summary> Участки из которых состоит эта труба. </summary>
        public ReadOnlyCollection<TubeFragment> Fragments
        {
            get
            {
                List<TubeFragment> fragments = new List<TubeFragment>();
                MapFragments((fragment => fragments.Add(fragment)));
                return fragments.AsReadOnly();
            }
        }
        
        /// <summary> Делегат для функций над участками труб. </summary>
        /// <param name="fragment"> Фрагмент, над которым будет выполнена функция. </param>
        public delegate void FragmentFunctionDel(TubeFragment fragment);

        /// <summary> Создает соединение труб. </summary>
        public void CreateTubesConnector()
        {
            if (_tubesConnector == null)
            {
                _tubesConnector = TubeUnityManager.CreateTubesConnector(this);
            }
        }

        /// <summary> Отвязывается от соединения труб. </summary>
        public void RemoveTubeConnection()
        {
            _tubesConnector = null;
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
            TubeManager.SelectTubeFragment(StartFragment);
            MapFragments((fragment => fragment.IsPlacing = true));
        }

        /// <summary> Выходит из режима размещения трубы. </summary>
        public void StopPlacing()
        {
            TubeUnityManager.ShowGrid(false);
            MapFragments((fragment => fragment.IsPlacing = false));
        }

        public void MapFragments(FragmentFunctionDel function)
        {
            TubeFragment current = StartFragment;
            while (current != null)
            {
                function.Invoke(current);
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
            }
        }

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

        #endregion
    }
}