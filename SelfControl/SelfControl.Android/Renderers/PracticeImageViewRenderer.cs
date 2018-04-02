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
        bool isHeating;
        bool isCooling;
        ValueAnimator animation;
        public PracticeImageViewRenderer(Context context) : base(context)
        {
            Console.WriteLine("new PracticeImageRenderer created" + System.Environment.NewLine);
            matrix = new ColorMatrix();
            saturation = 1f;
            isHeating = false;
            isCooling = false;
            animation = null;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            Console.WriteLine("OnElementChanged" + System.Environment.NewLine);
            base.OnElementChanged(e);
            var image = (SelfControl.Helpers.PracticeImageView)Element;
            image.SaturationChanged += this.OnElementPropertyChanged;
            Control.Invalidate();
        }

        public void IncreaseSaturation()
        {
            if(mImage != null)
            {
                animation = ValueAnimator.OfFloat(saturation, 2f);
                animation.SetDuration(10000);
                animation.AddUpdateListener(this);
                animation.Start();
                isHeating = true;
                isCooling = false;
            }
        }

        public void DecreaseSaturation()
        {
            if (mImage != null)
            {
                animation = ValueAnimator.OfFloat(saturation, 0f);
                animation.SetDuration(10000);
                animation.AddUpdateListener(this);
                animation.Start();
                isCooling = true;
                isHeating = false;
            }
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            matrix.SetSaturation((float)animation.AnimatedValue);
            saturation = (float)animation.AnimatedValue;
            ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);
            mImage.SetColorFilter(filter);
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
                    
                    Control.SetImageDrawable(mImage);
                }
            }
            if (increaseSaturation > 0 && !isHeating)
            {
                Console.WriteLine("IncreseSaturation" + System.Environment.NewLine);
                if (isCooling && animation != null)
                {
                    animation = null;
                    ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);
                    mImage.SetColorFilter(filter);
                    Control.SetImageDrawable(mImage);
                }
                IncreaseSaturation();
            }
            else if (increaseSaturation < 0 && !isCooling)
            {
                Console.WriteLine("IncreseSaturation" + System.Environment.NewLine);
                if (isHeating && animation != null)
                {
                    animation = null;
                    ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);
                    mImage.SetColorFilter(filter);
                    Control.SetImageDrawable(mImage);
                }
                DecreaseSaturation();
            }
            else if (animation != null)
            {
                if (increaseSaturation == 0)
                {
                    if(animation.IsStarted) animation.Pause();
                    ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);
                    mImage.SetColorFilter(filter);
                    Control.SetImageDrawable(mImage);
                }
                else if ((increaseSaturation > 0 || increaseSaturation < 0) && animation.IsPaused) animation.Resume();
            }
        }
    }
}