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

        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            Control.UserInteractionEnabled = true;
            UILongPressGestureRecognizer longp = new UILongPressGestureRecognizer(OnLongClick);
            Control.AddGestureRecognizer(longp);
            UITapGestureRecognizer clickp = new UITapGestureRecognizer(OnClick);
            Control.AddGestureRecognizer(clickp);
            //Control.BackgroundColor = UIColor.Black;
        }

        public void OnClick()
        {
            var image = (SelfControl.Helpers.ImageDisplay)Element;
            if (image.OnClick != null)
                image.OnClick.Execute(true);
        }

        public void OnLongClick()
        {
            Console.WriteLine("LongClick");
            var image = (SelfControl.Helpers.ImageDisplay)Element;
            if (image.OnTouch != null)
                image.OnTouch.Execute(true);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var image = (SelfControl.Helpers.ImageDisplay)Element;
           // var file = image.ImageFile;
           // var bytes = image.ImageByte;
            var IsSelected = image.IsSelected;

            //if ((Element.Width > 0 && Element.Height > 0))
            //{
               // if(file != null)
                //{
                    //uiImage = UIImage.FromFile(file);
                  //  uiImage = uiImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                   /// Control.Image = uiImage;
                   // Control.TintColor = new UIColor(0, 0, 0, 1);
                //}
                //else if (bytes != null)
                //{
                  //  uiImage = GlobalHelpers.ImageFromByteArray(bytes);
                    ////uiImage = uiImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    //uiImage = uiImage.ImageWithRenderingMode(UIKit.UIImageRenderingMode.Automatic);
                    //Control.Image = uiImage;
                    //Control.TintColor = null;
                //}
            if (Control.Image != null && IsSelected)
                {
                    Control.TintColor = new UIColor(0,0,0,0.645f);
                }
            //}
        }
    }
}
