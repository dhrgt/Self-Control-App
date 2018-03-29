using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SelfControl.Droid.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SelfControl.CameraPage), typeof(SelfControl.Droid.Renderers.CameraPageRenderer))]
namespace SelfControl.Droid.Renderers
{
    public class CameraPageRenderer : PageRenderer
    {
        Android.Views.View mView;
        Activity mActivity;

        public CameraPageRenderer(Context context) : base(context)
        {
            mActivity = Context as Activity;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            if (e.OldElement != null || Element == null)
                return;

            try
            {
                mView = mActivity.LayoutInflater.Inflate(Resource.Layout.camera_fragment, this, false);
                
                mActivity.Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn);

                AddView(mView);
                CameraFragment cameraFragment = new CameraFragment(this, mView, mActivity);
                cameraFragment.Start();

            }
            catch (Exception ex)
            {
                System.Console.Write(ex);
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            mView.Measure(msw, msh);
            mView.Layout(0, 0, r - l, b - t);
        }

        public void NavigateImageQuestionPage(String file, DateTime dateTime, int width, int height, byte[] imgBytes)
        {
            ((SelfControl.CameraPage)Element).PictureClickedHandler(file, dateTime, width, height, imgBytes);
        }

        public void NavigateToGallery()
        {
            ((SelfControl.CameraPage)Element).NavigateToGallery();
        }

        public void NavigateBack()
        {
            ((SelfControl.CameraPage)Element).NavigateBack();
        }
    }
}