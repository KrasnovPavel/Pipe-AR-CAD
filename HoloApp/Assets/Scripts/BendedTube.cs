using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendedTube : InteractionReceiver
{
    List<Mesh> meshes;
    GameObject tube;
    GameObject startPoint;
    GameObject endPoint;
    public GameObject buttonBar;

    bool _useSecondRadius;
    int _angle;

    [Range(0.035f, 1.2f)]
    public float firstBendRadius = 0.07f;
    [Range(0.055f, 0.87f)]
    public float secondBendRadius = 0.055f;
    [Range(0.014f, 0.426f)]
    public float diameter = 0.025f;
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

            setMesh();
        }
    }

    public bool useSecondRadius
    {
        get { return _useSecondRadius; }
        set
        {
            _useSecondRadius = value;

            setMesh();
        }
    }

    // Use this for initialization
    void Start ()
    {
        meshes = MeshFactory.CreateMeshes(diameter, firstBendRadius, secondBendRadius);
        tube = transform.Find("Tube").gameObject;
        startPoint = transform.Find("Start Point").gameObject;
        endPoint = transform.Find("End Point").gameObject;
        endPoint.GetComponent<Light>().range = diameter;
        startPoint.GetComponent<Light>().range = diameter;
        _useSecondRadius = false;
        _angle = MeshFactory.deltaAngle;
        buttonBar.GetComponent<ButtonBar>().offset = 0.7f * diameter;
        tube.GetComponent<MeshRenderer>().material.SetFloat("_Diameter", diameter);
        setMesh();
    }

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        switch (obj.name)
        {
            case "IncreaseAngleButton":
                angle += MeshFactory.deltaAngle;
                break;
            case "DecreaseAngleButton":
                angle -= MeshFactory.deltaAngle;
                break;
            case "ClockwiseButton":
                transform.localRotation *= Quaternion.Euler(0, 0, MeshFactory.deltaAngle);
                break;
            case "AnticlockwiseButton":
                transform.localRotation *= Quaternion.Euler(0, 0, -MeshFactory.deltaAngle);
                break;
            case "ChangeRadiusButton":
                useSecondRadius = !useSecondRadius;
                break;
            default:
                break;
        }
    }

    void setMesh()
    {
        tube.transform.localPosition = new Vector3(_useSecondRadius ? -secondBendRadius : -firstBendRadius, 0, 0);

        Quaternion rot = Quaternion.Euler(0, -angle, 0);
        Vector3 pos = new Vector3(_useSecondRadius ? secondBendRadius : firstBendRadius, 0, 0);
        endPoint.transform.localPosition = rot * pos - pos;
        endPoint.transform.localRotation = rot;
        int numberOfAngles = 180 / MeshFactory.deltaAngle;
        tube.GetComponent<MeshFilter>().mesh = meshes[_angle / MeshFactory.deltaAngle - 1 + (useSecondRadius ? numberOfAngles : 0)];
    }
}
