using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfControl.Helpers.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DailyReviewPage : ContentPage
	{
        private Dictionary<int, CustomRadioGroup> radioGroups;
        Label label;
        Button button;
        AbsoluteLayout absoluteLayout;

        public DailyReviewPage ()
		{
            NavigationPage.SetHasNavigationBar(this, false);

			InitializeComponent ();

            absoluteLayout = new AbsoluteLayout();

            button = new Button
            {
                Text = "Start",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            button.Clicked += OnStartDailyReview;

            label = new Label
            {
                Text = "Complete",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            DateTime t1 = Settings.LastDailyReviewValue;
            DateTime t2 = DateTime.Now;

            if (t2.Day - t1.Day > 0)
            {
                setView(button);
            }
            else
            {
                setView(label);
            }
            this.Content = absoluteLayout;
		}

        private void OnStartDailyReview(object sender, EventArgs e)
        {
            radioGroups = new Dictionary<int, CustomRadioGroup>();
            List<CustomRadioButton> list = new List<CustomRadioButton>();

            CustomRadioGroup Group = null;
            Label QuestionLabel = null;
            StackLayout questionsView = new StackLayout { Margin = 10 };

            foreach (var question in GlobalVariables.DailyReviewQuestions)
            {
                list.Clear();
                list.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                list.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                list.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                list.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                list.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                Group = new CustomRadioGroup
                {
                    ItemsSource = list,
                    Orientation = StackOrientation.Horizontal,
                    SelectedIndex = -1
                };
                QuestionLabel = new Label
                {
                    Text = question.Value,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    TextColor = Color.Black,
                    FontSize = 20,
                    Margin = new Thickness(10, 20, 0, 0)
                };
                radioGroups.Add(question.Key, Group);
                questionsView.Children.Add(QuestionLabel);
                questionsView.Children.Add(Group);
            }

            StackLayout buttonsView = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
            AbsoluteLayout.SetLayoutFlags(buttonsView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(buttonsView, new Rectangle(1, 1, 1, 0.1));
            Button doneButton = new Button
            {
                Text = "Done",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            doneButton.Clicked += OnDoneClickedAsync;
            Button cancelButton = new Button
            {
                Text = "Cancel",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            cancelButton.Clicked += OnCancelClicked;
            buttonsView.Children.Add(cancelButton);
            buttonsView.Children.Add(doneButton);

            ScrollView scrollView = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
                Content = questionsView
            };
            AbsoluteLayout.SetLayoutFlags(scrollView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(scrollView, new Rectangle(0, 0, 1, 0.9));

            AbsoluteLayout view = new AbsoluteLayout();
            view.Children.Add(scrollView);
            view.Children.Add(buttonsView);

            setView(view);
        }

        private void setView(View view)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                absoluteLayout.Children.Clear();
                AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(view, new Rectangle(1, 1, 1, 1));
                absoluteLayout.Children.Add(view);
            });
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            setView(button);
        }

        private async void OnDoneClickedAsync(object sender, EventArgs e)
        {
            bool radsFilled = true;
            foreach (var rads in radioGroups)
            {
                if (rads.Value.SelectedIndex == -1)
                {
                    radsFilled = false;
                    break;
                }
            }
            if (!radsFilled)
            {
                await DisplayAlert("Alert", "Please fill all the fields", "OK");
                return;
            }
            else
            {
                Models.DailyReviewTable dailyReview = new Models.DailyReviewTable();
                dailyReview.DATE = DateTime.Now;
                dailyReview.ID = 0;
                Dictionary<int, int> dict = new Dictionary<int, int>(radioGroups.Count);
                foreach (var rads in radioGroups)
                {
                    dict[rads.Key] = rads.Value.SelectedIndex;
                }
                dailyReview.RESPONSE = GlobalVariables.SerializeDictionary(dict);
                await GlobalVariables.dailyReviewDatabase.SaveItemAsync(dailyReview);
                Settings.LastDailyReviewValue = dailyReview.DATE;
                Settings.FirstDailyReviewValue = true;
                setView(label);
            }
        }
    }
}