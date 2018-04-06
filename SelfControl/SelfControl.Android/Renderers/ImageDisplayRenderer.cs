using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading.Forms;
using SelfControl.Droid.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.ImageDisplay), typeof(SelfControl.Droid.Renderers.ImageDisplayRenderer))]
namespace SelfControl.Droid.Renderers
{
    class ImageDisplayRenderer : ImageRenderer, Android.Views.View.IOnLongClickListener, Android.Views.View.IOnClickListener
    {
        Activity mActivity;
        Bitmap rotatedBitmap;
        public ImageDisplayRenderer(Context context) : base(context)
        {
        }

        public void OnClick(Android.Views.View v)
        {
            var image = (SelfControl.Helpers.ImageDisplay)Element;
            if(image.OnClick != null)
                image.OnClick.Execute(true);
        }

        public bool OnLongClick(Android.Views.View v)
        {
            Console.WriteLine("LongClick");
            var image = (SelfControl.Helpers.ImageDisplay)Element;
            if (image.OnTouch != null)
                image.OnTouch.Execute(true);
            return true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            Control.SetOnLongClickListener(this);
            Control.SetOnClickListener(this);
            Control.Invalidate();
        }
        
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            mActivity = Context as Activity;
            var image = (SelfControl.Helpers.ImageDisplay)Element;
            var file = image.ImageFile;
            var bytes = image.ImageByte;
            var IsSelected = image.IsSelected;

            if ((Element.Width > 0 && Element.Height > 0))
            {
                if (file != null)
                {
                    rotatedBitmap = null;
                    rotatedBitmap = GlobalHelpers.LoadBitmap(mActivity, file);
                }
                else if(bytes != null)
                {
                    rotatedBitmap = null;
                    BitmapFactory.Options options = new BitmapFactory.Options();
                    options.InMutable = true;
                    rotatedBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);
                    Control.SetImageBitmap(rotatedBitmap);
                }
                if(rotatedBitmap != null && IsSelected)
                {
                    Canvas canvas = new Canvas(rotatedBitmap);
                    Paint paint = new Paint();
                    paint.SetARGB(165, 0, 0, 0);
                    paint.SetStyle(Paint.Style.Fill);
                    paint.StrokeWidth = 2;
                    canvas.DrawRect(0, 0, rotatedBitmap.Width, rotatedBitmap.Height, paint);
                }
            }
        }
    }
}