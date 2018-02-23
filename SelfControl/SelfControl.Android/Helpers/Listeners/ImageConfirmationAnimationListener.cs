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
    class ImageConfirmationAnimationListener : Java.Lang.Object, Animation.IAnimationListener
    {
        private CameraFragment owner;

        public ImageConfirmationAnimationListener(CameraFragment fragment) {
            if (fragment == null)
                throw new System.ArgumentNullException("fragment");
            owner = fragment;
        }

        public void OnAnimationEnd(Animation animation)
        {
            ImageView imageDisplay = owner.imageDisplay;
            animation = AnimationUtils.LoadAnimation(owner.Activity, Resource.Animation.scale_down);
            imageDisplay.Animation = animation;
            animation.SetAnimationListener(new ImageEndAnimation(owner));
            imageDisplay.Animate().SetDuration(3000).Start();
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