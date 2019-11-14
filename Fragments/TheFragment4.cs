using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BottomNavigationViewPager.Classes;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static StartServices.Servicesclass.ExtStickyService;

namespace BottomNavigationViewPager.Fragments
{
    public class TheFragment4 : Fragment
    {
        string _title;
        string _icon;

        protected static ServiceWebView _wv;
        readonly ExtWebViewClient _wvc = new ExtWebViewClient();

        public static string _url = "https://www.bitchute.com/profile";
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
                _wv.LoadUrl(_url);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _view = inflater.Inflate(Resource.Layout.TheFragmentLayout4, container, false);

            _wv = (ServiceWebView)_view.FindViewById<ServiceWebView>(Resource.Id.webView4);

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(new ExtWebViewClient());
                _wv.Settings.MediaPlaybackRequiresUserGesture = false;
                _wv.Settings.DisplayZoomControls = false;
                _wv.LoadUrl(_url);
                _wv.Settings.JavaScriptEnabled = true;
                //_wv.Settings.AllowFileAccess = true;
                //_wv.Settings.AllowContentAccess = true;
                tabLoaded = true;
            }
            _wv.SetOnTouchListener(new ExtTouchListener());
            _wv.SetOnScrollChangeListener(new ExtScrollListener());
            return _view;
        }

        public static MainActivity _main = new MainActivity();

        public class ExtTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                _main.CustomOnTouch();
                return false;
            }
        }
        private static int _scrollY = 0;

        public class ExtScrollListener : Java.Lang.Object, View.IOnScrollChangeListener
        {
            public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
            {
                OnScrollChanged(scrollY);
            }
        }

        public static async void OnScrollChanged(int scrollY)
        {
            await Task.Delay(60);
            _scrollY += scrollY;
            if (Globals.AppState.Display._horizontal)
            {
                await Task.Delay(500);
                if (_scrollY >= 4000)
                {
                    ExpandVideoCards(false);
                    _scrollY = 0;
                }
            }
        }

        public static bool _wvRling = false;

        /// <summary>
        /// this is to allow faster phones and connections the ability to Pop2Root
        /// used to be set without delay inside OnPageFinished but I don't think 
        /// that would work on faster phones
        /// </summary>
        public static async void SetReload()
        {
            if (!_wvRling)
            {
                _wvRling = true;
                await Task.Delay(Globals.AppSettings._tabDelay);
                _wvRl = true;
                _wvRling = false;
            }
        }

        public void OnSettingsChanged(List<object> settings)
        {
            _wv.Settings.SetSupportZoom(Convert.ToBoolean(settings[0]));

            if (Convert.ToBoolean(settings[3]))
            {
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideCarousel);
            }
            if (TheFragment5._fanMode)
            {
            }

            if (TheFragment5._zoomControl)
            {
                _wv.Settings.BuiltInZoomControls = true;
                _wv.Settings.DisplayZoomControls = false;
            }
            else
            {
                _wv.Settings.BuiltInZoomControls = false;
            }

            if (TheFragment5._tab4OverridePreference == "feed" && TheFragment5._tab3Hide)
            {
                if (TheFragment5._tab3Hide)
                {
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideCarousel);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab1);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab2);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsSelectTab3);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideLabel);
                }
            }
        }

        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(Globals.AppSettings._linkOverflowFixDelay);

            _wv.LoadUrl(Globals.JavascriptCommands._jsLinkFixer);
            _wv.LoadUrl(Globals.JavascriptCommands._jsDisableTooltips);
        }
        
        public void LoadCustomUrl(string url)
        {
            _wv.LoadUrl(url);
        }

        public static async void HidePageTitle(int delay)
        {
            await Task.Delay(delay);

            if (_wv.Url != "https://www.bitchute.com/" && Globals.AppState.Display._horizontal)
            {
                _wv.LoadUrl(Globals.JavascriptCommands._jsHidePageBar);
                _wv.LoadUrl(Globals.JavascriptCommands._jsPageBarDelete);
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideTitle);
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideWatchTab);
            }
        }

        public void SetWebViewVis()
        {
            _wv.Visibility = ViewStates.Visible;
        }

        public static async void ExpandVideoCards(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(5000);
            }
            _wv.LoadUrl(Globals.JavascriptCommands._jsBorderBoxAll);
            _wv.LoadUrl(Globals.JavascriptCommands._jsRemoveMaxWidthAll);
        }

        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView view, string url)
            {
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideBanner);
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideBuff);

                if (Globals._t4Is == "Feed")
                {
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideCarousel);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab1);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab2);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsSelectTab3);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideTrending);
                    //_wv.LoadUrl(Globals.JavascriptCommands._jsHideLabel);
                }

                if (!TheFragment5._tab1FeaturedOn)
                {
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideCarousel);
                }

                if (Globals.AppState.Display._horizontal)
                {
                    HidePageTitle(5000);
                }
                
                _wv.LoadUrl(Globals.JavascriptCommands._jsLinkFixer);
                HideLinkOverflow();
                SetReload();
                base.OnPageFinished(view, url);
            }
        }
    }
}
