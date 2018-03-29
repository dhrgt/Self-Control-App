using FFImageLoading.Forms;
using SelfControl.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SelfControl.Models
{
    public static class CardFactory
    {
        public static Func<View> Creator { get; } = () =>
        {
            var content = new Grid();

            var image = new CachedImage
            {
                
            };

            image.SetBinding(CachedImage.SourceProperty, "ByteSource");

            content.Children.Add(image);

            return content;
        };
    }
}
