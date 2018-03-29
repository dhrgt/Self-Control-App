using PanCardView;
using PanCardView.Controls;
using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfControl.Helpers.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class PracticeViewer
    {
        public PanCardView.CardsView carouselView;
        public Dictionary<int, byte[]> list;
        public PracticeViewer(List<FoodItem> FoodList)
        {
            list = new Dictionary<int, byte[]>();
            foreach (var i in FoodList)
            {
                list.Add(i.ID, GlobalVariables.DeserializeStringToByteArray(i.IMGBYTES));
            }
            Assembly assembly = typeof(PracticeViewer).GetTypeInfo().Assembly;
            var blankStream = assembly.GetManifestResourceStream("SelfControl.Resources.blank.jpg");
            var blankByte = new byte[blankStream.Length];
            blankStream.Read(blankByte, 0, System.Convert.ToInt32(blankStream.Length));
            list.Add(-1, blankByte);

            carouselView = new PanCardView.CardsView
            {
                ItemTemplate = new DataTemplate(() => new PracticeFactory()),
                BackgroundColor = Color.Black
            };
            
            carouselView.SetBinding(CardsView.PrevContextProperty, nameof(PracticeViewerModel.PrevContext));
            carouselView.SetBinding(CardsView.CurrentContextProperty, nameof(PracticeViewerModel.CurrentContext));
            carouselView.SetBinding(CardsView.NextContextProperty, nameof(PracticeViewerModel.NextContext));
            carouselView.SetBinding(CardsView.PanStartedCommandProperty, nameof(PracticeViewerModel.PanStartedCommand));
            carouselView.SetBinding(CardsView.PositionChangedCommandProperty, nameof(PracticeViewerModel.PanPositionChangedCommand));

            Grid grid = new Grid
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                }
            };
            CustomPracticeButtons yesIcon = new CustomPracticeButtons
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 50,
                HeightRequest = 50
            };
            var heartStream = assembly.GetManifestResourceStream("SelfControl.Resources.heart_icon.png");
            var heartByte = new byte[heartStream.Length];
            heartStream.Read(heartByte, 0, System.Convert.ToInt32(heartStream.Length));
            yesIcon.IconBytes = heartByte;
            yesIcon.CommandParameter = false;
            yesIcon.SetBinding(CustomPracticeButtons.OnClickProperty, nameof(PracticeViewerModel.PanStartedCommand));
            yesIcon.SetBinding(CustomPracticeButtons.OnClickProperty, nameof(PracticeViewerModel.PanPositionChangedCommand));

            CustomPracticeButtons noIcon = new CustomPracticeButtons
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 50,
                HeightRequest = 50
            };
            var cancelStream = assembly.GetManifestResourceStream("SelfControl.Resources.bin_icon.png");
            var cancelByte = new byte[cancelStream.Length];
            cancelStream.Read(cancelByte, 0, System.Convert.ToInt32(cancelStream.Length));
            noIcon.IconBytes = cancelByte;
            noIcon.CommandParameter = true;
            noIcon.SetBinding(CustomPracticeButtons.OnClickProperty, nameof(PracticeViewerModel.PanStartedCommand));
            noIcon.SetBinding(CustomPracticeButtons.OnClickProperty, nameof(PracticeViewerModel.PanPositionChangedCommand));

            CustomPracticeButtons cool = new CustomPracticeButtons
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 50,
                HeightRequest = 50
            };
            var iceStream = assembly.GetManifestResourceStream("SelfControl.Resources.ice_button.png");
            var iceByte = new byte[iceStream.Length];
            iceStream.Read(iceByte, 0, System.Convert.ToInt32(iceStream.Length));
            cool.IconBytes = iceByte;
            cool.SetBinding(CustomPracticeButtons.OnTouchProperty, nameof(PracticeViewerModel.OnCool));

            grid.Children.Add(noIcon, 0, 0);
            grid.Children.Add(yesIcon, 1, 0);
            grid.Children.Add(cool, 2, 0);
            AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(grid, new Rectangle(1, 1, 1, 0.1));
            carouselView.Children.Add(grid);
        }
    }
}