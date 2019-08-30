using System;
using Vuforia;

namespace HoloCAD
{
    public class Mark : DefaultTrackableEventHandler
    {
        public bool IsActive { get; private set; }

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
    }
}
