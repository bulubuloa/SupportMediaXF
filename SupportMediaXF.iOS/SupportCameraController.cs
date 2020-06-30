using AVFoundation;
using Foundation;
using SupportMediaXF.iOS.Models;
using SupportMediaXF.iOS.SupportMediaExtended;
using SupportMediaXF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace SupportMediaXF.iOS
{
    public partial class SupportCameraController : UIViewController
    {
        AVCaptureSession captureSession;
        AVCaptureDeviceInput captureDeviceInput;
        AVCaptureStillImageOutput stillImageOutput;
        AVCaptureVideoPreviewLayer videoPreviewLayer;
        public SyncPhotoOptions syncPhotoOptions { set; get; }

        public event Action<PhotoSetNative> OnPicked;

        public SupportCameraController (IntPtr handle) : base (handle)
        {
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            var color = UIColor.FromRGB(64, 64, 64);
            ViewTop.BackgroundColor = color.ColorWithAlpha(0.7f);
            View.BackgroundColor = color.ColorWithAlpha(0.7f); ;

            await AuthorizeCameraUse();
            SetupLiveCameraStream();
        }

        private async Task AuthorizeCameraUse()
        {
            try
            {
                var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

                if (authorizationStatus != AVAuthorizationStatus.Authorized)
                {
                    await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            try
            {
                videoPreviewLayer.Frame = this.View.Bounds;
                videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;

                UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
                switch (orientation)
                {
                    case UIInterfaceOrientation.Portrait:
                        videoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
                        break;
                    case UIInterfaceOrientation.PortraitUpsideDown:
                        videoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.PortraitUpsideDown;
                        break;
                    case UIInterfaceOrientation.LandscapeLeft:
                        videoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
                        break;
                    case UIInterfaceOrientation.LandscapeRight:
                        videoPreviewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public void SetupLiveCameraStream()
        {
            captureSession = new AVCaptureSession();

            var viewLayer = CameraView.Layer;
            videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                Frame = this.View.Bounds
            };
            videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            CameraView.Layer.AddSublayer(videoPreviewLayer);
            
            var captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            ConfigureCameraForDevice(captureDevice);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
            captureSession.AddInput(captureDeviceInput);

            var dictionary = new NSMutableDictionary();
            dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
            stillImageOutput = new AVCaptureStillImageOutput()
            {
                OutputSettings = new NSDictionary()
            };

            captureSession.AddOutput(stillImageOutput);
            captureSession.StartRunning();


            ButtonBack.SetImage(UIImage.FromBundle("arrow_left").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            ButtonFlash.SetImage(UIImage.FromBundle("flash_off").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            ButtonSwitch.SetImage(UIImage.FromBundle("switch_camera").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            ButtonCapture.SetImage(UIImage.FromBundle("capture_camera").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);

            ButtonFlash.TouchUpInside += BttFlash_TouchUpInside;
            ButtonCapture.TouchUpInside += BttCapture_TouchUpInside;
            ButtonBack.TouchUpInside += BttBack_TouchUpInside;
            ButtonSwitch.TouchUpInside += BttSwitch_TouchUpInside;
        }

        void BttSwitch_TouchUpInside(object sender, EventArgs e)
        {
            var devicePosition = captureDeviceInput.Device.Position;
            if (devicePosition == AVCaptureDevicePosition.Front)
            {
                devicePosition = AVCaptureDevicePosition.Back;
            }
            else
            {
                devicePosition = AVCaptureDevicePosition.Front;
            }

            var device = GetCameraForOrientation(devicePosition);
            ConfigureCameraForDevice(device);

            captureSession.BeginConfiguration();
            captureSession.RemoveInput(captureDeviceInput);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(device);
            captureSession.AddInput(captureDeviceInput);
            captureSession.CommitConfiguration();
        }

        public AVCaptureDevice GetCameraForOrientation(AVCaptureDevicePosition orientation)
        {
            var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            foreach (var device in devices)
            {
                if (device.Position == orientation)
                {
                    return device;
                }
            }
            return null;
        }

        void BttBack_TouchUpInside(object sender, EventArgs e)
        {
            OnPicked?.Invoke(null);
            //DismissViewController(true, null);
        }

        void BttFlash_TouchUpInside(object sender, EventArgs e)
        {
            var device = captureDeviceInput.Device;

            var error = new NSError();
            if (device.HasFlash)
            {
                if (device.FlashMode == AVCaptureFlashMode.On)
                {
                    device.LockForConfiguration(out error);
                    device.FlashMode = AVCaptureFlashMode.Off;
                    device.UnlockForConfiguration();

                    ButtonFlash.SetImage(UIImage.FromBundle("flash_off").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                }
                else
                {
                    device.LockForConfiguration(out error);
                    device.FlashMode = AVCaptureFlashMode.On;
                    device.UnlockForConfiguration();

                    ButtonFlash.SetImage(UIImage.FromBundle("flash_on").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                }
            }
        }

        async void BttCapture_TouchUpInside(object sender, EventArgs e)
        {
            var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
            var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);

            var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);

            var image = UIKit.UIImage.LoadFromData(jpegImageAsNsData).ResizeImage(syncPhotoOptions);
            var jpegAsByteArray = image.AsJPEG(syncPhotoOptions.Quality).ToArray();

            var result = new PhotoSetNative();
            result.galleryImageXF.Checked = true;
            result.galleryImageXF.RawData = jpegAsByteArray;
            result.galleryImageXF.ImageSourceXF = ImageSource.FromStream(() => new System.IO.MemoryStream(jpegAsByteArray));

            OnPicked?.Invoke(result);

            //MessagingCenter.Send<SupportCameraController, List<PhotoSetNative>>(this, Utils.SubscribeImageFromCamera, new List<PhotoSetNative>(){
            //    result
            //});
            //DismissModalViewController(true);
        }

        void ConfigureCameraForDevice(AVCaptureDevice device)
        {
            var error = new NSError();
            if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration();
            }
            else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                device.LockForConfiguration(out error);
                device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                device.UnlockForConfiguration();
            }
            else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
            {
                device.LockForConfiguration(out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                device.UnlockForConfiguration();
            }
        }
    }
}