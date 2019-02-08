using HoloCore;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Класс, создающий объект участка трубы в Unity. </summary>
    [RequireComponent(typeof(SpatialMappingCollider), typeof(SpatialMappingRenderer))]
    public class TubeFactory : Singleton<TubeFactory> {
        private SpatialMappingCollider _mapCollider;
        private SpatialMappingRenderer _mapRenderer;

        /// <summary> Prefab участка прямой трубы. </summary>
        public GameObject DirectTubeFragmentPrefab;

        /// <summary> Prefab участка погиба. </summary>
        public GameObject BendedTubeFragmentPrefab;

        /// <summary> Prefab участка фланца. </summary>
        public GameObject StartTubeFragmentPrefab;

        /// <summary> Создает на сцене объект начального фланца трубы. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот фланец.</param>
        /// <returns> Созданный объект фланца. </returns>
        public StartTubeFragment CreateStartTubeFragment(Tube owner)
        {
            GameObject tube = Instantiate(StartTubeFragmentPrefab);
            tube.GetComponent<TubeFragment>().Owner = owner;
            return tube.GetComponent<StartTubeFragment>();
        }

        /// <summary> Создает на сцене объект прямого участка трубы. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот участок трубы.</param>
        /// <param name="pivot"> Местоположение нового участка трубы. </param>
        /// <returns> Созданный объект прямого участка трубы. </returns>
        public DirectTubeFragment CreateDirectTubeFragment(Tube owner, Transform pivot)
        {
            GameObject tube = Instantiate(DirectTubeFragmentPrefab, pivot);
            tube.transform.localPosition = Vector3.zero;
            tube.GetComponent<TubeFragment>().Owner = owner;
            return tube.GetComponent<DirectTubeFragment>();
        }

        /// <summary> Создает на сцене объект погиба трубы. </summary>
        /// <param name="owner"> Труба, которой принадлежит этот погиб трубы.</param>
        /// <param name="pivot"> Местоположение нового погиба. </param>
        /// <returns> Созданный объект прямого погиба трубы. </returns>
        public BendedTubeFragment CreateBendedTubeFragment(Tube owner, Transform pivot)
        {
            GameObject tube = Instantiate(BendedTubeFragmentPrefab, pivot);
            tube.transform.localPosition = Vector3.zero;
            tube.GetComponent<TubeFragment>().Owner = owner;
            return tube.GetComponent<BendedTubeFragment>();
        }

        /// <summary> Переключает отображение полигональной сетки на сцене. </summary>
        /// <param name="show"></param>
        public static void ShowGrid(bool show)
        {
            Instance._mapCollider.enabled = show;
            Instance._mapRenderer.enabled = show;
        }

        /// <summary> Функция, инициализирующая объект в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        protected void Start()
        {
            _mapCollider = gameObject.GetComponent<SpatialMappingCollider>();
            _mapRenderer = gameObject.GetComponent<SpatialMappingRenderer>();
            TubeManager.CreateTube();
        }
    }
}
