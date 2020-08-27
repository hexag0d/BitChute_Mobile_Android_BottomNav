using Android;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Classes;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Android.Views.View;
using static BitChute.Fragments.Tab4Frag;
using static StartServices.Servicesclass.ExtStickyService;

namespace BitChute.Fragments
{
    [Android.Runtime.Register("onWindowVisibilityChanged", "(I)V", "GetOnWindowVisibilityChanged_IHandler")]
    public class Tab0Frag : Fragment
    {
        string _title;
        string _icon;
        public static ServiceWebView Wv;
        protected static View _view;
        public static string RootUrl = "https://www.bitchute.com/";
        public static string LastLoadedUrl = "";
        readonly ExtWebViewClient Wvc = new ExtWebViewClient();
        private static ExtNotifications _extNotifications = MainActivity.notifications;
        private static ExtWebInterface _extWebInterface = MainActivity.ExtWebInterface;
        public static int TNo = 0;
        public static Tab0Frag NewInstance(string title, string icon) {
            var fragment = new Tab0Frag();
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
            _view = inflater.Inflate(Resource.Layout.Tab0FragLayout, container, false);
            Wv = _view.FindViewById<ServiceWebView>(Resource.Id.webView1);
            Wv.SetWebViewClient(Wvc);
            if (AppState.NotificationStartedApp){ SetAutoPlayWithDelay(1); }
            Wv.Settings.JavaScriptEnabled = true;
            Wv.Settings.DisplayZoomControls = false;
            Wv.LoadUrl(RootUrl);
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            //CustomSetTouchListener(AppState.Display.Horizontal);
            return _view;
        }
        public static async void SetAutoPlayWithDelay(int delay)
        {
            await Task.Delay(delay);
            Wv.Settings.MediaPlaybackRequiresUserGesture = false;
        }
        public static void SwapView(View v){}
        public override void OnResume()
        {
            base.OnResume();
        }
        /// <summary>
        /// sets the touch listener when device is in landscape mode
        /// sets it to null when the device goes back into portrait mode
        /// </summary>
        /// <param name="landscape"></param>
        public void CustomSetTouchListener(bool landscape)
        {
            if (landscape){Wv.SetOnTouchListener(new ExtTouchListener());}
            else{ Wv.SetOnTouchListener(null);}
        }
        public void OnSettingsChanged(List<object> settings)
        {
            Wv.Settings.SetSupportZoom(AppSettings.ZoomControl);
            if(!AppSettings.Tab1FeaturedOn){Wv.LoadUrl(JavascriptCommands._jsHideCarousel);}
            else { Wv.LoadUrl(JavascriptCommands._jsShowCarousel);}
            if(AppSettings.ZoomControl){Wv.Settings.BuiltInZoomControls = true;}
            else { Wv.Settings.BuiltInZoomControls = false;}
        }
        public class ExtTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                MainActivity.CustomOnTouch();
                CustomOnTouch();
                return false;
            }
        }
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
        private static int _scrollY = 0;
        /// <summary>
        /// tells the webview to GoBack, if it can
        /// </summary>
        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack())
                Wv.GoBack();
        }
        static bool WvRl = true;
        /// <summary>
        /// one press refreshes the page; two presses pops back to the root
        /// </summary>
        public void Pop2Root()
        {
            if (WvRl)
            {
                Wv.Reload();
                WvRl = false;
            }
            else { Wv.LoadUrl(@"https://bitchute.com/"); }
        }
        public static bool WvRling = false;
        /// <summary>
        /// this is to allow faster phones and connections the ability to Pop2Root
        /// used to be set without delay inside OnPageFinished but I don't think 
        /// that would work on faster phones
        /// </summary>
        public static async void SetReload()
        {
            if (!WvRling)
            {
                WvRling = true;
                await Task.Delay(AppSettings.TabDelay);
                WvRl = true;
                WvRling = false;
            }
        }
        static int _autoInt = 0;
        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings.LinkOverflowFixDelay);
            Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
            Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
            Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
        }
        public void LoadCustomUrl(string url){ Wv.LoadUrl(url); }
        public static async void HidePageTitle()
        {
            await Task.Delay(5000);

            if (Wv.Url != "https://www.bitchute.com/" && AppState.Display.Horizontal)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideTitle);
                Wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
                Wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                Wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
            }
        }
        private static async void HideWatchLabel()
        {
            await Task.Delay(4000);
            if (Wv.Url != "https://www.bitchute.com/")
            Wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }
        public void SetWebViewVis(){ Wv.Visibility = ViewStates.Visible; }
        public static async void ExpandVideoCards(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(5000);
            }
            Wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            Wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);
        }
        private static async void ExpandFeaturedChannels(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(3000);
            }
            Wv.LoadUrl(JavascriptCommands._jsFeaturedRemoveMaxWidth);
            Wv.LoadUrl(JavascriptCommands._jsExpandFeatured);
        }
        private static async void ExpandPage(bool delayed)
        {
            if (delayed){await Task.Delay(3000);}
        }




        private class ExtWebViewClient : WebViewClient
        {
            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (!request.Url.ToString().Contains("pest.") && !request.Url.ToString().Contains(@"https://www.bitchute.com/search?q="))
                { //Return immediately to optimize the ux
                    return base.ShouldInterceptRequest(view, request);
                }
                if (request.Url.ToString().Contains("pest.")) // this blocks the crazy big ads, which make it hard to read the comments; if they had kept them smaller, this wouldn't be necessary
                {
                    WebResourceResponse w = new WebResourceResponse("text/css", "UTF-8", null); return w;
                }
                if (AppSettings.SearchFeatureOverride && !SearchOverride.SearchOverrideInProg && request.Url.ToString().Contains(@"https://www.bitchute.com/search?q="))
                {
                    SearchOverride.SearchOverrideInProg = true;
                    MainActivity.Main.RunOnUiThread(() => { Wv.StopLoading(); });
                    var ro = SearchOverride.ReRouteSearch(request.Url.ToString());
                    SearchOverride.UI.WvSearchOverride(view, ro);
                    WebResourceResponse w = new WebResourceResponse("text/css", "UTF-8", null);
                    return w;
                }
                return base.ShouldInterceptRequest(view, request);
            }
            
            public override void OnPageFinished(WebView view, string url)
            {
                //Wv.LoadUrl(JavascriptCommands.ExpandSearchIcon());
                _autoInt++;
                if (_autoInt == 1 || AppState.NotificationStartedApp)
                {
                    Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                }
                WebViewHelpers.DelayedScrollToTop(TNo);
                Wv.LoadUrl(JavascriptCommands._jsHideBanner);
                Wv.LoadUrl(JavascriptCommands._jsHideBuff);
                HideWatchLabel();
                if (!AppSettings.Tab1FeaturedOn)
                {
                    Wv.LoadUrl(JavascriptCommands._jsHideCarousel);
                }
                if (AppState.Display.Horizontal)
                {
                    if (url != "https://www.bitchute.com/")
                    {
                        Wv.LoadUrl(JavascriptCommands._jsHideTitle);
                        Wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
                        Wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                        Wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
                    }
                    HidePageTitle();
                }
                Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
                SetReload();
                HideLinkOverflow();
                ExpandFeaturedChannels(true);
                ExpandVideoCards(true);
                Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
                Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
                ViewHelpers.AutoRestoreDisqusWithDelay(TNo);
                base.OnPageFinished(view, url);
            }
        }
    }
}
