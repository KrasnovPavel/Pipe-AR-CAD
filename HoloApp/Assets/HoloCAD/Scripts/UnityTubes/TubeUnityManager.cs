// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using HoloCore;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, создающий объект участка трубы в Unity. </summary>
    [RequireComponent(typeof(SpatialMappingCollider), typeof(SpatialMappingRenderer))]
    public class TubeUnityManager : Singleton<TubeUnityManager> 
    {
        /// <summary> Prefab прямого участка трубы. </summary>
        [Tooltip("Prefab участка прямой трубы.")]
        public GameObject DirectTubeFragmentPrefab;

        /// <summary> Prefab участка погиба. </summary>
        [Tooltip("Prefab участка погиба.")]
        public GameObject BendedTubeFragmentPrefab;

        /// <summary> Prefab фланца. </summary>
        [Tooltip("Prefab фланца.")]
        public GameObject StartTubeFragmentPrefab;

        /// <summary> Prefab объекта отображения расстояния между трубами. </summary>
        [Tooltip("Prefab объекта отображения расстояния между трубами.")]
        public GameObject TransformErrorPrefab;

        /// <summary> Ссылка на объект трехмерного курсора. </summary>
        [Tooltip("Ссылка на объект трехмерного курсора.")]
        public GameObject Cursor;

        /// <summary> Есть ли активный объект соединения. </summary>
        public static bool HasActiveTubesConnector => Instance._activeTubesConnector != null;

        /// <summary> Объект соединения, который в данный момент редактируется. </summary>
        public static TubesConnector ActiveTubesConnector
        {
            get => Instance._activeTubesConnector;
            private set => Instance._activeTubesConnector = value;
        }
        
        /// <summary> Создает на сцене объект начального фланца трубы. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот фланец.</param>
        /// <returns> Созданный объект фланца. </returns>
        [NotNull] public static StartTubeFragment CreateStartTubeFragment(Tube owner)
        {
            GameObject tube = Instantiate(Instance.StartTubeFragmentPrefab);
            tube.GetComponent<TubeFragment>().Owner = owner;
            AllFragments.Add(tube.GetComponent<TubeFragment>());
            return tube.GetComponent<StartTubeFragment>();
        }

        /// <summary> Создает на сцене объект прямого участка трубы. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот участок трубы.</param>
        /// <param name="pivot"> Местоположение нового участка трубы. </param>
        /// <returns> Созданный объект прямого участка трубы. </returns>
        [NotNull] public static DirectTubeFragment CreateDirectTubeFragment(Tube owner, Transform pivot)
        {
            GameObject tube = Instantiate(Instance.DirectTubeFragmentPrefab, pivot);
            tube.GetComponent<TubeFragment>().Owner = owner;
            tube.transform.localPosition = Vector3.zero;
            AllFragments.Add(tube.GetComponent<TubeFragment>());
            return tube.GetComponent<DirectTubeFragment>();
        }

        /// <summary> Создает на сцене объект погиба трубы. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот погиб трубы.</param>
        /// <param name="pivot"> Местоположение нового погиба. </param>
        /// <returns> Созданный объект прямого погиба трубы. </returns>
        [NotNull] public static BendedTubeFragment CreateBendedTubeFragment(Tube owner, Transform pivot)
        {
            GameObject tube = Instantiate(Instance.BendedTubeFragmentPrefab, pivot);
            tube.GetComponent<TubeFragment>().Owner = owner;
            tube.transform.localPosition = Vector3.zero;
            AllFragments.Add(tube.GetComponent<TubeFragment>());
            return tube.GetComponent<BendedTubeFragment>();
        }

        /// <summary> Создает на сцене объект соединения труб. </summary>
        /// <param name="firstOwner"> Первая из соединяемых труб. </param>
        /// <returns></returns>
        [NotNull] public static TubesConnector CreateTubesConnector(Tube firstOwner)
        {
            GameObject err = Instantiate(Instance.TransformErrorPrefab);
            err.GetComponent<TubesConnector>().FirstTube = firstOwner;
            err.GetComponent<TubesConnector>().Cursor = Instance.Cursor.transform;
            ActiveTubesConnector = err.GetComponent<TubesConnector>();
            return err.GetComponent<TubesConnector>();
        }

        /// <summary> Перестает отслеживать активный коннектор. </summary>
        public static void RemoveActiveTubesConnector()
        {
            ActiveTubesConnector = null;
        }

        /// <summary> Переключает отображение полигональной сетки на сцене. </summary>
        /// <param name="show"></param>
        public static void ShowGrid(bool show)
        {
            Instance._mapCollider.enabled = show;
            Instance._mapRenderer.enabled = show;
        }

        
        #region Unity event functions
        
        /// <summary> Функция, инициализирующая объект в Unity. </summary>
        protected void Start()
        {
            _mapCollider = gameObject.GetComponent<SpatialMappingCollider>();
            _mapRenderer = gameObject.GetComponent<SpatialMappingRenderer>();
            TubeManager.CreateTube();
        }

        #endregion

        #region Private definitions

        private SpatialMappingCollider _mapCollider;
        private SpatialMappingRenderer _mapRenderer;
        private TubesConnector _activeTubesConnector;
        private static readonly List<TubeFragment> AllFragments = new List<TubeFragment>();

        #endregion
    }
}
