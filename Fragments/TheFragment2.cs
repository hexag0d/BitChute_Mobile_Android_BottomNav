using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace BottomNavigationViewPager.Fragments
{
    public class TheFragment2 : Fragment
    {
        string _title;
        string _icon;

        protected static WebView _wv;

        bool tabLoaded = false;

        public static TheFragment2 NewInstance(string title, string icon) {
            var fragment = new TheFragment2();
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
            var view = inflater.Inflate(Resource.Layout.TheFragmentLayout2, container, false);

            _wv = view.FindViewById<WebView>(Resource.Id.webView2);

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(new WebViewClient());

                _wv.LoadUrl(@"https://www.bitchute.com/subscriptions/");

                _wv.Settings.JavaScriptEnabled = true;

                _wv.Settings.AllowContentAccess = true;

                _wv.Settings.AllowFileAccess = true;

                tabLoaded = true;
            }

            return view;
        }

        public void WebViewGoBack()
        {
            if (_wv.CanGoBack())
                _wv.GoBack();
        }

        private class MyWebViewClient : WebViewClient
        {
            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);


                //string _jsHideBanner = "javascript:(function() { " +
                //                "document.getElementById('content_homepage').style.display='none'; " + "})()";

               /* string _jsHideBannerC = "javascript:(function() { " +
                    "document.getElementsByClassName('logo-wrap--home').style.display='none'; " + "})()";
                    */
                //_wv.LoadUrl(_jsHideBanner);
            }


        }
    }
}
