using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Classes;
using BitChute;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static StartServices.Servicesclass.ExtStickyService;

namespace BitChute.Fragments
{
    public class Tab1Frag : Fragment
    {
        string _title;
        string _icon;
        public static ServiceWebView Wv;
        readonly ExtWebViewClient _wvc = new ExtWebViewClient();
        public static string RootUrl = "https://bitchute.com/subscriptions/";
        public static int TNo = 1;
        public static bool FirstPageLoaded = false;
        public static Tab1Frag NewInstance(string title, string icon) {
            var fragment = new Tab1Frag();
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
            var _view = inflater.Inflate(Resource.Layout.Tab1FragLayout, container, false);
            Wv = (ServiceWebView)_view.FindViewById<ServiceWebView>(Resource.Id.webView2);
                Wv.SetWebViewClient(_wvc);
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                Wv.LoadUrl(RootUrl);
                Wv.Settings.JavaScriptEnabled = true;
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            //CustomSetTouchListener(AppState.Display.Horizontal);
            return _view;
        }
        public void CustomSetTouchListener(bool landscape)
        {
            if (landscape){ Wv.SetOnTouchListener(new ExtTouchListener());  }
            else { Wv.SetOnTouchListener(null); }
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
                if (_scrollY >= 4000){ExpandVideoCards(false); _scrollY = 0;}
            }
        }
        public void OnSettingsChanged(List<object> settings)
        {
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            else { Wv.Settings.BuiltInZoomControls = false; }
        }
        private static int _scrollY = 0;
        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack()) { Wv.GoBack(); }
        }
        static bool _wvRl = true;
        public void Pop2Root()
        {
            if (_wvRl) { Wv.Reload(); _wvRl = false; }
            else { Wv.LoadUrl(RootUrl); }
        }
        public void SetWebViewVis() {  Wv.Visibility = ViewStates.Visible; }
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
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings.LinkOverflowFixDelay);
            Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
            Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
            Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
        }

        public void LoadCustomUrl(string url) { Wv.LoadUrl(url); }

        public static async void HidePageTitle(int delay)
        {
            await Task.Delay(delay);
            if (Wv.Url != "https://www.bitchute.com/" && AppState.Display.Horizontal)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideTitle);
                Wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
                Wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                Wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
            }
        }
        public static async void HideWatchTab(int delay)
        {
            if (delay != 0) {  await Task.Delay(delay); }
            if (Wv.Url != "https://www.bitchute.com/") {  Wv.LoadUrl(JavascriptCommands._jsHideWatchTab); }
        }
        public static async void ExpandVideoCards(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(4000);
            }
            Wv.LoadUrl(JavascriptCommands._jsExpandSubs);
            Wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            Wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);
        }
        public static void StartVideo() { Wv.LoadUrl(JavascriptCommands._jsPlayVideo); }
        public static string GetHtmlText()
        {
            string innerHtml = "";
            return innerHtml;
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
                WebViewHelpers.DelayedScrollToTop(TNo);
                Wv.LoadUrl(JavascriptCommands._jsHideBanner);
                Wv.LoadUrl(JavascriptCommands._jsHideBuff);
                Wv.LoadUrl(JavascriptCommands._jsHideNavTabsList);
                if (AppState.Display.Horizontal){ HidePageTitle(5000); }
                SetReload();
                HideLinkOverflow();
                HideWatchTab(5000);
                ExpandVideoCards(true);
                Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
                Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
                Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
                ViewHelpers.AutoRestoreDisqusWithDelay(TNo);
                if (!FirstPageLoaded) { Wv.LoadUrl(JavascriptCommands.RemoveEnterButtonListener); FirstPageLoaded = true; }
                base.OnPageFinished(view, url);
                //view.LoadUrl(JavascriptCommands.ExpandSearchIcon());
            }
        }
    }
}
