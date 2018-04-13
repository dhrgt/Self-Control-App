using PanCardView;
using SelfControl.Models;
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
	public partial class WeeklyReviewViewer : ContentPage
	{
        public PanCardView.CardsView carouselView;
        public List<FoodItem> list;
        public Dictionary<int, CustomRadioGroup> radioGroups;
        public Dictionary<int, Dictionary<int, int>> responses;
        int _currentIndex;
        AbsoluteLayout fullView;

        public static readonly BindableProperty CurrentIndexProperty =
        BindableProperty.Create("CurrentIndex", typeof(int), typeof(WeeklyReviewViewer), -1, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var iv = (WeeklyReviewViewer)bindable;
            iv.CurrentIndex = (int)newValue;
        });

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                if (_currentIndex == list.Count - 1) Navigation.PopModalAsync();
            }
        }

        public WeeklyReviewViewer ()
		{
            InitializeComponent ();

            carouselView = new PanCardView.CarouselView
            {
                ItemTemplate = new DataTemplate(() => new WeeklyReviewFactory()),
                BackgroundColor = Color.White,
                IsPanEnabled = false
            };
            AbsoluteLayout.SetLayoutFlags(carouselView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(carouselView, new Rectangle(0, 0, 1, 0.5));
            carouselView.SetBinding(CardsView.PrevContextProperty, nameof(WeeklyReviewViewerModel.PrevContext));
            carouselView.SetBinding(CardsView.CurrentContextProperty, nameof(WeeklyReviewViewerModel.CurrentContext));
            carouselView.SetBinding(CardsView.NextContextProperty, nameof(WeeklyReviewViewerModel.NextContext));
            carouselView.SetBinding(CardsView.PanStartedCommandProperty, nameof(WeeklyReviewViewerModel.PanStartedCommand));
            carouselView.SetBinding(CardsView.PositionChangedCommandProperty, nameof(WeeklyReviewViewerModel.PanPositionChangedCommand));

            this.SetBinding(WeeklyReviewViewer.CurrentIndexProperty, nameof(WeeklyReviewViewerModel.CurrentIndex));

            Task.Run(async() => {
                list = await GlobalVariables.foodItemsDatabse.QueryByDateTime();
                responses = new Dictionary<int, Dictionary<int, int>>(list.Count);
                foreach (var i in list)
                {
                    Dictionary<int, int> ques = new Dictionary<int, int>();
                    foreach (var q in GlobalVariables.Questions)
                    {
                        ques.Add(q.Key, -1);
                    }
                    responses.Add(i.ID, ques);
                }
                Device.BeginInvokeOnMainThread(() => {
                    List<CustomRadioButton> radioButtonList = new List<CustomRadioButton>();
                    CustomRadioGroup Group = null;
                    Label QuestionLabel = null;
                    StackLayout questionsView = new StackLayout();
                    radioGroups = new Dictionary<int, CustomRadioGroup>();
                    foreach (var question in GlobalVariables.Questions)
                    {
                        radioButtonList.Clear();
                        radioButtonList.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                        radioButtonList.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                        radioButtonList.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                        radioButtonList.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                        radioButtonList.Add(new CustomRadioButton { HorizontalOptions = LayoutOptions.CenterAndExpand });
                        Group = new CustomRadioGroup
                        {
                            ItemsSource = radioButtonList,
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
                    AbsoluteLayout.SetLayoutFlags(questionsView, AbsoluteLayoutFlags.All);
                    AbsoluteLayout.SetLayoutBounds(questionsView, new Rectangle(1, 1, 1, 0.5));

                    Button next = new Button
                    {
                        Text = "Next",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        CommandParameter = true
                    };
                    next.SetBinding(Button.CommandProperty, nameof(WeeklyReviewViewerModel.PanPositionChangedCommand));
                    Button prev = new Button
                    {
                        Text = "Previous",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        CommandParameter = false
                    };
                    prev.SetBinding(Button.CommandProperty, nameof(WeeklyReviewViewerModel.PanPositionChangedCommand));
                    StackLayout buttonStack = new StackLayout
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Orientation = StackOrientation.Horizontal
                    };
                    AbsoluteLayout.SetLayoutFlags(buttonStack, AbsoluteLayoutFlags.All);
                    AbsoluteLayout.SetLayoutBounds(buttonStack, new Rectangle(1, 1, 1, 0.1));

                    buttonStack.Children.Add(prev);
                    buttonStack.Children.Add(next);
                    BindingContext = new WeeklyReviewViewerModel(list, this);
                    fullView = new AbsoluteLayout();
                    fullView.Children.Add(carouselView);
                    fullView.Children.Add(questionsView);
                    fullView.Children.Add(buttonStack);
                    Content = fullView;
                });
            });
        }

        public void UpdateView()
        {
            Device.BeginInvokeOnMainThread(() => { Content = fullView; });
        }
    }
}