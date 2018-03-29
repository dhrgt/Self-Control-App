﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SelfControl.Helpers.CustomPracticeButtons), typeof(SelfControl.Droid.Renderers.PracticeButtonsRenderer))]
namespace SelfControl.Droid.Renderers
{
    class PracticeButtonsRenderer : ButtonRenderer, /*Android.Views.View.IOnClickListener,*/ Android.Views.View.IOnTouchListener
    {
        Activity mActivity;
        public PracticeButtonsRenderer(Context context) : base(context)
        {
        }

        /*public void OnClick(Android.Views.View v)
        {
            var button = (SelfControl.Helpers.CustomPracticeButtons)Element;
            button.OnClick.Execute(button.CommandParameter);
        }*/

        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            bool shouldClick = false;
            var button = (SelfControl.Helpers.CustomPracticeButtons)Element;
            switch (e.Action) {
                case MotionEventActions.Down:
                    button.OnTouch.Execute(true);
                    break;
                case MotionEventActions.Up:
                    if(shouldClick) v.PerformClick();
                    else
                    {
                        button.OnTouch.Execute(false);

                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            Control.SetOnTouchListener(this);
            //Control.SetOnClickListener(this);
            Control.Invalidate();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            mActivity = Context as Activity;
            var button = (SelfControl.Helpers.CustomPracticeButtons)Element;

            byte[] icon = button.IconBytes;

            if(icon != null)
            {
                MemoryStream bs = new MemoryStream(icon);
                Drawable drw = Drawable.CreateFromStream(bs, "buttonIcon");
                Control.SetBackground(drw);
                Control.Invalidate();
            }
        }
    }
}