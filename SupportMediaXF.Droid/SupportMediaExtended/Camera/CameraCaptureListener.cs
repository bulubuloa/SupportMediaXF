using System;
using System;

using Java.IO;

using Android.Hardware.Camera2;
using Android.Widget;

namespace SupportMediaXF.Droid.SupportMediaExtended.Camera
{
    public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
    {
        /// <summary>
        /// Occurs when photo complete.
        /// </summary>
        public event EventHandler PhotoComplete;

        /// <summary>
        /// Ons the capture completed.
        /// </summary>
        /// <param name="session">Session.</param>
        /// <param name="request">Request.</param>
        /// <param name="result">Result.</param>
        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request,TotalCaptureResult result)
        {
            PhotoComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}
