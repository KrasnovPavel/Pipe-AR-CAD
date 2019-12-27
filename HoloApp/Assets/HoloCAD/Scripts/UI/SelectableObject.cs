// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Компонент отвечающий за возможность выбора объекта. </summary>
    public class SelectableObject : MonoBehaviour
    {
        /// <summary> Выбранный в данный момент объект. </summary>
        public static GameObject SelectedObject { get; private set; }

        /// <summary> Сменить "выбранность". </summary>
        public void ToggleSelection()
        {
            if (SelectedObject == gameObject) DeselectThis();
            else                              SelectThis();
        }
            
        /// <summary> Выбрать этот объект. </summary>        
        public void SelectThis()
        {
            if (SelectedObject != null)
            {
                SelectedObject.GetComponent<SelectableObject>().DeselectThis();
            }

            SelectedObject = gameObject;
            OnSelect();
        }

        /// <summary> Снятие выбора этого объекта. </summary>
        public void DeselectThis()
        {
            SelectedObject = null;
            OnDeselect();
        }

        #region Private definition

        /// <summary> Событие вызываемое при выборе объекта. </summary>
        private void OnSelect()
        {
            foreach (var component in GetComponents<MonoBehaviour>())
            {
                (component as ISelectable)?.OnSelect();
            }
        }

        /// <summary> Событие вызываемое при снятии выбора с объекта. </summary>
        private void OnDeselect()
        {
            foreach (var component in GetComponents<MonoBehaviour>())
            {
                if (component is ISelectable selectable)
                {
                    selectable.OnDeselect();
                }
            }
        }

        #endregion
    }
}