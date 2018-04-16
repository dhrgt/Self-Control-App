using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SelfControl.Helpers
{
    public class CustomRadioGroup : StackLayout
    {
        public List<CustomRadioButton> rads;

        public CustomRadioGroup()
        {

            rads = new List<CustomRadioButton>();
        }



        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create("Items", typeof(IEnumerable<CustomRadioButton>), typeof(CustomRadioButton), null, propertyChanged: (bindable, oldValue, newValue) =>
            {
                CustomRadioGroup rg = (CustomRadioGroup)bindable;
                rg.ItemsSource = (IEnumerable<CustomRadioButton>)newValue;
                OnItemsSourceChanged(bindable, (IEnumerable<CustomRadioButton>)oldValue, (IEnumerable<CustomRadioButton>)newValue);
            });


        public static BindableProperty SelectedIndexProperty =
            BindableProperty.Create("Selected", typeof(int), typeof(CustomRadioButton), -1, propertyChanged: (bindable, oldValue, newValue) =>
            {
                CustomRadioGroup rg = (CustomRadioGroup)bindable;
                rg.SelectedIndex = (int)newValue;
                OnSelectedIndexChanged(bindable, (int)oldValue, (int)newValue);
            });

        public IEnumerable<CustomRadioButton> ItemsSource
        {
            get { return (IEnumerable<CustomRadioButton>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }


        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public event EventHandler<int> CheckedChanged;

        private static void OnItemsSourceChanged(BindableObject bindable, IEnumerable<CustomRadioButton> oldvalue, IEnumerable<CustomRadioButton> newvalue)
        {
            var radButtons = bindable as CustomRadioGroup;

            radButtons.rads.Clear();
            radButtons.Children.Clear();
            if (newvalue != null)
            {

                int radIndex = 0;
                foreach (var item in newvalue)
                {
                    var rad = item;
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
                    SelectedIndex = rad.Id;

                }

            }

        }

        private static void OnSelectedIndexChanged(BindableObject bindable, int oldvalue, int newvalue)
        {
            var bindableRadioGroup = bindable as CustomRadioGroup;

            if (newvalue == -1)
            {
                foreach (var rad in bindableRadioGroup.rads)
                {
                    rad.Checked = false;
                }
                return;
            }

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
