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
                stack.Children.Add(mealButton);
                stack.Children.Add(reviewButton);
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
