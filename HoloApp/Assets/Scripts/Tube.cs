using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using UnityEngine;

public class Tube : InteractionReceiver
{
    private GameObject _tube;
    private GameObject _endPoint;
    public GameObject ButtonBar;

    private float _length;
    private float _buttonBarOffset;
    
    public float Diameter = 0.105f;

    public float Length
    {
        get { return _length; }
        set
        {
            if (value <= 0)
            {
                return;
            }
            _length = value;

            _tube.transform.localScale = new Vector3(Diameter, _length, Diameter);
            _endPoint.transform.localPosition = new Vector3(0, 0, _length);
        }
    }

	// Use this for initialization
	void Start ()
    {
        _tube = transform.Find("Tube").gameObject;
        _endPoint = transform.Find("End Point").gameObject;
        Length = 0.5f;
        ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Diameter;
    }

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        switch (obj.name)
        {
            case "IncreaseLengthButton":
                Length += 0.1f;
                break;
            case "DecreaseLengthButton":
                Length -= 0.1f;
                break;
            case "AddBendButton":
                gameObject.GetComponent<TubeFactory>().CreateBendedTube(_endPoint.transform, Diameter);
                break;
        }
    }
}
