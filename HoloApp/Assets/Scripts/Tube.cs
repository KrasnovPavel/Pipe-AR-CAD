using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : InteractionReceiver {
    GameObject tube;
    GameObject startPoint;
    GameObject endPoint;

    [Range(0.01f, 1f)]
    public float diameter = 0.01f;
    [Range(0.1f, 10f)]
    public float length = 0.1f;

	// Use this for initialization
	void Start ()
    {
        tube = transform.Find("Tube").gameObject;
        startPoint = transform.Find("Start Point").gameObject;
        endPoint = transform.Find("End Point").gameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {
        tube.transform.localScale = new Vector3(diameter, length, diameter);
        endPoint.transform.localPosition = new Vector3(0, 0, length);
        endPoint.GetComponent<Light>().range = diameter;
        startPoint.GetComponent<Light>().range = diameter;
    }

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        switch (obj.name)
        {
            case "IncreaseLengthButton":
                length += 0.3f;
                break;
            case "DecreaseLengthButton":
                length -= 0.3f;
                break;
            case "IncreaseDiameterButton":
                diameter += 0.1f;
                break;
            case "DecreaseDiameterButton":
                diameter -= 0.1f;
                break;
            default:
                break;
        }
    }
}
