using SelfControl.iOS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Contexts;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.ImageDisplay), typeof(SelfControl.iOS.Renderers.ImageDisplayRenderer))]
namespace SelfControl.iOS.Renderers
{
    class ImageDisplayRenderer : ImageRenderer
    {
        UIImage uiImage;

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            uiImage = new UIImage();
            Control.Image = uiImage;
            Control.UserInteractionEnabled = true;

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var image = (SelfControl.Helpers.ImageDisplay)Element;
            var file = image.ImageFile;
            var bytes = image.ImageByte;
            var IsSelected = image.IsSelected;

            if ((Element.Width > 0 && Element.Height > 0))
            {
                if(file != null)
                {
                    uiImage = UIImage.FromFile(file);
                    Control.Image = uiImage;
                }
                else if (bytes != null)
                {
                    uiImage = GlobalHelpers.ImageFromByteArray(bytes);
                    Control.Image = uiImage;
                }
                if (uiImage != null && IsSelected)
                {

                }
            }
        }
    }
}
