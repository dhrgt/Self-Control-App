using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(SelfControl.Droid.Helpers.FileHelperAndroid))]
namespace SelfControl.Droid.Helpers
{
    class FileHelperAndroid : Java.Lang.Object, SelfControl.Interfaces.IFileHelper
    {
        public string GetLocalFilePath(string filename) {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }

        public string GetExternalFilePath(string filename)
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.ToString();
            return Path.Combine(path, filename);
        }

        public bool deleteFile(string path)
        {
            Java.IO.File file = new Java.IO.File(path);
            bool deleted = file.Delete();
            return deleted;
        }
    }
}