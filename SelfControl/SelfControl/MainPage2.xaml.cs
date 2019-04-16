using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage2 : TabbedPage
    {
        public MainPage2()
        {
            InitializeComponent();
            var pages = Children.GetEnumerator();
            pages.MoveNext();
            pages.MoveNext();
            CurrentPage = pages.Current;
            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);

            this.CurrentPageChanged += (object sender, EventArgs e) => {
                var i = this.Children.IndexOf(this.CurrentPage);
                //System.Diagnostics.Debug.WriteLine("Page No:" + i);
                if (i == 1)
                {
                    CurrentPage.Navigation.PushAsync(new PlanPage(), true);
                    //Navigation.PushAsync(new PlanPage(), true);
                }
            };
        }


    }
}