// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace HoloCore.UI.Transparent
{
    /// <summary> Компонент для управления прозрачностью объекта. </summary>
    /// <remarks> Это базовый класс. Реализацию для конкретных случаев определяйте в наследниках. </remarks>
    public class Transparency : MonoBehaviour
    {
        /// <summary> Прозрачность объекта. </summary>
        /// <remarks> Определена на отрезке [0, 1]. </remarks>
        public float Alpha
        {
            get => _alpha;
            set
            {
                if (value > 1) value = 1;
                if (value < 0) value = 0;

                _alpha = value;
                AlphaEditor = value;
                _alphaEditorLast = value;

                OnAlphaChanged();
            }
        }

        /// <summary> Функция, реализующая изменение прозрачности объекта. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnAlphaChanged()</c>.
        /// </remarks>
        public virtual void OnAlphaChanged()
        {
            foreach (Transform child in transform)
            {
                var transparency = child.GetComponent<Transparency>();
                if (transparency != null) transparency.Alpha = Alpha;
            }
        }

        /// <summary> Функция для обработки изменения положения слайдера из MRTK. </summary>
        /// <param name="data"> Данные слайдера. </param>
        public void OnAlphaSliderUpdated(SliderEventData data)
        {
            Alpha = data.NewValue + 0.2f;
        }

        #region Unity event functions

        /// <summary> Функция, выполняющаяся в Unity каждый кадр. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Update()</c>.
        /// </remarks>
        protected virtual void Update()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (AlphaEditor != _alphaEditorLast) //-V3024
            {
                Alpha = AlphaEditor;
                _alphaEditorLast = AlphaEditor;
            }
        }

        #endregion

        #region Private definitions

        private float _alpha;

        /// <summary> Поле для изменения прозрачности через редактор. </summary>
        [SerializeField, Tooltip("Прозрачность объекта. Определена на отрезке [0, 1].")]
        private float AlphaEditor = 1f;

        private float _alphaEditorLast;

        #endregion
    }
}