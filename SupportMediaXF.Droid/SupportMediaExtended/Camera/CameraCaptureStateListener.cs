using System;
using Android.Hardware.Camera2;

namespace SupportMediaXF.Droid.SupportMediaExtended.Camera
{
    public class CameraCaptureStateListener : CameraCaptureSession.StateCallback
    {
        /// <summary>
        /// The on configure failed action.
        /// </summary>
        public Action<CameraCaptureSession> OnConfigureFailedAction;

        /// <summary>
        /// The on configured action.
        /// </summary>
        public Action<CameraCaptureSession> OnConfiguredAction;

        /// <summary>
        /// Ons the configure failed.
        /// </summary>
        /// <param name="session">Session.</param>
        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            if (OnConfigureFailedAction != null)
            {
                OnConfigureFailedAction(session);
            }
        }

        /// <summary>
        /// Ons the configured.
        /// </summary>
        /// <param name="session">Session.</param>
        public override void OnConfigured(CameraCaptureSession session)
        {
            if (OnConfiguredAction != null)
            {
                OnConfiguredAction(session);
            }
        }
    }
}
