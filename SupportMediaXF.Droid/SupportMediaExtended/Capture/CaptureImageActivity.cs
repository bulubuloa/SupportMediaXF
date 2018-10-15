
using System;
using System.IO;
using Android.App;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace SupportMediaXF.Droid.SupportMediaExtended.Capture
{
    [Activity(Label = "CaptureImageActivity")]
    public class CaptureImageActivity : Activity, TextureView.ISurfaceTextureListener
    {
        Android.Hardware.Camera camera;
        Android.Widget.ImageButton takePhotoButton;
        Android.Widget.ImageButton toggleFlashButton;
        Android.Widget.ImageButton switchCameraButton;

        CameraFacing cameraType;
        TextureView textureView;
        SurfaceTexture surfaceTexture;

        bool flashOn;

        byte[] imageBytes;

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            camera = Android.Hardware.Camera.Open((int)cameraType);
            textureView.LayoutParameters = new RelativeLayout.LayoutParams(width, height);
            surfaceTexture = surface;
            surfaceTexture.SetDefaultBufferSize(2000, 2000);
            camera.SetPreviewTexture(surface);
            PrepareAndStartCamera();
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            camera.StopPreview();
            camera.Release();

            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            PrepareAndStartCamera();
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }

        private void PrepareAndStartCamera()
        {
            camera.StopPreview();

            var display = WindowManager.DefaultDisplay;
            if (display.Rotation == SurfaceOrientation.Rotation0)
            {
                camera.SetDisplayOrientation(90);
            }

            if (display.Rotation == SurfaceOrientation.Rotation270)
            {
                camera.SetDisplayOrientation(180);
            }

            camera.StartPreview();
        }

        private void SwitchCameraButtonTapped(object sender, EventArgs e)
        {
            if (cameraType == CameraFacing.Front)
            {
                cameraType = CameraFacing.Back;

                camera.StopPreview();
                camera.Release();
                camera = Android.Hardware.Camera.Open((int)cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
            else
            {
                cameraType = CameraFacing.Front;

                camera.StopPreview();
                camera.Release();
                camera = Android.Hardware.Camera.Open((int)cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        private void ToggleFlashButtonTapped(object sender, EventArgs e)
        {
            flashOn = !flashOn;
            if (flashOn)
            {
                if (cameraType == CameraFacing.Back)
                {
                    toggleFlashButton.SetImageResource(Resource.Drawable.flash_on);
                    cameraType = CameraFacing.Back;

                    camera.StopPreview();
                    camera.Release();
                    camera = Android.Hardware.Camera.Open((int)cameraType);
                    var parameters = camera.GetParameters();
                    parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
                    camera.SetParameters(parameters);
                    camera.SetPreviewTexture(surfaceTexture);
                    PrepareAndStartCamera();
                }
            }
            else
            {
                toggleFlashButton.SetImageResource(Resource.Drawable.flash_off);
                camera.StopPreview();
                camera.Release();

                camera = Android.Hardware.Camera.Open((int)cameraType);
                var parameters = camera.GetParameters();
                parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
                camera.SetParameters(parameters);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        private async void TakePhotoButtonTapped(object sender, EventArgs e)
        {
            camera.StopPreview();
            //DialogService.ShowLoading("Capturing Every Pixel");

            var image = textureView.Bitmap;
            using (var imageStream = new MemoryStream())
            {
                await image.CompressAsync(Bitmap.CompressFormat.Jpeg, 50, imageStream);
                image.Recycle();
                imageBytes = imageStream.ToArray();
            }

            //var navigationPage = new NavigationPage(new DrawMomentPage(imageBytes))
            //{
            //    BarBackgroundColor = Colors.NavigationBarColor,
            //    BarTextColor = Colors.NavigationBarTextColor
            //};

            //DialogService.HideLoading();
            camera.StartPreview();
            //await App.Current.MainPage.Navigation.PushModalAsync(navigationPage, false);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ActionBar.Hide();
            SetContentView(Resource.Layout.activity_camera_capture);

            cameraType = CameraFacing.Back;

            textureView = FindViewById<TextureView>(Resource.Id.texture);
            textureView.SurfaceTextureListener = this;

            takePhotoButton = FindViewById<Android.Widget.ImageButton>(Resource.Id.bttCapture);
            takePhotoButton.Click += TakePhotoButtonTapped;

            switchCameraButton = FindViewById<Android.Widget.ImageButton>(Resource.Id.bttSwitchCamera);
            switchCameraButton.Click += SwitchCameraButtonTapped;

            toggleFlashButton = FindViewById<Android.Widget.ImageButton>(Resource.Id.flash);
            toggleFlashButton.Click += ToggleFlashButtonTapped;
        }

    }
}
