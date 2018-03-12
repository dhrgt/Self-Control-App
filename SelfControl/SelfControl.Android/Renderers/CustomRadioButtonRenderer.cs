using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.CustomRadioButton), typeof(SelfControl.Droid.Renderers.CustomRadioButtonRenderer))]
namespace SelfControl.Droid.Renderers
{
    class CustomRadioButtonRenderer : ViewRenderer<SelfControl.Helpers.CustomRadioButton, RadioButton>
    {
        public CustomRadioButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SelfControl.Helpers.CustomRadioButton> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged += ElementOnPropertyChanged;
            }

            if (this.Control == null)
            {
                var radButton = new RadioButton(this.Context);
                radButton.CheckedChange += radButton_CheckedChange;

                this.SetNativeControl(radButton);
            }
            Control.ButtonTintList = Android.Content.Res.ColorStateList.ValueOf(Xamarin.Forms.Color.Blue.ToAndroid());
            Control.ButtonTintMode = PorterDuff.Mode.SrcIn;
            Control.Text = e.NewElement.Text;
            Control.Checked = e.NewElement.Checked;
            
            Element.PropertyChanged += ElementOnPropertyChanged;
        }

        void radButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            this.Element.Checked = e.IsChecked;
        }
        
        void ElementOnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Checked":
                    Control.Checked = Element.Checked;
                    break;
                case "Text":
                    Control.Text = Element.Text;
                    break;
            }
        }
    }
}