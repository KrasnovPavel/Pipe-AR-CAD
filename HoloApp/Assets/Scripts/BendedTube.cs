using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendedTube : MonoBehaviour {
    List<Mesh> meshes;
    GameObject tube;

    bool _useSecondRadius;
    int _angle;

    [Range(0.035f, 1.2f)]
    public float firstBendRadius;
    [Range(0.055f, 0.87f)]
    public float secondBendRadius;
    [Range(0.014f, 0.426f)]
    public float diameter;
    public Material material;
    
    public int angle
    {
        get { return _angle; }
        set
        {
            if (value < MeshFactory.deltaAngle || value > 180)
            {
                return;
            }
            _angle = value;

            int numberOfAngles = 180 / MeshFactory.deltaAngle;
            tube.GetComponent<MeshFilter>().mesh = meshes[_angle / MeshFactory.deltaAngle - 1 + (useSecondRadius ? numberOfAngles : 0)];
        }
    }

    public bool useSecondRadius
    {
        get { return _useSecondRadius; }
        set
        {
            _useSecondRadius = value;
            material.SetFloat("_BendRadius", _useSecondRadius ? secondBendRadius : firstBendRadius);
            tube.transform.localPosition = new Vector3(_useSecondRadius ? -secondBendRadius : -firstBendRadius, 0, 0);
        }
    }

    // Use this for initialization
    void Start () {
        meshes = MeshFactory.CreateMeshes(diameter, firstBendRadius, secondBendRadius);
        tube = new GameObject("Tube");
        tube.transform.SetParent(transform);
        tube.transform.localRotation = Quaternion.identity;
        tube.AddComponent<MeshFilter>();
        material.SetFloat("_Diameter", diameter);
        tube.AddComponent<MeshRenderer>().material = material;
        useSecondRadius = false;
        angle = MeshFactory.deltaAngle;
    }
}
