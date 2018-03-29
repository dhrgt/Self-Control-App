using SelfControl.Helpers;
using SelfControl.Helpers.Pages;
using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

            practiceViewer = new PracticeViewer(ChosenItems);
            this.SetBinding(PlanPage.CurrentIndexProperty, nameof(PracticeViewerModel.CurrentIndex));
            Content = practiceViewer.carouselView;
            BindingContext = new PracticeViewerModel(practiceViewer.list, 0);
        }
    }
}