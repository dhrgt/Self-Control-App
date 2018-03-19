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
        ConnectionManager cm;
        DateTime mDateTime;
        int mWidth, mHeight, mOrientation;

        public CameraPage ()
		{
            mFoodName = string.Empty;
            mFileName = string.Empty;
            cm = new ConnectionManager(DependencyService.Get<SelfControl.Interfaces.IFileHelper>().GetLocalFilePath(SelfControl.Helpers.GlobalVariables.DATABASE_NAME));
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.Black;
            InitializeComponent ();
        }

        public void PictureClickedHandler(string file, DateTime dateTime, int width, int height, int orientation)
        {
            mFileName = file;
            mDateTime = dateTime;
            mWidth = width;
            mHeight = height;
            mOrientation = orientation;
            InsertNewEntry();
        }

        async private void CheckForStage2()
        {
            List<FoodItem> food = await cm.QueryByDateTime();
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
            item.IMGORIENTATION = mOrientation;
            Dictionary<int, int> answers = new Dictionary<int, int>();
            foreach(var questions in GlobalVariables.Questions)
            {
                answers.Add(questions.Key, -1);
            }
            item.ANSWERS = GlobalVariables.SerializeDictionary(answers);
            if (cm != null)
            {
                int i = await cm.SaveItemAsync(item);
                if(i == 1)
                {
                    List<FoodItem> food = await cm.QueryIdByDate(mDateTime);
                    int id = food.First().ID;
                    var img = File.ReadAllBytes(mFileName);
                    var aspectRatio = GlobalVariables.GetAspectRatio(mWidth, mHeight);
                    byte[] thumbnail = null;
                    if (aspectRatio == GlobalVariables.AspectRatio.SixteenByNine)
                        thumbnail = DependencyService.Get<Interfaces.IResizeImage>().Resize(img, 640, 360);
                    else
                        thumbnail = DependencyService.Get<Interfaces.IResizeImage>().Resize(img, 640, 480);
                    await Navigation.PushAsync(new EditDetailsPage(id, thumbnail, GlobalVariables.EntryType.NEW_ENTRY));
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