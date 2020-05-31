using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Lang;

namespace BitChute.Classes
{
    class ExtJavaScriptInterface : Java.Lang.Object, Android.Webkit.IValueCallback
    {
        public ExtJavaScriptInterface()
        {

        }

        public void OnReceiveValue(Java.Lang.Object value)
        {
            var yo = value.ToString();
        }
    }
}