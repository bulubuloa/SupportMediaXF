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
    [Register ("SupportGalleryPickerController")]
    partial class SupportGalleryPickerController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonBack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonDone { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonSpinner { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView CollectionGallery { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ViewBottom { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ViewTop { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ButtonBack != null) {
                ButtonBack.Dispose ();
                ButtonBack = null;
            }

            if (ButtonDone != null) {
                ButtonDone.Dispose ();
                ButtonDone = null;
            }

            if (ButtonSpinner != null) {
                ButtonSpinner.Dispose ();
                ButtonSpinner = null;
            }

            if (CollectionGallery != null) {
                CollectionGallery.Dispose ();
                CollectionGallery = null;
            }

            if (ViewBottom != null) {
                ViewBottom.Dispose ();
                ViewBottom = null;
            }

            if (ViewTop != null) {
                ViewTop.Dispose ();
                ViewTop = null;
            }
        }
    }
}