using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;
using SelfControl.Droid.Helpers.Listeners;
using SelfControl.Droid.Renderers;
using System;
using System.Collections.Generic;
using Boolean = Java.Lang.Boolean;
using Math = Java.Lang.Math;
using Orientation = Android.Content.Res.Orientation;

namespace SelfControl.Droid.Helpers
{
    public class CameraFragment : Java.Lang.Object, View.IOnClickListener
    {
        private static readonly string TAG = "CameraFragment";

        private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();
        public static readonly int REQUEST_CAMERA_PERMISSION = 1;

        private const int FLASH_OFF = 0;
        private const int FLASH_ON = 1;
        private const int FLASH_AUTO = 2;

        private ImageView ImageDisplay;

        public Activity Activity;

        public const int STATE_PREVIEW = 0;
        public const int STATE_WAITING_LOCK = 1;
        public const int STATE_WAITING_PRECAPTURE = 2;
        public const int STATE_WAITING_NON_PRECAPTURE = 3;
        public const int STATE_PICTURE_TAKEN = 4;

        private DateTime mCaptureTime;

        private float mRadius;

        private int mFlashMode;
        private ImageButton mFlashButton;
        private FlashMenuListener flashMenuListner;

        private static readonly int MAX_PREVIEW_WIDTH = 2560;
        private static readonly int MAX_PREVIEW_HEIGHT = 1440;
        
        private CameraSurfaceTextureListener mSurfaceTextureListener;
        private string mCameraId;
        private AutoFitTextureView mTextureView;
        public CameraCaptureSession mCaptureSession;
        public CameraDevice mCameraDevice;
        private Size mPreviewSize;
        private CameraStateListener mStateCallback;

        private HandlerThread mBackgroundThread;
        public Handler mBackgroundHandler;

        private ImageReader mImageReader;
        public File mFile;
        private ImageAvailableListener mOnImageAvailableListener;
        public CaptureRequest.Builder mPreviewRequestBuilder;
        public CaptureRequest mPreviewRequest;
        
        public int mState = STATE_PREVIEW;

        public Semaphore mCameraOpenCloseLock = new Semaphore(1);
        
        private bool mFlashSupported;
        private int mSensorOrientation;
        
        public CameraCaptureListener mCaptureCallback;

        public CameraPageRenderer mCPR;

        public CameraFragment(CameraPageRenderer cpr, View view, Activity activity) {
            mCPR = cpr;
            
            mStateCallback = new CameraStateListener(this);
            mSurfaceTextureListener = new CameraSurfaceTextureListener(this);

            Activity = activity;

            // fill ORIENTATIONS list
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
        
            mTextureView = (AutoFitTextureView)view.FindViewById(Resource.Id.texture);
            view.FindViewById(Resource.Id.snap_button).SetOnClickListener(this);
            view.FindViewById(Resource.Id.gallery_button).SetOnClickListener(this);
            mFlashButton = view.FindViewById<ImageButton>(Resource.Id.flash_button);
            mFlashButton.SetOnClickListener(this);
            view.FindViewById(Resource.Id.back_button).SetOnClickListener(this);
            ImageDisplay = view.FindViewById<ImageView>(Resource.Id.img_display);
        
            mCaptureCallback = new CameraCaptureListener(this);
            mOnImageAvailableListener = new ImageAvailableListener(this);
            flashMenuListner = new FlashMenuListener(this);
            mFlashMode = 0;
        }

        public void Start()
        {
            StartBackgroundThread();

            // When the screen is turned off and turned back on, the SurfaceTexture is already
            // available, and "onSurfaceTextureAvailable" will not be called. In that case, we can open
            // a camera and start preview from here (otherwise, we wait until the surface is ready in
            // the SurfaceTextureListener).
            if (mTextureView.IsAvailable)
            {
                OpenCamera(mTextureView.Width, mTextureView.Height);
            }
            else
            {
                mTextureView.SurfaceTextureListener = mSurfaceTextureListener;
            }
        }

        private static Size ChooseOptimalSize(Size[] choices, int textureViewWidth,
            int textureViewHeight, int maxWidth, int maxHeight, Size aspectRatio)
        {
            // Collect the supported resolutions that are at least as big as the preview Surface
            var bigEnough = new List<Size>();
            // Collect the supported resolutions that are smaller than the preview Surface
            var notBigEnough = new List<Size>();
            int w = aspectRatio.Width;
            int h = aspectRatio.Height;

            for (var i = 0; i < choices.Length; i++)
            {
                Size option = choices[i];
                if ((option.Width <= maxWidth) && (option.Height <= maxHeight) &&
                       option.Height == option.Width * h / w)
                {
                    if (option.Width >= textureViewWidth &&
                        option.Height >= textureViewHeight)
                    {
                        bigEnough.Add(option);
                    }
                    else
                    {
                        notBigEnough.Add(option);
                    }
                }
            }

            // Pick the smallest of those big enough. If there is no one big enough, pick the
            // largest of those not big enough.
            if (bigEnough.Count > 0)
            {
                return (Size)Collections.Min(bigEnough, new CompareSizesByArea());
            }
            else if (notBigEnough.Count > 0)
            {
                return (Size)Collections.Max(notBigEnough, new CompareSizesByArea());
            }
            else
            {
                Log.Error(TAG, "Couldn't find any suitable preview size");
                return choices[0];
            }
        }

        public void setFlash(CaptureRequest.Builder CaptureRequestBuilder)
        {
            if (mFlashSupported)
            {
                switch (mFlashMode)
                {
                    case FLASH_OFF:
                        CaptureRequestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.On);
                        CaptureRequestBuilder.Set(CaptureRequest.FlashMode, (int)Android.Hardware.Camera2.FlashMode.Off);
                        break;
                    case FLASH_ON:
                        CaptureRequestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.On);
                        CaptureRequestBuilder.Set(CaptureRequest.FlashMode, (int)Android.Hardware.Camera2.FlashMode.Single);
                        break;
                    case FLASH_AUTO:
                        CaptureRequestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);
                        break;
                }
            }
        }

        private void SetUpCameraOutputs(int width, int height)
        {
            var activity = Activity;
            var manager = (CameraManager)activity.GetSystemService(Android.Content.Context.CameraService);
            try
            {
                for (var i = 0; i < manager.GetCameraIdList().Length; i++)
                {
                    var cameraId = manager.GetCameraIdList()[i];
                    CameraCharacteristics characteristics = manager.GetCameraCharacteristics(cameraId);

                    // We don't use a front facing camera in this sample.
                    var facing = (Integer)characteristics.Get(CameraCharacteristics.LensFacing);
                    if (facing != null && facing == (Integer.ValueOf((int)LensFacing.Front)))
                    {
                        continue;
                    }

                    var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                    if (map == null)
                    {
                        continue;
                    }

                    // For still image captures, we use the largest available size.
                    Size largest = (Size)Collections.Max(Arrays.AsList(map.GetOutputSizes((int)ImageFormatType.Jpeg)),
                        new CompareSizesByArea());
                    mImageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, /*maxImages*/2);
                    mImageReader.SetOnImageAvailableListener(mOnImageAvailableListener, mBackgroundHandler);

                    // Find out if we need to swap dimension to get the preview size relative to sensor
                    // coordinate.
                    var displayRotation = activity.WindowManager.DefaultDisplay.Rotation;
                    //noinspection ConstantConditions
                    mSensorOrientation = (int)characteristics.Get(CameraCharacteristics.SensorOrientation);
                    bool swappedDimensions = false;
                    switch (displayRotation)
                    {
                        case SurfaceOrientation.Rotation0:
                        case SurfaceOrientation.Rotation180:
                            if (mSensorOrientation == 90 || mSensorOrientation == 270)
                            {
                                swappedDimensions = true;
                            }
                            break;
                        case SurfaceOrientation.Rotation90:
                        case SurfaceOrientation.Rotation270:
                            if (mSensorOrientation == 0 || mSensorOrientation == 180)
                            {
                                swappedDimensions = true;
                            }
                            break;
                        default:
                            Log.Error(TAG, "Display rotation is invalid: " + displayRotation);
                            break;
                    }

                    DisplayMetrics displaySize = new DisplayMetrics();
                    activity.WindowManager.DefaultDisplay.GetRealMetrics(displaySize);
                    var rotatedPreviewWidth = width;
                    var rotatedPreviewHeight = height;
                    var maxPreviewWidth = displaySize.WidthPixels;
                    var maxPreviewHeight = displaySize.HeightPixels;

                    if (swappedDimensions)
                    {
                        rotatedPreviewWidth = height;
                        rotatedPreviewHeight = width;
                        maxPreviewWidth = displaySize.HeightPixels;
                        maxPreviewHeight = displaySize.WidthPixels;
                    }

                    if (maxPreviewWidth > MAX_PREVIEW_WIDTH)
                    {
                        maxPreviewWidth = MAX_PREVIEW_WIDTH;
                    }

                    if (maxPreviewHeight > MAX_PREVIEW_HEIGHT)
                    {
                        maxPreviewHeight = MAX_PREVIEW_HEIGHT;
                    }

                    // Danger, W.R.! Attempting to use too large a preview size could  exceed the camera
                    // bus' bandwidth limitation, resulting in gorgeous previews but the storage of
                    // garbage capture data.
                    mPreviewSize = ChooseOptimalSize(map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))),
                        rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth,
                        maxPreviewHeight, largest);

                    // We fit the aspect ratio of TextureView to the size of preview we picked.
                    var orientation = Activity.Resources.Configuration.Orientation;
                    if (orientation == Orientation.Landscape)
                    {
                        mTextureView.SetAspectRatio(mPreviewSize.Width, mPreviewSize.Height);
                    }
                    else
                    {
                        mTextureView.SetAspectRatio(mPreviewSize.Height, mPreviewSize.Width);
                    }

                    // Check if the flash is supported.
                    var available = (Boolean)characteristics.Get(CameraCharacteristics.FlashInfoAvailable);
                    if (available == null)
                    {
                        mFlashSupported = false;
                        mFlashButton.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        mFlashSupported = (bool)available;
                    }

                    mCameraId = cameraId;
                    return;
                }
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (NullPointerException e)
            {
                e.PrintStackTrace();
            }
        }

        private void RequestCameraPermission()
        {
            ActivityCompat.RequestPermissions(this.Activity, new string[] { Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage },
                  REQUEST_CAMERA_PERMISSION);
        }

        public void OnRequestPermissionsResult(int requestCode, string[] permissions, int[] grantResults)
        {
            if (requestCode != REQUEST_CAMERA_PERMISSION)
                return;

            if (grantResults.Length != 2 || 
                (grantResults[0] != (int)Permission.Granted && grantResults[1] != (int)Permission.Granted))
            {
                this.Activity.Finish();
            }
            else
            {
                OpenCamera(mTextureView.Width, mTextureView.Height);
            }
        }

        public void OpenCamera(int width, int height)
        {
            if ((ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.Camera) != Permission.Granted) &&
                (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) != Permission.Granted))
            {
                RequestCameraPermission();
                return;
            }
            SetUpCameraOutputs(width, height);
            ConfigureTransform(width, height);
            var activity = Activity;
            var manager = (CameraManager)activity.GetSystemService(Android.Content.Context.CameraService);
            try
            {
                if (!mCameraOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
                {
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                }
                manager.OpenCamera(mCameraId, mStateCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera opening.", e);
            }
        }
        
        public void CloseCamera()
        {
            try
            {
                mCameraOpenCloseLock.Acquire();
                if (null != mCaptureSession)
                {
                    mCaptureSession.Close();
                    mCaptureSession = null;
                }
                if (null != mCameraDevice)
                {
                    mCameraDevice.Close();
                    mCameraDevice = null;
                }
                if (null != mImageReader)
                {
                    mImageReader.Close();
                    mImageReader = null;
                }
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera closing.", e);
            }
            finally
            {
                mCameraOpenCloseLock.Release();
            }
        }

        private void StartBackgroundThread()
        {
            mBackgroundThread = new HandlerThread("CameraBackground");
            mBackgroundThread.Start();
            mBackgroundHandler = new Handler(mBackgroundThread.Looper);
        }
        
        private void StopBackgroundThread()
        {
            mBackgroundThread.QuitSafely();
            try
            {
                mBackgroundThread.Join();
                mBackgroundThread = null;
                mBackgroundHandler = null;
            }
            catch (InterruptedException e)
            {
                e.PrintStackTrace();
            }
        }

        float aspectRatio;
        public void CreateCameraPreviewSession()
        {
            try
            {
                SurfaceTexture texture = mTextureView.SurfaceTexture;
                if (texture == null)
                {
                    throw new IllegalStateException("texture is null");
                }

                // We configure the size of default buffer to be the size of camera preview we want.
                texture.SetDefaultBufferSize(mPreviewSize.Width, mPreviewSize.Height);

                // This is the output Surface we need to start preview.
                Surface surface = new Surface(texture);
                aspectRatio = (float)(mPreviewSize.Height) / (float)(mPreviewSize.Width);
                
                Activity.RunOnUiThread(new Runnable(() => {
                    Matrix affineMatrix = new Matrix();
                    mTextureView.GetTransform(affineMatrix);
                    affineMatrix.PostTranslate(-mTextureView.Width / 2, -mTextureView.Height / 2);
                    affineMatrix.PostScale((float)SelfControl.Helpers.Settings.CameraScaleSettings, (float)SelfControl.Helpers.Settings.CameraScaleSettings);
                    affineMatrix.PostTranslate(mTextureView.Width / 2, mTextureView.Height / 2);
                    mTextureView.SetTransform(affineMatrix);
                    ImageView roi = (ImageView)Activity.FindViewById(Resource.Id.roiView);
                    roi.Left = mTextureView.Left;
                    roi.Top = mTextureView.Top;
                    roi.Right = mTextureView.Right;
                    roi.Bottom = mTextureView.Bottom;
                    int orientation = (int)Activity.Resources.Configuration.Orientation;
                    Bitmap bitmap;
                    aspectRatio = (float)(mPreviewSize.Height) / (float)(mPreviewSize.Width);
                    roi.ScaleX = 1.0f;
                    roi.ScaleY = (((mPreviewSize.Height * 1) / aspectRatio) / mPreviewSize.Width);
                    bitmap = Bitmap.CreateBitmap(roi.Width, roi.Height, Bitmap.Config.Argb8888);
                    Canvas canvas = new Canvas(bitmap);
                    Paint paint = new Paint();
                    paint.SetARGB(191, 0, 0, 0);
                    /*float cellHeight = bitmap.Height / 7;
                    float cellWidth = bitmap.Width / 7;
                    canvas.DrawRect(0, 0, bitmap.Width, cellHeight, paint);
                    canvas.DrawRect(0, cellHeight, cellWidth, bitmap.Height - cellHeight, paint);
                    canvas.DrawRect(0, bitmap.Height - cellHeight, bitmap.Width, bitmap.Height, paint);
                    canvas.DrawRect(bitmap.Width - cellWidth, cellHeight, bitmap.Width, bitmap.Height - cellHeight, paint);*/
                    canvas.DrawRect(0, 0, bitmap.Width, bitmap.Height, paint);

                    paint.Color = Color.Transparent; // An obvious color to help debugging
                    paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcOut));
                    float centerX = bitmap.Width / 2;
                    float centerY = bitmap.Height / 2;
                    float radius = Math.Min(bitmap.Width, bitmap.Height) / 2 - Math.Min(bitmap.Width, bitmap.Height) / 5;
                    mRadius = radius;
                    canvas.DrawCircle(centerX, centerY, radius, paint);
                    roi.SetImageBitmap(bitmap);
                    roi.Invalidate();
                }));

            // We set up a CaptureRequest.Builder with the output Surface.
            mPreviewRequestBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                mPreviewRequestBuilder.AddTarget(surface);

                // Here, we create a CameraCaptureSession for camera preview.
                List<Surface> surfaces = new List<Surface>();
                surfaces.Add(surface);
                surfaces.Add(mImageReader.Surface);
                
                mCameraDevice.CreateCaptureSession(surfaces, new CameraCaptureSessionCallback(this), null);

            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public static T Cast<T>(Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }

        public void ConfigureTransform(int viewWidth, int viewHeight)
        {
            Activity activity = Activity;
            if (null == mTextureView || null == mPreviewSize || null == activity)
            {
                return;
            }
            var rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
            Matrix matrix = new Matrix();
            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, mPreviewSize.Height, mPreviewSize.Width);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            if ((int)SurfaceOrientation.Rotation90 == rotation || (int)SurfaceOrientation.Rotation270 == rotation)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = Math.Max((float)viewHeight / mPreviewSize.Height, (float)viewWidth / mPreviewSize.Width);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * (rotation - 2), centerX, centerY);
            }
            else if ((int)SurfaceOrientation.Rotation180 == rotation)
            {
                matrix.PostRotate(180, centerX, centerY);
            }
            mTextureView.SetTransform(matrix);
        }

        public void TakePicture()
        {
            LockFocus();
        }
        
        private void LockFocus()
        {
            try
            {
                // This is how to tell the camera to lock focus.

                mPreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Start);
                // Tell #mCaptureCallback to wait for the lock.
                mState = STATE_WAITING_LOCK;
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback,
                        mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public void RunPrecaptureSequence()
        {
            try
            {
                // This is how to tell the camera to trigger.
                mPreviewRequestBuilder.Set(CaptureRequest.ControlAePrecaptureTrigger, (int)ControlAEPrecaptureTrigger.Start);
                // Tell #mCaptureCallback to wait for the precapture sequence to be set.
                mState = STATE_WAITING_PRECAPTURE;
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        public int FlashMode { get => mFlashMode; set => mFlashMode = value; }
        public ImageButton FlashButton { get => mFlashButton; }

        public int Flash_off { get => FLASH_OFF; }
        public int Flash_on { get => FLASH_ON; }
        public int Flash_auto { get => FLASH_AUTO; }
        public ImageView imageDisplay { get => ImageDisplay; set => ImageDisplay = value; }
        public float CircleRadius { get => mRadius; }

        public void CaptureStillPicture()
        {
            try
            {
                var activity = Activity;
                if (null == activity || null == mCameraDevice)
                {
                    return;
                }
                // This is the CaptureRequest.Builder that we use to take a picture.
                CaptureRequest.Builder stillCaptureBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);

                stillCaptureBuilder.AddTarget(mImageReader.Surface);

                // Use the same AE and AF modes as the preview.
                stillCaptureBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);

                if(mFlashMode == FLASH_OFF)
                    setFlash(stillCaptureBuilder);

                // Orientation
                int rotation = (int)activity.WindowManager.DefaultDisplay.Rotation;
                stillCaptureBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(rotation));

                mCaptureSession.StopRepeating();
                mCaptureSession.Capture(stillCaptureBuilder.Build(), new CameraCaptureStillPictureSessionCallback(this), null);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }
        
        private int GetOrientation(int rotation)
        {
            // Sensor orientation is 90 for most devices, or 270 for some devices (eg. Nexus 5X)
            // We have to take that into account and rotate JPEG properly.
            // For devices with orientation of 90, we simply return our mapping from ORIENTATIONS.
            // For devices with orientation of 270, we need to rotate the JPEG 180 degrees.
            return (ORIENTATIONS.Get(rotation) + mSensorOrientation + 270) % 360;
        }

        void AfterPicture()
        { 
            int rotation = (int)Activity.WindowManager.DefaultDisplay.Rotation;
            mCPR.NavigateImageQuestionPage(mFile.AbsolutePath.ToString(), mCaptureTime);
        }
        
        public void UnlockFocus()
        {
            Activity.RunOnUiThread(() => AfterPicture());
            try
            {
                // Reset the auto-focus trigger
                mPreviewRequestBuilder.Set(CaptureRequest.ControlAfTrigger, (int)ControlAFTrigger.Cancel);
                setFlash(mPreviewRequestBuilder);
                mCaptureSession.Capture(mPreviewRequestBuilder.Build(), mCaptureCallback,
                        mBackgroundHandler);
                // After this, the camera will go back to the normal state of preview.
                mState = STATE_PREVIEW;
                mCaptureSession.SetRepeatingRequest(mPreviewRequest, mCaptureCallback,
                        mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }

        private string GetDateTime()
        {
            mCaptureTime = DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append(mCaptureTime.Year);
            sb.Append(mCaptureTime.Month);
            sb.Append(mCaptureTime.Day);
            sb.Append("_");
            sb.Append(mCaptureTime.Hour);
            sb.Append(mCaptureTime.Minute);
            sb.Append(mCaptureTime.Second);
            sb.Append(".jpg");
            return sb.ToString();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.snap_button:
                    string fileName = GetDateTime();
                    mFile = new File(Activity.GetExternalFilesDir(null), fileName);
                    TakePicture();
                    break;
                case Resource.Id.flash_button:
                    int[] location = new int[2];
                    ImageButton button = (ImageButton)v.FindViewById(Resource.Id.flash_button);
                    button.GetLocationOnScreen(location);
                    Point p = new Point();
                    p.X = location[0];
                    p.Y = location[1];
                    flashMenuListner.showFlashMenu(p);
                    break;
                case Resource.Id.back_button:
                    mCPR.NavigateBack();
                    break;
                case Resource.Id.gallery_button:
                    mCPR.NavigateToGallery();
                    break;
            }
        }
    }
}