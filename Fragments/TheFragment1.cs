using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using static Android.Views.View;

namespace BottomNavigationViewPager.Fragments
{
    [Android.Runtime.Register("onWindowVisibilityChanged", "(I)V", "GetOnWindowVisibilityChanged_IHandler")]
    public class TheFragment1 : Fragment
    {
        string _title;
        string _icon;

        protected static WebView _wv;
        protected static View _view;

        readonly ExtWebViewClient _wvc = new ExtWebViewClient();

        bool tabLoaded = false;

        public static TheFragment1 NewInstance(string title, string icon) {
            var fragment = new TheFragment1();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Arguments != null)
            {
                if (Arguments.ContainsKey("title"))
                    _title = (string)Arguments.Get("title");

                if (Arguments.ContainsKey("icon"))
                    _icon = (string)Arguments.Get("icon");
            }
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.TheFragmentLayout1, container, false);

            _wv = _view.FindViewById<WebView>(Resource.Id.webView1);

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(_wvc);

                _wv.Settings.JavaScriptEnabled = true;

                //_wv.Settings.AllowFileAccess = true;

                //_wv.Settings.AllowContentAccess = true;

                //this didn't work when I put it here.  strange.. it would disable the setting on 
                //every other tab
               // _wv.Settings.MediaPlaybackRequiresUserGesture = false;

                _wv.LoadUrl(@"https://www.bitchute.com/");

                tabLoaded = true;
            }
            return _view;
        }

        /// <summary>
        /// tells the webview to GoBack, if it can
        /// </summary>
        public void WebViewGoBack()
        {
            if (_wv.CanGoBack())
                _wv.GoBack();
        }

        static bool _wvRl = true;

        /// <summary>
        /// one press refreshes the page; two presses pops back to the root
        /// </summary>
        public void Pop2Root()
        {
            if (_wvRl)
            {
                _wv.Reload();
                _wvRl = false;
            }
            else
            {
                _wv.LoadUrl(@"https://bitchute.com/");
            }
        }

        static int _autoInt = 0;

        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageStarted(WebView _view, string url, Android.Graphics.Bitmap favicon)
            {
                base.OnPageStarted(_view, url, favicon);
            }

            public override void OnPageFinished(WebView _view, string url)
            {
                base.OnPageFinished(_view, url);

                string _jsHideBanner = "javascript:(function() { " +
                                "document.getElementById('nav-top-menu').style.display='none'; " + "})()";

                string _jsHideBuff = "javascript:(function() { " +
               "document.getElementById('nav-menu-buffer').style.display='none'; " + "})()";

                //string _jsHideBannerC = "javascript:(function() { " +
                //   "document.getElementsByClassName('logo-wrap--home').style.display='none'; " + "})()";

                _wv.LoadUrl(_jsHideBanner);

                _wv.LoadUrl(_jsHideBuff);

                _wvRl = true;

                //add one to the autoint... for some reason if Tab1 has 
                //_wv.Settings.MediaPlaybackRequiresUserGesture = false; set then it won't work on the other tabs
                //this is a workaround for that glitch
                _autoInt++;

                // if autoInt is 2 then we will set the MediaPlaybackRequiresUserGesture
                if (_autoInt == 2)
                {
                    _wv.Settings.MediaPlaybackRequiresUserGesture = false;
                }
            }
        }
    }
}
