// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCore.UI;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace HoloCAD.Tubes.UnityTubes
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary> Класс, принимающий событие нажатия на трубу. </summary>
    /// <remarks> Должен быть присоединен компонентом к объекту содержащему меш трубы. </remarks>
    public class TubeClickerReceiver : MonoBehaviour, IMixedRealityPointerHandler
    {
        /// <summary> Обработчик события нажатия на трубу. </summary>
        public void Click()
        {
            TubeFragment parent = transform.parent.GetComponent<TubeFragment>();
            if (parent == null) parent = transform.parent.parent.GetComponent<TubeFragment>();
            parent.GetComponent<SelectableObject>()?.ToggleSelection();
            parent.Owner.FinishTubesConnectorCreation();
        }

        #region MRTK event functions
        
        /// <summary> Обработчик события клика по трубе для MRTK. </summary>
        /// <param name="eventData"></param>
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            Click();
        }

        /// <summary> Обработчик события нажатия на трубу для MRTK. </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        
        /// <summary> Обработчик события перетягивания трубы для MRTK. </summary>
        /// <param name="eventData"></param>
        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        /// <summary> Обработчик события отпускания нажатия на трубу для MRTK. </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        #endregion
    }
}
