using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace SelfControl.Droid.Helpers
{
    class GlobalHelpers
    {
        public static Bitmap LoadBitmap(Activity activity, String FileName) {
            int rotation = 0;
            try
            {
                ExifInterface exif = new ExifInterface(FileName);
                rotation = GlobalHelpers.getRotateDegreeFromOrientation(exif.GetAttributeInt(
                        ExifInterface.TagOrientation,
                        (int)Android.Content.Res.Orientation.Undefined));
            }
            catch (IOException ex)
            {
                ex.PrintStackTrace();
            }

            Bitmap image;
            int width, height;
            DisplayMetrics displayMetrics = new DisplayMetrics();
            activity.WindowManager.DefaultDisplay.GetRealMetrics(displayMetrics);
            Matrix matrix = new Matrix();
            matrix.PostRotate(90);
            if (rotation == 90 || rotation == 270)
            {
                width = displayMetrics.HeightPixels;
                height = displayMetrics.WidthPixels;
                image = GlobalHelpers.decodeSampledBitmapFromFile(FileName, width, height);
            }
            else
            {
                width = displayMetrics.WidthPixels;
                height = displayMetrics.HeightPixels;
                image = GlobalHelpers.decodeSampledBitmapFromFile(FileName, width, height);
            }
            return Bitmap.CreateBitmap(image, 0, 0, image.Width, image.Height, matrix, true);
        }

        /**
         * These functions are used to load up images by reducing their size before putting them
         * on memory. This prevents OutOfMemoryErrors
         */
        public static int calculateInSampleSize(
                BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {

                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) >= reqHeight
                        && (halfWidth / inSampleSize) >= reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }

        public static Bitmap decodeSampledBitmapFromFile(String file, int reqWidth, int reqHeight)
        {

            // First decode with inJustDecodeBounds=true to check dimensions
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeFile(file, options);

            // Calculate inSampleSize
            options.InSampleSize = calculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeFile(file, options);
        }

        public static int getRotateDegreeFromOrientation(int orientation)
        {
            int degree = 0;
            switch (orientation)
            {
                case (int)Android.Media.Orientation.Rotate90:
                    degree = 90;
                    break;
                case (int)Android.Media.Orientation.Rotate180:
                    degree = 180;
                    break;
                case (int)Android.Media.Orientation.Rotate270:
                    degree = 270;
                    break;
                default:
                    break;
            }
            return degree;
        }



    }
}