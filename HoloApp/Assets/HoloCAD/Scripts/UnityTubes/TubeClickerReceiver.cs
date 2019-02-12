using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary> Класс, принимающий событие нажатия на трубу. </summary>
    /// <remarks> Должен быть присоединен компонентом к объекту содержащему меш трубы. </remarks>
    public class TubeClickerReceiver : MonoBehaviour, IInputClickHandler
    {
        /// <summary> Обработчик события нажатия на трубу. </summary>
        /// <param name="eventData"></param>
        public void OnInputClicked(InputClickedEventData eventData)
        {
            TubeFragment parent = transform.parent.GetComponent<TubeFragment>();
            TubeManager.ToggleTubeSelection(parent);
            parent.Owner.FinishTubesConnectorCreation();
        }
    }
}
