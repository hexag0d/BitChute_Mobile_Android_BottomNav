using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BottomNavigationViewPager.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public string _url = "https://bitchute.com/";

        readonly ExtWebViewClient _wvc = new ExtWebViewClient();

        bool tabLoaded = false;

        //static MainActivity _main = new MainActivity();

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
            _view = inflater.Inflate(Resource.Layout.TheFragmentLayout1, container, false);

            _wv = _view.FindViewById<WebView>(Resource.Id.webView1);

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(_wvc);

                _wv.Settings.JavaScriptEnabled = true;

                _wv.Settings.DisplayZoomControls = false;

                //_wv.Settings.AllowFileAccess = true;

                //_wv.Settings.AllowContentAccess = true;

                //this didn't work when I put it here.  strange.. it would disable the setting on 
                //every other tab
               // _wv.Settings.MediaPlaybackRequiresUserGesture = false;

                _wv.LoadUrl(_url);

                tabLoaded = true;
            }

            _wv.SetOnScrollChangeListener(new ExtScrollListener());
            //_wv.Touch += ViewOnTouch;

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
            }
            else
            {
                _wv.Settings.BuiltInZoomControls = false;
            }
        }

        /// <summary>
        /// gotta instantiate that MainActivity _maing
        ///compiler is all about that
        /// </summary>
        public static MainActivity _main = new MainActivity();

        //public bool OnTouch(object sender, MotionEvent e)
        //{
        //    return false;
        //}

        //private void ViewOnTouch(object sender, View.TouchEventArgs touchEventArgs)
        //{
        //    var test = "yo";

        //   // _wv.ComputeScroll();
        //    //Globals._wvHeight = _wv.ContentHeight;

        //    //string message;
        //    switch (touchEventArgs.Event.Action & MotionEventActions.Mask)
        //    {

        //        case MotionEventActions.Down:
        //        case MotionEventActions.Move:
        //            _main.CustomOnScroll();
        //            break;

        //        case MotionEventActions.Up:
        //            //_main.HideNavBarAfterDelay();
        //            break;

        //        default:
        //            break;
        //    }

            
        //}

        public class ExtScrollListener : Java.Lang.Object, View.IOnScrollChangeListener
        {
            public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
            {
                _main.CustomOnScroll();
            }
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

        //I'll explain this later
        static int _autoInt = 0;

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

        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView _view, string url)
            {
                HideLinkOverflow();

                if (!TheFragment5._tab1FeaturedOn)
                {
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideCarousel);
                }
                
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideBanner);

                _wv.LoadUrl(Globals.JavascriptCommands._jsHideBuff);

                base.OnPageFinished(_view, url);
                //string _jsHideBannerC = "javascript:(function() { " +
                //   "document.getElementsByClassName('logo-wrap--home').style.display='none'; " + "})()";


                //add one to the autoint... for some reason if Tab1 has 
                //_wv.Settings.MediaPlaybackRequiresUserGesture = false; set then it won't work on the other tabs
                //this is a workaround for that glitch
                _autoInt++;

                // if autoInt is 1 then we will set the MediaPlaybackRequiresUserGesture
                //strange.. i know.. but it works
                if (_autoInt == 1)
                {
                    _wv.Settings.MediaPlaybackRequiresUserGesture = false;
                }
                _wv.LoadUrl(Globals.JavascriptCommands._jsLinkFixer);

                SetReload();
            }
        }
    }
}
