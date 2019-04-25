using SelfControl.DatabaseManager;
using SelfControl.Helpers;
using SelfControl.Helpers.Pages;
using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        DateTime mDateTime;
        int mWidth, mHeight;
        byte[] mImageBytes;

        public CameraPage ()
		{
            mFoodName = string.Empty;
            mFileName = string.Empty;
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.Black;
            InitializeComponent ();
        }

        public void PictureClickedHandler(string file, DateTime dateTime, int width, int height, byte[] imageBytes)
        {
            mFileName = file;
            mDateTime = dateTime;
            mWidth = width;
            mHeight = height;
            mImageBytes = imageBytes;
            InsertNewEntry();
        }

        async private void CheckForStage2()
        {
            List<FoodItem> food = await GlobalVariables.foodItemsDatabase.QueryByDateTime();
            if (food.Count == GlobalVariables.SIZE_OF_FOOD_LIBRARY)
                Helpers.Settings.StageSettings = GlobalVariables.STAGE_2;
        }

        async void InsertNewEntry()
        {
            FoodItem item = new FoodItem();
            item.ID = 0;
            item.DATE = mDateTime;
            item.NAME = mFoodName;
            item.PATH = mFileName;
            item.HOTEFFECT = true;
            item.COOLEFFECT = false;
            item.IMGWIDTH = mWidth;
            item.IMGHEIGHT = mHeight;
            item.IMGBYTES = GlobalVariables.SerializeByteArrayToString(mImageBytes);
            Dictionary<int, int> answers = new Dictionary<int, int>();
            foreach(var questions in GlobalVariables.Questions)
            {
                answers.Add(questions.Key, -1);
            }
            item.ANSWERS = GlobalVariables.SerializeDictionary(answers);
            if (GlobalVariables.foodItemsDatabase != null)
            {
                int i = await GlobalVariables.foodItemsDatabase.SaveItemAsync(item);
                if(i == 1)
                {
                    List<FoodItem> food = await GlobalVariables.foodItemsDatabase.QueryIdByDate(mDateTime);
                    int id = food.First().ID;
                    await Navigation.PushAsync(new EditDetailsPage(id, GlobalVariables.EntryType.NEW_ENTRY));
                    if(Settings.StageSettings == GlobalVariables.STAGE_1)
                        CheckForStage2();
                }
            }
            else
            {
                Console.WriteLine("Unable to store data, no cm found");
            }
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