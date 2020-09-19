﻿using System;
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
using BitChute.Classes;
using BitChute.Web.Ui;
using System.Net;
using BitChute.Fragments;
using static BitChute.Classes.JavascriptCommands;

namespace BitChute.Web
{
    public class ViewClients
    {
        public static async void LoadInitialUrls(int delay = 400)
        {
            while (!CssHelper.CustomCssReadyForRead)
            {
                await Task.Delay(delay);
            }

            HomePageFrag.Wv.LoadUrl(HomePageFrag.RootUrl);
            SubscriptionFrag.Wv.LoadUrl(SubscriptionFrag.RootUrl);
            FeedFrag.Wv.LoadUrl(FeedFrag.RootUrl);
            MyChannelFrag.Wv.LoadUrl(MyChannelFrag.RootUrl);
            SettingsFrag.Wv.LoadUrl(SettingsFrag.RootUrl);
        }
        
        public static async void SetReload(int tab)
        {
            tab = MainActivity.ViewPager.CurrentItem;
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
        
        //These classes are static rather than differentiating on the fly because 
        //I don't want too much checking what tab we're on.  I think this will run faster
        //if each class is pre-specialized to it's purpose
        //the point here is to add styling and other customized html for android webview specifically

        public class Home : WebViewClient
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
                if (request.Url.ToString().Contains("common.css"))
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

        public class Subs : WebViewClient
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
                if (request.Url.ToString().Contains("common.css"))
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

        public class Feed : WebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                if (w.Url.Contains("bitchute.com/channel/") || w.Url == "https://www.bitchute.com/")
                { w.LoadUrl(GetInjectable(Display.ShowTabScrollInner +  Display.GetForAllTabs())); w.LoadUrl(_jsSelectSubscribed); }
                HidePageTitle(w, AppSettings.HidePageTitleDelay);
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().Contains("common.css"))
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

        public class MyChannel : WebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {
                await Task.Delay(d);
                if (w.Url.Contains("bitchute.com/channel/") || w.Url == "https://www.bitchute.com/")
                { w.LoadUrl(JavascriptCommands.GetInjectable(JavascriptCommands.Display.ShowTabScrollInner)); }
                try
                {
                    ExtWebInterface.CookieHeader = Android.Webkit.CookieManager.Instance.GetCookie("https://www.bitchute.com/");

                    Https.CookieString = ExtWebInterface.CookieHeader.ToString();
                    var cookiePairs = ExtWebInterface.CookieHeader.Split('&');

                    Https.CookieString = "";

                    foreach (var cookiePair in cookiePairs)
                    {
                        var cookiePieces = cookiePair.Split('=');
                        if (cookiePieces[0].Contains(":"))
                            cookiePieces[0] = cookiePieces[0].Substring(0, cookiePieces[0].IndexOf(":"));
                        ExtWebInterface.Cookies.Add(new Cookie
                        {
                            Name = cookiePieces[0],
                            Value = cookiePieces[1]
                        });
                    }

                    foreach (Cookie c in ExtWebInterface.Cookies)
                    {
                        c.Domain = "https://bitchute.com/";
                        if (Https.CookieString == "")
                        {
                            Https.CookieString = c.ToString();
                        }
                        else
                        {
                            Https.CookieString += c.ToString();
                        }
                    }
                }
                catch { }
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().Contains("common.css"))
                {
                    return CssHelper.GetCssResponse(CssHelper.CommonCssMyChannel);
                }
                return base.ShouldInterceptRequest(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                RunPageCommands(view);
            }
        }

        public class Settings : WebViewClient
        {
            public static async void RunPageCommands(WebView w, int d = 2000)
            {

            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().Contains("common.css"))
                {
                    return CssHelper.GetCssResponse(CssHelper.CommonCssSettings);
                }
                return base.ShouldInterceptRequest(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                RunPageCommands(view);
                base.OnPageFinished(view, url);
            }
        }
    }
}