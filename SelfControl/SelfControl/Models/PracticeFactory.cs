using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using FFImageLoading.Forms;
using SelfControl.Helpers;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using SelfControl.Helpers.Pages;
using System.IO;

namespace SelfControl.Models
{
    public class PracticeFactory : Grid
    {
        public byte[] ByteSource { get; set; }
        public static bool isPlaying;
        public ICommand hot, cool;

        public PracticeFactory()
        {
            isPlaying = false;
            var image = new PracticeImageView
            {
                Aspect = Aspect.AspectFit,
                Scale = GlobalVariables.StartingZoomValue
            };
            
            image.SetBinding(PracticeImageView.ImageByteProperty, "ByteSource");
            cool = new Command((p) =>
            {
                bool Continue = (bool)p;
                if (Continue && !isPlaying)
                {
                    isPlaying = true;
                    image.IncreaseSaturation = -1;
                }
                else if (!Continue)
                {
                    isPlaying = false;
                    image.IncreaseSaturation = 0;
                }
            });
            hot = new Command((p) =>
            {
                bool Continue = (bool)p;
                if (Continue && !isPlaying)
                {
                    isPlaying = true;
                    image.IncreaseSaturation = 1;
                }
                else if (!Continue)
                {
                    isPlaying = false;
                    image.IncreaseSaturation = 0;
                }
            });
            Children.Add(image);
        }
    }
}
