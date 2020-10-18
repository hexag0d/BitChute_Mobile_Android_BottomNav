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

namespace BitChute.Web
{
    public class Login
    {
        private static bool _userIsLoggingIn = false;
        public static bool UserIsLoggingIn
        {
            get { return _userIsLoggingIn; }
            set { _userIsLoggingIn = value; }
        }
    }
}