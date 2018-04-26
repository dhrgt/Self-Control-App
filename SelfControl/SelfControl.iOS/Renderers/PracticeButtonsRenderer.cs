using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.CustomPracticeButtons), typeof(SelfControl.iOS.Renderers.PracticeButtonsRenderer))]
namespace SelfControl.iOS.Renderers
{
    class PracticeButtonsRenderer : ButtonRenderer
    {
        private void HandleTouch(UIGestureRecognizer uIGestureRecognizer)
        {
            var button = (SelfControl.Helpers.CustomPracticeButtons)Element;
            if (uIGestureRecognizer.State == UIGestureRecognizerState.Began)
            {
                button.OnTouch.Execute(true);
            }
            else
            {
                button.OnTouch.Execute(false);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            Control.UserInteractionEnabled = true;
            UIGestureRecognizer touch = new UIGestureRecognizer();
            touch.AddTarget(() => HandleTouch(touch));
            Control.AddGestureRecognizer(touch);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var button = (SelfControl.Helpers.CustomPracticeButtons)Element;

            byte[] icon = button.IconBytes;

            if (icon != null)
            {
                UIImage img = Helpers.GlobalHelpers.ImageFromByteArray(icon);
                Control.SetImage(img, UIControlState.Normal);
                Control.SetImage(img, UIControlState.Selected);
                Control.SetImage(img, UIControlState.Focused);
                Control.SetImage(img, UIControlState.Highlighted);
            }
        }
    }
}