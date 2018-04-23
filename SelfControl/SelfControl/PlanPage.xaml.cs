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
        List<FoodItem> ChosenItems;
        Grid viewGallery;
        List<FoodItem> FoodItems;
        bool ContinueOption;
        ToolbarItem continueToolBar;

        public PlanPage()
        {
            ContinueOption = false;
            ChosenItems = new List<FoodItem>(GlobalVariables.PRACTICE_NUMBER);
            InitializeComponent();
            
            FoodItems = GlobalVariables.getFoodItems();

#if RANDOM
            NavigationPage.SetHasNavigationBar(this, false);
            Randomizer randomizer = new Randomizer();
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
            //TODO: Change value of health from 0 to 4 to -2 to 2 
            // Use Math.Abs(health - 5)
            // Use Math.Pow(base, exponent)
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

#elif SELECT
            Title = "Selected 0";
            continueToolBar = new ToolbarItem("Continue", "", new Action(() => { Navigation.PushAsync(new Helpers.Pages.PracticeViewer(ChosenItems), true); }), ToolbarItemOrder.Primary, 0);
            viewGallery = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) }
                 },
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start
                
            };
            SetView();
            Update();
#endif
        }

        public void Update()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.Content = viewGallery;
            });
        }

        private void SetView()
        {
            viewGallery.Children.Clear();
            int colNum = 0;
            int rowNum = 0;
            foreach (var food in FoodItems)
            {
                byte[] thumbnail = Helpers.GlobalVariables.DeserializeStringToByteArray(food.IMGBYTES);
                if (thumbnail != null)
                {
                    ImageDisplay bmp = new ImageDisplay
                    {
                         ImageByte = thumbnail,
                         WidthRequest = App.ScreenWidth / 4,
                         HeightRequest = App.ScreenWidth / 4,
                         Aspect = Aspect.AspectFill,
                         DatabaseItem = food
                    };
                    bmp.OnTouch = new Command((p) =>
                    {
                        Console.WriteLine("LongClick");
                        bmp.IsSelected = !bmp.IsSelected;
                        if (bmp.IsSelected)
                        {
                            ChosenItems.Add(food);
                            UpdateTitle();
                        }
                        else
                        {
                             ChosenItems.Remove(food);
                             UpdateTitle();
                        }
                    });
                    bmp.OnClick = new Command((p) =>
                    {
                        bmp.IsSelected = !bmp.IsSelected;
                        if (bmp.IsSelected)
                        {
                            ChosenItems.Add(food);
                            UpdateTitle();
                        }
                        else
                        {
                            ChosenItems.Remove(food);
                            UpdateTitle();
                        }
                    });
                    
                    if (colNum == 4)
                    {
                        rowNum++;
                        colNum = 0;
                    }
                    viewGallery.Children.Add(bmp, colNum, rowNum);
                    colNum++;
                }
            }
        }

        public void UpdateTitle()
        {
            Title = "Selected " + ChosenItems.Count.ToString();
            MakeOptionsVisible();
        }

        public void MakeOptionsVisible()
        {
            if (ChosenItems.Count < 1)
            {
                ToolbarItems.Remove(continueToolBar);
                ContinueOption = false;
            }
            else if (ChosenItems.Count > 0 && !ContinueOption)
            {
                ToolbarItems.Add(continueToolBar);
                ContinueOption = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
#if RANDOM
            Navigation.PushAsync(new Helpers.Pages.PracticeViewer(ChosenItems), true);
#endif
        }

    }
}