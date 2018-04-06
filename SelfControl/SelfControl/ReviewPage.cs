using Xamarin.Forms;

namespace SelfControl
{
    public class ReviewPage : TabbedPage
    {
        public ReviewPage()
        {
            var navigationPage = new NavigationPage(new Page1());
            navigationPage.Title = "Schedule";

            Children.Add(new Page1());
            Children.Add(navigationPage);
        }
    }
}