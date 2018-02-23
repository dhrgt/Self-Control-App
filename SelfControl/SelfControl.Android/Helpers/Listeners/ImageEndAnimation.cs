using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace SelfControl.Droid.Helpers.Listeners
{
    class ImageEndAnimation : Java.Lang.Object, Animation.IAnimationListener
    {
        private CameraFragment owner;

        public ImageEndAnimation(CameraFragment fragment)
        {
            if (fragment == null)
                throw new System.ArgumentNullException("fragment");
            owner = fragment;
        }

        public void OnAnimationEnd(Animation animation)
        {
            ImageView imageDisplay = owner.imageDisplay;
            imageDisplay.SetImageResource(Android.Resource.Color.Transparent);
            //owner.mCPR.NavigateFilteredImagePage(owner.mFile.AbsolutePath.ToString(), owner.CircleRadius);
        }

        public void OnAnimationRepeat(Animation animation)
        {
            return;
        }

        public void OnAnimationStart(Animation animation)
        {
            animation.Start();
        }
    }
}