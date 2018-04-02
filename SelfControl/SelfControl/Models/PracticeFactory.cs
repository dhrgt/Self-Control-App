using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using FFImageLoading.Forms;
using SelfControl.Helpers;
using System.Reflection;
using System.Threading;
using System.Windows.Input;

namespace SelfControl.Models
{
    public class PracticeFactory : Grid
    {
        public byte[] ByteSource { get; set; }
        public int Saturation { get; set; }
        public PracticeImageView image { get; }
        //static float value = 1f;

        public ICommand temp;

        public PracticeFactory()
        {
            image = new PracticeImageView
            {
                Aspect = Aspect.AspectFit,
            };

            image.SetBinding(PracticeImageView.ImageByteProperty, "ByteSource");
            image.SetBinding(PracticeImageView.IncreaseSaturationProperty, "Saturation");

            Children.Add(image);
        }
    }
}
