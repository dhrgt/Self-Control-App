using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfControl.Helpers
{
    public class CustomPracticeButtons : Button
    {
        public static readonly BindableProperty OnClickProperty =
        BindableProperty.Create("OnClick", typeof(ICommand), typeof(CustomPracticeButtons), null);

        public static readonly BindableProperty OnTouchProperty =
        BindableProperty.Create("OnTouch", typeof(ICommand), typeof(CustomPracticeButtons), null);

        public static readonly BindableProperty IconBytesProperty =
        BindableProperty.Create("IconBytes", typeof(byte[]), typeof(CustomPracticeButtons), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
        });

        public byte[] IconBytes
        {
            get { return (byte[])GetValue(IconBytesProperty); }
            set { SetValue(IconBytesProperty, value); }
        }

        public ICommand OnClick
        {
            get { return (ICommand)GetValue(OnClickProperty); }
            set { SetValue(OnClickProperty, value); }
        }

        public ICommand OnTouch
        {
            get { return (ICommand)GetValue(OnTouchProperty); }
            set { SetValue(OnTouchProperty, value); }
        }
    }
}
