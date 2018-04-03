using SelfControl.Helpers;
using SelfControl.Helpers.Pages;
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

namespace SelfControl
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlanPage : ContentPage
	{
        int _currentIndex;

        public static readonly BindableProperty CurrentIndexProperty =
        BindableProperty.Create("CurrentIndex", typeof(int), typeof(PlanPage), -1, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var iv = (PlanPage)bindable;
            iv.CurrentIndex = (int)newValue;
        });

        public static readonly BindableProperty CurrentPracticeImageProperty =
        BindableProperty.Create("CurrentPracticeImage", typeof(PracticeImageView), typeof(PlanPage), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
        });

        public PracticeImageView CurrentPracticeImage
        {
            get { return (PracticeImageView)GetValue(CurrentPracticeImageProperty); }
            set { SetValue(CurrentPracticeImageProperty, value); }
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                if (_currentIndex == GlobalVariables.PRACTICE_NUMBER) Navigation.PopAsync();
            }
        }

        List<FoodItem> ChosenItems;
        PracticeViewer practiceViewer;
        public Image yesIcon;
        public Image noIcon;

        public PlanPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            ChosenItems = new List<FoodItem>(GlobalVariables.PRACTICE_NUMBER);
            InitializeComponent();

            Randomizer randomizer = new Randomizer();
            List<FoodItem> FoodItems = GlobalVariables.getFoodItems();

#if RANDOM
            List<int> list;
            if (Settings.RandomCriteriaValue == (int)GlobalVariables.RandomCriteria.Random)
            {
                list = randomizer.GetIndecies(GlobalVariables.PRACTICE_NUMBER, FoodItems.Count);
                foreach (var i in list)
                {
                    ChosenItems.Add(FoodItems[i]);
                }
            }
            else if (Settings.RandomCriteriaValue == (int)GlobalVariables.RandomCriteria.Health)
            {
                int seed = 0;
                List<int> intervals = new List<int>(FoodItems.Count);
                for (int f = 0; f < FoodItems.Count; f++)
                {
                    FoodItem food = FoodItems[f];
                    int health = GlobalVariables.DeserializeDictionary(food.ANSWERS)[2];
                    seed += Math.Abs(health - 5);
                    for (int i = intervals.Count; i < seed; i++) intervals.Add(f);
                }
                list = randomizer.GetIndecies(GlobalVariables.PRACTICE_NUMBER, seed);
                foreach (var i in list)
                {
                    ChosenItems.Add(FoodItems[intervals[i]]);
                }
            }

            else if (Settings.RandomCriteriaValue == (int)GlobalVariables.RandomCriteria.DeltaFrequency)
            {
                int seed = 0;
                List<int> intervals = new List<int>(FoodItems.Count);
                for (int f = 0; f < FoodItems.Count; f++)
                {
                    FoodItem food = FoodItems[f];
                    Dictionary<int, int> dict = GlobalVariables.DeserializeDictionary(food.ANSWERS);
                    int health = dict[2];
                    int deltaFreq = Math.Abs(dict[0] - dict[1]);
                    seed += (health * deltaFreq);
                    for (int i = intervals.Count; i < seed; i++) intervals.Add(f);
                }
                list = randomizer.GetIndecies(GlobalVariables.PRACTICE_NUMBER, seed);
                foreach (var i in list)
                {
                    ChosenItems.Add(FoodItems[intervals[i]]);
                }
            }

            else if (Settings.RandomCriteriaValue == (int)GlobalVariables.RandomCriteria.HealthAndFrequency)
            {
                int seed = 0;
                List<int> intervals = new List<int>(FoodItems.Count);
                for (int f = 0; f < FoodItems.Count; f++)
                {
                    FoodItem food = FoodItems[f];
                    Dictionary<int, int> dict = GlobalVariables.DeserializeDictionary(food.ANSWERS);
                    int health = dict[2];
                    int Freq = dict[0];
                    seed += (health * Freq);
                    for (int i = intervals.Count; i < seed; i++) intervals.Add(f);
                }
                list = randomizer.GetIndecies(GlobalVariables.PRACTICE_NUMBER, seed);
                foreach (var i in list)
                {
                    ChosenItems.Add(FoodItems[intervals[i]]);
                }
            }
#endif
            Assembly assembly = typeof(PlanPage).GetTypeInfo().Assembly;
            practiceViewer = new PracticeViewer(ChosenItems);
            this.SetBinding(PlanPage.CurrentIndexProperty, nameof(PracticeViewerModel.CurrentIndex));

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
            var hot = new CustomPracticeButtons
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 60,
                HeightRequest = 60,
            };
            var fireStream = assembly.GetManifestResourceStream("SelfControl.Resources.fire_button.png");
            var fireByte = new byte[fireStream.Length];
            fireStream.Read(fireByte, 0, System.Convert.ToInt32(fireStream.Length));
            hot.IconBytes = fireByte;
            this.SetBinding(PlanPage.CurrentPracticeImageProperty, nameof(PracticeViewerModel.heatImage));
            hot.OnTouch = new Command((p) =>
            {
                bool Continue = (bool)p;
                if (Continue)
                {
                    CurrentPracticeImage.IncreaseSaturation = 1;
                }
                else if (!Continue)
                {
                    CurrentPracticeImage.IncreaseSaturation = 0;
                }
            });

            AbsoluteLayout absoluteLayout = new AbsoluteLayout
            {
                Margin = 10,
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            AbsoluteLayout.SetLayoutFlags(absoluteLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(absoluteLayout, new Rectangle(1, 1, 1, 0.1));
            StackLayout grid = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
            grid.Children.Add(yesIcon);
            grid.Children.Add(noIcon);
            grid.Children.Add(hot);
            AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(grid, new Rectangle(1, 1, 1, 1));
            absoluteLayout.Children.Add(grid);
            Grid l = new Grid();
            l.Children.Add(practiceViewer.carouselView);
            l.Children.Add(absoluteLayout);
            //practiceViewer.carouselView.Children.Add(absoluteLayout);
            Content = l;
            BindingContext = new PracticeViewerModel(practiceViewer.list, 0);
        }
    }
}