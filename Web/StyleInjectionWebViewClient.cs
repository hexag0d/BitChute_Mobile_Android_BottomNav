﻿using System;
using System.Collections.Generic;
using Android.Webkit;
using Android.Graphics;
using System.Threading.Tasks;
using BitChute.Web.Ui;
using BitChute.Fragments;
using static BitChute.JavascriptCommands;
using static BitChute.PlaystateManagement;
using static BitChute.Services.MainPlaybackSticky;
using BitChute.Web.Auth;
using static BitChute.ViewHelpers;
using BitChute.App;

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
                AppSettings.FirstTimeAppLoad = false;
            }

            int tabKey = -1;
            foreach (var frag in CommonFrag.FragmentByTab.Values)
            {
                tabKey++;
                if (MainActivity.NotificationStartedAppWithUrl != null && tabKey == 0)
                {
                    frag.Wv.LoadUrl(MainActivity.NotificationStartedAppWithUrl);
                    await Task.Delay(200);
                }
                else
                {
                    frag.Wv.LoadUrl(frag.RootUrl);
                    await Task.Delay(200);
                }
            }

            if (AppState.NotificationStartedApp)
            {
                MainActivity.NotificationStartedAppWithUrl = null;
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
            if (tab == -1) { tab = MainActivity.ViewPager.CurrentItem; }
            CommonFrag.GetFragmentById(-1, null, tab).SetReload();
        }

        public static async void RunAfterDelay(WebView wv, string command, int delay = 10)
        {
            await Task.Delay(delay);
            MainActivity.Instance.RunOnUiThread(() => { wv.LoadUrl(command); });
        }

        public static async void HidePageTitle(WebView w, int delay = 5000)
        {
            await Task.Delay(delay);
            DoActionOnUiThread(() =>
            {
                if (w.OriginalUrl != "https://www.bitchute.com/" && AppState.Display.Horizontal)
                {

                    w.LoadUrl(JavascriptCommands._jsHideTitle);
                    w.LoadUrl(JavascriptCommands._jsHidePageBar);
                    w.LoadUrl(JavascriptCommands._jsPageBarDelete);

                }
                else
                {

                    w.LoadUrl(JavascriptCommands._jsPageBarDelete);

                }
            });
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
                case @"https://_%26app_vidend_invoked_autoplay":
                    PlaystateChanged.Invoke(new PlaystateEventArgs(true, playerNumber, true, false));
                    break;
                case @"https://_%26app_vidend_invoked_noautoplay":
                    PlaystateChanged.Invoke(new PlaystateEventArgs(true, playerNumber, false, false));
                    break;
                case @"https://_%26app_isplaying_onminimized/":
                    PlaystateChanged.Invoke(new PlaystateEventArgs(true, playerNumber, true));
                    break;
                case @"https://_%26app_isnotplaying_onminimized/":
                    PlaystateChanged.Invoke(new PlaystateEventArgs(false, playerNumber, false));
                    break;
                case @"https://_&app_isplaying_and_loading_onminimized/":
                    PlaystateChanged.Invoke(new PlaystateEventArgs(true, playerNumber, true));
                    break;
            }
        }
        
        public static Tuple<int, ServiceWebView> VerifyPlayerPlayingOnMinimized()
        {
            _playerIdPlayingCallbackResponseList.Clear();

            Task.Factory.StartNew(() =>
            {
                while (_playerIdPlayingCallbackResponseList.Count < PlaystateManagement.WebViewIdDictionary.Count)
                {

                    System.Threading.Thread.Sleep(50);
                }
            });
            foreach (var player in _playerIdPlayingCallbackResponseList)
            {
                if (player.Item1 != -1)
                {
                    return new Tuple<int, ServiceWebView>(player.Item1, GetWebViewPlayerById(player.Item1));
                }
            }
            return null;
        }

        static List<Tuple<int, bool>> _playerIdPlayingCallbackResponseList = new List<Tuple<int, bool>>(); 

        public static void OnPlaystateChanged(PlaystateEventArgs e)
        {
            if (e.WebViewPlayerIdConfirmedInBackgroundIsPlaying != -1 && e.WebViewPlayerConfirmedInBackgroundIsPlaying)
            {
                PlaystateManagement.GetWebViewPlayerById(e.WebViewPlayerIdConfirmedInBackgroundIsPlaying).PlaybackShouldResumeOnMinimize = true;
                _playerIdPlayingCallbackResponseList
                    .Add(new Tuple<int, bool>(e.WebViewPlayerIdConfirmedInBackgroundIsPlaying, e.WebViewPlayerConfirmedInBackgroundIsPlaying));
            }
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
                            view.Visibility = Android.Views.ViewStates.Gone;
                        }
                        view.LoadUrl(view.RootUrl);
                    }
                    catch { }
                }
            }
            foreach (var frag in MainActivity.ViewFragmentArray)
            {
                ViewHelpers.DoActionOnUiThread(() =>
                {
                    ((CommonFrag)frag).SwapFragView(true);
                });
            }
            AppState.UserIsLoggedIn = true;
        }

        public static void RunOnLogout(LogoutEventArgs e)
        {
            AppSettings.SessionState.SessionId = "";
        }
        
        public static List<int> WindowLoadEventSet = new List<int>();

        public static async void RunBaseCommands(WebView w, int tabId = -1, int d = 2000)
        {
            await Task.Delay(20);
            //DoActionOnUiThread(() => w.LoadUrl(GetInjectable(CallBackInjection.OnWindowDocumentLoadEndPlayer)));
            DoActionOnUiThread(() => w.LoadUrl(GetInjectable(JavascriptCommands.JsHidePageBar)));
            GetWebViewPlayerById(w.Id).SetReload();
            DoActionOnUiThread(() => w.LoadUrl(JavascriptCommands._jsHideMargin));
            await Task.Delay(d);
            DoActionOnUiThread(() => w.LoadUrl(GetInjectable(JsDisableToolTips + JsHideTooltips)));
            HidePageTitle(w, AppSettings.HidePageTitleDelay);

            if (WebViewsAreReloadingAfterLogin)
            {
                if (WebViewIdLoginList.Contains(w.Id))
                {
                    DoActionOnUiThread(() => w.Visibility = Android.Views.ViewStates.Visible);
                    WebViewIdLoginList.Remove(w.Id);
                }
                if (WebViewIdLoginList.Count == 0)
                {
                    WebViewsAreReloadingAfterLogin = false;
                }
            }
            DoActionOnUiThread(() =>
            {
                w.LoadUrl(GetInjectable(
                    CallBackInjection.IsPlayingCallback +
                    CallBackInjection.PlayPauseButtonCallback));
                //w.LoadUrl(GetInjectable(CallBackInjection.OnWindowDocumentLoadEnd));
            });
            CustomComments.MakeAvatarsClickable(null, w, false, 4000);
        }

        public static void WebViewGoBack(WebView wv)
        {
            if (wv.CanGoBack()) wv.GoBack();
            BitChute.Web.ViewClients.RunBaseCommands(wv, 2000);
        }

        public class BaseWebViewClient : WebViewClient //WebViewClient shared between all applicable tabs
        {
            public override void OnPageStarted(WebView view, string url, Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
            }

            public BaseWebViewClient()
            {
                if (PlaystateChanged == null) PlaystateChanged += OnPlaystateChanged;
                
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.Url.Host.StartsWith(@"_")) // this reroutes javascript playstate callbacks from the webview 
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

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.LastPathSegment != null)
                {
                    if (request.Url.LastPathSegment.EndsWith("s")) //|| request.Url.Path.EndsWith(@"/bootstrap.min.css"))
                    {
                        if (request.Url.Path.EndsWith(@"/video.css")) { return CssHelper.GetCssResponse(CssHelper.VideoCss); }
                        //else if (CssHelper.BootstrapCss != "") { return CssHelper.GetCssResponse(CssHelper.BootstrapCss); }
                    }
                }
                return base.ShouldInterceptRequest(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                Task.Factory.StartNew(() => { RunBaseCommands(view, MainActivity.ViewPager.CurrentItem); });
            }
        }

        //more specialized webview overrides

        public class Home : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                DoActionOnUiThread(() =>{
                    if (w.Url.StartsWith("https://www.bitchute.com/channel/") || w.Url == "https://www.bitchute.com/")
                    {
                        w.LoadUrl(JavascriptCommands.GetInjectable(JavascriptCommands.Display.ShowTabScrollInner + JavascriptCommands.Display.GetForAllTabs()));
                    }
                });
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.LastPathSegment != null)
                {
                    if (request.Url.LastPathSegment.EndsWith("s"))
                    {
                        if (request.Url.LastPathSegment.EndsWith("common.css"))
                        {
                            return CssHelper.GetCssResponse(CssHelper.CommonCss);
                        }
                    }
                }
                return base.ShouldInterceptRequest(view, request);
            }
            
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                Task.Factory.StartNew(() => { RunPageCommands(view); });
            }
        }

        public class Subs : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                DoActionOnUiThread(() => {
                    if (w.OriginalUrl.StartsWith("https://www.bitchute.com/channel/") || w.OriginalUrl == "https://www.bitchute.com/")
                    {
                        w.LoadUrl(JavascriptCommands.GetInjectable(JavascriptCommands.Display.ShowTabScrollInner + 
                            JavascriptCommands.Display.GetForAllTabs()));
                    }
                });
                HidePageTitle(w, AppSettings.HidePageTitleDelay);
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.LastPathSegment != null)
                {
                    if (request.Url.LastPathSegment.EndsWith("s"))
                    {
                        if (request.Url.LastPathSegment.EndsWith("common.css"))
                        {
                            return CssHelper.GetCssResponse(CssHelper.CommonCss);
                        }
                    }
                }
                return base.ShouldInterceptRequest(view, request);
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.Url.LastPathSegment != null)
                {
                    if (request.Url.LastPathSegment.EndsWith("ns/"))
                    {
                        if (request.Url.ToString() == "https://www.bitchute.com/accounts/login/?next=/subscriptions/")
                        {
                            MainActivity.Fm1.SwapFragView(false, false, true);
                        }
                    }
                }
                return base.ShouldOverrideUrlLoading(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                Task.Factory.StartNew(() => RunPageCommands(view));
            }
        }

        public class Feed : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                DoActionOnUiThread(() => {
                    if (w.OriginalUrl.StartsWith("https://www.bitchute.com/channel/") || w.OriginalUrl == "https://www.bitchute.com/")
                    {
                        w.LoadUrl(GetInjectable(Display.ShowTabScrollInner + Display.GetForAllTabs()));
                        if (AppState.UserIsLoggedIn) { w.LoadUrl(_jsSelectSubscribed); }
                    }
                });
                HidePageTitle(w, AppSettings.HidePageTitleDelay);
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.LastPathSegment != null)
                {
                    if (request.Url.LastPathSegment.EndsWith("s"))
                    {
                        if (request.Url.LastPathSegment.EndsWith("common.css"))
                        {
                            return CssHelper.GetCssResponse(CssHelper.CommonCssFeed);
                        }
                    }
                }
                return base.ShouldInterceptRequest(view, request);
            }
            
            public override void OnPageFinished(WebView view, string url)
            {
                DoActionOnUiThread(() => view.LoadUrl(JavascriptCommands._jsHideMargin));
                base.OnPageFinished(view, url);
                Task.Factory.StartNew(() => { RunPageCommands(view); });
            }
        }

        public class MyChannel : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                DoActionOnUiThread(() =>
                {
                    if (w.Url.StartsWith("https://www.bitchute.com/channel/") || w.Url == "https://www.bitchute.com/")
                    {
                        w.LoadUrl(JavascriptCommands.GetInjectable(JavascriptCommands.Display.ShowTabScrollInner));
                    }
                });
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.LastPathSegment != null)
                {
                    if (request.Url.LastPathSegment.EndsWith("s"))
                    {
                        if (request.Url.LastPathSegment.EndsWith("common.css"))
                        {
                            return CssHelper.GetCssResponse(CssHelper.CommonCssMyChannel);
                        }
                    }
                }
                return base.ShouldInterceptRequest(view, request);
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.Url.LastPathSegment != null)
                {
                    if (request.Url.LastPathSegment.EndsWith(@"e/"))
                    {
                        if (request.Url.ToString() == "https://www.bitchute.com/accounts/login/?next=/profile/")
                        {
                            MainActivity.Fm3.SwapFragView(false, false, true);
                        }
                    }
                }
                return base.ShouldOverrideUrlLoading(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                Task.Factory.StartNew(()=>RunPageCommands(view));
            }
        }

        public class Settings : BaseWebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {

            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.LastPathSegment != null)
                {
                    if (request.Url.LastPathSegment.EndsWith("s"))
                    {
                        if (request.Url.LastPathSegment.EndsWith("common.css"))
                        {
                            return CssHelper.GetCssResponse(CssHelper.CommonCssSettings);
                        }
                    }
                    if (request.Url.LastPathSegment.EndsWith(@"t/"))
                    {
                        if (request.Url.ToString() == @"https://www.bitchute.com/accounts/logout/")
                        {
                            if (request.Method == "GET")
                            {
                                if (OnLogout == null)
                                {
                                    OnLogout += RunOnLogout;
                                    OnLogout += RunPostAuthEvent.OnPostLogout;
                                }
                                OnLogout.Invoke(new LogoutEventArgs());
                                AppState.UserIsLoggedIn = false;
                            }
                        }
                    }
                }

                return base.ShouldInterceptRequest(view, request);
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (request.Url.LastPathSegment != null)
                {
                    if (request.Url.LastPathSegment.EndsWith(@"s/") || request.Url.LastPathSegment.EndsWith(@"t/"))
                    {
                        if (request.Url.LastPathSegment.EndsWith(@"=/settings/") || request.Url.LastPathSegment.EndsWith(@"logout/"))
                        {
                            if (request.Url.ToString() == "https://www.bitchute.com/accounts/login/?next=/settings/" ||
                                request.Url.ToString() == "https://www.bitchute.com/accounts/login/?next=/accounts/logout/")
                            {
                                MainActivity.Fm4.SwapFragView(false, false, true);
                            }
                        }
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