using System;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using static Android.Views.View;

namespace BottomNavigationViewPager.Fragments
{
    [Android.Runtime.Register("onKeyDown", "(ILandroid/view/KeyEvent;)Z", "GetOnKeyDown_ILandroid_view_KeyEvent_Handler")]
    public class TheFragment3 : Fragment
    {
        string _title;
        string _icon;

        protected static WebView _wv;

        bool tabLoaded = false;

        public static TheFragment3 NewInstance(string title, string icon)
        {
            var fragment = new TheFragment3();
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
                if(Arguments.ContainsKey("title"))
                    _title = (string)Arguments.Get("title");

                if (Arguments.ContainsKey("icon"))
                    _icon = (string)Arguments.Get("icon");
            }
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.TheFragmentLayout3, container, false);

            _wv = view.FindViewById<WebView>(Resource.Id.webView3);

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(new ExtWebViewClient());

                _wv.Settings.MediaPlaybackRequiresUserGesture = false;

                _wv.LoadUrl(@"https://www.bitchute.com/playlists/");

                _wv.Settings.JavaScriptEnabled = true;

                //_wv.Settings.AllowFileAccess = true;

                //_wv.Settings.AllowContentAccess = true;

                tabLoaded = true;
            }

            return view;
        }


        public void WebViewGoBack()
        {
            if (_wv.CanGoBack())
                _wv.GoBack();
        }

        static bool _wvRl = true;

        public void Pop2Root()
        {
            if (_wvRl)
            {
                _wv.Reload();
                _wvRl = false;
            }
            else
            {
                _wv.LoadUrl(@"https://www.bitchute.com/playlists/");
            }
        }

        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                /*
                string _jsHideBanner = "javascript:(function() { " +
                                "document.getElementById('nav-top-menu').style.display='none'; " + "})()";

                string _jsHideBuff = "javascript:(function() { " +
               "document.getElementById('nav-menu-buffer').style.display='none'; " + "})()";

                //string _jsHideBannerC = "javascript:(function() { " +
                //   "document.getElementsByClassName('logo-wrap--home').style.display='none'; " + "})()";

                _wv.LoadUrl(_jsHideBanner);

                _wv.LoadUrl(_jsHideBuff);
                */
                _wvRl = true;
            }
        }
    }
}
