using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace HoloCore.UI
{
    public class HoverHighlighter : MonoBehaviour, IMixedRealityFocusHandler
    {
        public Material DefaultMaterial;
        public Material HoverMaterial;
        public Renderer HighlightedComponent;
        
        public void OnFocusEnter(FocusEventData eventData)
        {
            HighlightedComponent.material = HoverMaterial;
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            HighlightedComponent.material = DefaultMaterial;
        }
    }
}