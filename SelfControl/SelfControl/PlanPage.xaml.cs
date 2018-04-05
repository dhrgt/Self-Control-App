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
        CustomPracticeButtons hot, cool;

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
            AbsoluteLayout.SetLayoutFlags(practiceViewer.carouselView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(practiceViewer.carouselView, new Rectangle(0, 0, 1, 0.88));
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
            fullView.Children.Add(practiceViewer.carouselView);
            fullView.Children.Add(grid);
            practiceViewer.carouselView.ViewChanged += GetCurrentView;
            BindingContext = new PracticeViewerModel(practiceViewer.carouselView, practiceViewer.list, 0);
            this.BackgroundColor = Color.Black;
            Content = fullView;
        }

        private void GetCurrentView(object sender, EventArgs e)
        {
            var view = (PracticeFactory)practiceViewer.carouselView._currentView;
            if (view != null)
            {
                cool.OnTouch = view.cool;
                hot.OnTouch = view.hot;
            }
        }
    }
}