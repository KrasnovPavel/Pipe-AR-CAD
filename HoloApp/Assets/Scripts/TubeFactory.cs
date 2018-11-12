using UnityEngine;

public class TubeFactory : MonoBehaviour {
    public GameObject TubePrefab;
    public GameObject BendedTubePrefab;

    public GameObject CreateTube(Transform pivot, float diameter)
    {
        GameObject tube = Instantiate(TubePrefab, pivot);
        tube.GetComponent<Tube>().Diameter = diameter;

        return tube;
    }

    public GameObject CreateBendedTube(Transform pivot, float diameter)
    {
        GameObject tube = Instantiate(BendedTubePrefab, pivot);
        tube.GetComponent<BendedTube>().Diameter = diameter;

        return tube;
    }
}
