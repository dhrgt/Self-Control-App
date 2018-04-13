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
	public partial class WeeklyReviewPage : ContentPage
	{
        private Dictionary<int, CustomRadioGroup> radioGroups;
        AbsoluteLayout absoluteLayout;
        AbsoluteLayout view;
        List<Models.WeeklyReviewTable> list;
        int CompletedWeeks;
        ReviewSelectionButton selected;
        Label label;

        public WeeklyReviewPage()
        {
            CompletedWeeks = 0;
            view = new AbsoluteLayout();
            absoluteLayout = new AbsoluteLayout();
            Task.Run(async () =>
            {
                list = await GlobalVariables.weeklyReviewDatabse.QueryByDateTime();
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
                        Text = "Week " + i.WEEK,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        HeightRequest = 100,
                        WidthRequest = 100,
                        DatabaseItem = i
                    };
                    button.Clicked += OnStartWeeklyReview;
                    if (i.ISCOMPLETED)
                    {
                        button.IsEnabled = false;
                        CompletedWeeks++;
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
                    Text = "Completed Weeks: " + CompletedWeeks
                };
                AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(label, new Rectangle(1, 1, 1, 0.1));

                view.Children.Add(grid);
                view.Children.Add(label);
                setView(view);
            });
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();

            this.Content = absoluteLayout;
        }

        private void OnStartWeeklyReview(object sender, EventArgs e)
        {
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Navigation.PushModalAsync(new WeeklyReviewViewer());
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
    }
}