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

namespace SelfControl
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GalleryPage : ContentPage
	{
		public GalleryPage ()
		{
			InitializeComponent ();

            Grid imageGrid = new Grid
            {
                ColumnSpacing = 2,
                RowSpacing = 2
            };
            ConnectionManager cm = new ConnectionManager(DependencyService.Get<Interfaces.IFileHelper>().GetLocalFilePath(SelfControl.Helpers.GlobalVariables.DATABASE_NAME));

            Task.Run(async() =>
            {
                List<FoodItem> food = cm.QueryByDateTime();

                await Task.Run(() =>
                {
                    foreach (var i in food)
                    {
                        var img = File.ReadAllBytes(i.PATH);
                        byte[] thumbnail = DependencyService.Get<Interfaces.IResizeImage>().Resize(img, 50, 50);

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Image previewImage = new Image
                            {
                                
                            };
                        });
                    }
                });
            });
		}
	}
}