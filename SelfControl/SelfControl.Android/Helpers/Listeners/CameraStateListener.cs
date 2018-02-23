using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SelfControl.Droid.Helpers.Listeners
{
    class CameraStateListener : CameraDevice.StateCallback
    {
        private readonly CameraFragment owner;

        public CameraStateListener(CameraFragment owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnOpened(CameraDevice cameraDevice)
        {
            // This method is called when the camera is opened.  We start camera preview here.
            owner.mCameraOpenCloseLock.Release();
            owner.mCameraDevice = cameraDevice;
            owner.CreateCameraPreviewSession();
        }

        public override void OnDisconnected(CameraDevice cameraDevice)
        {
            owner.mCameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.mCameraDevice = null;
        }

        public override void OnError(CameraDevice cameraDevice, CameraError error)
        {
            owner.mCameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.mCameraDevice = null;
            if (owner == null)
                return;
            Activity activity = owner.Activity;
            if (activity != null)
            {
                activity.Finish();
            }
        }
    }
}