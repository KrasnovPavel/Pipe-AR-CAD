// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using System.Runtime.CompilerServices;
using HoloCAD.UI;
using HoloCore;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc cref="Singleton{T}"/>
    /// <summary> Класс, создающий объект участка трубы в Unity. </summary>
    [RequireComponent(typeof(SpatialMappingCollider), typeof(SpatialMappingRenderer))]
    public sealed class TubeUnityManager : Singleton<TubeUnityManager>, INotifyPropertyChanged
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
        public GameObject ConnectorPrefab;
        
        /// <summary> Prefab панели управления отростком трубы. </summary>
        [Tooltip("Prefab панели управления отростком трубы.")]
        public GameObject OutgrowthPanelPrefab;

        /// <summary> Есть ли активный объект соединения. </summary>
        public static bool HasActiveTubesConnector => Instance._activeTubesConnector != null;

        /// <summary> Объект соединения, который в данный момент редактируется. </summary>
        public static TubesConnector ActiveTubesConnector
        {
            get => Instance._activeTubesConnector;
            private set
            {
                Instance._activeTubesConnector = value;
                Instance.OnPropertyChanged();
                Instance.OnPropertyChanged(nameof(HasActiveTubesConnector));
            }
        }
        
        /// <summary> Использовать SpatialMapping для привязки объектов?  </summary>
        public static bool UseSpatialMapping = true;
        
        /// <summary> Создает на сцене объект начального фланца трубы. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот фланец.</param>
        /// <returns> Созданный объект фланца. </returns>
        [NotNull] public static StartTubeFragment CreateStartTubeFragment(Tube owner)
        {
            GameObject tube = Instantiate(Instance.StartTubeFragmentPrefab);
            tube.GetComponent<TubeFragment>().Owner = owner;
            return tube.GetComponent<StartTubeFragment>();
        }

        /// <summary> Создает на сцене объект прямого участка трубы. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот участок трубы.</param>
        /// <param name="pivot"> Местоположение нового участка трубы. </param>
        /// <param name="parent"> Предыдущий участок трубы. </param>
        /// <returns> Созданный объект прямого участка трубы. </returns>
        [NotNull] public static DirectTubeFragment CreateDirectTubeFragment(Tube owner, Transform pivot, TubeFragment parent)
        {
            GameObject tube = Instantiate(Instance.DirectTubeFragmentPrefab, pivot);
            tube.GetComponent<TubeFragment>().Owner = owner;
            tube.GetComponent<TubeFragment>().Parent = parent;
            tube.transform.localPosition = Vector3.zero;
            return tube.GetComponent<DirectTubeFragment>();
        }

        /// <summary> Создает на сцене объект погиба трубы. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот погиб трубы.</param>
        /// <param name="pivot"> Местоположение нового погиба. </param>
        /// <param name="parent"> Предыдущий участок трубы. </param>
        /// <returns> Созданный объект прямого погиба трубы. </returns>
        [NotNull] public static BendedTubeFragment CreateBendedTubeFragment(Tube owner, Transform pivot, TubeFragment parent)
        {
            GameObject tube = Instantiate(Instance.BendedTubeFragmentPrefab, pivot);
            tube.GetComponent<TubeFragment>().Owner = owner;
            tube.GetComponent<TubeFragment>().Parent = parent;
            tube.transform.localPosition = Vector3.zero;
            return tube.GetComponent<BendedTubeFragment>();
        }

        /// <summary> Создает на сцене объект соединения труб. </summary>
        /// <param name="firstOwner"> Первая из соединяемых труб. </param>
        /// <returns></returns>
        [NotNull] public static TubesConnector CreateTubesConnector(Tube firstOwner)
        {
            GameObject connector = Instantiate(Instance.ConnectorPrefab);
            connector.GetComponent<TubesConnector>().FirstTube = firstOwner;
            connector.GetComponent<TubesConnector>().Cursor = Instance._cursor.transform;
            ActiveTubesConnector = connector.GetComponent<TubesConnector>();
            return connector.GetComponent<TubesConnector>();
        }

        /// <summary> Создает на сцене объект отростка. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот погиб трубы.</param>
        /// <param name="parent"> Предыдущий участок трубы. </param>
        /// <returns></returns>
        [NotNull] public static Outgrowth CreateOutgrowth(Tube owner, DirectTubeFragment parent)
        {
            DirectTubeFragment outgrowth = CreateDirectTubeFragment(owner, parent.transform, parent);
            outgrowth.transform.localPosition = Vector3.forward * parent.Length / 2;
            outgrowth.transform.localRotation = Quaternion.Euler(0, 90, 0);
            outgrowth.gameObject.AddComponent<Outgrowth>();
            GameObject outgrowthPanel = Instantiate(Instance.OutgrowthPanelPrefab, 
                                             outgrowth.EndPoint.transform.Find("Button Bar"));
            outgrowthPanel.transform.localPosition += Vector3.down * 0.25f;
            return outgrowth.GetComponent<Outgrowth>();
        }

        /// <summary> Перестает отслеживать активный коннектор. </summary>
        public static void RemoveActiveTubesConnector()
        {
            ActiveTubesConnector = null;
            Instance.OnPropertyChanged(nameof(HasActiveTubesConnector));
        }

        /// <summary> Переключает отображение полигональной сетки на сцене. </summary>
        /// <param name="show"></param>
        public static void ShowGrid(bool show)
        {
            if (UseSpatialMapping)
            {
                Instance._mapCollider.enabled = show;
                Instance._mapRenderer.enabled = show;
            }
        }
        
        /// <summary> Событие, вызываемое при изменении какого-либо свойства. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        #region Unity event functions
        
        /// <summary> Функция, инициализирующая объект в Unity. </summary>
        private void Start()
        {
            _mapCollider = gameObject.GetComponent<SpatialMappingCollider>();
            _mapRenderer = gameObject.GetComponent<SpatialMappingRenderer>();
            TubeManager.CreateTube();
            _cursor = GameObject.Find("DefaultCursor(Clone)");
        }

        #endregion

        #region Private definitions

        private SpatialMappingCollider _mapCollider;
        private SpatialMappingRenderer _mapRenderer;
        private TubesConnector _activeTubesConnector;
        private GameObject _cursor;

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
