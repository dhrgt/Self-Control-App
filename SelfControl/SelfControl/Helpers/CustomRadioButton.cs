using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SelfControl.Helpers
{
    public class CustomRadioButton : View
    {
        public static readonly BindableProperty CheckedProperty =
                   BindableProperty.Create("Checked", typeof(bool), typeof(CustomRadioButton), false, propertyChanged: (bindable, oldValue, newValue) =>
                   {
                       CustomRadioButton r = (CustomRadioButton)bindable;
                       r.Checked = (bool)newValue;
                   });

        /// <summary>
        /// The default text property.
        /// </summary>
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(CustomRadioButton), "", propertyChanged: (bindable, oldValue, newValue) =>
            {
                CustomRadioButton r = (CustomRadioButton)bindable;
                r.Text = (string)newValue;
            });

        /// <summary>
        /// The checked changed event.
        /// </summary>
        public EventHandler<EventArgs<bool>> CheckedChanged;


        /// <summary>
        /// Identifies the TextColor bindable property.
        /// </summary>
        /// 
        /// <remarks/>
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create("TextColor", typeof(Color), typeof(CustomRadioButton), Color.Black, propertyChanged: (bindable, oldValue, newValue) =>
            {
                CustomRadioButton r = (CustomRadioButton)bindable;
                r.TextColor = (Color)newValue;
            });

        /// <summary>
        /// Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <value>The checked state.</value>
        public bool Checked
        {
            get
            {
                return (bool)this.GetValue(CheckedProperty);
            }

            set
            {
                this.SetValue(CheckedProperty, value);
                var eventHandler = this.CheckedChanged;
                if (eventHandler != null)
                {
                    eventHandler.Invoke(this, value);
                }
            }
        }

        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }

            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        public Color TextColor
        {
            get
            {
                return (Color)this.GetValue(TextColorProperty);
            }

            set
            {
                this.SetValue(TextColorProperty, value);
            }
        }

        public int Id { get; set; }
    }
}
