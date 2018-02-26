using SelfControl.DatabaseManager;
using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfControl
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CameraPage : ContentPage
	{
        string mFoodName;
        string mFileName;
        bool clicked;
        ConnectionManager cm;
        DateTime mDateTime;

        public CameraPage ()
		{
            mFoodName = string.Empty;
            mFileName = string.Empty;
            clicked = false;
            cm = new ConnectionManager(DependencyService.Get<SelfControl.Interfaces.IFileHelper>().GetLocalFilePath(SelfControl.Helpers.GlobalVariables.DATABASE_NAME));
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.Black;
            InitializeComponent ();
            okButton.Clicked += OnAlertYesNoClicked;
        }

        public void PictureClickedHandler(string file, DateTime dateTime)
        {
            clicked = true;
            EnteredName.Text = string.Empty;
            overlay.IsVisible = true;
            EnteredName.Focus();
            mFileName = file;
            mDateTime = dateTime;
        }

        void OnOKButtonClicked(object sender, EventArgs args)
        {
            overlay.IsVisible = false;
            mFoodName = EnteredName.Text;
            Console.WriteLine("Entered Name: " + mFoodName);
        }

        void OnCancelButtonClicked(object sender, EventArgs args)
        {
            overlay.IsVisible = false;
            clicked = false;
            DependencyService.Get<SelfControl.Interfaces.IFileHelper>().deleteFile(mFileName);
        }

        async void OnAlertYesNoClicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("", "Are you trying to eat/drink more " + mFoodName + "?", "Yes", "No");
            Console.WriteLine("Answer: " + answer);
            clicked = false;
            FoodItem item = new FoodItem();
            if (mFoodName != string.Empty)
            {
                item.ID = 0;
                item.DATE = mDateTime;
                item.NAME = mFoodName;
                item.PATH = mFileName;
                item.HOTEFFECT = answer;
                item.COOLEFFECT = !answer;
            }
            if (cm != null)
            {
                await cm.SaveItemAsync(item);
            }
            else
            {
                Console.WriteLine("Unable to store data, no cm found");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (clicked)
            {
                overlay.IsVisible = false;
                clicked = false;
                DependencyService.Get<SelfControl.Interfaces.IFileHelper>().deleteFile(mFileName);
                return true;
            }
            return base.OnBackButtonPressed();
        }

        public void NavigateBack()
        {
            Navigation.PopToRootAsync();
        }

        async public void NavigateToGallery()
        {
            await Navigation.PushAsync(new GalleryPage(), true);
        }
    }
}