using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class TubeClickerReceiver : MonoBehaviour, IInputClickHandler
{
    public void OnInputClicked(InputClickedEventData eventData)
    {
        TubeManager.ToggleTubeSelection(transform.parent.GetComponent<BaseTube>());
    }
}
