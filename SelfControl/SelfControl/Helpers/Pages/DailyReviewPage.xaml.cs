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
        AbsoluteLayout absoluteLayout;
        AbsoluteLayout view;
        List<Models.DailyReviewTable> list;
        int CompletedDays;
        ReviewSelectionButton selected;
        Label label;

        public DailyReviewPage()
		{
            CompletedDays = 0;
            view = new AbsoluteLayout();
            absoluteLayout = new AbsoluteLayout();
            Task.Run(async () =>
            {
                list = await GlobalVariables.dailyReviewDatabase.QueryByDateTime();
                Grid grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) }
                    },
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = 5
                };
                AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(grid, new Rectangle(0, 0, 1, 0.9));
                int colNum = 0;
                int rowNum = 0;
                foreach (var i in list)
                {
                    ReviewSelectionButton button = new ReviewSelectionButton
                    {
                        Text = i.DATECREATED.ToLongDateString(),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        HeightRequest = 100,
                        WidthRequest = 100,
                        DatabaseItem = i
                    };
                    button.Clicked += OnStartDailyReview;
                    if (i.ISCOMPLETED)
                    {
                        button.IsEnabled = false;
                        CompletedDays++;
                    }
                    
                    if (colNum == 3)
                    { 
                        rowNum++;
                        colNum = 0;
                    }
                    grid.Children.Add(button, colNum, rowNum);
                    colNum++;
                }

                label = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = 5,
                    Text = "Completed Days: " + CompletedDays
                };
                AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(label, new Rectangle(1, 1, 1, 0.1));

                view.Children.Add(grid);
                view.Children.Add(label);
                setView(view);
            });
            NavigationPage.SetHasNavigationBar(this, false);

			InitializeComponent ();
            
            this.Content = absoluteLayout;
		}

        private void OnStartDailyReview(object sender, EventArgs e)
        {
            selected = (ReviewSelectionButton)sender;
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
            setView(view);
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
                var databaseItem = (Models.DailyReviewTable)selected.DatabaseItem;
                databaseItem.DATE = DateTime.Now;
                Dictionary<int, int> dict = new Dictionary<int, int>(radioGroups.Count);
                foreach (var rads in radioGroups)
                {
                    dict[rads.Key] = rads.Value.SelectedIndex;
                }
                databaseItem.RESPONSE = GlobalVariables.SerializeDictionary(dict);
                databaseItem.ISCOMPLETED = true;
                await GlobalVariables.dailyReviewDatabase.SaveItemAsync(databaseItem);
                selected.IsEnabled = false;
                CompletedDays++;
                label.Text = "Completed Days: " + CompletedDays;
                setView(view);
            }
        }
    }
}