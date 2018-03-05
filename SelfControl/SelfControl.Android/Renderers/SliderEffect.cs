using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SelfControl.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.CustomSlider), typeof(SelfControl.Droid.Renderers.SliderEffect))]
namespace SelfControl.Droid.Renderers
{
    class SliderEffect : SliderRenderer
    {
        public SliderEffect(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || e.NewElement == null)
                return;

            var view = (SelfControl.Helpers.CustomSlider)Element;

            Control.Thumb.SetColorFilter(Xamarin.Forms.Color.Blue.ToAndroid(), PorterDuff.Mode.SrcIn);
            Control.ProgressTintList = Android.Content.Res.ColorStateList.ValueOf(Xamarin.Forms.Color.Blue.ToAndroid());
            Control.ProgressTintMode = PorterDuff.Mode.SrcIn;
            //this is for Maximum Slider line Color  
            Control.ProgressBackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Xamarin.Forms.Color.Gray.ToAndroid());
            Control.ProgressBackgroundTintMode = PorterDuff.Mode.SrcIn;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }
}