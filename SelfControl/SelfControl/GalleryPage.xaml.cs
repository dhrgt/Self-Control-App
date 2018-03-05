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
	public partial class GalleryPage : ContentPage
	{
        Grid imageGrid;
        Grid view;
        int totalCols;
        Dictionary<int, ImageDisplay> images;
        ImageDisplay previewImage;
        ConnectionManager cm;


        public GalleryPage ()
		{
            InitializeComponent ();
            Title = "Gallery";
            ToolbarItem deleteOption = new ToolbarItem("Delete", "", new Action(() => { DeleteSelectedImage(); }), ToolbarItemOrder.Primary, 0);
            ToolbarItem editOption = new ToolbarItem("Edit", "", new Action(() => { Navigation.PushAsync(new EditDetailsPage(previewImage.ID, previewImage.ImageByte)); }), ToolbarItemOrder.Secondary, 0);
            ToolbarItem detailsOption = new ToolbarItem("Details", "", new Action(async () => { await DisplayAlert("Details", await SetDetails(previewImage.ID), "OK"); }), ToolbarItemOrder.Secondary, 1);
            ToolbarItems.Add(deleteOption);
            ToolbarItems.Add(editOption);
            ToolbarItems.Add(detailsOption);
            
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
            previewImage = new ImageDisplay();
            view.RowDefinitions.Add(new RowDefinition { Height = new GridLength(App.ScreenHeight - 150) } );
            view.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(App.ScreenWidth) });
            //view.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) });
            view.Children.Add(previewImage, 0, 0);
            view.Children.Add(scrollView, 0, 1);
            cm = new ConnectionManager(DependencyService.Get<Interfaces.IFileHelper>().GetLocalFilePath(SelfControl.Helpers.GlobalVariables.DATABASE_NAME));
            
            Task.Run(async() =>
            {
                List<FoodItem> food = await cm.QueryByDateTime();
                images = new Dictionary<int, ImageDisplay>(food.Count);
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
                        ImageDisplay bmp = new ImageDisplay
                        {
                            ImageByte = thumbnail,
                            WidthRequest = 100,
                            HeightRequest = 100,
                            Aspect = Aspect.AspectFit,
                            ID = i.ID
                        };
                        var thumbnailTap = new TapGestureRecognizer();
                        thumbnailTap.Tapped += thumbnailTapped;
                        bmp.GestureRecognizers.Add(thumbnailTap);
                        images.Add(i.ID, bmp);
                    }
                }
                if (images.Count > 0)
                {
                    fillGrid();
                    if (previewImage.ID == -1)
                    {
                        var preview = images.ElementAt(0);
                        SetPreviewImage(preview.Value.ID);
                    }
                }
                Update();
            });
		}

        async private Task<string> SetDetails(int id)
        {
            List<FoodItem> food = await cm.QueryById(id);
            if (food.Count == 1)
            {
                StringBuilder sb = new StringBuilder();
                var i = food.First();
                sb.Append("Name: ");
                sb.Append(i.NAME);
                sb.AppendLine();
                sb.Append("Date: ");
                DateTime date = i.DATE;
                sb.Append(date.ToLongDateString());
                sb.AppendLine();
                sb.Append("Time: ");
                sb.Append(date.ToShortTimeString());
                sb.AppendLine();
                sb.Append("Resolution: ");
                sb.Append(i.IMGWIDTH + "x" + i.IMGHEIGHT);
                return sb.ToString();
            }

            return "";
        }

        private void fillGrid()
        {
            int colNum = 0;
            int rowNum = 0;
            imageGrid.Children.Clear();
            foreach (var bmp in images)
            {
                imageGrid.Children.Add(bmp.Value, colNum, rowNum);
                colNum++;
            }
        }

        private void SetPreviewImage(int Id)
        {
            ImageDisplay preview = null;
            images.TryGetValue(Id, out preview);
            foreach (var i in images)
            {
                i.Value.IsSelected = false;
            }
            preview.IsSelected = true;
            previewImage.ImageByte = preview.ImageByte;
            previewImage.ID = preview.ID;
        }

        public void Update()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.Content = view;
            });
        }

        private void thumbnailTapped(object sender, EventArgs e)
        {
            var imgClicked = (ImageDisplay)sender;
            if (imgClicked.ID == previewImage.ID) return;
            
            SetPreviewImage(imgClicked.ID);
            Update();
        }

        private void DeleteSelectedImage()
        {
            var selectedId = previewImage.ID;
            var index = images.Keys.ToList().IndexOf(selectedId);
            if (images.Remove(selectedId))
            {
                if (index == images.Count) index -= 1;
                fillGrid();
                SetPreviewImage(images.ElementAt(index).Key);
                Update();
                Task.Run(async () => {
                    if (cm != null)
                    {
                        List<FoodItem> food = await cm.QueryById(selectedId);
                        if (food.Count == 1) await cm.DeleteItemAsync(food.First());
                        else throw new Exception();
                    }
                });
            }
        }
    }
}