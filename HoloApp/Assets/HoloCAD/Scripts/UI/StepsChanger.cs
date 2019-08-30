using System.ComponentModel;
using HoloCAD.UnityTubes;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
    public sealed class StepsChanger : MonoBehaviour
    {
        /// <summary> Кнопка увеличения шага изменения угла. </summary>
        [Tooltip("Кнопка увеличения шага изменения угла.")]
        [CanBeNull] public Button3D IncreaseAngularStepButton;

        /// <summary> Кнопка уменьшения шага изменения угла. </summary>
        [Tooltip("Кнопка уменьшения шага изменения угла.")]
        [CanBeNull] public Button3D DecreaseAngularStepButton;
        
        /// <summary> Кнопка увеличения шага изменения линейных размеров. </summary>
        [Tooltip("Кнопка увеличения шага изменения линейных размеров.")]
        [CanBeNull] public Button3D IncreaseLinearStepButton;

        /// <summary> Кнопка уменьшения шага изменения угла. </summary>
        [Tooltip("Кнопка уменьшения шага изменения линейных размеров.")]
        [CanBeNull] public Button3D DecreaseLinearStepButton;

        /// <summary> Объект, отображающий текстовые данные о шагах. </summary>
        [Tooltip("Объект, отображающий текстовые данные о шагах.")]
        [CanBeNull] public TextMesh TextLabel;

        #region Unity event functions

        private void Start()
        {
            if (IncreaseAngularStepButton != null)
            {
                IncreaseAngularStepButton.OnClick += delegate { Steps.IncreaseAngular(); };
            }
            if (DecreaseAngularStepButton != null)
            {
                DecreaseAngularStepButton.OnClick += delegate { Steps.DecreaseAngular(); };
            }
            if (IncreaseLinearStepButton != null)
            {
                IncreaseLinearStepButton.OnClick += delegate { Steps.IncreaseLinear(); };
            }
            if (DecreaseLinearStepButton != null)
            {
                DecreaseLinearStepButton.OnClick += delegate { Steps.DecreaseLinear(); };
            }

            SetText();
            Steps.Instance.PropertyChanged += OnStepChanged;
        }
        
        #endregion

        #region Private definitions

        private void OnStepChanged(object o, PropertyChangedEventArgs args)
        {
            SetText();
        }

        private void SetText()
        {
            if (TextLabel != null)
            {
                TextLabel.text = $"ΔL: {Steps.Linear:0.000}м. \n Δα: {Steps.Angular:0.0}°.";
            }
        }

        #endregion
    }
}