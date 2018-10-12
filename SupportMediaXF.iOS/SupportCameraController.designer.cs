// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SupportMediaXF.iOS
{
    [Register ("SupportCameraController")]
    partial class SupportCameraController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonBack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonCapture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonFlash { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView CameraView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ViewTop { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ButtonBack != null) {
                ButtonBack.Dispose ();
                ButtonBack = null;
            }

            if (ButtonCapture != null) {
                ButtonCapture.Dispose ();
                ButtonCapture = null;
            }

            if (ButtonFlash != null) {
                ButtonFlash.Dispose ();
                ButtonFlash = null;
            }

            if (ButtonSwitch != null) {
                ButtonSwitch.Dispose ();
                ButtonSwitch = null;
            }

            if (CameraView != null) {
                CameraView.Dispose ();
                CameraView = null;
            }

            if (ViewTop != null) {
                ViewTop.Dispose ();
                ViewTop = null;
            }
        }
    }
}