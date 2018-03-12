using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SelfControl.Helpers
{
    class CustomRadioGroup : StackLayout
    {
        public List<CustomRadioButton> rads;

        public CustomRadioGroup()
        {

            rads = new List<CustomRadioButton>();
        }



        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create("Items", typeof(IEnumerable<string>), typeof(CustomRadioButton), null, propertyChanged: (bindable, oldValue, newValue) =>
            {
                CustomRadioGroup rg = (CustomRadioGroup)bindable;
                rg.ItemsSource = (IEnumerable<string>)newValue;
                OnItemsSourceChanged(bindable, (IEnumerable<string>)oldValue, (IEnumerable<string>)newValue);
            });


        public static BindableProperty SelectedIndexProperty =
            BindableProperty.Create("Selected", typeof(int), typeof(CustomRadioButton), -1, propertyChanged: (bindable, oldValue, newValue) =>
            {
                CustomRadioGroup rg = (CustomRadioGroup)bindable;
                rg.SelectedIndex = (int)newValue;
                OnSelectedIndexChanged(bindable, (int)oldValue, (int)newValue);
            });

        public IEnumerable<string> ItemsSource
        {
            get { return (IEnumerable<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }


        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public event EventHandler<int> CheckedChanged;

        private static void OnItemsSourceChanged(BindableObject bindable, IEnumerable<string> oldvalue, IEnumerable<string> newvalue)
        {
            var radButtons = bindable as CustomRadioGroup;

            radButtons.rads.Clear();
            radButtons.Children.Clear();
            if (newvalue != null)
            {

                int radIndex = 0;
                foreach (var item in newvalue)
                {
                    var rad = new CustomRadioButton();
                    rad.Text = item.ToString();
                    rad.Id = radIndex;

                    rad.CheckedChanged += radButtons.OnCheckedChanged;

                    radButtons.rads.Add(rad);

                    radButtons.Children.Add(rad);
                    radIndex++;
                }
            }
        }

        private void OnCheckedChanged(object sender, EventArgs<bool> e)
        {

            if (e.Value == false) return;

            var selectedRad = sender as CustomRadioButton;

            foreach (var rad in rads)
            {
                if (!selectedRad.Id.Equals(rad.Id))
                {
                    rad.Checked = false;
                }
                else
                {
                    if (CheckedChanged != null)
                        CheckedChanged.Invoke(sender, rad.Id);

                }

            }

        }

        private static void OnSelectedIndexChanged(BindableObject bindable, int oldvalue, int newvalue)
        {
            if (newvalue == -1) return;

            var bindableRadioGroup = bindable as CustomRadioGroup;
            
            foreach (var rad in bindableRadioGroup.rads)
            {
                if (rad.Id == bindableRadioGroup.SelectedIndex)
                {
                    rad.Checked = true;
                }

            }
        }
    }
}
