using SelfControl.DatabaseManager;
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
	public partial class EditDetailsPage : ContentPage
	{
        int mID;
        
        Entry mName;
        CustomSlider mFrequencySlider;
        CustomSlider mPlanSlider;
        CustomSlider mHealthSlider;
        Label FrequencyResultLabel;
        Label PlanResultLabel;
        Label HealthResultLabel;
        bool dirty;

        GlobalVariables.EntryType EntryType;

        RelativeLayout view;

        ConnectionManager cm;
        FoodItem food;

        public EditDetailsPage (int id, byte[] image, GlobalVariables.EntryType e)
		{
            dirty = false;
            cm = new ConnectionManager(DependencyService.Get<Interfaces.IFileHelper>().GetLocalFilePath(SelfControl.Helpers.GlobalVariables.DATABASE_NAME));
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, true);
            Title = "Edit Details";
            EntryType = e;
            mID = id;

            mName = new Entry
            {
                WidthRequest = 200,
                Placeholder = " Name",
            };
            mName.Completed += OnNameChanged;
            mName.Unfocused += OnNameChanged;
            mFrequencySlider = new CustomSlider
            {
                Maximum = 4.0,
                Minimum = 0.0,
                Value = 2.0
            };
            mFrequencySlider.ValueChanged += OnFrequencySliderValueChanged;

            mHealthSlider = new CustomSlider
            {
                Maximum = 6.0,
                Minimum = 0.0,
                Value = 3.0
            };
            mHealthSlider.ValueChanged += OnHealthSliderValueChanged;

            mPlanSlider = new CustomSlider
            {
                Maximum = 4.0,
                Minimum = 0.0,
                Value = 2.0
            };
            mPlanSlider.ValueChanged += OnFrequencySliderValueChanged;

            Label nameLabel = new Label
            {
                Text = "Name ",
                TextColor = Color.Black,
                FontSize = 25
            };
            Label FrequencyLabel = new Label
            {
                Text = "How frequently do you eat this?",
                TextColor = Color.Black,
                FontSize = 20,
                Margin = 20
            };
            FrequencyResultLabel = new Label
            {
                TextColor = Color.Black,
                FontSize = 15,
                Margin = new Thickness(20,0,0,0)
            };
            PlanResultLabel = new Label
            {
                TextColor = Color.Black,
                FontSize = 15,
                Margin = new Thickness(20, 0, 0, 0)
            };
            HealthResultLabel = new Label
             {
                 TextColor = Color.Black,
                 FontSize = 15,
                 Margin = new Thickness(20, 0, 0, 0)
             };
            Label PlanLabel = new Label
            {
                Text = "How much do you plan to eat this in the future?",
                TextColor = Color.Black,
                FontSize = 20,
                Margin = 20
            };
            Label HealthLabel = new Label
            {
                Text = "How healthy do you think this food is?",
                TextColor = Color.Black,
                FontSize = 20,
                Margin = 20
            };

            view = new RelativeLayout();
            ImageDisplay imageView = new ImageDisplay
            {
                WidthRequest = 100,
                HeightRequest = 100,
                ImageByte = image
            };
            
            view.Children.Add(imageView,
                Constraint.RelativeToParent((parent) => 
                {
                    return parent.X + 20;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Y + 20;
                }),
                Constraint.Constant(100), Constraint.Constant(100)
            );

            view.Children.Add(nameLabel, Constraint.RelativeToView(imageView, 
                (Parent, sibling) => 
                {
                    return sibling.Width + 30;
                }), 
                Constraint.RelativeToView(imageView,
                (Parent, sibling) =>
                {
                    return Parent.Y + 30;
                }));

            view.Children.Add(mName, Constraint.RelativeToView(imageView,
                (Parent, sibling) =>
                {
                    return sibling.Width + 30;
                }),
                Constraint.RelativeToView(nameLabel,
                (Parent, sibling) =>
                {
                    return sibling.Y + 30;
                }));
            StackLayout questionsView = new StackLayout
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start
            };

            List<string> list = new List<string>();
            list.Add("1"); list.Add("2"); list.Add("3");

            CustomRadioGroup radioGroup = new CustomRadioGroup
            {
                ItemsSource = list,
                SelectedIndex = 1,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            questionsView.Children.Add(FrequencyLabel);
            questionsView.Children.Add(mFrequencySlider);
            questionsView.Children.Add(FrequencyResultLabel);
            questionsView.Children.Add(HealthLabel);
            questionsView.Children.Add(mHealthSlider);
            questionsView.Children.Add(HealthResultLabel);
            questionsView.Children.Add(PlanLabel);
            questionsView.Children.Add(mPlanSlider);
            questionsView.Children.Add(PlanResultLabel);
            questionsView.Children.Add(radioGroup);

            view.Children.Add(questionsView, Constraint.RelativeToView(imageView,
                (Parent, sibling) =>
                {
                    return Parent.X;
                }),
                Constraint.RelativeToView(imageView,
                (Parent, sibling) =>
                {
                    return sibling.Height + 50;
                }));

            Button doneButton = new Button
            {
                Text = "Done"
            };
            doneButton.Clicked += OnDoneClicked;
            Button cancelButton = new Button
            {
                Text = "Cancel"
            };
            cancelButton.Clicked += OnCancelClicked;

            view.Children.Add(cancelButton, Constraint.RelativeToParent(
                (Parent) =>
                {
                    return Parent.X;
                }),
                Constraint.RelativeToParent(
                (Parent) =>
                {
                    return Parent.Height - 40;
                }),
                Constraint.RelativeToParent(
                (Parent) =>
                {
                    return Parent.Width * 0.5;
                }),
                Constraint.RelativeToParent(
                (Parent) =>
                {
                    return 40;
                }));
            view.Children.Add(doneButton, Constraint.RelativeToView(cancelButton,
                (Parent, sibling) =>
                {
                    return sibling.Width;
                }),
                Constraint.RelativeToParent(
                (Parent) =>
                {
                    return Parent.Height - 40;
                }),
                Constraint.RelativeToParent(
                (Parent) =>
                {
                    return Parent.Width * 0.5;
                }),
                Constraint.RelativeToParent(
                (Parent) =>
                {
                    return 40;
                }));

            populateFields();
		}

        private void OnNameChanged(object sender, EventArgs e)
        {
            if (food.NAME != mName.Text)
            {
                dirty = true;
            }
            else
            {
                dirty = false;
            }
        }

        public void Update()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.Content = view;
            });
        }

        private void populateFields()
        {
            Task.Run(async() => {
                List<FoodItem> f = await cm.QueryById(mID);
                food = f.First();
                if (EntryType == GlobalVariables.EntryType.UPDATE_ENTRY)
                {
                    mName.Text = food.NAME;
                    mFrequencySlider.Value = food.FREQUENCY;
                    mHealthSlider.Value = food.HEALTH;
                    mPlanSlider.Value = food.PLAN;
                }
                PlanResultLabel.Text = GlobalVariables.FrequencyResult[(int)mPlanSlider.Value];
                FrequencyResultLabel.Text = GlobalVariables.FrequencyResult[(int)mFrequencySlider.Value];
                HealthResultLabel.Text = GlobalVariables.HealthResult[(int)mHealthSlider.Value];
                Update();
            });
            
        }

        void OnFrequencySliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            CustomSlider slider = (CustomSlider)sender;
            var newStep = Math.Round(e.NewValue / 1.0);
            dirty = true;
            slider.Value = newStep * 1.0;
            string text = "";
            switch (slider.Value)
            {
                case 0:
                    text = GlobalVariables.FrequencyResult[0];
                    break;
                case 1:
                    text = GlobalVariables.FrequencyResult[1];
                    break;
                case 2:
                    text = GlobalVariables.FrequencyResult[2];
                    break;
                case 3:
                    text = GlobalVariables.FrequencyResult[3];
                    break;
                case 4:
                    text = GlobalVariables.FrequencyResult[4];
                    break;
                default:
                    break;
            }
            if (slider == mFrequencySlider)
                FrequencyResultLabel.Text = text;
            else if (slider == mPlanSlider)
                PlanResultLabel.Text = text;
        }

        void OnHealthSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            CustomSlider slider = (CustomSlider)sender;
            var newStep = Math.Round(e.NewValue / 1.0);
            dirty = true;
            slider.Value = newStep * 1.0;
            string text = "";
            switch (slider.Value)
            {
                case 0:
                    text = GlobalVariables.HealthResult[0];
                    break;
                case 1:
                    text = GlobalVariables.HealthResult[1];
                    break;
                case 2:
                    text = GlobalVariables.HealthResult[2];
                    break;
                case 3:
                    text = GlobalVariables.HealthResult[3];
                    break;
                case 4:
                    text = GlobalVariables.HealthResult[4];
                    break;
                case 5:
                    text = GlobalVariables.HealthResult[5];
                    break;
                case 6:
                    text = GlobalVariables.HealthResult[6];
                    break;
                default:
                    break;
            }
            HealthResultLabel.Text = text;
        }

        async private void OnDoneClicked(object sender, EventArgs e)
        {
            // TODO: Name cannot be null
            if (food != null && dirty)
            {
                food.NAME = mName.Text;
                food.FREQUENCY = (int)mFrequencySlider.Value;
                food.PLAN = (int)mPlanSlider.Value;
                food.HEALTH = (int)mHealthSlider.Value;
                await cm.SaveItemAsync(food);
                await Navigation.PopAsync();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Task.Run(async () =>
            {
                if (EntryType == GlobalVariables.EntryType.NEW_ENTRY)
                {
                    DependencyService.Get<SelfControl.Interfaces.IFileHelper>().deleteFile(food.PATH);
                    await cm.DeleteItemAsync(food);
                }
            });
            return base.OnBackButtonPressed();
        }

        async private void OnCancelClicked(object sender, EventArgs e)
        {
            if(EntryType == GlobalVariables.EntryType.NEW_ENTRY)
            {
                DependencyService.Get<SelfControl.Interfaces.IFileHelper>().deleteFile(food.PATH);
                await cm.DeleteItemAsync(food);
            }
            await Navigation.PopAsync();
        }
    }
}