﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using BitChute.Classes;
using static StartServices.Servicesclass.ExtStickyService;

namespace BitChute.Fragments
{
    //[Android.Runtime.Register("onKeyDown", "(ILandroid/view/KeyEvent;)Z", "GetOnKeyDown_ILandroid_view_KeyEvent_Handler")]
    public class TheFragment2 : Fragment
    {
        string _title;
        string _icon;

        public static ServiceWebView Wv;
        readonly ExtWebViewClient _wvc = new ExtWebViewClient();

        bool tabLoaded = false;
        public static int TNo = 2;

        public static string RootUrl = "https://www.bitchute.com/";

        public static TheFragment2 NewInstance(string title, string icon)
        {
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
            var _view = inflater.Inflate(Resource.Layout.TheFragmentLayout3, container, false);
            Wv = (ServiceWebView)_view.FindViewById<ServiceWebView>(Resource.Id.webView3);

                Wv.SetWebViewClient(_wvc);
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;

                Wv.Settings.JavaScriptEnabled = true;
                //_wv.Settings.AllowFileAccess = true;
                //_wv.Settings.AllowContentAccess = true;
                tabLoaded = true;
            
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            Wv.Settings.DisplayZoomControls = false;
            if (AppSettings.Browsing) { LoadUrlWithDelay(RootUrl, 2000); }
            CustomSetTouchListener(AppState.Display.Horizontal);
            //  _wv.SetOnScrollChangeListener(new ExtScrollListener());
            return _view;
        }

        public async void LoadUrlWithDelay(string url, int delay)
        {
            await Task.Delay(delay);
            Wv.LoadUrl(url);
        }

        public void OnSettingsChanged(List<object> settings)
        {
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            else
            {
                Wv.Settings.BuiltInZoomControls = false;
            }

            if (AppSettings.Tab3Hide)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideCarousel);
            }
            else
            {
                Wv.LoadUrl(JavascriptCommands._jsShowCarousel);
            }
        }

        public void CustomSetTouchListener(bool landscape)
        {
            if (landscape)
            {
                Wv.SetOnTouchListener(new ExtTouchListener());
            }
            else
            {
                Wv.SetOnTouchListener(null);
            }
        }

        public static MainActivity Main = MainActivity.Main;

        public class ExtTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                MainActivity.CustomOnTouch();
                CustomOnTouch();
                return false;
            }
        }

        private static int _scrollY = 0;

        //public class ExtScrollListener : Java.Lang.Object, View.IOnScrollChangeListener
        //{
        //    public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        //    {
        //        OnScrollChanged(scrollY);
        //    }
        //}

        //public static async void OnScrollChanged(int scrollY)
        //{
        //    await Task.Delay(60);
        //    _scrollY += scrollY;
        //    if (AppState.Display._horizontal)
        //    {
        //        await Task.Delay(100);
        //        if (_scrollY >= 3500)
        //        {
        //            ExpandVideoCards(false);
        //            _scrollY = 0;
        //        }
        //    }
        //}

        private static async void CustomOnTouch()
        {
            _scrollY += Wv.ScrollY;
            if (AppState.Display.Horizontal)
            {
                await Task.Delay(500);
                if (_scrollY >= 4000)
                {
                    ExpandVideoCards(false);
                    _scrollY = 0;
                }
            }
        }

        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack())
                Wv.GoBack();
        }

        static bool _wvRl = true;

        public void Pop2Root()
        {
            if (_wvRl)
            {
                Wv.Reload();
                _wvRl = false;
            }
            else
            {
                Wv.LoadUrl(@"https://www.bitchute.com/");
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
                await Task.Delay(AppSettings.TabDelay);
                _wvRl = true;
                _wvRling = false;
            }
        }
        
            /// <summary>
            /// hides the link overflow
            /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings.LinkOverflowFixDelay);
            Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
            Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
            Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
        }

        public void LoadCustomUrl(string url)
        {
            Wv.LoadUrl(url);
        }

        public static async void HidePageTitle(int delay)
        {
            if (delay != 0)
            {
                await Task.Delay(delay);
            }

            if (Wv.Url != "https://www.bitchute.com/" && AppState.Display.Horizontal)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideTitle);
                Wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
                Wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                Wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
            }
        }

        private static async void HideWatchLabel(int delay)
        {
            await Task.Delay(delay);
            Wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }
        
        public static async void ExpandVideoCards(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(5000);
            }
            Wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            Wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);
        }

        public static async void SelectSubscribedTab(int delay)
        {
            await Task.Delay(delay);
            Wv.LoadUrl(JavascriptCommands._jsSelectSubscribed);
        }
        
        public class ExtWebViewClient : WebViewClient
        {

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().Contains("pest."))
                {
                    WebResourceResponse w = new WebResourceResponse("text/css", "UTF-8", null);
                    return w;
                }
                return base.ShouldInterceptRequest(view, request);
            }
            public override void OnPageFinished(WebView view, string url)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideBanner);
                Wv.LoadUrl(JavascriptCommands._jsHideBuff);

                WebViewHelpers.DelayedScrollToTop(TNo);
                if (url != "https://www.bitchute.com/")
                {
                    HideWatchLabel(2000);
                }

                if (AppSettings.Tab3Hide)
                {
                    Wv.LoadUrl(JavascriptCommands._jsHideCarousel);

                    if (Wv.Url == "https://www.bitchute.com/")
                    {
                        TheFragment2.SelectSubscribedTab(2000);
                    }
                }

                SelectSubscribedTab(4000);

                if (AppState.Display.Horizontal)
                {
                    HidePageTitle(5000);
                }

                Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
                Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
                Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
                SetReload();
                HideLinkOverflow();
                ExpandVideoCards(true);
                //_wv.LoadUrl(JavascriptCommands._jsFillAvailable);
                //AdBlock.RemoveDiscusIFrame(TNo);
                base.OnPageFinished(view, url);

            }
        }
    }
}
