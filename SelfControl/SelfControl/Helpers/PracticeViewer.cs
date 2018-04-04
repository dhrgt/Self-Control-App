using PanCardView;
using PanCardView.Controls;
using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
                BackgroundColor = Color.Black,
                IsPanEnabled = false
            };
            //carouselView.SetBinding(CardsView.IsPanEnabledProperty, nameof(PracticeViewerModel.PanEnable));
            carouselView.SetBinding(CardsView.PrevContextProperty, nameof(PracticeViewerModel.PrevContext));
            carouselView.SetBinding(CardsView.CurrentContextProperty, nameof(PracticeViewerModel.CurrentContext));
            carouselView.SetBinding(CardsView.NextContextProperty, nameof(PracticeViewerModel.NextContext));
            carouselView.SetBinding(CardsView.PanStartedCommandProperty, nameof(PracticeViewerModel.PanStartedCommand));
            carouselView.SetBinding(CardsView.PositionChangedCommandProperty, nameof(PracticeViewerModel.PanPositionChangedCommand));
        }
    }
}