using UnityEngine;

public class TubeFactory : MonoBehaviour {
    public GameObject TubePrefab;
    public GameObject BendedTubePrefab;

    public GameObject CreateTube(Transform pivot, float diameter, bool isBended, GameObject startTube)
    {
        GameObject tube = Instantiate(isBended ? BendedTubePrefab : TubePrefab, pivot);
        tube.GetComponent<BaseTube>().Diameter = diameter;
        tube.GetComponent<BaseTube>().StartTube = startTube;
        TubeManager.AddTube(tube.GetComponent<BaseTube>());

        return tube;
    }
}
