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
        Slider mHealthStatus;

        public EditDetailsPage()
        {
            new EditDetailsPage(-1);
        }
        public EditDetailsPage (int id)
		{
			InitializeComponent ();
            Title = "Edit";
            mID = id;

            mName = new Entry
            {
                Placeholder = "Name "
            };

            StackLayout view = new StackLayout
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                Children =
                {
                    mName
                }
            };

            Content = view;
		}
	}
}