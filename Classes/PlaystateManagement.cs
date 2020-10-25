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
        private static bool _nativeMediaPlayerIsStreaming = false;
        private static int _nativeMediaPlayerNumberIsStreaming = -1;
        private static bool _nativeMediaPlayerIsQueued = false;
        private static int _nativeMediaPlayerNumberIsQueued = -1;

        public class PlaystateEventArgs : EventArgs
        {
            public PlaystateEventArgs(
                int webViewId = -1, bool webViewPlayRequested = false, bool webViewPauseRequested = false,
                bool webViewAutoPlayDetected = false, int nativeMediaPlayerNumber = -1, bool nativeMediaPlayerPlayRequested = false,
                bool nativeMediaPlayerStopRequested = false, bool nativeMediaPlayerQueueRequested = false, int nativeMediaPlayerNumberQueued = -1)
            {
                if (webViewPlayRequested)
                {
                    if (!WebViewPlayerIsStreaming) {
                        WebViewPlayerIsStreaming = true;
                        WebViewPlayerNumberIsStreaming = webViewId;
                        PlayerTypeQueued(PlayerType.WebViewPlayer);
                        PlayerTypeCurrentlyStreaming(PlayerType.WebViewPlayer);
                    }
                    else { MediaPlayerIsStreaming = false; }
                }
                else if (webViewPauseRequested)
                {
                    WebViewPlayerIsStreaming = false;
                    WebViewPlayerNumberIsStreaming = webViewId;
                    MediaPlayerIsStreaming = false;
                    PlayerTypeQueued(PlayerType.WebViewPlayer);
                    PlayerTypeCurrentlyStreaming(PlayerType.WebViewPlayer);
                }
                if (webViewAutoPlayDetected)
                {
                    WebViewPlayerAutoPlayDetected(webViewId);
                    PlayerTypeQueued(PlayerType.WebViewPlayer);
                    PlayerTypeCurrentlyStreaming(PlayerType.WebViewPlayer);
                }
                else if (nativeMediaPlayerPlayRequested)
                {
                    _nativeMediaPlayerIsStreaming = true;
                    _nativeMediaPlayerNumberIsStreaming = nativeMediaPlayerNumber;
                    _webViewMediaPlayerIsStreaming = false;
                    PlayerTypeQueued(PlayerType.NativeMediaPlayer);
                    PlayerTypeCurrentlyStreaming(PlayerType.NativeMediaPlayer);
                }
            }
            public bool WebViewMediaPlayerIsStreaming { get { return WebViewPlayerIsStreaming; } }
            public int WebViewMediaPlayerNumberIsStreaming { get { return WebViewPlayerNumberIsStreaming; } }
            public bool NativeMediaPlayerIsStreaming { get { return MediaPlayerIsStreaming; } }
            public int NativeMediaPlayerNumberIsStreaming { get { return MediaPlayerNumberIsStreaming; } }
            public int NativeMediaPlayerNumberIsQueued { get { return _nativeMediaPlayerNumberIsQueued; } }
            public bool NativeMediaPlayerIsQueued { get { return _nativeMediaPlayerIsQueued; } }
        }

        /// <summary>
        /// 
        /// the int is the id of the fragment that spawned the player
        /// </summary>
        public static int MediaPlayerNumberIsStreaming {
            get { return _nativeMediaPlayerNumberIsStreaming; }
            set { _nativeMediaPlayerNumberIsStreaming = value; }
        }

        /// <summary>
        /// this bool is set true when a native media player is currently streaming
        /// it should be set to false when WebView is streaming audio
        /// </summary>
        public static bool MediaPlayerIsStreaming
        {
            get { return _nativeMediaPlayerIsStreaming; }
            set { _nativeMediaPlayerIsStreaming = value; }
        }

        public static int MediaPlayerNumberIsQueued {
            get { return _nativeMediaPlayerNumberIsQueued; }
            set { _nativeMediaPlayerNumberIsQueued = value; }
        }

        public static bool MediaPlayerIsQueued {
            get { return _nativeMediaPlayerIsQueued; }
            set { _nativeMediaPlayerIsQueued = value; }
        }

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

        private static PlayerType _playerTypeIsQueued = PlayerType.WebViewPlayer;

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

        private static PlayerType _playerTypeCurrentlyStreaming = PlayerType.None;
        public static PlayerType PlayerTypeCurrentlyStreaming(PlayerType playerType = PlayerType.None)
        {
            if (playerType != PlayerType.None) { _playerTypeCurrentlyStreaming = playerType; return _playerTypeCurrentlyStreaming; }
            if (playerType == PlayerType.None) { _playerTypeCurrentlyStreaming = playerType; return _playerTypeCurrentlyStreaming; }
            return _playerTypeCurrentlyStreaming;
        }

        public enum PlayerType
        {
            None,
            WebViewPlayer,
            NativeMediaPlayer
        };

        public static Dictionary<int, WebView> WebViewIdDictionary = new Dictionary<int, WebView>();
        public static Dictionary<int, WebView> WebViewTabDictionary = new Dictionary<int, WebView>();

        public static WebView GetWebViewPlayerById(int id = -1, int tabNo = -1)
        {
            if (id == -1 && tabNo == -1)
            {
                id = _webViewMediaPlayerNumberIsStreaming;
                return WebViewIdDictionary[id];
            }
            else if (id == -1 && tabNo != -1)
            {
                return WebViewTabDictionary[tabNo];
            }
            else if (id != -1)
            {
                return WebViewIdDictionary[id];
            }
            else { return WebViewIdDictionary?.First().Value; }
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