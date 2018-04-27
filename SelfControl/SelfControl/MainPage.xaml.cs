using SelfControl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SelfControl
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.White;
            GlobalVariables.UpdateDateDiary();
            InitializeComponent();

            var title = new Label
            {
                Text = "Self Control",
                FontSize = 40,
                FontAttributes = FontAttributes.Italic,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(0, 50, 0, 20)
            };

            var cameraButton = new Button
            {
                Text = "Take a Picture",
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            cameraButton.Clicked += OnCameraButtonClicked;

            var galleryButton = new Button
            {
                Text = "Visit the Gallery",
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            galleryButton.Clicked += OnGalleryButtonClicked;

            var mealButton = new Button
            {
                Text = "Plan a Meal",
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            mealButton.Clicked += OnPlanButtonClicked;

            var reviewButton = new Button
            {
                Text = "Review Your Meals",
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            reviewButton.Clicked += OnReviewButtonClicked;

            Padding = Device.RuntimePlatform == Device.iOS? new Thickness(10, 20, 10, 5) : new Thickness(10, 0, 10, 5);
            var stack = new StackLayout();
            stack.Children.Add(title);
            stack.Children.Add(cameraButton);
            stack.Children.Add(galleryButton);
            if(Helpers.Settings.StageSettings == Helpers.GlobalVariables.STAGE_2)
            {
                if (!Settings.FirstDailyReviewValue)
                {
                    Settings.LastDailyReviewValue = DateTime.Now.AddDays(-1);
                    Settings.LastWeeklyReviewValue = DateTime.Now;
                    Settings.FirstDailyReviewValue = true;
                }
                stack.Children.Add(mealButton);
                stack.Children.Add(reviewButton);
            }

            if (Settings.FirstDailyReviewValue)
            {
                Task.Run(() => new Helpers.DB3ToCSV());
                Task.Run(() => SetDailyReviews());
                Task.Run(() => SetWeeklyReviews());
            }

                var info = new Label
            {
                Text = "This is the General Purpose of the App...",
                FontSize = 10,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand
            };

            this.Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    stack,
                    info
                }
            };
        }

        private void SetDailyReviews()
        {
            DateTime CurrentTime = DateTime.Now;
            DateTime LastUpdated = Settings.LastDailyReviewValue;

            Models.DailyReviewTable dailyReviewEntry;
            DateTime creationDate = LastUpdated;
            for(int i = 0; i < CurrentTime.Day - LastUpdated.Day; i++)
            {
                Settings.DailyReviewDayValue++;
                creationDate = creationDate.AddDays(1);
                dailyReviewEntry = new Models.DailyReviewTable();
                dailyReviewEntry.DAY = Settings.DailyReviewDayValue;
                dailyReviewEntry.DATECREATED = creationDate;
                dailyReviewEntry.ISCOMPLETED = false;
                Dictionary<int, int> dict = new Dictionary<int, int>(GlobalVariables.DailyReviewQuestions.Count);
                foreach (var rads in GlobalVariables.DailyReviewQuestions)
                {
                    dict[rads.Key] = -1;
                }
                dailyReviewEntry.RESPONSE = GlobalVariables.SerializeDictionary(dict);
                GlobalVariables.dailyReviewDatabase.SaveItemAsync(dailyReviewEntry);
            }

            Settings.LastDailyReviewValue = DateTime.Now;
        }

        private void SetWeeklyReviews()
        {
            DateTime CurrentTime = DateTime.Now;
            DateTime LastUpdated = Settings.LastWeeklyReviewValue;

            Models.WeeklyReviewTable weeklyReviewEntry;
            for (int i = 6; i < CurrentTime.Day - LastUpdated.Day; i+=7)
            {
                Settings.WeeklyReviewDayValue++;
                weeklyReviewEntry = new Models.WeeklyReviewTable();
                weeklyReviewEntry.WEEK = Settings.WeeklyReviewDayValue;
                weeklyReviewEntry.ISCOMPLETED = false;
                GlobalVariables.weeklyReviewDatabse.SaveItemAsync(weeklyReviewEntry);
            }

            if(CurrentTime.Day - LastUpdated.Day > 7)
                Settings.LastWeeklyReviewValue = CurrentTime;
        }

        async void OnCameraButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraPage(), true);
        }

        async void OnGalleryButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GalleryPage(), true);
        }

        async void OnPlanButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PlanPage(), true);
        }

        async void OnReviewButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReviewPage(), true);
        }
    }
}
