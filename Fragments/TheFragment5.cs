using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace BottomNavigationViewPager.Fragments
{
    public class TheFragment5 : Fragment
    {
        string _title;
        string _icon;

        protected static WebView _wv;

        bool tabLoaded = false;

        public static TheFragment5 NewInstance(string title, string icon)
        {
            var fragment = new TheFragment5();
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

        public void WebViewGoBack()
        {
            if (_wv.CanGoBack())
                _wv.GoBack();
        }

        int _tbc;

        public void Pop2Root()
        {
            if (_tbc == 0)
            {
                _wv.Reload();
                _tbc = 1;
            }
            else
            {
                _wv.LoadUrl(@"https://www.bitchute.com/settings/");
                _tbc = 0;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //hex
            var view = inflater.Inflate(Resource.Layout.TheFragmentLayout5, container, false);

            WebView _wv = view.FindViewById<WebView>(Resource.Id.webView5);

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(new ExtWebViewClient());

                _wv.LoadUrl(@"https://www.bitchute.com/settings/");

                _wv.Settings.JavaScriptEnabled = true;

                _wv.Settings.AllowFileAccess = true;

                _wv.Settings.AllowContentAccess = true;

                tabLoaded = true;
            }

            return view;
        }

        private class ExtWebViewClient : WebViewClient
        {

            protected virtual void OnWindowVisibilityChanged([Android.Runtime.GeneratedEnum] ViewStates visibility)
            {
                if (visibility != ViewStates.Gone)
                {
                    this.OnWindowVisibilityChanged(ViewStates.Visible);
                }
            }

            protected virtual void OnVisibilityChanged(View changedView, [Android.Runtime.GeneratedEnum] ViewStates visibility)
            {

            }

        }
    }
}
