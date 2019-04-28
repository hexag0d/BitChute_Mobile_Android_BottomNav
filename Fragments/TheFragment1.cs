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
        protected static View view;

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


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.TheFragmentLayout1, container, false);

            _wv = view.FindViewById<WebView>(Resource.Id.webView1);

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(_wvc);

                //string _wtf = "header";

                _wv.Settings.JavaScriptEnabled = true;

                _wv.Settings.AllowFileAccess = true;

                _wv.Settings.AllowContentAccess = true;

                _wv.LoadUrl(@"https://www.bitchute.com/");

                tabLoaded = true;
            }
            else
            {
                //_global.tabSelected = 0;
            }

            return view;
        }

        public void WebViewGoBack()
        {
            if (_wv.CanGoBack())
                _wv.GoBack();
        }

        public void Pop2Root()
        {
            _wv.LoadUrl(@"https://bitchute.com/");
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

                string _jsHideBanner = "javascript:(function() { " +
                                "document.getElementById('nav-top-menu').style.display='none'; " + "})()";

                string _jsHideBuff = "javascript:(function() { " +
               "document.getElementById('nav-menu-buffer').style.display='none'; " + "})()";

                //string _jsHideBannerC = "javascript:(function() { " +
                //   "document.getElementsByClassName('logo-wrap--home').style.display='none'; " + "})()";

                _wv.LoadUrl(_jsHideBanner);

                _wv.LoadUrl(_jsHideBuff);

                //_global.tabSelected = 0;
            }

            protected virtual void OnWindowVisibilityChanged([Android.Runtime.GeneratedEnum] ViewStates visibility)
            {
               // _global.tabSelected = 1;
                /*
                if (visibility != ViewStates.Gone)
                {
                    this.OnWindowVisibilityChanged(ViewStates.Visible);
                }*/
            }

            protected virtual void OnVisibilityChanged(View changedView, [Android.Runtime.GeneratedEnum] ViewStates visibility)
            {
               // _global.tabSelected = 1;
              /*  changedView = view;
                if (changedView.Visibility == ViewStates.Gone)
                {
                    if (_wv.Visibility == ViewStates.Gone)
                    {
                        _wv.Visibility = ViewStates.Visible;
                    }
                }*/
            
            }
              
        }
    }
}
