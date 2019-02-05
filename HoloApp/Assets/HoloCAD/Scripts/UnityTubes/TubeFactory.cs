using System;
using HoloCore;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary>
    /// Класс, создающий объект трубы в Unity.
    /// </summary>
    public class TubeFactory : Singleton<TubeFactory> {
        
        /// <summary> Тип трубы. </summary>
        public enum TubeType
        {
            /// <value> Прямая труба </value>
            Direct,
            /// <value> Погиб </value>
            Bended,
            /// <value> Фланец </value>
            Start
        }
        
        /// <value> Prefab прямой трубы. </value>
        public GameObject DirectTubePrefab;
        
        /// <value> Prefab погиба. </value>
        public GameObject BendedTubePrefab;

        /// <value> Prefab фланца. </value>
        public GameObject StartTubePrefab;

        /// <value> Collider стен </value>
        public SpatialMappingCollider MapCollider { get; private set; }

        /// <value> Mesh стен </value>
        public SpatialMappingRenderer MapRenderer { get; private set; }

        /// <summary>
        /// Создает объект трубы типа <paramref name="type"/>
        /// с параметрами <paramref name="data"/> из стандарта <paramref name="standardName"/>.
        /// Устанавливает ему родителя <paramref name="pivot"/>
        /// </summary>
        /// <param name="pivot"> Родитель создаваемого объекта в Unity/</param>
        /// <param name="standardName">Имя стандарта по которому выполняется погиб</param>
        /// <param name="data">Параметры трубы</param>
        /// <param name="type">Тип трубы</param>
        /// <returns> Созданный объект трубы. </returns>
        public GameObject CreateTube(Transform pivot, TubeLoader.TubeData data, string standardName, TubeType type)
        {
            GameObject tubePrefab;
            switch (type)
            {
                case TubeType.Direct:
                    tubePrefab = DirectTubePrefab;
                    break;
                case TubeType.Bended:
                    tubePrefab = BendedTubePrefab;
                    break;
                case TubeType.Start:
                    tubePrefab = StartTubePrefab;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
            GameObject tube = Instantiate(tubePrefab, pivot);
            tube.transform.localPosition = Vector3.zero;
            tube.GetComponent<BaseTube>().Data = data;
            tube.GetComponent<BaseTube>().StandardName = standardName;
            TubeManager.AddTube(tube.GetComponent<BaseTube>());

            return tube;
        }

        public static void ShowGrid(bool show)
        {
            Instance.MapCollider.enabled = show;
            Instance.MapRenderer.enabled = show;
        }

        private void Start()
        {
            MapCollider = gameObject.GetComponent<SpatialMappingCollider>();
            MapRenderer = gameObject.GetComponent<SpatialMappingRenderer>();
        }
    }
}
