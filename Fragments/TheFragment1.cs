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
using static BottomNavigationViewPager.Fragments.TheFragment5;
using static StartServices.Servicesclass.ExtStickyService;

namespace BottomNavigationViewPager.Fragments
{
    [Android.Runtime.Register("onWindowVisibilityChanged", "(I)V", "GetOnWindowVisibilityChanged_IHandler")]
    public class TheFragment1 : Fragment
    {
        string _title;
        string _icon;

        public static ServiceWebView _wv;
        protected static View _view;

        public static string _url = "https://www.bitchute.com/";

        readonly ExtWebViewClient _wvc = new ExtWebViewClient();

        private static ExtNotifications _extNotifications = MainActivity.notifications;
        private static ExtWebInterface _extWebInterface = MainActivity._extWebInterface;

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
            _wv = (ServiceWebView)_view.FindViewById<ServiceWebView>(Resource.Id.webView1);

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(_wvc);
                _wv.Settings.JavaScriptEnabled = true;
                _wv.Settings.DisplayZoomControls = false;
                //_wv.Settings.AllowFileAccess = true;
                //_wv.Settings.AllowContentAccess = true;
                
                tabLoaded = true;
            }
            _wv.LoadUrl(_url);
            if (AppSettings._zoomControl)
            {
                _wv.Settings.BuiltInZoomControls = true;
                _wv.Settings.DisplayZoomControls = false;
            }
            CustomSetTouchListener(AppState.Display._horizontal);
            //_wv.SetOnScrollChangeListener(new ExtScrollListener());
            //_wv.SetOnTouchListener(new ExtTouchListener());
            return _view;
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
                _wv.SetOnTouchListener(new ExtTouchListener());
            }
            else
            {
                _wv.SetOnTouchListener(null);
            }
        }

        public void OnSettingsChanged()
        {
            _wv.Settings.SetSupportZoom(AppSettings._zoomControl);

            if (!AppSettings._tab1FeaturedOn)
            {
                _wv.LoadUrl(JavascriptCommands._jsHideCarousel);
            }
            else
            {
                _wv.LoadUrl(JavascriptCommands._jsShowCarousel);
            }

            if (AppSettings._zoomControl)
            {
                _wv.Settings.BuiltInZoomControls = true;
            }
            else
            {
                _wv.Settings.BuiltInZoomControls = false;
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
            _scrollY += _wv.ScrollY;
            if (AppState.Display._horizontal)
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
                await Task.Delay(AppSettings._tabDelay);
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
            await Task.Delay(AppSettings._linkOverflowFixDelay);
            _wv.LoadUrl(JavascriptCommands._jsLinkFixer);
            _wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
            _wv.LoadUrl(JavascriptCommands._jsHideTooltips);
        }

        public void LoadCustomUrl(string url)
        {
            _wv.LoadUrl(url);
        }

        public static async void HidePageTitle()
        {
            await Task.Delay(5000);

            if (_wv.Url != "https://www.bitchute.com/" && AppState.Display._horizontal)
            {
                _wv.LoadUrl(JavascriptCommands._jsHideTitle);
                _wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
                _wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                _wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
            }
            //_wv.LoadUrl(JavascriptCommands._jsHideNavTabsList);
        }

        private static async void HideWatchLabel()
        {
            await Task.Delay(4000);
            if (_wv.Url != "https://www.bitchute.com/")
            _wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }

        public void SetWebViewVis()
        {
            _wv.Visibility = ViewStates.Visible;
        }

        public static async void ExpandVideoCards(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(5000);
            }
            _wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            _wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);
        }

        private static async void ExpandFeaturedChannels(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(3000);
            }
            _wv.LoadUrl(JavascriptCommands._jsFeaturedRemoveMaxWidth);
            _wv.LoadUrl(JavascriptCommands._jsExpandFeatured);
        }

        private static async void ExpandPage(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(3000);
            }
            //_wv.LoadUrl(JavascriptCommands._jsHideVideoMargin);
            //_wv.LoadUrl(JavascriptCommands._jsPut5pxMarginOnRows);
        }
        
        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView _view, string url)
            {
                _wv.LoadUrl(JavascriptCommands._jsHideBanner);
                _wv.LoadUrl(JavascriptCommands._jsHideBuff);
               
                //_wv.LoadUrl(JavascriptCommands._jsHideNavTabsList);

                    HideWatchLabel();
                

                if (!AppSettings._tab1FeaturedOn)
                {
                    _wv.LoadUrl(JavascriptCommands._jsHideCarousel);
                }
                
                if (AppState.Display._horizontal)
                {
                    if (url != "https://www.bitchute.com/")
                    {
                        _wv.LoadUrl(JavascriptCommands._jsHideTitle);
                        _wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
                        _wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                        _wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
                        //_wv.LoadUrl(JavascriptCommands._jsHideNavTabsList);
                    }

                    HidePageTitle();
                }
                
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
                _wv.LoadUrl(JavascriptCommands._jsLinkFixer);
                SetReload();
                HideLinkOverflow();
                ExpandFeaturedChannels(true);
                ExpandVideoCards(true);
                //ExpandPage(true);
                _wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
                _wv.LoadUrl(JavascriptCommands._jsHideTooltips);
                base.OnPageFinished(_view, url);
            }
        }
    }
}
