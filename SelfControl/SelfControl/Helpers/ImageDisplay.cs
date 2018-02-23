using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SelfControl.Helpers
{
    public class ImageDisplay : Image
    {
       public static readonly BindableProperty ImageSourceProperty =
       BindableProperty.Create("ImageSource", typeof(string), typeof(ImageDisplay), default(string), propertyChanged: (bindable, oldValue, newValue) =>
       {
           if (Device.RuntimePlatform != Device.Android)
           {
               var image = (ImageDisplay)bindable;

               var baseImage = (Image)bindable;
               baseImage.Source = image.ImageSource;
           }
       });

       public string ImageSource
       {
           get { return GetValue(ImageSourceProperty) as string; }
           set { SetValue(ImageSourceProperty, value); }
       }
    }
}
