using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SelfControl.CameraPage), typeof(SelfControl.iOS.Renderers.CameraPageRenderer))]
namespace SelfControl.iOS.Renderers
{
    class CameraPageRenderer : PageRenderer
    {
        string jpgFilename;
        AVCaptureSession captureSession;
        AVCaptureDeviceInput captureDeviceInput;
        UIButton backButton;
        UIButton galleryButton;
        UIButton toggleFlashButton;
        UIView liveCameraStream;
        AVCaptureStillImageOutput stillImageOutput;
        UIButton takePhotoButton;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetupUserInterface();
            SetupEventHandlers();

            AuthorizeCameraUse();
            SetupLiveCameraStream();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public async void AuthorizeCameraUse()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

            if (authorizationStatus != AVAuthorizationStatus.Authorized)
            {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
            }
        }

        public void SetupLiveCameraStream()
        {
            captureSession = new AVCaptureSession();

            var viewLayer = liveCameraStream.Layer;
            var videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                Frame = liveCameraStream.Bounds
            };
            liveCameraStream.Layer.AddSublayer(videoPreviewLayer);

            var captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            ConfigureCameraForDevice(captureDevice);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);

            var dictionary = new NSMutableDictionary();
            dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
            stillImageOutput = new AVCaptureStillImageOutput()
            {
                OutputSettings = new NSDictionary()
            };

            captureSession.AddOutput(stillImageOutput);
            captureSession.AddInput(captureDeviceInput);
            captureSession.StartRunning();
        }

        public async void CapturePhoto()
        {
            var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
            var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);

            //var jpegImageAsBytes = AVCaptureStillImageOutput.JpegStillToNSData (sampleBuffer).ToArray ();
            var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);
            var image = new UIImage (jpegImageAsNsData);
            var image2 = new UIImage (image.CGImage, image.CurrentScale, UIImageOrientation.UpMirrored);
            var data = image2.AsJPEG().ToArray();
            
            ((SelfControl.CameraPage)Element).PictureClickedHandler("", DateTime.Now, (int)image2.Size.Width, (int)image2.Size.Height, data);
        }

        public void ConfigureCameraForDevice(AVCaptureDevice device)
        {
            var error = new NSError();
            if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration();
            }
            if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                device.LockForConfiguration(out error);
                device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                device.UnlockForConfiguration();
            }
            if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
            {
                device.LockForConfiguration(out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                device.UnlockForConfiguration();
            }
        }

        public void ToggleFlash()
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

                    toggleFlashButton.SetBackgroundImage(UIImage.FromFile("flash_off.9.png"), UIControlState.Normal);
                }
                else if (device.FlashMode == AVCaptureFlashMode.Off)
                {
                    device.LockForConfiguration(out error);
                    device.FlashMode = AVCaptureFlashMode.On;
                    device.UnlockForConfiguration();

                    toggleFlashButton.SetBackgroundImage(UIImage.FromFile("flash_on.9.png"), UIControlState.Normal);
                }
            }
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

        private void SetupUserInterface()
        {
            var centerButtonX = View.Bounds.GetMidX() - 37.5f;
            var topLeftX = View.Bounds.X;
            var topRightX = View.Bounds.Right - 65;
            var bottomButtonY = View.Bounds.Bottom - 85;
            var topButtonY = View.Bounds.Top;
            var buttonWidth = 75;
            var buttonHeight = 75;

            liveCameraStream = new UIView()
            {
                Frame = new CGRect(0f, 0f, View.Bounds.Width, View.Bounds.Height)
            };

            var roi = new UIImage();
            CGRect rect = new CGRect(0f, 0f, View.Bounds.Width, View.Bounds.Height);
            UIGraphics.BeginImageContextWithOptions(View.Bounds.Size, false, 0);
            var context = UIGraphics.GetCurrentContext();
            context.SetFillColor(red: 255, green: 0, blue: 0, alpha: 0f);
            context.FillRect(rect);
            context.SetBlendMode(CGBlendMode.SourceOut);
            float centerX = (float)View.Bounds.Width / 2;
            float centerY = (float)View.Bounds.Height / 2;
            double radius = Math.Min(View.Bounds.Width, View.Bounds.Height) / 2 - Math.Min(View.Bounds.Width, View.Bounds.Height) / 5;
            context.SetFillColor(UIColor.Clear.CGColor);
            context.FillEllipseInRect(new CGRect(centerX - radius, centerY - radius, radius * 2, radius * 2));
            roi.Draw(rect, CGBlendMode.Normal, 1);

            var RoiView = new UIImageView(roi);

            takePhotoButton = new UIButton()
            {
                Frame = new CGRect(centerButtonX, bottomButtonY, buttonWidth, buttonHeight)
            };
            takePhotoButton.SetBackgroundImage(UIImage.FromFile("button_default.9.png"), UIControlState.Normal);

            toggleFlashButton = new UIButton()
            {
                Frame = new CGRect(topRightX, topButtonY, 65, 65)
            };
            toggleFlashButton.SetBackgroundImage(UIImage.FromFile("flash_off.9.png"), UIControlState.Normal);

            backButton = new UIButton()
            {
                Frame = new CGRect(topLeftX, topButtonY, 65, 65)
            };
            backButton.SetBackgroundImage(UIImage.FromFile("back.png"), UIControlState.Normal);

            galleryButton = new UIButton()
            {
                Frame = new CGRect(topLeftX, bottomButtonY, 65, 65)
            };
            galleryButton.SetBackgroundImage(UIImage.FromFile("gallery.png"), UIControlState.Normal);

            View.Add(liveCameraStream);
            //View.Add(RoiView);
            View.Add(takePhotoButton);
            View.Add(toggleFlashButton);
            View.Add(backButton);
            View.Add(galleryButton);
        }

        private void SetupEventHandlers()
        {
            takePhotoButton.TouchUpInside += (object sender, EventArgs e) => {
                CapturePhoto();
            };

            toggleFlashButton.TouchUpInside += (object sender, EventArgs e) => {
                ToggleFlash();
            };

            galleryButton.TouchUpInside += (object sender, EventArgs e) => {
                ((SelfControl.CameraPage)Element).NavigateToGallery();
            };

            backButton.TouchUpInside += (object sender, EventArgs e) => {
                ((SelfControl.CameraPage)Element).NavigateBack();
            };
        }
    }
}