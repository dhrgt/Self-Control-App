using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SelfControl.iOS.Helpers.ResizeImageIOS))]
namespace SelfControl.iOS.Helpers
{
    class ResizeImageIOS : SelfControl.Interfaces.IResizeImage
    {
        public byte[] Resize(byte[] image, int width, int height)
        {
            UIImage originalImage = GlobalHelpers.ImageFromByteArray(image);
            UIImageOrientation orientation = originalImage.Orientation;

            //create a 24bit RGB image
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                                                 (int)width, (int)height, 8,
                                                 4 * (int)width, CGColorSpace.CreateDeviceRGB(),
                                                 CGImageAlphaInfo.PremultipliedFirst))
            {

                RectangleF imageRect = new RectangleF(0, 0, width, height);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

                // save the image as a jpeg
                return resizedImage.AsJPEG().ToArray();
            }
        }
    }
}
