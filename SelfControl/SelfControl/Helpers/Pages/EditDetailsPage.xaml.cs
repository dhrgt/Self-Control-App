using SelfControl.DatabaseManager;
using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        Dictionary<int, CustomRadioGroup> radioGroups;
        Image imageView;
        GlobalVariables.EntryType EntryType;

        RelativeLayout view;
        FoodItem food;

        public EditDetailsPage (int id, GlobalVariables.EntryType e)
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, true);
            Title = "Edit Details";
            EntryType = e;
            mID = id;
            radioGroups = new Dictionary<int, CustomRadioGroup>();

            Label nameLabel = new Label
            {
                Text = "Name ",
                TextColor = Color.Black,
                FontSize = 25
            };
            mName = new Entry
            {
                WidthRequest = 200,
                Placeholder = " Name",
            };

            List<CustomRadioButton> list = new List<CustomRadioButton>();

            CustomRadioGroup Group = null;
            Label QuestionLabel = null;
            StackLayout questionsView = new StackLayout();

            foreach (var question in GlobalVariables.Questions)
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
                    Margin = new Thickness(10,20,0,0)
                }; 
                radioGroups.Add(question.Key, Group);
                questionsView.Children.Add(QuestionLabel);
                questionsView.Children.Add(Group);
            }
            
            view = new RelativeLayout();
            imageView = new Image
            {
                WidthRequest = 100,
                HeightRequest = 100,
                Aspect = Aspect.AspectFill
            };
            
            view.Children.Add(imageView,
                Constraint.RelativeToParent((parent) => 
                {
                    return parent.X + 10;
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
                    return sibling.Width + 40;
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

        public void Update()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.Content = view;
            });
        }
        Dictionary<int, int> dict;
        private void populateFields()
        {
            Task.Run(async() => {
                List<FoodItem> f = await GlobalVariables.foodItemsDatabase.QueryById(mID);
                food = f.First();
                dict = GlobalVariables.DeserializeDictionary(food.ANSWERS);
                imageView.Source = ImageSource.FromStream(() => new MemoryStream(GlobalVariables.DeserializeStringToByteArray(food.IMGBYTES)));
                if (EntryType == GlobalVariables.EntryType.UPDATE_ENTRY)
                {
                    mName.Text = food.NAME;
                    foreach (var question in dict)
                    {
                        CustomRadioGroup group = null; 
                        if(radioGroups.TryGetValue(question.Key, out group))
                        {
                            group.SelectedIndex = question.Value;
                        }
                    }
                }
                Update();
            });
        }

        

        async private void OnDoneClicked(object sender, EventArgs e)
        {
            bool radsFilled = true;
            foreach (var rads in radioGroups)
            {
                if(rads.Value.SelectedIndex == -1)
                {
                    radsFilled = false;
                    break;
                }
            }
            if (mName.Text == string.Empty || !radsFilled)
            {
                await DisplayAlert("Alert", "Please fill all the fields", "OK");
                return;
            }
            else if (food != null)
            {
                food.NAME = mName.Text;
                foreach (var rads in radioGroups)
                {
                    dict[rads.Key] = rads.Value.SelectedIndex;
                }
                food.ANSWERS = GlobalVariables.SerializeDictionary(dict);
                await GlobalVariables.foodItemsDatabase.SaveItemAsync(food);
                await Task.Run(() => { GlobalVariables.UpdateDateDiary(food.ID); });
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
                    await GlobalVariables.foodItemsDatabase.DeleteItemAsync(food);
                }
            });
            return base.OnBackButtonPressed();
        }

        async private void OnCancelClicked(object sender, EventArgs e)
        {
            if(EntryType == GlobalVariables.EntryType.NEW_ENTRY)
            {
                DependencyService.Get<SelfControl.Interfaces.IFileHelper>().deleteFile(food.PATH);
                await GlobalVariables.foodItemsDatabase.DeleteItemAsync(food);
            }
            await Navigation.PopAsync();
        }
    }
}