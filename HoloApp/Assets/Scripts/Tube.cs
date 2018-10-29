using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : InteractionReceiver
{
    GameObject tube;
    GameObject startPoint;
    GameObject endPoint;
    public GameObject buttonBar;

    float _length;
    float buttonBarOffset;
    
    float diameter = 0.105f;

    public float length
    {
        get { return _length; }
        set
        {
            if (value <= 0)
            {
                return;
            }
            _length = value;

            tube.transform.localScale = new Vector3(diameter, _length, diameter);
            endPoint.transform.localPosition = new Vector3(0, 0, _length);
        }
    }

	// Use this for initialization
	void Start ()
    {
        tube = transform.Find("Tube").gameObject;
        startPoint = transform.Find("Start Point").gameObject;
        endPoint = transform.Find("End Point").gameObject;
        length = 0.5f;
        buttonBar.GetComponent<ButtonBar>().offset = 0.7f * diameter;
    }

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        switch (obj.name)
        {
            case "IncreaseLengthButton":
                length += 0.1f;
                break;
            case "DecreaseLengthButton":
                length -= 0.1f;
                break;
            default:
                break;
        }
    }
}
