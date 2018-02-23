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
using SelfControl.Droid.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.ImageDisplay), typeof(SelfControl.Droid.Renderers.ImageDisplayRenderer))]
namespace SelfControl.Droid.Renderers
{
    class ImageDisplayRenderer : ImageRenderer
    {
        Activity mActivity;
        public ImageDisplayRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            base.OnElementChanged(e);
            Control.Invalidate();
        }

        private bool _isDecoded;
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            mActivity = Context as Activity;
            var image = (SelfControl.Helpers.ImageDisplay)Element;
            var file = image.ImageSource;

            if ((Element.Width > 0 && Element.Height > 0 && !_isDecoded && image.ImageSource != null))
            {
                Bitmap rotatedBitmap = GlobalHelpers.LoadBitmap(mActivity, file);
                Control.SetImageBitmap(rotatedBitmap);
                //Control.Invalidate();

                _isDecoded = true;
            }
        }
    }
}