using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PanCardView.Controls;
using SelfControl.Models;
using PanCardView;

namespace SelfControl.Helpers.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImageViewer : ContentPage
	{
        public static readonly BindableProperty CurrentIndexProperty = 
        BindableProperty.Create("CurrentIndex", typeof(int), typeof(ImageViewer), -1, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var iv = (ImageViewer)bindable;
            iv.CurrentIndex = (int)newValue;
            iv.UpdateStats();
            iv.UpdateTitle();
        });

        async private void UpdateTitle()
        {
            List<FoodItem> Items = await GlobalVariables.foodItemsDatabase.QueryById(getCurrentID());
            FoodItem foodItem = Items.First();
            Title = foodItem.NAME;
        }

        async private void UpdateStats()
        {
            List<FoodItem> Items = await GlobalVariables.foodItemsDatabase.QueryById(getCurrentID());
            FoodItem foodItem = Items.First();
            Dictionary<int, int> dictionary = GlobalVariables.DeserializeDictionary(foodItem.ANSWERS);
            frequencyRating.Text = textToRating(dictionary[1]);
            healthRating.Text = textToRating(dictionary[2]);
        }

        private string textToRating(int i)
        {
            string rc = string.Empty;
            switch (i)
            {
                case 0:
                    rc = "O";
                    break;
                case 1:
                    rc = "O O";
                    break;
                case 2:
                    rc = "O O O";
                    break;
                case 3:
                    rc = "O O O O";
                    break;
                case 4:
                    rc = "O O O O O";
                    break;
            }
            return rc;
        }

        public int CurrentIndex
        {
            get;
            set;
        }

        private int getCurrentID()
        {
            return list.ElementAt(CurrentIndex).Key;
        }

        Label healthRating;
        Label frequencyRating;
        Dictionary<int, byte[]> list;
        public ImageViewer (FoodItem food)
		{   
            int index = 0;
            CurrentIndex = index;
            bool indexSet = false;
            List<FoodItem> FoodItems = GlobalVariables.getFoodItems();
            list = new Dictionary<int, byte[]>();
            foreach (var i in FoodItems)
            {
                list.Add(i.ID, GlobalVariables.DeserializeStringToByteArray(i.IMGBYTES));
                if (i == food && !indexSet)
                {
                    index = list.Count - 1;
                    indexSet = true;
                }
            }
            
            InitializeComponent ();
            PanCardView.CarouselView carouselView = new PanCardView.CarouselView
            {
                ItemTemplate = new DataTemplate(() => CardFactory.Creator.Invoke()),
                BackgroundColor = Color.Black
            };

            carouselView.SetBinding(CardsView.CurrentContextProperty, nameof(ImageViewerModel.CurrentContext));
            carouselView.SetBinding(CardsView.NextContextProperty, nameof(ImageViewerModel.NextContext));
            carouselView.SetBinding(CardsView.PrevContextProperty, nameof(ImageViewerModel.PrevContext));
            this.SetBinding(ImageViewer.CurrentIndexProperty, nameof(ImageViewerModel.CurrentIndex));
            carouselView.SetBinding(CardsView.PanStartedCommandProperty, nameof(ImageViewerModel.PanStartedCommand));
            carouselView.SetBinding(CardsView.PositionChangedCommandProperty, nameof(ImageViewerModel.PanPositionChangedCommand));

            //ToolbarItem deleteOption = new ToolbarItem("Delete", "", new Action(() => { DeleteSelectedImage(); }), ToolbarItemOrder.Primary, 0);
            ToolbarItem editOption = new ToolbarItem("Edit", "", new Action(() => { Navigation.PushAsync(new EditDetailsPage(getCurrentID(), GlobalVariables.EntryType.UPDATE_ENTRY)); }), ToolbarItemOrder.Primary, 0);

            AbsoluteLayout UserPrefs = new AbsoluteLayout
            {
                Margin = 10
            };
            AbsoluteLayout.SetLayoutFlags(UserPrefs, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserPrefs, new Rectangle(1,1,1,0.11));
            StackLayout prefsView = new StackLayout();
            AbsoluteLayout.SetLayoutFlags(prefsView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(prefsView, new Rectangle(1, 1, 1, 1));
            Grid grid = new Grid
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(25, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(25, GridUnitType.Absolute) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                }
            };

            Image healthIcon = new Image
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = 25,
                HeightRequest = 25,
                Source = ImageSource.FromResource("SelfControl.Resources.health_icon.png")
            };
            Image frequencyIcon = new Image
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = 25,
                HeightRequest = 25,
                Source = ImageSource.FromResource("SelfControl.Resources.frequency_icon.png")
            };
            healthRating = new Label
            {
                Text = "",
                FontSize = 15,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.White
            };
            frequencyRating = new Label
            {
                Text = "",
                FontSize = 15,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.White
            };
            grid.Children.Add(healthIcon, 0, 0);
            grid.Children.Add(healthRating, 1, 0);
            grid.Children.Add(frequencyIcon, 0, 1);
            grid.Children.Add(frequencyRating, 1, 1);

            prefsView.Children.Add(grid);
            UserPrefs.Children.Add(prefsView);
            carouselView.Children.Add(UserPrefs);

            Content = carouselView;
            //ToolbarItems.Add(deleteOption);
            ToolbarItems.Add(editOption);
            BindingContext = new ImageViewerModel(list, index);
        }
        protected override void OnAppearing()
        {
            UpdateStats();
            UpdateTitle();
            base.OnAppearing();
        }

        private void DeleteSelectedImage()
        {
            throw new NotImplementedException();
        }
    }
}