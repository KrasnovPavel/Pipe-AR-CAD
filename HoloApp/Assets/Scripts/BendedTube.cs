using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BendedTube : InteractionReceiver
{
    private List<Mesh> _meshes;
    private GameObject _tube;
    private GameObject _endPoint;
    public GameObject ButtonBar;

    private bool _useSecondRadius;
    private int _angle;

    [Range(0.035f, 1.2f)]
    public float FirstBendRadius = 0.07f;
    [Range(0.055f, 0.87f)]
    public float SecondBendRadius = 0.055f;
    [Range(0.014f, 0.426f)]
    public float Diameter = 0.025f;
    public Material Material;

    public int Angle
    {
        get { return _angle; }
        set
        {
            if (value < MeshFactory.DeltaAngle || value > 180)
            {
                return;
            }
            _angle = value;

            SetMesh();
        }
    }

    public bool UseSecondRadius
    {
        get { return _useSecondRadius; }
        set
        {
            _useSecondRadius = value;

            SetMesh();
        }
    }

    // Use this for initialization
    void Start ()
    {
        _meshes = MeshFactory.CreateMeshes(Diameter, FirstBendRadius, SecondBendRadius);
        _tube = transform.Find("Tube").gameObject;
        _endPoint = transform.Find("End Point").gameObject;
        _useSecondRadius = false;
        _angle = MeshFactory.DeltaAngle;
        ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Diameter;
        _tube.GetComponent<MeshRenderer>().material.SetFloat("_Diameter", Diameter);
        SetMesh();
    }

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        switch (obj.name)
        {
            case "IncreaseAngleButton":
                Angle += MeshFactory.DeltaAngle;
                break;
            case "DecreaseAngleButton":
                Angle -= MeshFactory.DeltaAngle;
                break;
            case "ClockwiseButton":
                transform.localRotation *= Quaternion.Euler(0, 0, MeshFactory.DeltaAngle);
                break;
            case "AnticlockwiseButton":
                transform.localRotation *= Quaternion.Euler(0, 0, -MeshFactory.DeltaAngle);
                break;
            case "ChangeRadiusButton":
                UseSecondRadius = !UseSecondRadius;
                break;
            case "AddBendButton":
                gameObject.GetComponent<TubeFactory>().CreateBendedTube(_endPoint.transform, Diameter);
                break;
            case "AddTubeButton":
                gameObject.GetComponent<TubeFactory>().CreateTube(_endPoint.transform, Diameter);
                break;
        }
    }

    private void SetMesh()
    {
        _tube.transform.localPosition = new Vector3(_useSecondRadius ? -SecondBendRadius : -FirstBendRadius, 0, 0);

        Quaternion rot = Quaternion.Euler(0, -Angle, 0);
        Vector3 pos = new Vector3(_useSecondRadius ? SecondBendRadius : FirstBendRadius, 0, 0);
        _endPoint.transform.localPosition = rot * pos - pos;
        _endPoint.transform.localRotation = rot;
        int numberOfAngles = 180 / MeshFactory.DeltaAngle;
        _tube.GetComponent<MeshFilter>().mesh = _meshes[_angle / MeshFactory.DeltaAngle - 1 + (UseSecondRadius ? numberOfAngles : 0)];
    }
}
