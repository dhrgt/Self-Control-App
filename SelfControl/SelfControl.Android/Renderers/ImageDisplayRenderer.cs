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
        Bitmap rotatedBitmap;
        public ImageDisplayRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            base.OnElementChanged(e);
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
                    Control.SetImageBitmap(rotatedBitmap);
                }
                else if(bytes != null)
                {
                    rotatedBitmap = null;
                    BitmapFactory.Options options = new BitmapFactory.Options();
                    options.InMutable = true;
                    Bitmap img = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);
                    Matrix matrix = new Matrix();
                    matrix.PostRotate(90, img.Width / 2, img.Height / 2);
                    rotatedBitmap = Bitmap.CreateBitmap(img, 0, 0, img.Width, img.Height, matrix, true);
                    img.Recycle();
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
                    Control.SetImageBitmap(rotatedBitmap);
                }
                //Control.Invalidate();
            }
        }
    }
}