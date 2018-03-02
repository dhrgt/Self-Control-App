using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace SelfControl.Helpers
{
    public class ImageDisplay : Image
    {
        public static readonly BindableProperty ImageFileProperty =
        BindableProperty.Create("ImageFile", typeof(string), typeof(ImageDisplay), default(string), propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (Device.RuntimePlatform != Device.Android)
            {
                var image = (ImageDisplay)bindable;

                var baseImage = (Image)bindable;
                baseImage.Source = image.ImageFile;
            }
        });

        public static readonly BindableProperty ImageIdProperty =
        BindableProperty.Create("ID", typeof(int), typeof(ImageDisplay), -1, propertyChanged: (bindable, oldValue, newValue) =>
        {
        });

        public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create("IsSelected", typeof(bool), typeof(ImageDisplay), false, propertyChanged: (bindable, oldValue, newValue) =>
        {
        });

        public static readonly BindableProperty ImageByteProperty =
        BindableProperty.Create("ImageByte", typeof(byte[]), typeof(ImageDisplay), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (Device.RuntimePlatform != Device.Android)
            {
                var image = (ImageDisplay)bindable;

                var baseImage = (Image)bindable;
                baseImage.Source = ImageSource.FromStream(() => new MemoryStream(image.ImageByte));
            }
        });

        public string ImageFile
        {
            get { return GetValue(ImageFileProperty) as string; }
            set { SetValue(ImageFileProperty, value); }
        }

        public byte[] ImageByte
        {
            get { return (byte[])GetValue(ImageByteProperty); }
            set { SetValue(ImageByteProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public int ID
        {
            get { return (int)GetValue(ImageIdProperty); }
            set { SetValue(ImageIdProperty, value); }
        }
    }
}
