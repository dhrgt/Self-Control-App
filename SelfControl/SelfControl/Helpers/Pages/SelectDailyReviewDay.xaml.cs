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
	public partial class SelectDailyReviewDay : ContentPage
	{
        List<DailyReviewTable> list;

        Grid grid;
        DailyReviewPage parent;

        public SelectDailyReviewDay (DailyReviewPage parent)
		{
            this.parent = parent;
			InitializeComponent ();

            grid = new Grid
            {
                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(App.ScreenWidth / 4, GridUnitType.Absolute) }
                    },
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                Margin = 10
            };

            Assembly assembly = typeof(SelectDailyReviewDay).GetTypeInfo().Assembly;
            var backStream = assembly.GetManifestResourceStream("SelfControl.Resources.back.png");
            var backByte = new byte[backStream.Length];
            backStream.Read(backByte, 0, System.Convert.ToInt32(backStream.Length));

            Task.Run(async () =>
            {
                list = await GlobalVariables.dailyReviewDatabase.QueryByDateTime();
                foreach(var i in list)
                {
                    ReviewSelectionButton button = new ReviewSelectionButton
                    {
                        Text = i.DAY.ToString(),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        HeightRequest = 100,
                        WidthRequest = 100,
                        DatabaseItem = i
                    };
                    button.Clicked += OnSelect;
                    grid.Children.Add(button);
                }
            });

            Content = grid;
        }

        private void OnSelect(object sender, EventArgs e)
        {
            var button = (ReviewSelectionButton)sender;
            var item = button.DatabaseItem;

            parent.selected = item;
            parent.UpdateSelected();
            Navigation.PopModalAsync();
        }
    }
}