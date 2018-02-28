using SelfControl.DatabaseManager;
using SelfControl.Helpers;
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
	public partial class GalleryPage : ContentPage
	{
        Grid imageGrid;
        Grid view;
        int totalCols;
        int colNum;
        int rowNum;
        List<byte[]> images;
        ImageDisplay previewImage;
        bool previewSet;

		public GalleryPage ()
		{
            previewSet = false;
            previewImage = new ImageDisplay();
            InitializeComponent ();
            this.Title = "Gallery";
            colNum = 0;
            rowNum = 0;

            imageGrid = new Grid
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                ColumnSpacing = 1
            };
            ScrollView scrollView = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                Content = imageGrid
            };
            view = new Grid
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
            };
            view.RowDefinitions.Add(new RowDefinition { Height = new GridLength(App.ScreenHeight - 150) } );
            view.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(App.ScreenWidth) });
            //view.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) });
            view.Children.Add(previewImage, 0, 0);
            view.Children.Add(scrollView, 0, 1);
            ConnectionManager cm = new ConnectionManager(DependencyService.Get<Interfaces.IFileHelper>().GetLocalFilePath(SelfControl.Helpers.GlobalVariables.DATABASE_NAME));
            
            Task.Run(async() =>
            {
                List<FoodItem> food = await cm.QueryByDateTime();
                images = new List<byte[]>(food.Count);
                foreach (var i in food)
                {
                    var img = File.ReadAllBytes(i.PATH);
                    var aspectRatio = GlobalVariables.GetAspectRatio(i.IMGWIDTH, i.IMGHEIGHT);
                    byte[] thumbnail = null;
                    if (aspectRatio == GlobalVariables.AspectRatio.SixteenByNine)
                        thumbnail = DependencyService.Get<Interfaces.IResizeImage>().Resize(img, 640, 360);
                    else
                        thumbnail = DependencyService.Get<Interfaces.IResizeImage>().Resize(img, 640, 480);
                    if(thumbnail != null)
                    {
                        images.Add(thumbnail);
                    }
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    Update();
                });
            });
		}

        public void Update()
        {
            if (images.Count > 0)
            {
                foreach (var thumbnail in images)
                {
                    ImageDisplay img = new ImageDisplay
                    {
                        ImageByte = thumbnail,
                        WidthRequest = 100,
                        HeightRequest = 100,
                        Aspect = Aspect.AspectFit,
                    };
                    img.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(() =>
                        {
                            previewImage.ImageByte = img.ImageByte;
                            img.IsSelected = true;
                            this.Content = view;
                        }),
                        NumberOfTapsRequired = 1
                    });
                    imageGrid.Children.Add(img, colNum, rowNum);
                    colNum++;
                }
                if (!previewSet)
                {
                    previewImage.ImageByte = images.ElementAt(0);
                    previewSet = true;
                }
                this.Content = view;
            }
        }
    }
}