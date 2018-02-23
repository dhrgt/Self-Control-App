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

namespace SelfControl.Droid.Helpers.Listeners
{
    class FlashMenuListener : Java.Lang.Object, View.IOnClickListener
    {
        private CameraFragment owner;
        private Point MenuPoint;
        private PopupWindow flashMenuPopUp;

        public FlashMenuListener(CameraFragment fragment)
        {
            if (fragment == null)
                throw new System.ArgumentNullException("fragment");
            owner = fragment;
            MenuPoint = null;
        }

        public void OnClick(View v)
        {
            ImageButton flashButton = owner.FlashButton;
            switch (v.Id)
            {
                case Resource.Id.off:
                    owner.FlashMode = owner.Flash_off;
                    flashButton.SetBackgroundResource(Resource.Drawable.flash_off);
                    flashMenuPopUp.Dismiss();
                    break;
                case Resource.Id.on:
                    owner.FlashMode = owner.Flash_on;
                    flashButton.SetBackgroundResource(Resource.Drawable.flash_on);
                    flashMenuPopUp.Dismiss();
                    break;
                case Resource.Id.auto:
                    owner.FlashMode = owner.Flash_auto;
                    flashButton.SetBackgroundResource(Resource.Drawable.flash_auto);
                    flashMenuPopUp.Dismiss();
                    break;
            }
        }

        public void showFlashMenu(Point p)
        {

            // Inflate the popup_layout.xml
            LinearLayout viewGroup = (TableLayout)owner.Activity.FindViewById(Resource.Id.flash_menu);
            LayoutInflater layoutInflater = (LayoutInflater)owner.Activity.GetSystemService(Context.LayoutInflaterService);
            View layout = layoutInflater.Inflate(Resource.Layout.flash_menu, null);

            // Creating the PopupWindow
            flashMenuPopUp = new PopupWindow(owner.Activity);
            flashMenuPopUp.ContentView = layout;
            flashMenuPopUp.Width = LinearLayout.LayoutParams.WrapContent;
            flashMenuPopUp.Height = LinearLayout.LayoutParams.WrapContent;
            flashMenuPopUp.Focusable = true;

            // Some offset to align the popup a bit to the left, and a bit down, relative to button's position.
            int OFFSET_X = 0;
            int OFFSET_Y = 100;

            // Displaying the popup at the specified location, + offsets.
            flashMenuPopUp.ShowAtLocation(layout, GravityFlags.NoGravity, p.X + OFFSET_X, p.Y + OFFSET_Y);

            ImageButton off = (ImageButton)layout.FindViewById(Resource.Id.off);
            ImageButton on = (ImageButton)layout.FindViewById(Resource.Id.on);
            ImageButton auto = (ImageButton)layout.FindViewById(Resource.Id.auto);

            off.SetOnClickListener(this);
            on.SetOnClickListener(this);
            auto.SetOnClickListener(this);
        }
    }
}