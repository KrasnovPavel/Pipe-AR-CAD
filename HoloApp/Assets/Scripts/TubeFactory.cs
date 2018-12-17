using UnityEngine;

namespace HoloCAD
{
    /// <inheritdoc />
    /// <summary>
    /// Класс, создающий объект трубы в Unity.
    /// </summary>
    public class TubeFactory : Singleton<TubeFactory> {
        /// <value> Prefab прямой трубы. </value>
        public GameObject DirectTubePrefab;
        
        /// <value> Prefab погиба. </value>
        public GameObject BendedTubePrefab;

        /// <summary>
        /// Создает объект погиба с параметрами: <paramref name="data"/> из стандарта <paramref name="standardName"/>,
        /// если <paramref name="isBended"/> == <c>true</c>,
        /// или объект прямой трубы диаметра: <paramref name="data"/> из стандарта <paramref name="standardName"/>,
        /// если <paramref name="isBended"/> == <c>false</c>.
        /// Устанавливает ему родителя <paramref name="pivot"/>
        /// </summary>
        /// <param name="pivot"> Родитель создаваемого объекта в Unity/</param>
        /// <param name="standardName">Имя стандарта по которому выполняется погиб</param>
        /// <param name="isBended"> Флаг, какую создавать трубу: прямую или погиб. </param>
        /// <param name="data">Параметры трубы</param>
        /// <returns> Созданный объект трубы. </returns>
        public GameObject CreateTube(Transform pivot, TubeLoader.TubeData data, string standardName, bool isBended)
        {
            GameObject tube = Instantiate(isBended ? BendedTubePrefab : DirectTubePrefab, pivot);
            tube.GetComponent<BaseTube>().Data = data;
            tube.GetComponent<BaseTube>().StandardName = standardName;
            TubeManager.AddTube(tube.GetComponent<BaseTube>());

            return tube;
        }
    }
}
