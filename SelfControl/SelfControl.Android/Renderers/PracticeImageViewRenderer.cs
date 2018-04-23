using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Animation.ValueAnimator;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.PracticeImageView), typeof(SelfControl.Droid.Renderers.PracticeImageViewRenderer))]
namespace SelfControl.Droid.Renderers
{
    class PracticeImageViewRenderer : ImageRenderer, IAnimatorUpdateListener
    {
        Drawable mImage;
        ColorMatrix matrix;
        float saturation;
        float zoom;
        bool isHeating;
        bool isCooling;
        ValueAnimator animationSat;
        ValueAnimator animationZoomIn;

        public PracticeImageViewRenderer(Context context) : base(context)
        {
            Console.WriteLine("new PracticeImageRenderer created" + System.Environment.NewLine);
            matrix = new ColorMatrix();
            saturation = 1f;
            zoom = SelfControl.Helpers.GlobalVariables.StartingZoomValue;
            isHeating = false;
            isCooling = false;
            animationSat = null;
            animationZoomIn = null;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            Console.WriteLine("OnElementChanged" + System.Environment.NewLine);
            base.OnElementChanged(e);
            Control.Invalidate();
        }

        public void IncreaseSaturation()
        {
            if(mImage != null)
            {
                animationSat = ValueAnimator.OfFloat(saturation, 2f);
                animationSat.SetDuration(5000);
                animationSat.AddUpdateListener(this);
                animationZoomIn = ValueAnimator.OfFloat(zoom, 2f);
                animationZoomIn.SetDuration(5000);
                animationZoomIn.AddUpdateListener(this);
                if (SelfControl.Helpers.GlobalVariables.SaturationAnimation) { animationSat.Start(); }
                if (SelfControl.Helpers.GlobalVariables.ZoomAnimation) { animationZoomIn.Start(); }
                isHeating = true;
                isCooling = false;
            }
        }

        public void DecreaseSaturation()
        {
            if (mImage != null)
            {
                animationSat = ValueAnimator.OfFloat(saturation, 0f);
                animationSat.SetDuration(5000);
                animationSat.AddUpdateListener(this);
                animationZoomIn = ValueAnimator.OfFloat(zoom, 1f);
                animationZoomIn.SetDuration(5000);
                animationZoomIn.AddUpdateListener(this);
                if (SelfControl.Helpers.GlobalVariables.SaturationAnimation) { animationSat.Start(); }
                if (SelfControl.Helpers.GlobalVariables.ZoomAnimation) { animationZoomIn.Start(); }
                isCooling = true;
                isHeating = false;
            }
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            if(animation == animationSat)
            {
                matrix.SetSaturation((float)animation.AnimatedValue);
                saturation = (float)animation.AnimatedValue;
                ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);
                mImage.SetColorFilter(filter);
            }
            else if(animation == animationZoomIn)
            {
                Control.ScaleX = (float)animation.AnimatedValue;
                Control.ScaleY = (float)animation.AnimatedValue;
                zoom = (float)animation.AnimatedValue;
            }
            Control.SetImageDrawable(mImage);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("OnElementPropertyChanged : " + e.PropertyName + System.Environment.NewLine);
            base.OnElementPropertyChanged(sender, e);
            Activity mActivity = Context as Activity;
            var image = (SelfControl.Helpers.PracticeImageView)sender;
            var bytes = image.ImageByte;
            var increaseSaturation = image.IncreaseSaturation;

            if ((Element.Width > 0 && Element.Height > 0))
            {
                if (bytes != null)
                {
                    mImage = new BitmapDrawable(mActivity.Resources, BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length));
                    Control.ScaleX = (float)image.Scale;
                    Control.ScaleY = (float)image.Scale;
                    Control.SetImageDrawable(mImage);
                }
            }

            if (increaseSaturation > 0 && !isHeating)
            {
                Console.WriteLine("IncreseSaturation" + System.Environment.NewLine);
                if (isCooling && (animationSat != null || animationZoomIn != null))
                {
                    animationSat = null;
                    animationZoomIn = null;
                    ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);
                    mImage.SetColorFilter(filter);
                    Control.ScaleX = (float)zoom;
                    Control.ScaleY = (float)zoom;
                    Control.SetImageDrawable(mImage);
                }
                IncreaseSaturation();
            }
            else if (increaseSaturation < 0 && !isCooling)
            {
                Console.WriteLine("IncreseSaturation" + System.Environment.NewLine);
                if (isHeating && (animationSat != null || animationZoomIn != null))
                {
                    animationSat = null;
                    animationZoomIn = null;
                    ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);
                    mImage.SetColorFilter(filter);
                    Control.ScaleX = (float)zoom;
                    Control.ScaleY = (float)zoom;
                    Control.SetImageDrawable(mImage);
                }
                DecreaseSaturation();
            }
            else if (animationSat != null || animationZoomIn != null)
            {
                if (increaseSaturation == 0)
                {
                    if (animationSat.IsStarted) animationSat.Pause();
                    if (animationZoomIn.IsStarted) animationZoomIn.Pause();
                    ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);
                    mImage.SetColorFilter(filter);
                    Control.ScaleX = (float)zoom;
                    Control.ScaleY = (float)zoom;
                    Control.SetImageDrawable(mImage);
                }
                else if (increaseSaturation > 0 || increaseSaturation < 0)
                {
                    if(SelfControl.Helpers.GlobalVariables.SaturationAnimation && animationSat.IsPaused) animationSat.Resume();
                    if(SelfControl.Helpers.GlobalVariables.ZoomAnimation && animationZoomIn.IsPaused) animationZoomIn.Resume();
                }
            }
        }
    }
}