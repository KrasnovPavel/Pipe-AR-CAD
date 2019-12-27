// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

namespace HoloCAD.UI
{
    /// <summary> Интерфейс для компонентов, которые хотят реагировать на выбор. </summary>
    public interface ISelectable
    {
        /// <summary> Выбран ли объект. </summary>
        bool IsSelected { get; }
        
        /// <summary> Переключить состояние выбора. </summary>
        void ToggleSelection();
        
        /// <summary> Выбрать этот объект. </summary>
        void SelectThis();
        
        /// <summary> Снять выбор этого объекта. </summary>
        void DeselectThis();
        
        /// <summary> Событие вызываемое при выборе объекта. </summary>
        void OnSelect();
        
        /// <summary> Событие вызываемое при снятии выбора объекта. </summary>
        void OnDeselect();
    }
}