using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HoloCAD.UnityTubes;
using UnityEngine;

namespace HoloCAD
{
    /// <summary> Класс трубы. </summary>
    public class Tube
    {
        private readonly List<TubeFragment> _fragments = new List<TubeFragment>();

        private string _standardName;
        private TubeLoader.TubeData _data;

        /// <summary> Параметры трубы, взятые из стандарта. </summary>
        public TubeLoader.TubeData Data
        {
            get { return _data; }
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
            get { return _standardName; }
            set
            {
                if (_standardName == value || TubeLoader.GetAvailableTubes(value).Count == 0) return;
                
                _standardName = value;
                Data = TubeLoader.GetAvailableTubes(_standardName)[0];
            }
        }

        /// <summary> Участки из которых состоит эта труба. </summary>
        public ReadOnlyCollection<TubeFragment> Fragments => _fragments.AsReadOnly();

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

        /// <summary> Создает объект начального фланца для этой трубы. </summary>
        private void CreateStartTubeFragment()
        {
            _fragments.Add(TubeFactory.Instance.CreateStartTubeFragment(this));
        }
        
        /// <summary> Создает для этой трубы прямой участок. </summary>
        /// <param name="pivot"> Местоположение нового фрагмента. </param>
        public void CreateDirectTubeFragment(Transform pivot)
        {
            _fragments.Add(TubeFactory.Instance.CreateDirectTubeFragment(this, pivot));
            _fragments[_fragments.Count - 2].HasChild = true;
        }

        /// <summary> Создает для этой трубы участок погиба. </summary>
        /// <param name="pivot"> Местоположение нового фрагмента. </param>
        public void CreateBendedTubeFragment(Transform pivot)
        {
            _fragments.Add(TubeFactory.Instance.CreateBendedTubeFragment(this, pivot));
            _fragments[_fragments.Count - 2].HasChild = true;
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
            }
            else if (_fragments.Count > 1)
            {
                _fragments[_fragments.Count - 2].HasChild = false;
            }
            else
            {
                _fragments[_fragments.Count - 1].HasChild = false;
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
            TubeFactory.ShowGrid(true);
            TubeManager.SelectTubeFragment(_fragments[0]);
            foreach (TubeFragment tubeFragment in _fragments)
            {
                tubeFragment.IsPlacing = true;
            }
        }

        /// <summary> Выходит из режима размещения трубы. </summary>
        public void StopPlacing()
        {
            TubeFactory.ShowGrid(false);
            foreach (TubeFragment tubeFragment in _fragments)
            {
                tubeFragment.IsPlacing = false;
            }
        }
    }
}