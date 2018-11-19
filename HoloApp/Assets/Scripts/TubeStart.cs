using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class TubeStart : BaseTube
{
    private const float Length = 0.03f;

    protected new void Start()
    {
        base.Start();
        StartTube = gameObject;
        EndPoint.transform.localPosition = new Vector3(0, 0, Length);
        Diameter = 0.05f;
        TubeManager.AddTube(this);
        TubeManager.SelectTube(this);
    }

    protected void Update()
    {
        Tube.transform.localScale = new Vector3(Diameter, Length, Diameter);
        if (Diameter < 0)
        {
            Diameter = 0.05f;
        }
        Label.GetComponent<TextMesh>().text = "Диаметр: " + Diameter.ToString("0.00") + "м.";
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
                gameObject.GetComponent<TubeFactory>().CreateTube(EndPoint.transform, Diameter, true, StartTube);
                break;
            case "AddTubeButton":
                gameObject.GetComponent<TubeFactory>().CreateTube(EndPoint.transform, Diameter, false, StartTube);
                break;
        }
    }
}
