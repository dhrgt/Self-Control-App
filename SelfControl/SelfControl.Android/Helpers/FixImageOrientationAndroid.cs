using System;
using System.Collections.Generic;
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
using Xamarin.Forms;

[assembly: Dependency(typeof(SelfControl.Droid.Helpers.FixImageOrientationAndroid))]
namespace SelfControl.Droid.Helpers
{
    class FixImageOrientationAndroid : Java.Lang.Object, SelfControl.Interfaces.IFixImageOrientation
    {
        public byte[] fixOrientation(byte[] bytes)
        {
            Bitmap rotatedBitmap = null;
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InMutable = true;
            Bitmap img = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);
            Matrix matrix = new Matrix();
            matrix.PostRotate(90, img.Width / 2, img.Height / 2);
            rotatedBitmap = Bitmap.CreateBitmap(img, 0, 0, img.Width, img.Height, matrix, true);
            img.Recycle();
            using (MemoryStream ms = new MemoryStream())
            {
                rotatedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, ms);
                rotatedBitmap.Recycle();
                return ms.ToArray();
            }
        }
    }
}