﻿using PanCardView;
using PanCardView.Controls;
using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfControl.Helpers.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class PracticeViewer
    {
        public PanCardView.CardsView carouselView;
        public Dictionary<int, byte[]> list;
        public PracticeViewer(List<FoodItem> FoodList)
        {
            list = new Dictionary<int, byte[]>();
            foreach (var i in FoodList)
            {
                list.Add(i.ID, GlobalVariables.DeserializeStringToByteArray(i.IMGBYTES));
            }
            Assembly assembly = typeof(PracticeViewer).GetTypeInfo().Assembly;
            var blankStream = assembly.GetManifestResourceStream("SelfControl.Resources.blank.jpg");
            var blankByte = new byte[blankStream.Length];
            blankStream.Read(blankByte, 0, System.Convert.ToInt32(blankStream.Length));
            list.Add(-1, blankByte);

            carouselView = new PanCardView.CardsView
            {
                ItemTemplate = new DataTemplate(() => new PracticeFactory()),
                BackgroundColor = Color.Black
            };
            
            carouselView.SetBinding(CardsView.PrevContextProperty, nameof(PracticeViewerModel.PrevContext));
            carouselView.SetBinding(CardsView.CurrentContextProperty, nameof(PracticeViewerModel.CurrentContext));
            carouselView.SetBinding(CardsView.NextContextProperty, nameof(PracticeViewerModel.NextContext));
            carouselView.SetBinding(CardsView.PanStartedCommandProperty, nameof(PracticeViewerModel.PanStartedCommand));
            carouselView.SetBinding(CardsView.PositionChangedCommandProperty, nameof(PracticeViewerModel.PanPositionChangedCommand));
            AbsoluteLayout absoluteLayout = new AbsoluteLayout
            {
                Margin = 10
            };
            AbsoluteLayout.SetLayoutFlags(absoluteLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(absoluteLayout, new Rectangle(1, 1, 1, 0.1));
            StackLayout grid = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
            Image yesIcon = new Image
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 50,
                HeightRequest = 50
            };
            var heartStream = assembly.GetManifestResourceStream("SelfControl.Resources.heart_icon.png");
            var heartByte = new byte[heartStream.Length];
            heartStream.Read(heartByte, 0, System.Convert.ToInt32(heartStream.Length));
            yesIcon.Source = ImageSource.FromStream(() => new MemoryStream(heartByte));
            TapGestureRecognizer yesTapped = new TapGestureRecognizer();
            yesTapped.CommandParameter = false;
            yesTapped.SetBinding(TapGestureRecognizer.CommandProperty, nameof(PracticeViewerModel.PanPositionChangedCommand));
            yesIcon.GestureRecognizers.Add(yesTapped);

            Image noIcon = new Image
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 50,
                HeightRequest = 50
            };
            var cancelStream = assembly.GetManifestResourceStream("SelfControl.Resources.bin_icon.png");
            var cancelByte = new byte[cancelStream.Length];
            cancelStream.Read(cancelByte, 0, System.Convert.ToInt32(cancelStream.Length));
            noIcon.Source = ImageSource.FromStream(() => new MemoryStream(cancelByte));
            TapGestureRecognizer noTapped = new TapGestureRecognizer();
            noTapped.CommandParameter = true;
            noTapped.SetBinding(TapGestureRecognizer.CommandProperty, nameof(PracticeViewerModel.PanPositionChangedCommand));
            noIcon.GestureRecognizers.Add(noTapped);

            CustomPracticeButtons cool = new CustomPracticeButtons
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 50,
                HeightRequest = 50
            };
            var iceStream = assembly.GetManifestResourceStream("SelfControl.Resources.ice_button.png");
            var iceByte = new byte[iceStream.Length];
            iceStream.Read(iceByte, 0, System.Convert.ToInt32(iceStream.Length));
            cool.IconBytes = iceByte;
            cool.SetBinding(CustomPracticeButtons.OnTouchProperty, nameof(PracticeViewerModel.CurrentContext));

            grid.Children.Add(noIcon);
            grid.Children.Add(yesIcon);
            grid.Children.Add(cool);
            AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(grid, new Rectangle(1, 1, 1, 1));
            absoluteLayout.Children.Add(grid);
            carouselView.Children.Add(absoluteLayout);
        }
    }
}