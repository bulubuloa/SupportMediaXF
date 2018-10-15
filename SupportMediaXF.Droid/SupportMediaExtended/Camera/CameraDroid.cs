using System;
using System.Collections.Generic;

using Java.IO;
using Java.Lang;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Graphics;
using Android.Content.Res;
using Android.Media;


namespace SupportMediaXF.Droid.SupportMediaExtended.Camera
{
    public class Logger
    {
        public void WriteLine(string text)
        {
            Log.Debug("",text);
        }

        /// <summary>
        /// Writes the line time.
        /// </summary>
        /// <returns>The line time.</returns>
        /// <param name="text">Text.</param>
        /// <param name="args">Arguments.</param>
        public void WriteLineTime(string text, params object[] args)
        {
            Log.Debug("",DateTime.Now.Ticks + " " + string.Format(text, args));
        }

    }

    public class CameraDroid : FrameLayout, TextureView.ISurfaceTextureListener
    {
        #region Static Properties

        /// <summary>
        /// The orientations.
        /// </summary>
        private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when busy.
        /// </summary>
        public event EventHandler<bool> Busy;

        /// <summary>
        /// Occurs when available.
        /// </summary>
        public event EventHandler<bool> Available;

        /// <summary>
        /// Occurs when photo.
        /// </summary>
        public event EventHandler<byte[]> Photo;

        #endregion

        #region Private Properties

        /// <summary>
        /// The tag.
        /// </summary>
        private readonly string _tag;

        /// <summary>
        /// The log.
        /// </summary>
        private readonly Logger _log;

        /// <summary>
        /// The m state listener.
        /// </summary>
        private CameraStateListener _stateListener;

        /// <summary>
        /// The CameraRequest.Builder for camera preview.
        /// </summary>
        private CaptureRequest.Builder _previewBuilder;

        /// <summary>
        /// The CameraCaptureSession for camera preview.
        /// </summary>
        private CameraCaptureSession _previewSession;

        /// <summary>
        /// The view surface.
        /// </summary>
        private SurfaceTexture _viewSurface;

        /// <summary>
        /// The camera texture.
        /// </summary>
        private AutoFitTextureView _cameraTexture;

        /// <summary>
        /// The media sound.
        /// </summary>
        private MediaActionSound _mediaSound;

        /// <summary>
        /// The size of the camera preview.
        /// </summary>
        private Android.Util.Size _previewSize;

        /// <summary>
        /// The context.
        /// </summary>
        private Context _context;

        /// <summary>
        /// The camera manager.
        /// </summary>
        private CameraManager _manager;

        /// <summary>
        /// The media sound loaded.
        /// </summary>
        private bool _mediaSoundLoaded;

        /// <summary>
        /// The opening camera.
        /// </summary>
        private bool _openingCamera;

        #endregion

        #region Public Properties

        /// <summary>
        /// The opening camera.
        /// </summary>
        public bool OpeningCamera
        {
            get
            {
                return _openingCamera;
            }
            set
            {
                if (_openingCamera != value)
                {
                    _openingCamera = value;
                    Busy?.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// The reference to the opened CameraDevice.
        /// </summary>
        public CameraDevice _cameraDevice;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Android_Shared.Controls.CameraDroid"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public CameraDroid(Context context) : base(context)
        {
            _context = context;
            _mediaSoundLoaded = LoadShutterSound();

           // _log = IoC.Resolve<ILogger>();
            _tag = $"{GetType()} ";

            var inflater = LayoutInflater.FromContext(context);

            if (inflater != null)
            {
                var view = inflater.Inflate(Resource.Layout.CameraLayout, this);

                _cameraTexture = view.FindViewById<AutoFitTextureView>(Resource.Id.CameraTexture);
                _cameraTexture.SurfaceTextureListener = this;

               // _stateListener = new CameraStateListener() { Camera = this };

                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
                ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
            }
        }

        public CameraDroid(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected CameraDroid(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CameraDroid(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public CameraDroid(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the camera preview, StartPreview() needs to be called in advance
        /// </summary>
        private void UpdatePreview()
        {
            if (_cameraDevice != null && _previewSession != null)
            {
                try
                {
                    // The camera preview can be run in a background thread. This is a Handler for the camere preview
                    _previewBuilder.Set(CaptureRequest.ControlMode, new Java.Lang.Integer((int)ControlMode.Auto));

                    // We create a Handler since we want to handle the resulting JPEG in a background thread
                    HandlerThread thread = new HandlerThread("CameraPicture");
                    thread.Start();
                    Handler backgroundHandler = new Handler(thread.Looper);

                    // Finally, we start displaying the camera preview
                    //if (_previewSession.IsReprocessable)
                    _previewSession.SetRepeatingRequest(_previewBuilder.Build(), null, backgroundHandler);
                }
                catch (CameraAccessException error)
                {
                    _log.WriteLineTime(_tag + "\n" +
                        "UpdatePreview() Camera access exception.  \n " +
                        "ErrorMessage: \n" +
                        error.Message + "\n" +
                        "Stacktrace: \n " +
                        error.StackTrace);
                }
                catch (IllegalStateException error)
                {
                    _log.WriteLineTime(_tag + "\n" +
                        "UpdatePreview() Illegal exception.  \n " +
                        "ErrorMessage: \n" +
                        error.Message + "\n" +
                        "Stacktrace: \n " +
                        error.StackTrace);
                }
            }
        }

        /// <summary>
        /// Loads the shutter sound.
        /// </summary>
        /// <returns><c>true</c>, if shutter sound was loaded, <c>false</c> otherwise.</returns>
        private bool LoadShutterSound()
        {
            try
            {
                _mediaSound = new MediaActionSound();
                _mediaSound.LoadAsync(MediaActionSoundType.ShutterClick);

                return true;
            }
            catch (Java.Lang.Exception error)
            {
                _log.WriteLineTime(_tag + "\n" +
                    "LoadShutterSound() Error loading shutter sound  \n " +
                    "ErrorMessage: \n" +
                    error.Message + "\n" +
                    "Stacktrace: \n " +
                    error.StackTrace);
            }

            return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the camera.
        /// </summary>
        public void OpenCamera()
        {
            if (_context == null || OpeningCamera)
            {
                return;
            }

            OpeningCamera = true;

            _manager = (CameraManager)_context.GetSystemService(Context.CameraService);

            try
            {
                string cameraId = _manager.GetCameraIdList()[0];

                // To get a list of available sizes of camera preview, we retrieve an instance of
                // StreamConfigurationMap from CameraCharacteristics
                CameraCharacteristics characteristics = _manager.GetCameraCharacteristics(cameraId);
                StreamConfigurationMap map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                _previewSize = map.GetOutputSizes(Java.Lang.Class.FromType(typeof(SurfaceTexture)))[0];
                Android.Content.Res.Orientation orientation = Resources.Configuration.Orientation;
                if (orientation == Android.Content.Res.Orientation.Landscape)
                {
                    _cameraTexture.SetAspectRatio(_previewSize.Width, _previewSize.Height);
                }
                else
                {
                    _cameraTexture.SetAspectRatio(_previewSize.Height, _previewSize.Width);
                }

                HandlerThread thread = new HandlerThread("CameraPreview");
                thread.Start();
                Handler backgroundHandler = new Handler(thread.Looper);

                // We are opening the camera with a listener. When it is ready, OnOpened of mStateListener is called.
                _manager.OpenCamera(cameraId, _stateListener, null);
            }
            catch (Java.Lang.Exception error)
            {
                _log.WriteLineTime(_tag + "\n" +
                    "OpenCamera() Failed to open camera  \n " +
                    "ErrorMessage: \n" +
                    error.Message + "\n" +
                    "Stacktrace: \n " +
                    error.StackTrace);

                Available?.Invoke(this, false);
            }
            catch (System.Exception error)
            {
                _log.WriteLineTime(_tag + "\n" +
                    "OpenCamera() Failed to open camera  \n " +
                    "ErrorMessage: \n" +
                    error.Message + "\n" +
                    "Stacktrace: \n " +
                    error.StackTrace);

                Available?.Invoke(this, false);
            }
        }

        /// <summary>
        /// Takes the photo.
        /// </summary>
        public void TakePhoto()
        {
            if (_context != null && _cameraDevice != null)
            {
                try
                {
                    Busy?.Invoke(this, true);

                    if (_mediaSoundLoaded)
                    {
                        _mediaSound.Play(MediaActionSoundType.ShutterClick);
                    }

                    // Pick the best JPEG size that can be captures with this CameraDevice
                    var characteristics = _manager.GetCameraCharacteristics(_cameraDevice.Id);
                    Android.Util.Size[] jpegSizes = null;
                    if (characteristics != null)
                    {
                        jpegSizes = ((StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap)).GetOutputSizes((int)ImageFormatType.Jpeg);
                    }
                    int width = 640;
                    int height = 480;

                    if (jpegSizes != null && jpegSizes.Length > 0)
                    {
                        width = jpegSizes[0].Width;
                        height = jpegSizes[0].Height;
                    }

                    // We use an ImageReader to get a JPEG from CameraDevice
                    // Here, we create a new ImageReader and prepare its Surface as an output from the camera
                    var reader = ImageReader.NewInstance(width, height, ImageFormatType.Jpeg, 1);
                    var outputSurfaces = new List<Surface>(2);
                    outputSurfaces.Add(reader.Surface);
                    outputSurfaces.Add(new Surface(_viewSurface));

                    CaptureRequest.Builder captureBuilder = _cameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                    captureBuilder.AddTarget(reader.Surface);
                    captureBuilder.Set(CaptureRequest.ControlMode, new Integer((int)ControlMode.Auto));

                    // Orientation
                    var windowManager = _context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
                    SurfaceOrientation rotation = windowManager.DefaultDisplay.Rotation;

                    captureBuilder.Set(CaptureRequest.JpegOrientation, new Integer(ORIENTATIONS.Get((int)rotation)));

                    // This listener is called when an image is ready in ImageReader 
                    ImageAvailableListener readerListener = new ImageAvailableListener();

                    readerListener.Photo += (sender, e) =>
                    {
                        Photo?.Invoke(this, e);
                    };

                    // We create a Handler since we want to handle the resulting JPEG in a background thread
                    HandlerThread thread = new HandlerThread("CameraPicture");
                    thread.Start();
                    Handler backgroundHandler = new Handler(thread.Looper);
                    reader.SetOnImageAvailableListener(readerListener, backgroundHandler);

                    var captureListener = new CameraCaptureListener();

                    captureListener.PhotoComplete += (sender, e) =>
                    {
                        Busy?.Invoke(this, false);
                        StartPreview();
                    };

                    _cameraDevice.CreateCaptureSession(outputSurfaces, new CameraCaptureStateListener()
                    {
                        OnConfiguredAction = (CameraCaptureSession session) =>
                        {
                            try
                            {
                                _previewSession = session;
                                session.Capture(captureBuilder.Build(), captureListener, backgroundHandler);
                            }
                            catch (CameraAccessException ex)
                            {
                                Log.WriteLine(LogPriority.Info, "Capture Session error: ", ex.ToString());
                            }
                        }
                    }, backgroundHandler);
                }
                catch (CameraAccessException error)
                {
                    _log.WriteLineTime(_tag + "\n" +
                        "TakePhoto() Failed to take photo  \n " +
                        "ErrorMessage: \n" +
                        error.Message + "\n" +
                        "Stacktrace: \n " +
                        error.StackTrace);
                }
                catch (Java.Lang.Exception error)
                {
                    _log.WriteLineTime(_tag + "\n" +
                        "TakePhoto() Failed to take photo  \n " +
                        "ErrorMessage: \n" +
                        error.Message + "\n" +
                        "Stacktrace: \n " +
                        error.StackTrace);
                }
            }
        }

        /// <summary>
        /// Changes the focus point.
        /// </summary>
        /// <param name="e">E.</param>
        public void ChangeFocusPoint(Xamarin.Forms.Point e)
        {
            string cameraId = _manager.GetCameraIdList()[0];

            // To get a list of available sizes of camera preview, we retrieve an instance of
            // StreamConfigurationMap from CameraCharacteristics
            CameraCharacteristics characteristics = _manager.GetCameraCharacteristics(cameraId);

            var rect = characteristics.Get(CameraCharacteristics.SensorInfoActiveArraySize) as Rect;

            int areaSize = 200;
            int right = rect.Right;
            int bottom = rect.Bottom;
            int viewWidth = _cameraTexture.Width;
            int viewHeight = _cameraTexture.Height;
            int ll, rr;

            Rect newRect;
            int centerX = (int)e.X;
            int centerY = (int)e.Y;

            ll = ((centerX * right) - areaSize) / viewWidth;
            rr = ((centerY * bottom) - areaSize) / viewHeight;

            int focusLeft = Clamp(ll, 0, right);
            int focusBottom = Clamp(rr, 0, bottom);

            newRect = new Rect(focusLeft, focusBottom, focusLeft + areaSize, focusBottom + areaSize);
            MeteringRectangle meteringRectangle = new MeteringRectangle(newRect, 500);
            MeteringRectangle[] meteringRectangleArr = { meteringRectangle };
            _previewBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Cancel);
            _previewBuilder.Set(CaptureRequest.ControlAeRegions, meteringRectangleArr);
            _previewBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Start);

            UpdatePreview();
        }

        /// <summary>
        /// Clamp the specified value, min and max.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        private int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        /// <summary>
        /// Starts the camera previe
        /// </summary>
        public void StartPreview()
        {
            if (_cameraDevice != null && _cameraTexture.IsAvailable && _previewSize != null)
            {
                try
                {
                    var texture = _cameraTexture.SurfaceTexture;
                    System.Diagnostics.Debug.Assert(texture != null);

                    // We configure the size of the default buffer to be the size of the camera preview we want
                    texture.SetDefaultBufferSize(_previewSize.Width, _previewSize.Height);

                    // This is the output Surface we need to start the preview
                    Surface surface = new Surface(texture);

                    // We set up a CaptureRequest.Builder with the output Surface
                    _previewBuilder = _cameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                    _previewBuilder.AddTarget(surface);

                    // Here, we create a CameraCaptureSession for camera preview.
                    _cameraDevice.CreateCaptureSession(new List<Surface>() { surface },
                        new CameraCaptureStateListener()
                        {
                            OnConfigureFailedAction = (CameraCaptureSession session) =>
                            {
                            },
                            OnConfiguredAction = (CameraCaptureSession session) =>
                            {
                                _previewSession = session;
                                UpdatePreview();
                            }
                        },
                        null);
                }
                catch (Java.Lang.Exception error)
                {
                    _log.WriteLineTime(_tag + "\n" +
                        "TakePhoto() Failed to start preview \n " +
                        "ErrorMessage: \n" +
                        error.Message + "\n" +
                        "Stacktrace: \n " +
                        error.StackTrace);
                }
            }
        }

        /// <summary>
        /// Switchs the flash.
        /// </summary>
        /// <param name="flashOn">If set to <c>true</c> flash on.</param>
        public void SwitchFlash(bool flashOn)
        {
            try
            {
                _previewBuilder.Set(CaptureRequest.FlashMode, new Integer(flashOn ? (int)FlashMode.Torch : (int)FlashMode.Off));
                UpdatePreview();
            }
            catch (System.Exception error)
            {
                _log.WriteLineTime(_tag + "\n" +
                    "TakePhoto() Failed to switch flash on/off \n " +
                    "ErrorMessage: \n" +
                    error.Message + "\n" +
                    "Stacktrace: \n " +
                    error.StackTrace);

            }
        }

        /// <summary>
        /// Notifies the available.
        /// </summary>
        /// <param name="isAvailable">If set to <c>true</c> is available.</param>
        public void NotifyAvailable(bool isAvailable)
        {
            Available?.Invoke(this, isAvailable);
        }

        /// <summary>
        /// Ons the layout.
        /// </summary>
        /// <param name="l">L.</param>
        /// <param name="t">T.</param>
        /// <param name="r">The red component.</param>
        /// <param name="b">The blue component.</param>
        public void OnLayout(int l, int t, int r, int b)
        {
            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            _cameraTexture.Measure(msw, msh);
            _cameraTexture.Layout(0, 0, r - l, b - t);
        }

        /// <summary>
        /// Configures the transform.
        /// </summary>
        /// <param name="viewWidth">View width.</param>
        /// <param name="viewHeight">View height.</param>
        public void ConfigureTransform(int viewWidth, int viewHeight)
        {
            if (_viewSurface != null && _previewSize != null && _context != null)
            {
                var windowManager = _context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

                var rotation = windowManager.DefaultDisplay.Rotation;
                var matrix = new Matrix();
                var viewRect = new RectF(0, 0, viewWidth, viewHeight);
                var bufferRect = new RectF(0, 0, _previewSize.Width, _previewSize.Height);

                var centerX = viewRect.CenterX();
                var centerY = viewRect.CenterY();

                if (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270)
                {
                    bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                    matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);

                    var scale = System.Math.Max((float)viewHeight / _previewSize.Height, (float)viewWidth / _previewSize.Width);
                    matrix.PostScale(scale, scale, centerX, centerY);
                    matrix.PostRotate(90 * ((int)rotation - 2), centerX, centerY);
                }

                _cameraTexture.SetTransform(matrix);
            }
        }

        /// <summary>
        /// Raises the surface texture available event.
        /// </summary>
        /// <param name="surface">Surface.</param>
        /// <param name="w">The width.</param>
        /// <param name="h">The height.</param>
        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
        {
            _viewSurface = surface;

            ConfigureTransform(w, h);
            StartPreview();
        }

        /// <summary>
        /// Raises the surface texture destroyed event.
        /// </summary>
        /// <param name="surface">Surface.</param>
        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            _previewSession.StopRepeating();

            return true;
        }

        /// <summary>
        /// Raises the surface texture size changed event.
        /// </summary>
        /// <param name="surface">Surface.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            ConfigureTransform(width, height);
            StartPreview();
        }

        /// <summary>
        /// Raises the surface texture updated event.
        /// </summary>
        /// <param name="surface">Surface.</param>
        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }

        #endregion
    }
}
