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
        public CustomPracticeButtons cool { get; }
        public static bool isPlaying;
        PracticeViewerModel parentModel { get; set; }
        public PracticeImageView image { get; set; }
        public ICommand heat { get; set; }

        public PracticeFactory(PracticeViewerModel practiceViewerModel = null)
        {
            parentModel = practiceViewerModel;
            isPlaying = false;
            image = new PracticeImageView
            {
                Aspect = Aspect.AspectFit,
            };
            
            image.SetBinding(PracticeImageView.ImageByteProperty, "ByteSource");
            cool = new CustomPracticeButtons
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 60,
                HeightRequest = 60
            };
            Assembly assembly = typeof(PracticeFactory).GetTypeInfo().Assembly;
            var iceStream = assembly.GetManifestResourceStream("SelfControl.Resources.ice_button.png");
            var iceByte = new byte[iceStream.Length];
            iceStream.Read(iceByte, 0, System.Convert.ToInt32(iceStream.Length));
            cool.IconBytes = iceByte;
            cool.OnTouch = new Command((p) =>
            {
                bool Continue = (bool)p;
                if (Continue && !isPlaying)
                {
                    isPlaying = true;
                    if (parentModel != null) parentModel.PanEnable = false;
                    image.IncreaseSaturation = -1;
                }
                else if (!Continue)
                {
                    isPlaying = false;
                    if (parentModel != null) parentModel.PanEnable = true;
                    image.IncreaseSaturation = 0;
                }
            });
            
            heat = new Command((p) =>
            {
                bool Continue = (bool)p;
                if (Continue && !isPlaying)
                {
                    isPlaying = true;
                    if (parentModel != null)
                    {
                        parentModel.PanEnable = false;
                    }
                    image.IncreaseSaturation = 1;
                }
                else if (!Continue)
                {
                    isPlaying = false;
                    if (parentModel != null) parentModel.PanEnable = true;
                    image.IncreaseSaturation = 0;
                }
            });
            AbsoluteLayout absoluteLayout = new AbsoluteLayout
            {
                Margin = 10,
                VerticalOptions = LayoutOptions.EndAndExpand,
                HeightRequest = 60,
                WidthRequest = App.ScreenWidth
            };
            
            StackLayout grid = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
            grid.Children.Add(cool);
            AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(grid, new Rectangle(1, 1, 1, 1));
            absoluteLayout.Children.Add(grid);
            Children.Add(image);
            Children.Add(absoluteLayout);
        }
    }
}
