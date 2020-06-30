﻿using System;
using System.Linq;
using UIKit;

namespace SupportMediaXF.iOS.SupportMediaExtended
{
    public static class Ext
    {
        public const string DateTimeStandardFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
        public static string ToFormatString(this DateTime dateTime, string format = DateTimeStandardFormat)
        {
            var result = dateTime.ToString(format);
            return result;
        }
    }

    public class NaviExtensions
    {
        

        public static UIViewController GetTopController()
        {
            UIViewController viewController = null;
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            if (window == null)
                throw new InvalidOperationException("There's no current active window");

            if (window.WindowLevel == UIWindowLevel.Normal)
                viewController = window.RootViewController;

            if (viewController == null)
            {
                window = UIApplication.SharedApplication.Windows.OrderByDescending(w => w.WindowLevel).FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);
                if (window == null)
                    throw new InvalidOperationException("Could not find current view controller");
                else
                    viewController = window.RootViewController;
            }

            while (viewController.PresentedViewController != null)
                viewController = viewController.PresentedViewController;

            return viewController;

            //MediaPickerDelegate ndelegate = new MediaPickerDelegate(viewController, sourceType, options);
            //var od = Interlocked.CompareExchange(ref pickerDelegate, ndelegate, null);
            //if (od != null)
            //throw new InvalidOperationException("Only one operation can be active at at time");


            //if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)// && sourceType == UIImagePickerControllerSourceType.PhotoLibrary)
            //{
            //    //ndelegate.Popover = popover = new UIPopoverController(picker);
            //    //ndelegate.Popover.Delegate = new MediaPickerPopoverDelegate(ndelegate, picker);
            //    //ndelegate.DisplayPopover();
            //    viewController.PresentModalViewController(openController, true);
            //}
            //else
            //{
            //    if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            //    {
            //        //picker.ModalPresentationStyle = options?.ModalPresentationStyle == MediaPickerModalPresentationStyle.OverFullScreen  ? UIModalPresentationStyle.OverFullScreen : UIModalPresentationStyle.FullScreen;
            //    }
            //    //openController.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            //    //openController.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
            //    // openController.ModalPresentationCapturesStatusBarAppearance = true;
            //    viewController.PresentModalViewController(openController, true);
            //}
        }

        public static void OpenController(UIViewController openController)
        {
            UIViewController viewController = null;
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            if (window == null)
                throw new InvalidOperationException("There's no current active window");

            if (window.WindowLevel == UIWindowLevel.Normal)
                viewController = window.RootViewController;

            if (viewController == null)
            {
                window = UIApplication.SharedApplication.Windows.OrderByDescending(w => w.WindowLevel).FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);
                if (window == null)
                    throw new InvalidOperationException("Could not find current view controller");
                else
                    viewController = window.RootViewController;
            }

            while (viewController.PresentedViewController != null)
                viewController = viewController.PresentedViewController;

            //MediaPickerDelegate ndelegate = new MediaPickerDelegate(viewController, sourceType, options);
            //var od = Interlocked.CompareExchange(ref pickerDelegate, ndelegate, null);
            //if (od != null)
            //throw new InvalidOperationException("Only one operation can be active at at time");


            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)// && sourceType == UIImagePickerControllerSourceType.PhotoLibrary)
            {
                //ndelegate.Popover = popover = new UIPopoverController(picker);
                //ndelegate.Popover.Delegate = new MediaPickerPopoverDelegate(ndelegate, picker);
                //ndelegate.DisplayPopover();
                viewController.PresentModalViewController(openController, true);
            }
            else
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                {
                    //picker.ModalPresentationStyle = options?.ModalPresentationStyle == MediaPickerModalPresentationStyle.OverFullScreen  ? UIModalPresentationStyle.OverFullScreen : UIModalPresentationStyle.FullScreen;
                }
                //openController.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                //openController.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                // openController.ModalPresentationCapturesStatusBarAppearance = true;
                viewController.PresentModalViewController(openController, true);
            }
        }
    }
}
