using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Views;
using BitChute.Models;

namespace BitChute.Classes
{
    class ExtViewTreeListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        public void OnGlobalLayout()
        {
           // var test = MainActivity.HideKeyboard();
        }
    }
}