using HoloCore;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary>Класс, который содержит параметры для выталкивания панелей меток при попадании их внутрь других объектов </summary>
    public class MarkControlPusherData : Singleton<MarkControlPusherData>
    {
        /// <summary> Маска всех слоев, с которыми проверяется коллизия </summary>
        public int LayerMask
        {
            get
            {
                return _layerMask;
            }
        }
    
        /// <summary> Растояние между камерой и меткой, при которой активируется выталкивание </summary>
        public float TriggerDistance;
        
        /// <summary> Список слоев, с которыми проверяется коллизия у панелей</summary>
        public int[] LayersOfColliders;
   
        /// <summary> Глубина выталкивания из объекта коллизии </summary>
        public float PushDepth;
        
        /// <summary> Объект главной камеры </summary>
        public  Camera MainCamera;


        

        void Start()
        {
            foreach (int layer in LayersOfColliders)
                _layerMask |= 1 << layer;
        }


        #region Private defenitions

        private int _layerMask;

        #endregion
    }
}
