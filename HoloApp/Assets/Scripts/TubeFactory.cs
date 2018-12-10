using UnityEngine;

namespace HoloCAD
{
    /// <inheritdoc />
    /// <summary>
    /// Класс создающий объект трубы в Unity/
    /// </summary>
    public class TubeFactory : Singleton<TubeFactory> {
        /// <value> Prefab прямой трубы. </value>
        public GameObject DirectTubePrefab;
        
        /// <value> Prefab погиба. </value>
        public GameObject BendedTubePrefab;

        /// <summary>
        /// Создает объект погиба диаметра: <paramref name="diameter"/>, если <paramref name="isBended"/> == <c>true</c>,
        /// или объект прямой трубы диаметра: <paramref name="diameter"/>, если <paramref name="isBended"/> == <c>false</c>.
        /// Устанавливает ему родителя <paramref name="pivot"/>
        /// </summary>
        /// <param name="pivot"> Родитель создаваемого объекта в Unity/</param>
        /// <param name="diameter"> Диаметр трубы. </param>
        /// <param name="isBended"> Флаг, какую создавать трубу: прямую или погиб. </param>
        /// <returns> Созданный объект трубы. </returns>
        public GameObject CreateTube(Transform pivot, float diameter, bool isBended)
        {
            GameObject tube = Instantiate(isBended ? BendedTubePrefab : DirectTubePrefab, pivot);
            tube.GetComponent<BaseTube>().Diameter = diameter;
            TubeManager.AddTube(tube.GetComponent<BaseTube>());

            return tube;
        }
    }
}
