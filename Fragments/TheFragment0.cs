﻿using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BottomNavigationViewPager.Classes;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Android.Views.View;
using static BottomNavigationViewPager.Fragments.TheFragment4;
using static StartServices.Servicesclass.ExtStickyService;

namespace BottomNavigationViewPager.Fragments
{
    [Android.Runtime.Register("onWindowVisibilityChanged", "(I)V", "GetOnWindowVisibilityChanged_IHandler")]
    public class TheFragment0 : Fragment
    {
        string _title;
        string _icon;

        public static ServiceWebView Wv;
        protected static View _view;

        public static string _url = "https://www.bitchute.com/";

        readonly ExtWebViewClient Wvc = new ExtWebViewClient();

        private static ExtNotifications _extNotifications = MainActivity.notifications;
        private static ExtWebInterface _extWebInterface = MainActivity._extWebInterface;

        bool tabLoaded = false;
        //static MainActivity _main = new MainActivity();

        public static TheFragment0 NewInstance(string title, string icon) {
            var fragment = new TheFragment0();
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
            _view = inflater.Inflate(Resource.Layout.TheFragmentLayout0, container, false);
            Wv = _view.FindViewById<ServiceWebView>(Resource.Id.webView1);

            //ViewHelpers.Tab0.FragmentContainerLayout = inflater.Inflate(Resource.Layout.TheFragmentLayout0, container, false);
            //ViewHelpers.Tab0.DownloaderLayout = inflater.Inflate(Resource.Layout.DownloaderLayout, container, false);
            //ViewHelpers.Tab0.TabFragmentLinearLayout = 
            //    (RelativeLayout)ViewHelpers.Tab0.FragmentContainerLayout.FindViewById<RelativeLayout>(Resource.Id.tab0relativeLayout);
            //ViewHelpers.Tab0.DownloadButton = ViewHelpers.Tab0.DownloaderLayout.FindViewById<Button>(Resource.Id.downloadButton);
            //Wv = (ServiceWebView)ViewHelpers.Tab0.WebViewFragmentLayout.FindViewById<ServiceWebView>(Resource.Id.webView3Swapable);
            //ViewHelpers.Container = container;
            //ViewHelpers.Tab0.TabFragmentLinearLayout.AddView(ViewHelpers.Tab0.WebViewFragmentLayout);

            //ViewHelpers.Tab0.DownloadButton.Click += VideoDownloader.VideoDownloadButton_OnClick;


            if (!tabLoaded)
            {
                Wv.SetWebViewClient(Wvc);
                Wv.Settings.JavaScriptEnabled = true;
                Wv.Settings.DisplayZoomControls = false;
                //Wv.Settings.AllowFileAccess = true;
                //Wv.Settings.AllowContentAccess = true;
                
                tabLoaded = true;
            }
            Wv.LoadUrl(_url);
            if (AppSettings._zoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            CustomSetTouchListener(AppState.Display.Horizontal);
            //Wv.SetOnScrollChangeListener(new ExtScrollListener());
            //Wv.SetOnTouchListener(new ExtTouchListener());
            return _view;
        }

        public static void SwapView(View v)
        {

        }

        public override void OnResume()
        {
            base.OnResume();
            IntentFilter intentFilter = new IntentFilter(Intent.ActionHeadsetPlug);

        }

        /// <summary>
        /// sets the touch listener when device is in landscape mode
        /// sets it to null when the device goes back into portrait mode
        /// </summary>
        /// <param name="landscape"></param>
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

        public void OnSettingsChanged(List<object> settings)
        {
            Wv.Settings.SetSupportZoom(AppSettings._zoomControl);

            if (!AppSettings._tab1FeaturedOn)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideCarousel);
            }
            else
            {
                Wv.LoadUrl(JavascriptCommands._jsShowCarousel);
            }

            if (AppSettings._zoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
            }
            else
            {
                Wv.Settings.BuiltInZoomControls = false;
            }
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

        //public class ExtScrollListener : Java.Lang.Object, View.IOnScrollChangeListener
        //{
        //    public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        //    {
        //         OnScrollChanged(scrollY);
        //    }
        //}

        //public static async void OnScrollChanged(int scrollY)
        //{
        //    await Task.Delay(60);
        //    _scrollY += scrollY;
        //    if (AppState.Display._horizontal)
        //    {
        //        await Task.Delay(500);
        //        if (_scrollY >= 4000)
        //        {
        //            ExpandVideoCards(false);
        //            _scrollY = 0;
        //        }
        //    }
        //}

        /// <summary>
        /// tells the webview to GoBack, if it can
        /// </summary>
        public void WebViewGoBack()
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
            else
            {
                Wv.LoadUrl(@"https://bitchute.com/");
            }
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
                await Task.Delay(AppSettings._tabDelay);
                WvRl = true;
                WvRling = false;
            }
        }

        //I'll explain this later
        static int _autoInt = 0;

        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings._linkOverflowFixDelay);
            Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
            Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
            Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
        }

        public void LoadCustomUrl(string url)
        {
            Wv.LoadUrl(url);
        }

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
            //Wv.LoadUrl(JavascriptCommands._jsHideNavTabsList);
        }

        private static async void HideWatchLabel()
        {
            await Task.Delay(4000);
            if (Wv.Url != "https://www.bitchute.com/")
            Wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }

        public void SetWebViewVis()
        {
            Wv.Visibility = ViewStates.Visible;
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
            if (delayed)
            {
                await Task.Delay(3000);
            }
            //Wv.LoadUrl(JavascriptCommands._jsHideVideoMargin);
            //Wv.LoadUrl(JavascriptCommands._jsPut5pxMarginOnRows);
        }
        
        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView _view, string url)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideBanner);
                Wv.LoadUrl(JavascriptCommands._jsHideBuff);
               
                //Wv.LoadUrl(JavascriptCommands._jsHideNavTabsList);

                    HideWatchLabel();
                

                if (!AppSettings._tab1FeaturedOn)
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
                        //Wv.LoadUrl(JavascriptCommands._jsHideNavTabsList);
                    }

                    HidePageTitle();
                }
                
                //add one to the autoint... for some reason if Tab1 has 
                //Wv.Settings.MediaPlaybackRequiresUserGesture = false; set then it won't work on the other tabs
                //this is a workaround for that glitch
                _autoInt++;

                // if autoInt is 1 then we will set the MediaPlaybackRequiresUserGesture
                //strange.. i know.. but it works
                if (_autoInt == 1)
                {
                    Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                }
                Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
                SetReload();
                HideLinkOverflow();
                ExpandFeaturedChannels(true);
                ExpandVideoCards(true);
                //ExpandPage(true);
                Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
                Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
                base.OnPageFinished(_view, url);
            }
        }
    }
}
