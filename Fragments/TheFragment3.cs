using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BottomNavigationViewPager.Classes;
using static Android.Views.View;

namespace BottomNavigationViewPager.Fragments
{
    //[Android.Runtime.Register("onKeyDown", "(ILandroid/view/KeyEvent;)Z", "GetOnKeyDown_ILandroid_view_KeyEvent_Handler")]
    public class TheFragment3 : Fragment
    {
        string _title;
        string _icon;

        protected static WebView _wv;

        bool tabLoaded = false;

        public static string _url = "https://bitchute.com";

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
            var _view = inflater.Inflate(Resource.Layout.TheFragmentLayout3, container, false);

            _wv = _view.FindViewById<WebView>(Resource.Id.webView3);

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

            _wv.SetOnScrollChangeListener(new ExtScrollListener());

            return _view;
        }

        public void OnSettingsChanged(List<object> settings)
        {
            _wv.Settings.SetSupportZoom(Convert.ToBoolean(settings[0]));

            if (Convert.ToBoolean(settings[3]))
            {
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideCarousel);

            }
            else
            {
                _wv.LoadUrl(Globals.JavascriptCommands._jsShowCarousel);
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

            if (TheFragment5._tab3Hide)
            {
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideCarousel);

                _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab1);

                _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab2);

                _wv.LoadUrl(Globals.JavascriptCommands._jsSelectTab3);

                _wv.LoadUrl(Globals.JavascriptCommands._jsHideTrending);

                //_wv.LoadUrl(Globals.JavascriptCommands._jsHideLabel);
            }
        }

        public static MainActivity _main = new MainActivity();

        public class ExtScrollListener : Java.Lang.Object, View.IOnScrollChangeListener
        {
            public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
            {
                _main.CustomOnScroll();
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
                _wv.LoadUrl(@"https://www.bitchute.com/");
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

        //public static bool _showMoreTimeout = false;

        //public void ShowMore()
        //{
        //    if (!_showMoreTimeout)
        //    {
        //        _showMoreTimeout = true;
        //        System.Threading.Thread.Sleep(5000);
        //        _wv.LoadUrl(Globals.JavascriptCommands._jqShowMore);

        //        _showMoreTimeout = false;

        //    }
        //}

        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(Globals.AppSettings._linkOverflowFixDelay);

            _wv.LoadUrl(Globals.JavascriptCommands._jsLinkFixer);
        }


        public void LoadCustomUrl(string url)
        {
            _wv.LoadUrl(url);
        }

        public class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView view, string url)
            {

                base.OnPageFinished(view, url);

                _wv.LoadUrl(Globals.JavascriptCommands._jsHideBanner);

                _wv.LoadUrl(Globals.JavascriptCommands._jsHideBuff);

                if (TheFragment5._tab3Hide)
                {
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideCarousel);

                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab1);

                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab2);

                    _wv.LoadUrl(Globals.JavascriptCommands._jsSelectTab3);

                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideTrending);

                    //_wv.LoadUrl(Globals.JavascriptCommands._jsHideLabel);
                }
                _wv.LoadUrl(Globals.JavascriptCommands._jsLinkFixer);

                SetReload();
                
                HideLinkOverflow();
            }
        }
    }
}
