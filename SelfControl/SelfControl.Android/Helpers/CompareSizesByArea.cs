using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;

namespace SelfControl.Droid.Helpers
{
    class CompareSizesByArea : Java.Lang.Object, IComparator
    {
        public int Compare(Java.Lang.Object lhs, Java.Lang.Object rhs)
        {
            var lhsSize = (Size)lhs;
            var rhsSize = (Size)rhs;
            // We cast here to ensure the multiplications won't overflow
            return Java.Lang.Long.Signum((long)lhsSize.Width * lhsSize.Height - (long)rhsSize.Width * rhsSize.Height);
        }
    }
}