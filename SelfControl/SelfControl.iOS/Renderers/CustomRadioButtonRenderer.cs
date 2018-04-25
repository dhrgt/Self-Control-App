using SelfControl.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UIKit;
using SelfControl.iOS.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Drawing;
using Foundation;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.CustomRadioButton), typeof(SelfControl.iOS.Renderers.CustomRadioButtonRenderer))]
namespace SelfControl.iOS.Renderers
{
    class CustomRadioButtonRenderer : ViewRenderer<SelfControl.Helpers.CustomRadioButton, RadioButtonView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomRadioButton> e)
        {
            base.OnElementChanged(e);

            BackgroundColor = Element.BackgroundColor.ToUIColor();

            if (Control == null)
            {
                var checkBox = new RadioButtonView((RectangleF)Bounds);
                checkBox.TouchUpInside += (s, args) => Element.Checked = Control.Checked;

                SetNativeControl(checkBox);
            }
            Control.LineBreakMode = UILineBreakMode.CharacterWrap;
            Control.VerticalAlignment = UIControlContentVerticalAlignment.Top;
            Control.Text = e.NewElement.Text;
            Control.Checked = e.NewElement.Checked;
            Control.SetTitleColor(e.NewElement.TextColor.ToUIColor(), UIControlState.Normal);
            Control.SetTitleColor(e.NewElement.TextColor.ToUIColor(), UIControlState.Selected);
        }

        private void ResizeText()
        {
            var text = this.Element.Text;
            
            var bounds = this.Control.Bounds;

            var width = this.Control.TitleLabel.Bounds.Width;

            var height = StringHeight(text, this.Control.Font, (float)width);

            var minHeight = StringHeight(string.Empty, Control.Font, (float)width);

            var requiredLines = Math.Round(height / minHeight, MidpointRounding.AwayFromZero);

            var supportedLines = Math.Round(bounds.Height / minHeight, MidpointRounding.ToEven);

            if (supportedLines != requiredLines)
            {
                bounds.Height += (float)(minHeight * (requiredLines - supportedLines));
                this.Control.Bounds = bounds;
                this.Element.HeightRequest = bounds.Height;
            }
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);
            this.ResizeText();
        }

        public static float StringHeight(string text, UIFont font, float width)
        {
            var nativeString = new NSString(text);

            var rect = nativeString.GetBoundingRect(
                new System.Drawing.SizeF(width, float.MaxValue),
                NSStringDrawingOptions.UsesLineFragmentOrigin,
                new UIStringAttributes() { Font = font },
                null);

            return (float)rect.Height;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case "Checked":
                    Control.Checked = Element.Checked;
                    break;
                case "Text":
                    Control.Text = Element.Text;
                    break;
                case "TextColor":
                    Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Normal);
                    Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Selected);
                    break;
                case "Element":
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Property change for {0} has not been implemented.", e.PropertyName);
                    return;
            }
        }
    }
}
