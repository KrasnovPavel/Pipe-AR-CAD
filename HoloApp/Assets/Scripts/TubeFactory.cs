using UnityEngine;

namespace HoloCAD
{
    public class TubeFactory : Singleton<TubeFactory> {
        public GameObject TubePrefab;
        public GameObject BendedTubePrefab;

        public GameObject CreateTube(Transform pivot, float diameter, bool isBended)
        {
            GameObject tube = Instantiate(isBended ? BendedTubePrefab : TubePrefab, pivot);
            tube.GetComponent<BaseTube>().Diameter = diameter;
            TubeManager.AddTube(tube.GetComponent<BaseTube>());

            return tube;
        }
    }
}
