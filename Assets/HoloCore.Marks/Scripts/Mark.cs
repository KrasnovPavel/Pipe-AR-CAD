// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Vuforia;

namespace HoloCore.Marks
{
    public class Mark : DefaultTrackableEventHandler, INotifyPropertyChanged
    {
        /// <summary> Распознана ли метка? </summary>
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                if (value == _isActive) return;
                _isActive = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public override void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
        {
            base.OnTrackableStateChanged(previousStatus, newStatus);
            switch (newStatus)
            {
                case TrackableBehaviour.Status.TRACKED:
                    IsActive = true;
                    break;
                case TrackableBehaviour.Status.EXTENDED_TRACKED:
                    IsActive = false;
                    break;
                case TrackableBehaviour.Status.NO_POSE:
                    break;
                case TrackableBehaviour.Status.LIMITED:
                    break;
                case TrackableBehaviour.Status.DETECTED:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newStatus), newStatus, null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private bool _isActive;
    }
}
