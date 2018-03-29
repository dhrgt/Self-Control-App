using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using SelfControl.Helpers;
using System.Reflection;
using System.Threading;
using System.Windows.Input;

namespace SelfControl.Models
{
    class PracticeFactory : AbsoluteLayout
    {
        Grid content;
        public ImageSource ByteSource { get; set; }
        public CachedImage image { get; }
        static float value = 1f;

        public ICommand temp;

        public PracticeFactory()
        {
            content = new Grid();
            image = new CachedImage
            {
                Aspect = Aspect.AspectFit,
                Transformations = new List<FFImageLoading.Work.ITransformation>(),
                BitmapOptimizations = true,
                TransformPlaceholders = false
            };

            image.SetBinding(CachedImage.SourceProperty, "ByteSource");
            content.Children.Add(image);

            temp = new Command((p) => 
            {
                if ((bool)p)
                {
                    value += 0.5f;

                    image.Transformations.Add(new CornersTransformation());
                }
                image.ReloadImage();
            });

            Children.Add(content, new Rectangle(0, 0, App.ScreenWidth, App.ScreenHeight), AbsoluteLayoutFlags.PositionProportional);
        }

        public Grid getContent() { return content; }
    }
}
