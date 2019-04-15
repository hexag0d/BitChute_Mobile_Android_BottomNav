using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace BottomNavigationViewPager.Fragments
{
    public class TheFragment4 : Fragment
    {
        string _title;
        string _icon;

        protected static WebView _wv;

        bool tabLoaded = false;

        public static TheFragment4 NewInstance(string title, string icon) {
            var fragment = new TheFragment4();
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

        public void WebViewGoBack()
        {
            if (_wv.CanGoBack())
                _wv.GoBack();
        }

        //createview frag4
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.TheFragmentLayout4, container, false);

            _wv = view.FindViewById<WebView>(Resource.Id.webView4);

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(new WebViewClient());

                _wv.LoadUrl(@"https://www.bitchute.com/profile/");

                _wv.Settings.JavaScriptEnabled = true;

                _wv.Settings.AllowFileAccess = true;

                _wv.Settings.AllowContentAccess = true;

                tabLoaded = true;
            }

            return view;
        }


    }
}
