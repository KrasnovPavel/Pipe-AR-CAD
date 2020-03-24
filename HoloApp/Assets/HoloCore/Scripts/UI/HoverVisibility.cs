using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace HoloCore.UI
{
    /// <summary> Компонент, включающий видимость объекта только при наведении. </summary>
    public class HoverVisibility : MonoBehaviour, IMixedRealityFocusHandler
    {
        private void Start()
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        /// <inheritdoc />
        public void OnFocusEnter(FocusEventData eventData)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }

        /// <inheritdoc />
        public void OnFocusExit(FocusEventData eventData)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}