using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeStart : InteractionReceiver
{
    private GameObject _tube;
    private GameObject _endPoint;

    private const float Length = 0.03f;

    private float _diameter = 0.014f;

    public float Diameter
    {
        get { return _diameter; }
        set
        {
            if (value <= 0)
            {
                return;
            }
            _diameter = value;
            Debug.Log(_diameter);
            _tube.transform.localScale = new Vector3(_diameter, Length, _diameter);
        }
    }

    // Use this for initialization
    void Start()
    {
        _tube = transform.Find("Tube").gameObject;
        _endPoint = transform.Find("End Point").gameObject;
        _endPoint.transform.localPosition = new Vector3(0, 0, Length);
        Diameter = 0.05f;
    }

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        switch (obj.name)
        {
            case "IncreaseDiameterButton":
                Diameter += 0.01f;
                break;
            case "DecreaseDiameterButton":
                Diameter -= 0.01f;
                break;
            case "AddBendButton":
                gameObject.GetComponent<TubeFactory>().CreateBendedTube(_endPoint.transform, Diameter);
                break;
            case "AddTubeButton":
                gameObject.GetComponent<TubeFactory>().CreateTube(_endPoint.transform, Diameter);
                break;
        }
    }
}
