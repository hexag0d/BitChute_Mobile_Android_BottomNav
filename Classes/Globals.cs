using System.Collections.Generic;
using Android.Content;

namespace BottomNavigationViewPager.Classes
{
    public class Globals
    {
        public static string _appVersion = "18.6.4.4.NotificationAdder.nonGPS";

        //   Android.App.ActivityManager _am = (Android.App.ActivityManager)Android.App.Application
        //         .Context.GetSystemService(Context.ActivityService);
        
        //public static bool _setWebView { get; set; }
        //public static bool _navIsTimingOut { get; set; }
        //public static int _wvHeight { get; set; }

        /// <summary>
        /// tab 4 string should be set to strings like
        /// "Subs" "Home" or "Feed"
        /// </summary>
        public static string _t4Is { get; set; }
        
        /// <summary>
        /// tab 5 string should be set to strings like
        /// "Subs" "Home" or "Feed"
        /// </summary>
        public static string _t5Is { get; set; }

        /// <summary>
        /// this is a string containing cookies for the app.
        /// WebView cookies don't transfer automatically to httprequests
        /// </summary>
        public static string _cookieString { get; set; }

        public static Android.App.ActivityManager _am = (Android.App.ActivityManager)Android.App.Application
             .Context.GetSystemService(Context.ActivityService);

        public static MainActivity _main;
        /// <summary>
        /// global bool setting: 
        /// returns/should be set to false if this app is in the foreground
        /// returns/should be set to true when the app goes background
        /// 
        /// it doesn't override the OS setting; it keeps the status for you
        /// </summary>
        public static bool _bkgrd = true;

        /// <summary>
        /// returns false when the ActivityManager contains
        /// an entry for this app running in foreground: 
        /// importance is present in package.name with OS focus
        /// 
        /// requires a modified an  droid manifest for get_task ALLOWED
        /// </summary>
        /// <returns>bool</returns>
        public bool IsInBkGrd()
        {
            var _ctx = Android.App.Application.Context;

            var runningAppProcesses = _am.RunningAppProcesses;

            List<Android.App.ActivityManager.RunningAppProcessInfo> list
                = new List<Android.App.ActivityManager.RunningAppProcessInfo>();

            if (_am != null && list != null)
            {
                try
                {
                    list.AddRange(_am.RunningAppProcesses);
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    _main = MainActivity._main;
                    _am = _main.CustomGetActivityManager();
                    list.AddRange(_am.RunningAppProcesses);
                }
                catch
                {

                }
                _bkgrd = true;
                return _bkgrd;
            }

            if (list != null)
            {
                foreach (var _process in list)
                {
                    if (_process.Importance == Android.App.ActivityManager.RunningAppProcessInfo.ImportanceForeground)
                    {
                        foreach (var _pkg in _process.PkgList)
                        {
                            if (_pkg == _ctx.PackageName)
                            {
                                _bkgrd = false;
                            }

                            else
                            {
                                _bkgrd = true;
                            }
                        }
                    }
                }
            }
            return _bkgrd;
        }
        public class AppState
        {
            public class Display
            {
                /// <summary>
                /// returns true when the app detects that device
                /// has been rotated horizontally
                /// </summary>
                public static bool _horizontal = false;
            }
        }
        
        /// <summary>
        /// contains global var settings for the app
        /// </summary>
        public class AppSettings
        {
            /// <summary>
            /// the ms delay for setting a pop back to root for each tab
            /// </summary>
            public static int _tabDelay = 800;

            /// <summary>
            /// the ms delay for fixing link overflow on mobile
            /// </summary>
            public static int _linkOverflowFixDelay = 6000;

            public static int _notificationDelay = 120000;

            public static bool _notifying = false;
        }

        /// <summary>
        /// this class contains javascript commands in the form of strings that
        /// can be run via LoadUrl
        /// </summary>
        public class JavascriptCommands
        {
            /// <summary>
            /// fixes the link overflow issue
            /// </summary>
            public static string _jsLinkFixer = "javascript:(function() { " +
                 "document.getElementById('video-description').style.overflow='hidden'; " + "})()";

            /// <summary>
            /// hides the static banner
            /// </summary>
            public static string _jsHideBanner = "javascript:(function() { " +
                 "document.getElementById('nav-top-menu').style.display='none'; " + "})()";

            /// <summary>
            /// hides the banner buffer
            /// </summary>
            public static string _jsHideBuff = "javascript:(function() { " +
                "document.getElementById('nav-menu-buffer').style.display='none'; " + "})()";

            /// <summary>
            /// hides the carousel aka featured creators
            /// </summary>
            public static string _jsHideCarousel = "javascript:(function() { " +
                "document.getElementById('carousel').style.display='none'; " + "})()";

            /// <summary>
            /// shows the carousel aka featured creators
            /// </summary>
            public static string _jsShowCarousel = "javascript:(function() { " +
                "document.getElementById('carousel').style.display='block'; " + "})()";
            
            /// <summary>
            /// hides the listing all element
            /// </summary>
            public static string _jsHideTab1 = "javascript:(function() { " +
                            "document.getElementById('listing-all').style.display='none'; " + "})()";
            
            /// <summary>
            /// shows the listing all element
            /// </summary>
            public static string _jsShowTab1 = "javascript:(function() { " +
                            "document.getElementById('listing-all').style.display='block'; " + "})()";
            
            /// <summary>
            /// hides the popular listings
            /// </summary>
            public static string _jsHideTab2 = "javascript:(function() { " +
                            "document.getElementById('listing-popular').style.display='none'; " + "})()";

            /// <summary>
            /// shows the popular listings
            /// </summary>
            public static string _jsShowTab2 = "javascript:(function() { " +
                            "document.getElementById('listing-popular').style.display='block'; " + "})()";
            
            /// <summary>
            /// shows the subscribed feed
            /// </summary>
            public static string _jsSelectTab3 = "javascript:(function() { " +
                            "document.getElementById('listing-subscribed').style.display='block'; " + "})()";
            
            /// <summary>
            /// hides the tab scroll inner element
            /// </summary>
            public static string _jsHideLabel = "javascript:(function() { " +
                            "document.getElementsByClassName('tab-scroll-inner')[0].style.display='none'; " + "})()";
            
            /// <summary>
            /// shows the tab scroll inner element
            /// </summary>
            public static string _jsShowLabel = "javascript:(function() { " +
                            "document.getElementsByClassName('tab-scroll-inner')[0].style.display='block'; " + "})()";

            /// <summary>
            /// hides the trending tab
            /// </summary>
            public static string _jsHideTrending = "javascript:(function() { " +
                            "document.getElementById('listing-trending').style.display='none'; " + "})()";
            
            /// <summary>
            /// shows the trending tab
            /// </summary>
            public static string _jsShowTrending = "javascript:(function() { " +
                            "document.getElementById('listing-trending').style.display='block'; " + "})()";
            //$('.show-more').click(function(){listingExtend(40);});

            public static string _jqShowMore = "javascript:(function() { " +
                            "document.listingExtend(40);" + "})()";

            /// <summary>
            /// disables the tooltips because they block the controls and stick
            /// on screen in all android browsers
            /// </summary>
            public static string _jsDisableTooltips = "javascript:(function() { " +
                            "document.getElementById('video-like').data-toggle=''; " + "})()" + "\r\n"
                + "javascript:(function() { " +
                            "document.getElementById('video-dislike').data-toggle=''; " + "})()";

            /// <summary>
            /// hides the video title
            /// </summary>
            public static string _jsHideTitle = "javascript:(function() { " +
                            "document.getElementById('video-title').style.display='none'; " + "})()";

            /// <summary>
            /// shows the title bar
            /// </summary>
            public static string _jsShowTitle = "javascript:(function() { " +
                            "document.getElementById('video-title').style.display='block'; " + "})()";

            /// <summary>
            /// hides the video watch block
            /// </summary>
            public static string _jsHideWatchTab = "javascript:(function() { " +
                            "document.getElementsByClassName('tab-scroll-outer')[0].style.display='none'; " + "})()";

            /// <summary>
            /// shows the video watch block
            /// </summary>
            public static string _jsShowWatchTab = "javascript:(function() { " +
                            "document.getElementsByClassName('tab-scroll-outer')[0].style.display='block'; " + "})()";
            
            public static string _jsHidePageBar = "javascript:(function() { " +
                            "document.getElementsByClassName('page-bar')[0].style.display='none'; " + "})()";
            
            public static string _jsShowPageBar = "javascript:(function() { " +
                            "document.getElementsByClassName('page-bar')[0].style.display='block'; " + "})()";
            
            public static string  _jsHideNavTabsList = "javascript:(function() { " +
                            "document.getElementsByClassName('nav nav-tabs nav-tabs-list')[0].style.display='none'; " + "})()";

            //tab-scroll-inner            
            public static string  _jsHideTabInner = "javascript:(function() { " +
                            "document.getElementsByClassName('tab-scroll-inner')[0].style.display='none'; " + "})()";

        }

        /// <summary>
        /// contains url strings
        /// </summary>
        public class URLs
        {
            public static string _homepage = "https://www.bitchute.com/";

            public static string _subspage = "https://www.bitchute.com/subscriptions/";

            public static string _explore =  "https://www.bitchute.com/channels/";

            public static string _settings = "https://www.bitchute.com/settings/";

            public static string _myChannel = "https://www.bitchute.com/channel/";
        }
    }
}