using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Webkit;
using System.IO;
using Android.Graphics;
using System.Threading.Tasks;
using BitChute;
using BitChute.Web.Ui;
using System.Net;
using BitChute.Fragments;
using static BitChute.JavascriptCommands;
using static BitChute.PlaystateManagement;
using static BitChute.Services.MainPlaybackSticky;
using System.Net.Http;
using BitChute.Web.Auth;

namespace BitChute.Web
{
    public class ViewClients
    {
        public delegate void PlaystateEventDelegate(PlaystateEventArgs _args);
        public static event PlaystateEventDelegate PlaystateChanged;
        public delegate void LoginEventDelegate(LoginEventArgs args);
        public static event LoginEventDelegate OnLogin;
        public delegate void LogoutEventDelegate(LogoutEventArgs args);
        public static event LogoutEventDelegate OnLogout;
        public static bool IsObtainingResourceFromWebView = false;

        public static async void LoadInitialUrls(int delay = 400)
        {
            while (!CssHelper.CustomCssReadyForRead)
            {
                await Task.Delay(delay);
            }

            if (AppSettings.FirstTimeAppLoad)
            {
                CookieManager.Instance.SetAcceptCookie(true);
                CookieManager.Instance.SetCookie("https://www.bitchute.com/", "preferences=" + @"{%22theme%22:%22night%22%2C%22autoplay%22:true}");
                HomePageFrag.Wv.LoadUrl(MainActivity.Fm0.RootUrl);
                await Task.Delay(200);
                SubscriptionFrag.Wv.LoadUrl(MainActivity.Fm1.RootUrl);
                await Task.Delay(200);
                FeedFrag.Wv.LoadUrl(MainActivity.Fm2.RootUrl);
                await Task.Delay(200);
                MyChannelFrag.Wv.LoadUrl(MainActivity.Fm3.RootUrl);
                await Task.Delay(200);
                SettingsFrag.Wv.LoadUrl(MainActivity.Fm4.RootUrl);
                AppSettings.FirstTimeAppLoad = false;
            }
            else
            {
                HomePageFrag.Wv.LoadUrl(MainActivity.Fm0.RootUrl);
                await Task.Delay(200);
                SubscriptionFrag.Wv.LoadUrl(MainActivity.Fm1.RootUrl);
                await Task.Delay(200);
                FeedFrag.Wv.LoadUrl(MainActivity.Fm2.RootUrl);
                await Task.Delay(200);
                MyChannelFrag.Wv.LoadUrl(MainActivity.Fm3.RootUrl);
                await Task.Delay(200);
                SettingsFrag.Wv.LoadUrl(MainActivity.Fm4.RootUrl);
            }

            if (AppState.NotificationStartedApp)
            {
                MainActivity.Fm0.RootUrl = "https://www.bitchute.com/";
                AppState.NotificationStartedApp = false;
            }
            
        }

        public static void SetWebViewClientFromObject(ServiceWebView wv, object webViewClient)
        {
            wv.SetWebViewClient(null);
            var type = webViewClient.GetType();
            if (type == typeof(Home)) wv.SetWebViewClient((Home)webViewClient);
            else if (type == typeof(Subs)) wv.SetWebViewClient((Subs)webViewClient);
            else if (type == typeof(Feed)) wv.SetWebViewClient((Feed)webViewClient);
            else if (type == typeof(MyChannel)) wv.SetWebViewClient((MyChannel)webViewClient);
            else if (type == typeof(Settings)) wv.SetWebViewClient((Settings)webViewClient);
            else { wv.SetWebViewClient((Subs)webViewClient); }
        }


        public static async void SetReload(int tab = -1)
        {
            if (tab == -1) tab = MainActivity.ViewPager.CurrentItem;
            switch (tab)
            {
                case 0: if (!HomePageFrag.WvRling) { HomePageFrag.WvRling = true; await Task.Delay(AppSettings.TabDelay); HomePageFrag.WvRl = true; HomePageFrag.WvRling = false; }
                    break;
                case 1:
                    if (!SubscriptionFrag.WvRling) { SubscriptionFrag.WvRling = true; await Task.Delay(AppSettings.TabDelay); SubscriptionFrag.WvRl = true; SubscriptionFrag.WvRling = false; }
                    break;
                case 2:
                    if (!FeedFrag.WvRling) { FeedFrag.WvRling = true; await Task.Delay(AppSettings.TabDelay); FeedFrag.WvRl = true; FeedFrag.WvRling = false; }
                    break;
                case 3:
                    if (!MyChannelFrag.WvRling) { MyChannelFrag.WvRling = true; await Task.Delay(AppSettings.TabDelay); MyChannelFrag.WvRl = true; MyChannelFrag.WvRling = false; }
                    break;
                case 4:
                    if (!SettingsFrag.WvRling) { SettingsFrag.WvRling = true; await Task.Delay(AppSettings.TabDelay); SettingsFrag.WvRl = true; SettingsFrag.WvRling = false; }
                    break;
            }
        }

        public static async void RunAfterDelay(WebView wv, string command, int delay = 10)
        {
            await Task.Delay(delay);
            MainActivity.Main.RunOnUiThread(() => { wv.LoadUrl(command); });
        }
        
        public static async void HidePageTitle(WebView w, int delay = 5000)
        {
            await Task.Delay(delay);
            if (w.Url != "https://www.bitchute.com/" && AppState.Display.Horizontal)
            {
                w.LoadUrl(JavascriptCommands._jsHideTitle);
                w.LoadUrl(JavascriptCommands._jsHidePageBar);
                w.LoadUrl(JavascriptCommands._jsPageBarDelete);
            }
        }

        public static void ReRouteToAppPlaystate(string url, int playerNumber = -1)
        {
            switch (url)
            {
                case @"https://_%26app_play_invoked/":
                    PlaystateChanged.Invoke(new PlaystateEventArgs(playerNumber, true));
                    break;
                case @"https://_%26app_pause_invoked/":
                    PlaystateChanged.Invoke(new PlaystateEventArgs(playerNumber, false, true));
                    break;
                case @"https://_%26app_play_isplaying/":
                    PlaystateChanged.Invoke(new PlaystateEventArgs(playerNumber, false, false, true));
                    break;
            }
        }

        public static void OnPlaystateChanged(PlaystateEventArgs e)
        {

        }

        public static void Run_OnLoginFailure()
        {
            foreach (var frag in CommonFrag.FragmentDictionary.Values)
            {
                if (frag.LoginErrorTextView != null)
                {
                    frag.LoginErrorTextView.Visibility = Android.Views.ViewStates.Visible;
                }
            }
        }

        public static bool WebViewsAreReloadingAfterLogin = false;
        public static List<int> WebViewIdLoginList = new List<int>();

        public static void Run_OnLogin()
        {
            WebViewsAreReloadingAfterLogin = true;
            foreach (ServiceWebView view in PlaystateManagement.WebViewTabDictionary.Values)
            {
                if (PlaystateManagement.WebViewPlayerIsStreaming)
                {
                    if (PlaystateManagement.WebViewPlayerNumberIsStreaming != view.Id)
                    {
                        try
                        {
                            view.ClearCache(true);
                            view.LoadUrl(view.RootUrl);
                        }
                        catch { }
                    }
                    else
                    {
                        view.ClearCache(false);
                    }
                }
                else
                {
                    try
                    {
                        view.ClearCache(true);
                        if (view.Url.Contains("/accounts/login/"))
                        {
                            WebViewIdLoginList.Add(view.Id);
                            //view.LoadUrl("file:///android_asset/html/blank.html");
                            view.Visibility = Android.Views.ViewStates.Gone;
                        }
                        view.LoadUrl(view.RootUrl);
                    }
                    catch { }
                }
            }
            MainActivity.Fm0.SwapLoginView(true);
            MainActivity.Fm1.SwapLoginView(true);
            MainActivity.Fm2.SwapLoginView(true);
            MainActivity.Fm3.SwapLoginView(true);
            MainActivity.Fm4.SwapLoginView(true);

            AppState.UserIsLoggedIn = true;
        }

        public static void RunOnLogout(LogoutEventArgs e)
        {
            AppSettings.SessionState.SessionId = "";
        }

        public static List<int> WindowLoadEventSet = new List<int>();
        
        public static async void RunBaseCommands(WebView w, int d = 2000)
        {
            //w.LoadUrl(GetInjectable(JavascriptCommands.CallBackInjection.DocumentReady));
            w.LoadUrl(GetInjectable(CallBackInjection.OnWindowDocumentLoadEnd));
            //w.LoadUrl(GetInjectable(CallBackInjection.GetWindowDocumentEvents));

            await Task.Delay(d);
            HidePageTitle(w, AppSettings.HidePageTitleDelay);

            w.LoadUrl(GetInjectable(JsDisableToolTips+JsHideTooltips+JsLinkFixer));
           // w.LoadUrl(GetInjectable(JavascriptCommands.CallBackInjection.DocumentReady));
            if (WebViewsAreReloadingAfterLogin)
            {
                if (WebViewIdLoginList.Contains(w.Id))
                {
                    w.Visibility = Android.Views.ViewStates.Visible;
                    WebViewIdLoginList.Remove(w.Id);
                }
                if (WebViewIdLoginList.Count == 0)
                {
                    WebViewsAreReloadingAfterLogin = false;
                }
            }
            w.LoadUrl(GetInjectable(
                CallBackInjection.IsPlayingCallback +
                CallBackInjection.PlayPauseButtonCallback) ); 
            //w.LoadUrl(GetInjectable(CallBackInjection.OnWindowDocumentLoadEnd));
        }

        public static void WebViewGoBack(WebView wv)
        {
            if (wv.CanGoBack()) wv.GoBack();
            BitChute.Web.ViewClients.RunBaseCommands(wv, 2000);
        }

        public static void RunPostLoginSuccess(int viewCalled, string cookieString = null)
        {

            foreach (ServiceWebView view in PlaystateManagement.WebViewTabDictionary.Values)
            {
                if (view.Id != viewCalled && view.RootUrl != null)
                {
                    view.ClearCache(false);
                    view.LoadUrl(view.RootUrl);
                }
            }
            MainActivity.Fm0.SwapLoginView(true);
            MainActivity.Fm1.SwapLoginView(true);
            MainActivity.Fm2.SwapLoginView(true);
            MainActivity.Fm3.SwapLoginView(true);
            MainActivity.Fm4.SwapLoginView(true);
            AppState.UserIsLoggedIn = true;
        }

        public class BaseWebViewClient : WebViewClient //WebViewClient shared between all applicable tabs
        {
            public BaseWebViewClient()
            {
                if (PlaystateChanged == null) PlaystateChanged += OnPlaystateChanged;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().StartsWith(@"https://_"))
                {
                    //check if we're currently on a bitchute page and otherwise don't reroute
                    if (view.Url.StartsWith("https://www.bitchute.com") || view.Url.StartsWith("https://bitchute.com"))
                    {
                        ReRouteToAppPlaystate(request.Url.ToString(), view.Id);
                    }
                    request = null;
                    return true;
                }
                return base.ShouldOverrideUrlLoading(view, request);
            }
            
            public override void OnPageFinished(WebView view, string url)
            {
                RunBaseCommands(view);
                base.OnPageFinished(view, url);
                
            }
        }

        //more specialized webview overrides

        public class Home : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                if (w.Url.Contains("bitchute.com/channel/") || w.Url == "https://www.bitchute.com/")
                { w.LoadUrl(JavascriptCommands.GetInjectable(JavascriptCommands.Display.ShowTabScrollInner + JavascriptCommands.Display.GetForAllTabs())); }
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().EndsWith($"common.css"))
                {
                    return CssHelper.GetCssResponse(CssHelper.CommonCss);
                }
                return base.ShouldInterceptRequest(view, request);
            }
            
            public override void OnPageFinished(WebView view, string url)
            {
                RunPageCommands(view);
                base.OnPageFinished(view, url);
            }
        }

        public class Subs : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                if (w.Url.Contains("bitchute.com/channel/") || w.Url == "https://www.bitchute.com/")
                { w.LoadUrl(JavascriptCommands.GetInjectable(JavascriptCommands.Display.ShowTabScrollInner + JavascriptCommands.Display.GetForAllTabs())); }
                HidePageTitle(w, AppSettings.HidePageTitleDelay);
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().EndsWith("common.css"))
                {
                    return CssHelper.GetCssResponse(CssHelper.CommonCss);

                }

                return base.ShouldInterceptRequest(view, request);
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().EndsWith("=/subscriptions/"))
                {
                    if (request.Url.ToString() == "https://www.bitchute.com/accounts/login/?next=/subscriptions/")
                    {
                        MainActivity.Fm1.SwapLoginView(false, false, true);
                    }
                }
                return base.ShouldOverrideUrlLoading(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                RunPageCommands(view);
                base.OnPageFinished(view, url);

            }
        }

        public class Feed : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                if (w.Url.Contains("bitchute.com/channel/") || w.Url == "https://www.bitchute.com/")
                {
                    if (w.Url == "https://www.bitchute.com/" && AppState.UserIsLoggedIn)
                    {
                        w.LoadUrl(GetInjectable(Display.ShowTabScrollInner + Display.GetForAllTabs()));
                        w.LoadUrl(_jsSelectSubscribed);
                    }
                    if (w.Url == "https://www.bitchute.com/" && !AppState.UserIsLoggedIn)
                    {
                        w.LoadUrl(GetInjectable(Display.ShowTabScrollInner + Display.GetForAllTabs()));
                    }
                }
                HidePageTitle(w, AppSettings.HidePageTitleDelay);
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().EndsWith($"common.css"))
                {
                    return CssHelper.GetCssResponse(CssHelper.CommonCssFeed);
                }
                return base.ShouldInterceptRequest(view, request);
            }



            public override void OnPageFinished(WebView view, string url)
            {
                RunPageCommands(view);
                base.OnPageFinished(view, url);
            }
        }

        public class MyChannel : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                if (w.Url.Contains("bitchute.com/channel/") || w.Url == "https://www.bitchute.com/")
                { w.LoadUrl(JavascriptCommands.GetInjectable(JavascriptCommands.Display.ShowTabScrollInner)); }
                try
                {
                    
                }
                catch { }
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().EndsWith($"/common.css"))
                {
                    return CssHelper.GetCssResponse(CssHelper.CommonCssMyChannel);
                }
                return base.ShouldInterceptRequest(view, request);
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().EndsWith("=/profile/"))
                {
                    if (request.Url.ToString() == "https://www.bitchute.com/accounts/login/?next=/profile/")
                    {
                        MainActivity.Fm3.SwapLoginView(false, false, true);
                    }
                }
                return base.ShouldOverrideUrlLoading(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                RunPageCommands(view);
            }
        }

        public class Settings : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {

            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().EndsWith($"/common.css"))
                {
                    return CssHelper.GetCssResponse(CssHelper.CommonCssSettings);
                }
                if (request.Url.ToString() == @"https://www.bitchute.com/accounts/logout/")
                {
                    if (request.Method == "GET")
                    {
                        if (OnLogout == null) {
                            OnLogout += RunOnLogout;
                            OnLogout += RunPostAuthEvent.OnPostLogout;
                        }
                        OnLogout.Invoke(new LogoutEventArgs());
                        AppState.UserIsLoggedIn = false;
                        
                    }
                }
                return base.ShouldInterceptRequest(view, request);
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().EndsWith("=/settings/"))
                {
                    if (request.Url.ToString() == "https://www.bitchute.com/accounts/login/?next=/settings/")
                    {
                        MainActivity.Fm4.SwapLoginView(false, false, true);
                    }
                }
                return base.ShouldOverrideUrlLoading(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                RunPageCommands(view);
                base.OnPageFinished(view, url);
            }
        }
    }
}