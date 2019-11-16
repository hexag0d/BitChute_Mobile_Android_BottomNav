using System.Collections.Generic;
using Android.Content;

namespace BottomNavigationViewPager.Classes
{
    public class Globals
    {
        /// <summary>
        /// this string is sent into the app settings fragment to notify user 
        /// of version they're running
        /// </summary>
        public static string _appVersion = "20.0.1.OptimizeUI";

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

        //public static Android.App.ActivityManager _am = (Android.App.ActivityManager)Android.App.Application
        //     .Context.GetSystemService(Context.ActivityService);
        
        /// <summary>
        /// should be set to and return variables based upon the current appstate
        /// </summary>
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

            /// <summary>
            /// global bool state: 
            /// returns/should be set to false if this app is in the foreground
            /// returns/should be set to true when the app goes background
            /// 
            /// it doesn't override the OS setting; it keeps the status for you
            /// </summary>
            public static bool _bkgrd = true;
            
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

            /// <summary>
            /// this int controls the delay in ms of notifications being 
            /// parsed and then sent out.  It should be set to a high int
            /// so as to not overload bitchute.com with httprequests
            /// </summary>
            public static int _notificationDelay = 120000;

            /// <summary>
            /// this bool should be set to/returns whether or not the navbar
            /// will be hidden by default when the device is held horizontally
            /// </summary>
            public static bool _hideHorizontalNavBar = true;

            public static bool _notifying = true;
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
            
            /// <summary>
            /// hides the page bar
            /// </summary>
            public static string _jsHidePageBar = "javascript:(function() { " +
                            "document.getElementsByClassName('page-bar')[0].style.display='none'; " + "})()";
            
            /// <summary>
            /// shows the page bar
            /// </summary>
            public static string _jsShowPageBar = "javascript:(function() { " +
                            "document.getElementsByClassName('page-bar')[0].style.display='block'; " + "})()";
            
            /// <summary>
            /// hides the nav tab list (warning, not good on tab 3)
            /// </summary>
            public static string  _jsHideNavTabsList = "javascript:(function() { " +
                            "document.getElementsByClassName('nav nav-tabs nav-tabs-list')[0].style.display='none'; " + "})()";
      
            /// <summary>
            /// hides the tab inner
            /// </summary>
            public static string  _jsHideTabInner = "javascript:(function() { " +
                            "document.getElementsByClassName('tab-scroll-inner')[0].style.display='none'; " + "})()";
              
            /// <summary>
            /// sets
            /// </summary>
            public static string _jsFillAvailable = "javascript:(function() { " +
                            @"document.getElementsByClassName('video-card-image')[0].style.width='fill-available'; " + "})()";
            
            public static string _jsBorderBox = "javascript:(function() { " +
                            @"document.getElementsByClassName('video-card-image').style.box-sizing='border-box'; " + "})()";

            public static string _jsBorderBoxAll = "javascript:(function() { " +
                @"var video_card_image_array = document.getElementsByClassName('img-responsive lazyloaded');
                                    for (var i = 0; i < video_card_image_array.length; ++i) {
                                             var item66 = video_card_image_array[i];  
                                            item66.style.boxSizing = 'border-box';
                                            item66.style.width = '100%';
                                        }" + "})()";

            public static string _jsVideoCardImage = "javascript:(function() { " +
            @"var video_card_image_array = document.getElementsByClassName('video-card-image');
                                    for (var i = 0; i < video_card_image_array.length; ++i) {
                                             var item66 = video_card_image_array[i];  
                                            item66.style.width = '100%';
                                            item66.style.boxSizing = 'border-box';
                                        }" + "})()";

            
            public static string _jsRemoveMaxWidthAll = "javascript:(function() { " +
                            @"var videocard_array = document.getElementsByClassName('video-card');
                                    for (var i = 0; i < videocard_array.length; ++i) {
                                             var item55 = videocard_array[i];  
                                            item55.style.maxWidth = 'none';
                                            item55.style.width = '100%';
                                        }" + "})()";

            public static string _jsExpandSubs = "javascript:(function() { " +
                @"var videocard_array = document.getElementsByClassName('subscription-container');
                                    for (var i = 0; i < videocard_array.length; ++i) {
                                             var item55 = videocard_array[i];  
                                            item55.style.maxWidth = 'none';
                                            item55.style.width = '100%';
                                        }" + "})()";

            public static string _jsExpandFeatured = "javascript:(function() { " +
                 @"var videocard_array = document.getElementsByClassName('mg-responsive hidden-md hidden-lg lazyloaded');
                                    for (var i = 0; i < videocard_array.length; ++i) {
                                             var ite5 = videocard_array[i];  
                                            ite5.style.maxWidth = 'none';
                                            ite5.style.width = '100%';
                                        }" + "})()";

            public static string _jsFeaturedRemoveMaxWidth = "javascript:(function() { " +
                  @"var videocard_array = document.getElementsByClassName('channel-card');
                                    for (var i = 0; i < videocard_array.length; ++i) {
                                             var ite5 = videocard_array[i];  
                                            ite5.style.maxWidth = 'none';
                                        }" + "})()";


            public static string _jsHideVideoMargin = "javascript:(function() { " +
                            @"document.getElementsByClassName('container')[0].style.paddingLeft='0px'; " +
                            @"document.getElementsByClassName('container')[0].style.paddingRight='0px'; "+
                            @"document.getElementsByClassName('video-container')[0].style.paddingLeft='0px'; " +
                            @"document.getElementsByClassName('video-container')[0].style.paddingRight='0px'; " +
                            @"document.getElementsByClassName('row')[2].style.marginLeft='0px';"+ 
                            @"document.getElementsByClassName('row')[2].style.marginRight='0px';" + "})()";

            public static string _jsPageBarDelete =  "javascript:(function() { " +
                            @"document.getElementById('page-bar').style='padding-top: 0px;padding-bottom: 0px;border-bottom-width: 0px;'" + "})()";

            public static string _jsPut5pxMarginOnRows = "javascript:(function() { " +
                             @"var row_array = document.getElementsByClassName('row');
                                    for (var i = 3; i < row_array.length; ++i) {
                                             var ite5 = row_array[i];  
                                            ite5.style.marginLeft = '5px';
                                            ite5.style.marginRight = '5px';
                                        }" + "})()";
            
            public static string _jsRemoveMaxWidth = "javascript:(function() { " +
                            @"document.getElementsByClassName('video-card').style.max-width=''; " + "})()";

            public static string _jsSelectSubscribed = "javascript:(function() { " +
                            @"document.querySelectorAll("
                            + "\"" + @"a[data-toggle]" + "\"" + @")[16].click()" + @"})()";
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
            //https://www.bitchute.com/playlist/watch-later/ https://www.bitchute.com/playlists/
        }
    }
}