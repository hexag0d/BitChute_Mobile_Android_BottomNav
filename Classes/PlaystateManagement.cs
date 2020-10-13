using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Fragments;
using BitChute.Services;
using static BitChute.PlaystateManagement;
using static BitChute.Services.MainPlaybackSticky;

namespace BitChute
{
    public class PlaystateManagement
    {
        private static bool _webViewMediaPlayerIsStreaming = false;
        private static bool _webViewMediaPlayerAutoPlayDetected = false;
        private static int _webViewMediaPlayerNumberIsStreaming = -1;

        public class PlaystateEventArgs : EventArgs
        {
            public PlaystateEventArgs(
                int webViewId = -1, bool webViewPlayRequested = false, bool webViewPauseRequested = false,
                bool webViewAutoPlayDetected = false, int nativeMediaPlayerNumber = -1, bool nativeMediaPlayerPlayRequested = false,
                bool nativeMediaPlayerStopRequested = false)
            {
                if (webViewPlayRequested)
                {
                    if (!WebViewPlayerIsStreaming) {
                        WebViewPlayerIsStreaming = true;
                        WebViewPlayerNumberIsStreaming = webViewId;
                        PlayerTypeQueued(PlayerType.WebViewPlayer);
                    }
                    else { MediaPlayerIsStreaming = false; }
                }
                else if (webViewPauseRequested)
                {
                    WebViewPlayerIsStreaming = false;
                    WebViewPlayerNumberIsStreaming = webViewId;
                    MediaPlayerIsStreaming = false;
                    PlayerTypeQueued(PlayerType.WebViewPlayer);
                }
                if (webViewAutoPlayDetected)
                {
                    WebViewPlayerAutoPlayDetected(webViewId);
                    PlayerTypeQueued(PlayerType.WebViewPlayer);
                }
            }
            public bool WebViewMediaPlayerIsStreaming { get { return WebViewPlayerIsStreaming; } }
            public int WebViewMediaPlayerNumberIsStreaming { get { return WebViewPlayerNumberIsStreaming; } }
            public bool NativeMediaPlayerIsStreaming { get { return MediaPlayerIsStreaming; } }
            public int NativeMediaPlayerNumberIsStreaming { get { return MediaPlayerNumberIsStreaming; } }
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

        public static int WebViewPlayerAutoPlayDetected(int webViewId)
        {
            _webViewMediaPlayerNumberIsStreaming = webViewId;
            _webViewMediaPlayerAutoPlayDetected = true;
            _webViewMediaPlayerIsStreaming = true;
            return webViewId;
        }

        private static PlayerType _playerTypeIsQueued { get; set; }

        /// <summary>
        /// Gets the player type that will be used for queueing and playing videos.
        /// 
        ///   If passed PlayerType.None as argument returns the last known queue type
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public static PlayerType PlayerTypeQueued(PlayerType playerType = PlayerType.None){
            if (playerType == PlayerType.None) { playerType = _playerTypeIsQueued; }
            else { _playerTypeIsQueued = playerType; }
            return _playerTypeIsQueued;
        }

        public enum PlayerType
        {
            None,
            WebViewPlayer,
            NativeMediaPlayer
        };

        public static Dictionary<int, WebView> WebViewIdDictionary = new Dictionary<int, WebView>();

        public static WebView GetWebViewPlayerById(int id = -1)
        {
            if (id == -1) { id = _webViewMediaPlayerNumberIsStreaming; }
            return WebViewIdDictionary[id];
        }

        public static void SendPauseVideoCommand()
        {
            if (PlaystateManagement.WebViewPlayerIsStreaming)
            {
                try
                {
                    GetWebViewPlayerById(_webViewMediaPlayerNumberIsStreaming).LoadUrl(JavascriptCommands._jsPauseVideo);
                    PlaystateManagement.WebViewPlayerIsStreaming = false;
                    PlayerTypeQueued(PlayerType.WebViewPlayer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (PlaystateManagement.MediaPlayerIsStreaming)
            {
                PlayerTypeQueued(PlayerType.NativeMediaPlayer);
                try
                {
                    if (MainPlaybackSticky.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem] == null)
                        return;
                    if (MainPlaybackSticky.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].IsPlaying)
                        MainPlaybackSticky.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Pause();
                    PlaystateManagement.MediaPlayerIsStreaming = false;
                }
                catch { }
            }
        }

        public static bool UserRequestedBackgroundPlayback = false;
    }
}