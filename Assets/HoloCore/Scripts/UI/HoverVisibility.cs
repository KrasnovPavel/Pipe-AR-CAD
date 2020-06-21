// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace HoloCore.UI
{
    /// <summary> Компонент, включающий видимость объекта только при наведении. </summary>
    public class HoverVisibility : MonoBehaviour, IMixedRealityFocusHandler
    {
        private void Start() //-V3013
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