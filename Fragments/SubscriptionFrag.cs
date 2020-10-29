using Android.OS;
using Android.Views;
using Android.Webkit;
using BitChute;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BitChute.Services.MainPlaybackSticky;
using BitChute.Web;
using static BitChute.ViewHelpers.Tab1;
using static BitChute.Web.ViewClients;

namespace BitChute.Fragments
{
    public class SubscriptionFrag : CommonFrag
    {
        string _title;
        string _icon;
        public static ServiceWebView Wv;
        public static object WebViewClient;
        public static int TNo = 1;

        public static SubscriptionFrag NewInstance(string title, string icon, string rootUrl = null)
        {
            //if (AppSettings.UserWasLoggedInLastAppClose) { WebViewClient = new Subs(); }
            //else { WebViewClient = new LoginWebViewClient(); }
            WebViewClient = new Subs();
            var fragment = new SubscriptionFrag();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            if (rootUrl == null) rootUrl = "https://www.bitchute.com/subscriptions/";
            fragment.RootUrl = rootUrl;
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
            try
            {

                if (FragmentContainerLayout == null)
                { FragmentContainerLayout = inflater.Inflate(Resource.Layout.Tab1FragLayout, container, false); }

                Wv = FragmentContainerLayout.FindViewById<ServiceWebView>(Resource.Id.webView1);
                if (WebViewClient.GetType() == typeof(LoginWebViewClient)) { Wv.SetWebViewClient((LoginWebViewClient)WebViewClient); }
                else  { Wv.SetWebViewClient((Subs)WebViewClient); }
                Wv.RootUrl = RootUrl;
                Wv.Settings.JavaScriptEnabled = true;
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                Wv.Settings.DisplayZoomControls = false;
                if (AppSettings.ZoomControl)
                {
                    Wv.Settings.BuiltInZoomControls = true;
                    Wv.Settings.DisplayZoomControls = false;
                }
                GetFragmentById(this.Id, this);
                return FragmentContainerLayout;
            }
            catch { }
            return null;
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


        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack()) Wv.GoBack();
            BitChute.Web.ViewClients.RunBaseCommands(Wv, 2000);
        }


        public static bool WvRl = true;
        public void Pop2Root()
        {
            if (WvRl) { Wv.Reload(); WvRl = false; }
            else { Wv.LoadUrl(RootUrl); }
        }

        public void SetWebViewVis() { Wv.Visibility = ViewStates.Visible; }
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
        
        public static async void HideWatchTab(int delay)
        {
            if (delay != 0) { await Task.Delay(delay); }
            if (Wv.Url != "https://www.bitchute.com/") { Wv.LoadUrl(JavascriptCommands._jsHideWatchTab); }
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

        private class ExtWebViewClient : Android.Webkit.WebViewClient
        {

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (AppSettings.SearchFeatureOverride && !SearchOverride.SearchOverrideInProg)
                {
                    if (!request.Url.ToString().Contains(@"https://www.bitchute.com/search?q="))
                    { //Return immediately to optimize the ux
                        return base.ShouldInterceptRequest(view, request);
                    }
                    if (request.Url.ToString().Contains(@"https://www.bitchute.com/search?q="))
                    {
                        SearchOverride.SearchOverrideInProg = true;
                        MainActivity.Main.RunOnUiThread(() => { Wv.StopLoading(); });
                        var ro = SearchOverride.ReRouteSearch(request.Url.ToString());
                        SearchOverride.UI.WvSearchOverride(view, ro);
                        WebResourceResponse w = new WebResourceResponse("text/css", "UTF-8", null);
                        return w;
                    }
                }
                return base.ShouldInterceptRequest(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                WebViewHelpers.DelayedScrollToTop(TNo);
                Wv.LoadUrl(JavascriptCommands._jsHideBanner);
                Wv.LoadUrl(JavascriptCommands._jsHideBuff);
                Wv.LoadUrl(JavascriptCommands._jsHideNavTabsList);
                //if (AppState.Display.Horizontal) { HidePageTitle(5000); }
                SetReload();
                HideLinkOverflow();
                HideWatchTab(5000);
                ExpandVideoCards(true);
                Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
                Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
                Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
                base.OnPageFinished(view, url);
            }
        }
    }
}
