using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using SelfControl.iOS.Helpers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.PracticeImageView), typeof(SelfControl.iOS.Renderers.PracticeImageViewRenderer))]
namespace SelfControl.iOS.Renderers
{
    class PracticeImageViewRenderer : ImageRenderer
    {
        float saturation;
        float zoom;
        bool isHeating;
        bool isCooling;
        private UIImage uiImage;

        public PracticeImageViewRenderer()
        {
            Console.WriteLine("new PracticeImageRenderer created" + System.Environment.NewLine);
            saturation = 1f;
            zoom = SelfControl.Helpers.GlobalVariables.StartingZoomValue;
            isHeating = false;
            isCooling = false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            Console.WriteLine("OnElementChanged" + System.Environment.NewLine);
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("OnElementPropertyChanged : " + e.PropertyName + System.Environment.NewLine);
            base.OnElementPropertyChanged(sender, e);
            var image = (SelfControl.Helpers.PracticeImageView)sender;
            var bytes = image.ImageByte;
            var increaseSaturation = image.IncreaseSaturation;

            if ((Element.Width > 0 && Element.Height > 0))
            {
                if (bytes != null)
                {
                    uiImage = GlobalHelpers.ImageFromByteArray(bytes);
                    uiImage = uiImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    Control.Image = uiImage;
                    Control.TintColor = new UIColor(0, 0, 0, 1);
                }
            }

            if (increaseSaturation > 0 && !isHeating)
            {
            }
            else if (increaseSaturation < 0 && !isCooling)
            {
            }
        }
    }
}