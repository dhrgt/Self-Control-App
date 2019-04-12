using PanCardView;
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
    public partial class PracticeViewer : ContentPage
    {
        public PanCardView.CardsView carouselView;
        public Dictionary<int, byte[]> list;

        int _currentIndex;

        public static readonly BindableProperty CurrentIndexProperty =
        BindableProperty.Create("CurrentIndex", typeof(int), typeof(PlanPage), -1, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var iv = (PracticeViewer)bindable;
            iv.CurrentIndex = (int)newValue;
        });

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                if (_currentIndex == list.Count - 1)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(2000);
                        Device.BeginInvokeOnMainThread(() => Navigation.PopToRootAsync());
                    });
                }
            }
        }

        public Image yesIcon;
        public Image noIcon;
        CustomPracticeButtons hot, cool;

        public PracticeViewer(List<FoodItem> FoodList)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            list = new Dictionary<int, byte[]>();
            foreach (var i in FoodList)
            {
                list.Add(i.ID, GlobalVariables.DeserializeStringToByteArray(i.IMGBYTES));
            }
            Assembly assembly = typeof(PracticeViewer).GetTypeInfo().Assembly;
            var blankStream = assembly.GetManifestResourceStream("SelfControl.Resources.takemorepics.png");
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

            this.SetBinding(PracticeViewer.CurrentIndexProperty, nameof(PracticeViewerModel.CurrentIndex));

            noIcon = new Image
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 60,
                HeightRequest = 60
            };
            var cancelStream = assembly.GetManifestResourceStream("SelfControl.Resources.bin_icon.png");
            var cancelByte = new byte[cancelStream.Length];
            cancelStream.Read(cancelByte, 0, System.Convert.ToInt32(cancelStream.Length));
            noIcon.Source = ImageSource.FromStream(() => new MemoryStream(cancelByte));
            TapGestureRecognizer noTapped = new TapGestureRecognizer();
            noTapped.CommandParameter = true;
            noTapped.SetBinding(TapGestureRecognizer.CommandProperty, nameof(PracticeViewerModel.PanPositionChangedCommand));
            noIcon.GestureRecognizers.Add(noTapped);

            yesIcon = new Image
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 60,
                HeightRequest = 60
            };
            var heartStream = assembly.GetManifestResourceStream("SelfControl.Resources.heart_icon.png");
            var heartByte = new byte[heartStream.Length];
            heartStream.Read(heartByte, 0, System.Convert.ToInt32(heartStream.Length));
            yesIcon.Source = ImageSource.FromStream(() => new MemoryStream(heartByte));
            TapGestureRecognizer yesTapped = new TapGestureRecognizer();
            yesTapped.CommandParameter = false;
            yesTapped.SetBinding(TapGestureRecognizer.CommandProperty, nameof(PracticeViewerModel.PanPositionChangedCommand));
            yesIcon.GestureRecognizers.Add(yesTapped);


            cool = new CustomPracticeButtons
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 60,
                HeightRequest = 60
            };
            var iceStream = assembly.GetManifestResourceStream("SelfControl.Resources.ice_button.png");
            var iceByte = new byte[iceStream.Length];
            iceStream.Read(iceByte, 0, System.Convert.ToInt32(iceStream.Length));
            cool.IconBytes = iceByte;
            hot = new CustomPracticeButtons
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 60,
                HeightRequest = 60,
            };
            var fireStream = assembly.GetManifestResourceStream("SelfControl.Resources.fire_button.png");
            var fireByte = new byte[fireStream.Length];
            fireStream.Read(fireByte, 0, System.Convert.ToInt32(fireStream.Length));
            hot.IconBytes = fireByte;
            AbsoluteLayout.SetLayoutFlags(carouselView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(carouselView, new Rectangle(0, 0, 1, 0.88));
            StackLayout grid = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.Black
            };
            grid.Children.Add(hot);
            grid.Children.Add(yesIcon);
            grid.Children.Add(noIcon);
            grid.Children.Add(cool);
            AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(grid, new Rectangle(1, 1, 1, 0.12));

            AbsoluteLayout fullView = new AbsoluteLayout();
            fullView.Children.Add(carouselView);
            fullView.Children.Add(grid);
            carouselView.ViewChanged += GetCurrentView;
            BindingContext = new PracticeViewerModel(carouselView, list, 0);
            this.BackgroundColor = Color.Black;
            Content = fullView;

        }

        private void GetCurrentView(object sender, EventArgs e)
        {
            var view = (PracticeFactory)carouselView._currentView;
            if (view != null)
            {
                cool.OnTouch = view.cool;
                hot.OnTouch = view.hot;
            }
        }
    }
}