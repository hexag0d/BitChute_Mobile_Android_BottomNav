using Android.Webkit;
using static BitChute.Services.MainPlaybackSticky;

namespace BitChute
{
    class JavascriptCommands
    {
        /// <summary>
        /// fixes the link overflow issue
        /// </summary>
        public static string _jsLinkFixer = "javascript:(function() { " +
             "window.document.getElementById('video-description').style.overflow='hidden';" + "})()";

        public static string JsLinkFixer = "window.document.getElementById('video-description').style.overflow='hidden'; ";

        public static string JsHideTeaserOverflow = "$('.teaser').style.overflow='hidden';";

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

        public static string JsDisableToolTips = "document.getElementById('video-like').data-toggle='';" +
            "document.getElementById('video-dislike').data-toggle='';";

        /// <summary>
        /// hides the video title
        /// </summary>
        public static string _jsHideTitle = "javascript:(function() { " +
                        "document.getElementById('video-title').style.display='none'; " + "})()";

        /// <summary>
        /// hides the video title
        /// </summary>
        public static string JsHideTitle = "document.getElementById('video-title').style.display='none';";


        /// <summary>
        /// shows the title 
        /// </summary>
        public static string _jsShowTitle = "javascript:(function() { " +
                        "document.getElementById('video-title').style.display='block'; " + "})()";

        /// <summary>
        /// shows the title 
        /// </summary>
        public static string JsShowTitle =  "document.getElementById('video-title').style.display='block';";
        
        /// <summary>
        /// hides the video watch block
        /// </summary>
        public static string _jsHideWatchTab = "javascript:(function() { " +
                        "document.getElementsByClassName('tab-scroll-outer')[0].style.display='none'; " + "})()";

        /// <summary>
        /// hides the video watch block
        /// </summary>
        public static string JsHideWatchTab = "document.getElementsByClassName('tab-scroll-outer')[0].style.display='none';";
        
        /// <summary>
        /// shows the video watch block
        /// </summary>
        public static string _jsShowWatchTab = "javascript:(function() { " +
                        "document.getElementsByClassName('tab-scroll-outer')[0].style.display='block'; " + "})()";

        /// <summary>
        /// shows the video watch block
        /// </summary>
        public static string JsShowWatchTab =  "document.getElementsByClassName('tab-scroll-outer')[0].style.display='block'; ";
        
        /// <summary>
        /// hides the page bar
        /// </summary>
        public static string _jsHidePageBar = "javascript:(function() { " +
                        "document.getElementsByClassName('page-bar')[0].style.display='none'; " + "})()";

        /// <summary>
        /// hides the page bar
        /// </summary>
        public static string JsHidePageBar = "window.document.getElementsByClassName('page-bar')[0].style.display='none';";
        
        /// <summary>
        /// shows the page bar
        /// </summary>
        public static string _jsShowPageBar = "javascript:(function() { " +
                        "document.getElementsByClassName('page-bar')[0].style.display='block'; " + "})()";

        /// <summary>
        /// shows the page bar
        /// </summary>
        public static string JsShowPageBar = "document.getElementsByClassName('page-bar')[0].style.display='block';";

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

        public static string _jsHideMargin = "javascript:(function() { " +
                        @"window.document.getElementsByClassName('container')[0].style.paddingLeft='1px'; " +
                        @"window.document.getElementsByClassName('container')[0].style.paddingRight='1px'; " +
                        @"window.document.getElementsByClassName('container')[0].style.marginLeft='1px'; " +
                        @"window.document.getElementsByClassName('container')[0].style.marginRight='1px'; " +
                        @"window.document.getElementsByClassName('container')[0].style.overflow='hidden'; " +
                        @"window.document.getElementsByClassName('video-container')[0].style.paddingLeft='1px'; " +
                        @"window.document.getElementsByClassName('video-container')[0].style.paddingRight='1px'; " +
                        @"window.document.getElementsByClassName('video-container')[0].style.marginLeft='1px'; " +
                        @"window.document.getElementsByClassName('video-container')[0].style.marginRight='1px'; " +
                        @"window.document.getElementsByClassName('row')[2].style.marginLeft='0px';" +
                        @"window.document.getElementsByClassName('row')[2].style.marginRight='0px';" + "})()";

        public static string _jsPageBarDelete = "javascript:(function() { " +
                        @"window.document.getElementById('page-bar').style='padding-top: 0px;padding-bottom: 0px;border-bottom-width: 0px;'" + "})()";

        public static string JsPageBarDelete = @"document.getElementById('page-bar').style='padding-top: 0px;padding-bottom: 0px;border-bottom-width: 0px;';";

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

        public static string JsHideTooltips = @"$(" + "\"" + @".tooltip" + "\"" + @").tooltip(" + "\"" + "hide" + "\"" + ");";

        /// <summary>
        /// pauses the video
        /// </summary>
        public static string _jsPauseVideo = "javascript:(function() { " + "plyr.pause()" +  "})()";


        /// <summary>
        /// stops the video
        /// </summary>
        public static string _jsStopVideo = "javascript:(function() { " + "plyr.stop()" + "})()";


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

        public static string AppendToHead (string text)
        {
            return @"javascript:(function() { " + $"$('head').append('{text}');"
            + @"})()";
        }

        public static string EnterUploadView = @"$(" + "\"" + @"a[href='/myupload/']" + "\"" + @")[0].click()";

        public static string ClickLikeVideo = @"$('#video-like').click()";

        public static string ClickDislikeVideo = @"$('#video-dislike').click()";

        public static string GetInjectable(string js)
        {
            return @"javascript:(function(){" + js + @"})()";
        }

        public class Display
        {
            public static string GetForAllTabs()
            {
                return RemoveTooltipAttr + RemoveAdvertHeight;
            }

            public static string RemoveTooltipAttr = @"document.getElementById('video-like').removeAttribute('data-original-title');"
                    + @"document.getElementById('video-dislike').removeAttribute('data-original-title');";

            public static string ShowTabScrollInner = @"$('.tab-scroll-inner')[0].style.height='50px';";

            public static string RemoveAdvertHeight = @"$('.advert-container-inner')[0].style.height='';";

            public static string SelectSubscribedTab = @"$(" + "\"" + @"a[href*='#listing-subscribed']" + "\"" + @").click();"; 
        }



        /// <summary>
        /// javascript/jquery commands that add observable callbacks into the webview
        /// </summary>
        public class CallBackInjection
        {
            // the playpause callback is actually backwards. if the plyr.playing = true then we should be observing a pause event.
            //however, that's not what's happening when I run the app.  It is backwards but just leaving for now
            public static string PlayPauseButtonCallback = @"$(""button[data-plyr*='play']"")[0].addEventListener('click', function(){if(plyr.playing){window.location='https://_&app_play_invoked/';}else{window.location='https://_&app_pause_invoked/';}});"
                + @"$(""button[data-plyr*='play']"")[1].addEventListener('click', function(){if(plyr.playing){window.location='https://_&app_play_invoked/';}else{window.location='https://_&app_pause_invoked/';}});";
            public static string IsPlayingCallback = @"if(plyr.playing||(plyr.currentTime>=plyr.duration&&plyr.autoplay)){window.location='https://_&app_play_isPlaying/'};";
            public static string FullscreenCallback = @"$(""button[data-plyr*='fullscreen']"")[0].addEventListener('click',function(){window.location='https://_&app_fullscreen_invoked/'})";
            public static string IsPlayingCallbackSimple = @"if(plyr!= null&& plyr.playing == true){if(plyr.loading){window.location='https://_&app_isplaying_and_loading_onminimized';}else{window.location='https://_&app_isplaying_onminimized';}}else{window.location='https://_&app_isnotplaying_onminimized';}";
            public static string IsPlayingCallbackSimpleWithBuffer = @"if(plyr!=null&&plyr.playing==true){if(plyr.loading||(plyr.buffering==0)){window.location='https://_&app_isplaying_and_loading_onminimized';}else{window.location='https://_&app_isplaying_onminimized';}}else if((plyr!=null&&plyr.paused)||(plyr!=null&&plyr.stopped)){window.location='https://_&app_isnotplaying_onminimized';}else{window.location='https://_&app_isnotplaying_onminimized';}";


            //if(plyr!= null&& plyr.playing == true){if(plyr.loading || (plyr.buffering == 0)){window.location='https://_&app_isplaying_and_loading_onminimized';}else{window.location='https://_&app_isplaying_onminimized';}}else{window.location='https://_&app_isnotplaying_onminimized';}
            public static string VideoPlayerCallbacks = @"if (window.document.getElementsByTagName('video')[0]!=undefined){
    window.document.getElementsByTagName('video')[0].addEventListener('play', (event) => {window.location='https://_&app_play_invoked/';});
    window.document.getElementsByTagName('video')[0].addEventListener('pause', (event) => {window.location='https://_&app_pause_invoked/';});
    window.document.getElementsByTagName('video')[0].addEventListener('ended', (event) => {
        if(plyr.autoplay){window.location='https://_&app_vidend_invoked_autoplay';}
        else{window.location='https://_&app_vidend_invoked_noautoplay'}});
}";

            public static string GetWindowDocumentEvents = "if(_wn_s_t == null){" + OnWindowDocumentLoadEnd + "};";
            public static string OnWindowDocumentLoadEnd = "var _wn_s_t=true;window.document.addEventListener('onloadend', function() { " +
                     $"{PlayPauseButtonCallback}{IsPlayingCallback}" + "});";
            public static string OnWindowDocumentLoadEndPlayer = "var _wn_s_t=true;window.document.addEventListener('onloadend', function() { " +
         $"{VideoPlayerCallbacks};" + "});";
            public static string DocumentReady = "$(document).ready(()=>{" + IsPlayingCallback+ PlayPauseButtonCallback + "});";
        }
    }
}