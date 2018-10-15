using System;
using System.Diagnostics;
using Android.Hardware.Camera2;
using Android.Util;

namespace SupportMediaXF.Droid.SupportMediaExtended.Camera
{
    /// <summary>
    /// Camera state listener.
    /// </summary>
    public class CameraStateListener : CameraDevice.StateCallback
    {
        /// <summary>
        /// The camera.
        /// </summary>
        public CamActivity Camera;

        /// <summary>
        /// Called when camera is connected.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public override void OnOpened(CameraDevice camera)
        {
            Debug.WriteLine("OnOpened");
            Log.Debug("CameraStateListener", "OnOpened");
            if (Camera != null)
            {
                Camera.cameraDevice = camera;
                Camera.StartPreview();
                //Camera.OpeningCamera = false;

            }
        }

        /// <summary>
        /// Called when camera is disconnected.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public override void OnDisconnected(CameraDevice camera)
        {
            Debug.WriteLine("OnDisconnected");
            Log.Debug("CameraStateListener", "OnDisconnected");
            if (Camera != null)
            {
                camera.Close();
                Camera.cameraDevice = null;
                //Camera.OpeningCamera = false;

            }
        }

        /// <summary>
        /// Called when an error occurs.
        /// </summary>
        /// <param name="camera">Camera.</param>
        /// <param name="error">Error.</param>
        public override void OnError(CameraDevice camera, CameraError error)
        {
            Debug.WriteLine("OnError");
            Log.Debug("CameraStateListener", "OnError");
            camera.Close();

            if (Camera != null)
            {
                Camera.cameraDevice = null;
                //Camera.OpeningCamera = false;

            }
        }
    }
}
