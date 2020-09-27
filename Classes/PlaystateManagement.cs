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
        public class PlaystateEventArgs : EventArgs
        {
            private static bool _webViewMediaPlayerIsStreaming;
            private static int _webViewMediaPlayerNumberIsStreaming;
            private static bool _nativeMediaPlayerIsStreaming;
            private static int _nativeMediaPlayerNumberIsStreaming;

            public PlaystateEventArgs(
                int webViewNumber = -1, bool webViewPlayRequested = false,
                int nativeMediaPlayerNumber = -1, bool nativeMediaPlayerPlayRequested = false,
                bool nativeMediaPlayerStopRequested = false) {
                if (webViewPlayRequested)
                {
                   if (!MediaPlayerIsStreaming) { MediaPlayerIsStreaming = true; }
                   else { MediaPlayerIsStreaming = false; }
                }
                else
                {

                }
            }
            public static bool WebViewMediaPlayerIsStreaming { get { return _webViewMediaPlayerIsStreaming; } }
            public static int WebViewMediaPlayerNumberIsStreaming { get { return _webViewMediaPlayerNumberIsStreaming; } }
            public static bool NativeMediaPlayerIsStreaming { get { return _nativeMediaPlayerIsStreaming; } }
            public static int NativeMediaPlayerNumberIsStreaming { get { return _nativeMediaPlayerNumberIsStreaming; } }
        }

        private static void PlayPauseRequested()
        {
            if (WebViewPlayerIsStreaming)
            {
                WebViewPlayerIsStreaming = false;
                WebViewPlayerNumberIsStreaming = -1;
            }
            else
            {
                WebViewPlayerIsStreaming = true;
                WebViewPlayerNumberIsStreaming = MainActivity.ViewPager.CurrentItem;
            }
        }

            /// <summary>
            /// there are 5 different media players and this keeps track of which one is playing
            /// 
            /// returns -1 if none are playing otherwise it's 0-4 coinciding with the tab played from
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
            public static int WebViewPlayerNumberIsStreaming { get; set; }

            public static bool WebViewPlayerIsStreaming = false;

            public static bool UserRequestedBackgroundPlayback = false;
        
    }
}