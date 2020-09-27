using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static BitChute.Classes.PlaystateManagement;

namespace BitChute.Classes
{
    public class PlaystateManagement
    {
        private static bool _webViewMediaPlayerIsStreaming = false;
        private static bool _webViewMediaPlayerAutoPlayDetected = false;
        private static int _webViewMediaPlayerNumberIsStreaming = -1;

        public class PlaystateEventArgs : EventArgs
        {
            public PlaystateEventArgs(
                int webViewNumber = -1, bool webViewPlayRequested = false, bool webViewPauseRequested = false,
                bool autoPlayDetected = false, int nativeMediaPlayerNumber = -1, bool nativeMediaPlayerPlayRequested = false,
                bool nativeMediaPlayerStopRequested = false)
            {
                if (webViewPlayRequested)
                {
                    if (!WebViewPlayerIsStreaming) { WebViewPlayerIsStreaming = true;
                        WebViewPlayerNumberIsStreaming = MainActivity.ViewPager.CurrentItem; }
                    else { MediaPlayerIsStreaming = false; }
                }
                else if (webViewPauseRequested)
                {
                    WebViewPlayerIsStreaming = false;
                    WebViewPlayerNumberIsStreaming = MainActivity.ViewPager.CurrentItem;
                    MediaPlayerIsStreaming = false; 
                }
                if (autoPlayDetected)
                {
                    WebViewPlayerAutoPlayDetected = autoPlayDetected;
                }
            }
            public static bool WebViewMediaPlayerIsStreaming { get { return WebViewPlayerIsStreaming; } }
            public static int WebViewMediaPlayerNumberIsStreaming { get { return WebViewPlayerNumberIsStreaming; } }
            public static bool NativeMediaPlayerIsStreaming { get { return MediaPlayerIsStreaming; } }
            public static int NativeMediaPlayerNumberIsStreaming { get { return MediaPlayerNumberIsStreaming; } }
        }
        
        /// <summary>
        /// there are different media players and this keeps track of which one is playing
        /// 
        /// returns -1 if none are playing otherwise it's 0-? coinciding with the tab played from
        /// </summary>
        public static int MediaPlayerNumberIsStreaming { get; set; }

        /// <summary>
        /// this bool is set true when a native media player is currently streaming
        /// it should be set to false when WebView is streaming audio
        /// </summary>
        public static bool MediaPlayerIsStreaming = false;

        /// <summary>
        /// this keeps track of which webview is currently streaming video
        /// 
        /// set to any int and -1 means none is currently streaming
        /// </summary>
        public static int WebViewPlayerNumberIsStreaming
        {
            get { return _webViewMediaPlayerNumberIsStreaming; }
            set { _webViewMediaPlayerNumberIsStreaming = value; }
        } 

        public static bool WebViewPlayerIsStreaming
        {
            get { return _webViewMediaPlayerIsStreaming; }
            set { _webViewMediaPlayerIsStreaming = value; }
        }

        public static bool WebViewPlayerAutoPlayDetected
        {
            get { return _webViewMediaPlayerIsStreaming; }
            set { _webViewMediaPlayerAutoPlayDetected = value;
                if (_webViewMediaPlayerAutoPlayDetected)
                {
                    _webViewMediaPlayerIsStreaming = value;
                    WebViewPlayerNumberIsStreaming = MainActivity.ViewPager.CurrentItem;
                }
            }
        }

        public static bool UserRequestedBackgroundPlayback = false;
    }
}