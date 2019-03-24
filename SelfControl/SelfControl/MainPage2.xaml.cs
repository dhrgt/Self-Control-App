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
        }


    }
}