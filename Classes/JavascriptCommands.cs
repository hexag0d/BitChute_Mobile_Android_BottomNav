using static StartServices.Servicesclass.ExtStickyService;

namespace BitChute.Classes
{
    class JavascriptCommands
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
        /// on screen in all android browsers.. note: this does not actually
        /// hide the sticking tooltips, it just disables them from appearing
        /// and due to android OnPageFinished being EXTREMELY flaky, this
        /// doesn't work all the time
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
        /// shows the title 
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
        /// hides the nav tab list
        /// </summary>
        public static string _jsHideNavTabsList = "javascript:(function() { " +
                        "document.getElementsByClassName('nav nav-tabs nav-tabs-list')[0].style.display='none'; " + "})()";

        /// <summary>
        /// hides the tab inner
        /// </summary>
        public static string _jsHideTabInner = "javascript:(function() { " +
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
                        @"document.getElementsByClassName('container')[0].style.paddingRight='0px'; " +
                        @"document.getElementsByClassName('video-container')[0].style.paddingLeft='0px'; " +
                        @"document.getElementsByClassName('video-container')[0].style.paddingRight='0px'; " +
                        @"document.getElementsByClassName('row')[2].style.marginLeft='0px';" +
                        @"document.getElementsByClassName('row')[2].style.marginRight='0px';" + "})()";

        public static string _jsPageBarDelete = "javascript:(function() { " +
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
                        @"$(" + "\"" + @"a[href*='#listing-subscribed']" + "\"" + @").click()" + @"})()";

        /// <summary>
        /// Hides the sticking tooltips that happen on all android browsers.  Not to be
        /// confused with disabletooltips, which disables the data-toggle attribute.
        /// </summary>
        public static string _jsHideTooltips = "javascript:(function() { " +
                        @"$(" + "\"" + @".tooltip" + "\"" + @").tooltip(" + "\"" + "hide" + "\"" + ")" + @"})()";

        /// <summary>
        /// pauses the video
        /// </summary>
        public static string _jsPauseVideo = "javascript:(function() { " + "plyr.pause()" +  "})()";


        /// <summary>
        /// plays the video
        /// </summary>
        public static string _jsPlayVideo = "javascript:(function() { " + "plyr.play()" + "})()";

        /// <summary>
        /// skips to next video
        /// </summary>
        public static string _jsNextVideo = @"javascript:(function() { " 
            + @"document.getElementsByClassName(/""img-responsive lazyloaded/"")[1].click()" + "})()";

        /// <summary>
        /// skips to next video
        /// </summary>
        public static string _jsNextVideoByASpa = @"javascript:(function() { " 
            + @"$(" + "\"" + @"a[class='spa'][href*='/video/']" + "\"" + @")[0].click()"  + "})()";


        public static string _jsNextByImg = "javascript:(function() { " +
                @"$(" + "\"" + @"img[class*='img-responsive lazyloaded']" + "\"" + @")[1].click()" + @"})()";


        public static string RemoveDisqusIframeZero = "javascript:(function() { "
            + @"$('#disqus_thread').children('iframe')[0].src = "";" + @"})()";

        public static string RemoveDisqusIframeTwo = "javascript:(function() { "
            +  @"})()";

        //public static string RemoveDisqusIframeTwo = "javascript:(function() { "
        //     + @"$('#disqus_thread').children('iframe')[0].display = none;" + @"})()";


        //public static string RemoveDisqusIframeZero = "javascript:(function() { "
        //    + @"if ($('#disqus_thread').children('iframe').length > 3){ $('#disqus_thread').children('iframe')[0].remove(); }" + @"})()";


        //public static string RemoveDisqusIframeTwo = "javascript:(function() { "
        //    + @"if ($('#disqus_thread').children('iframe').length > 2){ $('#disqus_thread').children('iframe')[2].remove(); }" + @"})()";

            /// <summary>
            /// javascript/jquery commands that add observable callbacks into the webview
            /// </summary>
        public class CallBackInjection
        {
            //public static string AddFullScreenCallback = @"javascript:(" + @"function() { " +
            // @"customFullScreen = function() {  $('#loader-container').load('https://dlink.bitchute.com/callbacks/fullscreen'); } "
            //  + @"document.getElementsByClassName('plyr__controls__item plyr__control plyr__tab-focus')[0].addEventListener('click', customFullScreen, false);"
            //        + @"})()";

            //public static string AddFullScreenCallback = @"javascript:(function() { var customFullScreen = function() {  $('#loader-container').load('https://dlink.bitchute.com/callbacks/fullscreen'); }" +
            // @"document.getElementsByClassName('plyr__controls__item plyr__control')[0].addEventListener('click', customFullScreen, false); })();";

            //public static string AddFullScreenEventListener = @"document.getElementsByClassName('plyr__controls__item plyr__control')[0].addEventListener('click', customFullScreen, false); })();";
            //public static async void SetCallbackWithDelay (ServiceWebView wv, string js, int delay)
            //{
            //    await System.Threading.Tasks.Task.Delay(delay);
            //    await System.Threading.Tasks.Task.Run(() => {
            //        ViewHelpers.Main.UiHandler.Post(() => { wv.LoadUrl(js); });
            //    });
            //}
        }
    }
}