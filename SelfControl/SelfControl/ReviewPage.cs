using Xamarin.Forms;

namespace SelfControl
{
    public class ReviewPage : TabbedPage
    {
        public ReviewPage()
        {
            Title = "Weekly Review";
            //var chartsPage = new NavigationPage(new Page1());
            //chartsPage.Title = "Charts";

            var weeklyPage = new NavigationPage(new Helpers.Pages.WeeklyReviewPage());
            //Children.Add(chartsPage);

            Padding = new Thickness(0, 20, 0, 0);
            Children.Add(weeklyPage);
        }
    }
}