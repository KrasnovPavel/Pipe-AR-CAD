// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HoloCore;
using JetBrains.Annotations;

namespace HoloCAD.UnityTubes
{
    /// <summary> Класс управляющий шагами изменения разных величин. </summary>
    public sealed class Steps : Singleton<Steps>, INotifyPropertyChanged
    {
        /// <summary> Шаг изменения линейных размеров. </summary>
        public static float Linear => Instance.LinearSteps[_currentLinearStepIndex];

        /// <summary> Шаг изменения угла. </summary>
        public static float Angular => Instance.AngularSteps[_currentAngularStepIndex];
        
        public List<float> LinearSteps = new List<float>();
        
        public List<float> AngularSteps = new List<float>();

        public event PropertyChangedEventHandler PropertyChanged;
        
        public static void IncreaseLinear()
        {
            if (_currentLinearStepIndex < Instance.LinearSteps.Count - 1)
            {
                _currentLinearStepIndex++;
                Instance.OnPropertyChanged(nameof(Linear));
            }
        }

        public static void DecreaseLinear()
        {
            if (_currentLinearStepIndex > 0)
            {
                _currentLinearStepIndex--;
                Instance.OnPropertyChanged(nameof(Linear));
            }
        }
        
        public static void IncreaseAngular()
        {
            if (_currentAngularStepIndex < Instance.AngularSteps.Count - 1)
            {
                _currentAngularStepIndex++;
                Instance.OnPropertyChanged(nameof(Angular));
            }
        }

        public static void DecreaseAngular()
        {
            if (_currentAngularStepIndex > 0)
            {
                _currentAngularStepIndex--;
                Instance.OnPropertyChanged(nameof(Angular));
            }
        }

        #region Private definitions

        private static int _currentLinearStepIndex;
        private static int _currentAngularStepIndex;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}