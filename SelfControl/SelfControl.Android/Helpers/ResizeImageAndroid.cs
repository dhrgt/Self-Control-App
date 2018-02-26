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

[assembly: Dependency(typeof(SelfControl.Droid.Helpers.ResizeImageAndroid))]
namespace SelfControl.Droid.Helpers
{
    public class ResizeImageAndroid : Java.Lang.Object, SelfControl.Interfaces.IResizeImage
    {
        public byte[] Resize(byte[] imageData, int width, int height)
        {
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 90, ms);
                return ms.ToArray();
            }
        }
    }
}