// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace HoloCore.UI
{
    public class Highlighter : MonoBehaviour, IMixedRealityFocusHandler, IMixedRealityPointerHandler, ISelectable
    {
        [CanBeNull] public Material DefaultMaterial;
        [CanBeNull] public Material HoverMaterial;
        [CanBeNull] public Material PressedMaterial;
        [CanBeNull] public Material SelectedMaterial;
        [CanBeNull] public Material SelectedHoverMaterial;
        [CanBeNull] public Material SelectedPressedMaterial;
        public             Renderer HighlightedComponent;

        #region Event functions

        public void OnFocusEnter(FocusEventData eventData)
        {
            _focused = true;
            SetMaterial(SelectedHoverMaterial, HoverMaterial);
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            _focused = false;
            SetMaterial(SelectedMaterial, DefaultMaterial);
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            SetMaterial(SelectedPressedMaterial, PressedMaterial);
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (_focused)
            {
                SetMaterial(SelectedHoverMaterial, HoverMaterial);
            }
            else
            {
                SetMaterial(SelectedMaterial, DefaultMaterial);
            }
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            // Do nothing
        }

        public void OnSelect()
        {
            _selected = true;
            if (_focused)
            {
                SetMaterial(SelectedHoverMaterial, HoverMaterial);
            }
            else
            {
                SetMaterial(SelectedMaterial, DefaultMaterial);
            }
        }

        public void OnDeselect()
        {
            _selected = false;
            if (_focused)
            {
                SetMaterial(SelectedHoverMaterial, HoverMaterial);
            }
            else
            {
                SetMaterial(SelectedMaterial, DefaultMaterial);
            }
        }

        #endregion

        #region Private definitions

        private bool _selected;
        private bool _focused;

        private void SetMaterial(Material selectedMaterial, Material unselectedMaterial)
        {
            if (_selected && selectedMaterial != null)
            {
                HighlightedComponent.material = selectedMaterial;
            }
            else if (unselectedMaterial != null)
            {
                HighlightedComponent.material = unselectedMaterial;
            }
        }

        #endregion
    }
}