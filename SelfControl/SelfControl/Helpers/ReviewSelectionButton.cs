using SelfControl.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SelfControl.Helpers
{
    public class ReviewSelectionButton : Button
    {
        public static readonly BindableProperty ButtonIdProperty =
        BindableProperty.Create("DataBaseItem", typeof(object), typeof(ReviewSelectionButton), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
        });

        public object DatabaseItem
        {
            get { return (object)GetValue(ButtonIdProperty); }
            set { SetValue(ButtonIdProperty, value); }
        }
    }
}
