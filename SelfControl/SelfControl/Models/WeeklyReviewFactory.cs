using SelfControl.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace SelfControl.Models
{
    class WeeklyReviewFactory : AbsoluteLayout
    {
        public ImageSource ByteSource { get; set; }
        public string NameSource { get; set; }
        public Dictionary<int, CustomRadioGroup> radioGroups;

        public WeeklyReviewFactory()
        {
            Label nameLabel = new Label
            {
                TextColor = Color.Black,
                FontSize = 25,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            nameLabel.SetBinding(Label.TextProperty, "NameSource");
            AbsoluteLayout.SetLayoutFlags(nameLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(nameLabel, new Rectangle(0, 0, 1, 0.1));

            Image image = new Image
            {
                WidthRequest = 500,
                HeightRequest = 500,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            image.SetBinding(Image.SourceProperty, "ByteSource");
            AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(image, new Rectangle(1, 1, 1, 0.89));

            AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(this, new Rectangle(1, 1, 1, 1));

            Children.Add(nameLabel);
            Children.Add(image);
        }
    }
}
