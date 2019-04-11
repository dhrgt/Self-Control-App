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
            InitializeComponent();
            startImage.Source = ImageSource.FromResource("SelfControl.Resources.3takemorepics.png");


            var title = new Label
            {
                Text = "Self Control Game",
                FontSize = 40,
                FontAttributes = FontAttributes.Italic,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(0, 50, 0, 20)
            };

            //var cameraButton = new Button
            //{
            //    Text = "Take a Picture",
            //    BorderWidth = 1,
            //    HorizontalOptions = LayoutOptions.Center,
            //    VerticalOptions = LayoutOptions.Center
            //};
            //cameraButton.Clicked += OnCameraButtonClicked;

            var mealButton = new Button
            {
                Text = "Practice",
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center

            };


            var someText = new Label
            {
                Text = "OR",
                FontSize = 14,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = new Thickness(0, 50, 0, 20)
            };


            var someText1 = new Label
            {
                Text = "Take another picture",
                FontSize = 14,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = new Thickness(0, 50, 0, 20)
            };


            mealButton.Clicked += OnPlanButtonClicked;

            Padding = Device.RuntimePlatform == Device.iOS ? new Thickness(10, 20, 10, 5) : new Thickness(10, 0, 10, 5);
            var stack = new StackLayout();
            stack.Children.Add(title);
            //stack.Children.Add(cameraButton);
            stack.Children.Add(mealButton);

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

            //async void OnCameraButtonClicked(object sender, EventArgs e)
            //{
            //    await Navigation.PushAsync(new CameraPage(), true);
            //}

            async void OnPlanButtonClicked(object sender, EventArgs e)
            {
                await Navigation.PushAsync(new PlanPage(), true);

            }


        }
    }

