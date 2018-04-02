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

        public PracticeImageViewRenderer(Context context) : base(context)
        {
            matrix = new ColorMatrix();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            Control.Invalidate();
        }

        public void IncreaseSaturation()
        {
            if(mImage != null)
            {
                ValueAnimator animation = ValueAnimator.OfFloat(0f, 1f);
                animation.SetDuration(5000);
                animation.AddUpdateListener(this);
                animation.Start();
            }
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            matrix.SetSaturation(animation.AnimatedFraction);
            ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);
            mImage.SetColorFilter(filter);
            Control.SetImageDrawable(mImage);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Activity mActivity = Context as Activity;
            var image = (SelfControl.Helpers.PracticeImageView)Element;
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

            if (increaseSaturation > 0) IncreaseSaturation();
        }
    }
}