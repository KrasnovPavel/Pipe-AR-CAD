using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloCAD
{
    public class TubeClickerReceiver : MonoBehaviour, IInputClickHandler
    {
        public void OnInputClicked(InputClickedEventData eventData)
        {
            TubeManager.ToggleTubeSelection(transform.parent.GetComponent<BaseTube>());
        }
    }
}
