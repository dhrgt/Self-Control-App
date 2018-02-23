using Android.Hardware.Camera2;
using Android.Util;
using Android.Views.Animations;
using Android.Widget;
using static Android.Resource;

namespace SelfControl.Droid.Helpers.Listeners
{
    public class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private static readonly string TAG = "CameraCaptureStillPictureSessionCallback";

        private readonly CameraFragment owner;

        public CameraCaptureStillPictureSessionCallback(CameraFragment owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            Log.Debug(TAG, owner.mFile.ToString());
            owner.UnlockFocus();
            //owner.mCPR.NavigateFilteredImagePage(owner.mFile.AbsolutePath.ToString(), owner.CircleRadius);
        }
    }
}