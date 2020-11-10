using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Util;
using BitChute;
using System.Threading.Tasks;
using BitChute.Fragments;
using Android.Net.Wifi;
using Android.Media;
using static BitChute.Models.VideoModel;
using static BitChute.Fragments.SettingsFrag;
using Android.Views;
using System.Reflection;
using System.ComponentModel;
using BitChute.Web;
using static BitChute.PlaystateManagement;

namespace BitChute.Services
{
    /// <summary>
    /// This is the main background sticky service that controls playback
    /// and notifications
    /// </summary>
    [Service(Exported = true)]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop, ActionTogglePlayback,
        ActionNext, ActionPrevious, ActionLoadUrl, ActionBkgrdNote, ActionResumeNote })]
    public class MainPlaybackSticky : Service, 
        MediaController.IMediaPlayerControl, MediaPlayer.IOnSeekCompleteListener
    {
        #region members

        //Actions
        public const string ActionPlay = "com.xamarin.action.PLAY";
        public const string ActionPause = "com.xamarin.action.PAUSE";
        public const string ActionStop = "com.xamarin.action.STOP";
        public const string ActionTogglePlayback = "com.xamarin.action.TOGGLEPLAYBACK";
        public const string ActionNext = "com.xamarin.action.NEXT";
        public const string ActionPrevious = "com.xamarin.action.PREVIOUS";
        public const string ActionLoadUrl = "com.xamarin.action.LOADURL";
        public const string ActionBkgrdNote = "com.xamarin.action.NOTIFICATIONSHOULDBKGRD";
        public const string ActionResumeNote = "com.xamarin.action.NOTIFICATIONSHOULDRESUME";

        public static bool NotificationShouldPlayInBkgrd = false;
        
        private static Java.Util.Timer _timer = new Java.Util.Timer();
        private static NotificationTimerTask _timerTask = new NotificationTimerTask();

        public delegate void PlaystateEventDelegate(PlaystateEventArgs _args);
        public static event PlaystateEventDelegate PlaystateChanged;

        public static MainPlaybackSticky ExtStickyServ;
        public static PowerManager Pm;

        public static WifiManager WifiManager;
        public static WifiManager.WifiLock WifiLock;
        public static AudioManager AudioMan;

        public static bool ServiceIsRunningInForeground = false;

        public static bool NotificationsHaveBeenSent = false;
        private static ExtNotifications _extNotifications = new ExtNotifications();
        
        private static ActivityManager.RunningAppProcessInfo _dProcess = new ActivityManager.RunningAppProcessInfo();
        private static int StartForegroundNotificationId = 6666;

        private static bool _notificationStackExecutionInProgress = false;
        private static bool _notificationLongTimerSet = false;

        public static Dictionary<int, MediaPlayer> MediaPlayerDictionary = new Dictionary<int, MediaPlayer>();
        public static Dictionary<int, ExtMediaController> MediaControllerDictionary
                     = new Dictionary<int, ExtMediaController>();
        
        #endregion

        public static void OnPlaystateChanged(PlaystateEventArgs e)
        {

        }

        #region mediaplayer
        /// <summary>
        /// initializes the mediaplayer object on tab of your choice.  
        /// if the mediaplayer is already instantiated then it gets reset for new playback
        /// </summary>
        /// <param name="mp"></param>
        /// <returns></returns>
        public static async Task<MediaPlayer> InitializePlayer(int id, Android.Net.Uri uri, Context ctx, string mediaPath = null)
        {
            if (ctx == null)
            {
                ctx = Android.App.Application.Context;
            }
            var cookieHeader = ExtWebInterface.GetCookieDictionary();

            if (!MainPlaybackSticky.MediaPlayerDictionary.ContainsKey(id))
            {
                MainPlaybackSticky.MediaPlayerDictionary.Add(id, new MediaPlayer());
            }
            else if (MediaPlayerDictionary[id] == null)
            {
                MediaPlayerDictionary[id] = new MediaPlayer();
            }
            else
            {
                MediaPlayerDictionary[id].Reset();
                MediaPlayerDictionary[id].Release();
                
                MediaPlayerDictionary[id] = null;
                MediaPlayerDictionary[id] = new MediaPlayer();
            }
            if (mediaPath != null) { await MediaPlayerDictionary[id].SetDataSourceAsync(mediaPath); }
            else 
            {
                //await MediaPlayerDictionary[id].SetDataSourceAsync(ctx, uri);
                await MediaPlayerDictionary[id].SetDataSourceAsync(Android.App.Application.Context, uri, cookieHeader);
            }
            MediaPlayerDictionary[id].PrepareAsync();
            
            PlaystateChanged.Invoke(new PlaystateEventArgs(-1, false, false, false, id, false, false, true, id));
            MediaPlayerDictionary[id].SetOnSeekCompleteListener(ExtStickyServ);

            //Wake mode will be partial to keep the CPU still running under lock screen
            MediaPlayerDictionary[id].SetWakeMode(Android.App.Application.Context, WakeLockFlags.Partial);

            //When we have prepared the song start playback
            MediaPlayerDictionary[id].Prepared += (sender, args) => ExtStickyServ.Play(id, PlayerType.NativeMediaPlayer);

            //When we have reached the end of the song stop ourselves, however you could signal next track here.
            MediaPlayerDictionary[id].Completion += (sender, args) => OnVideoFinished(false, id);

            MediaPlayerDictionary[id].Error += (sender, args) =>
            {
                //playback error
                Console.WriteLine("Error in playback resetting: " + args.What);
                Stop();//this will clean up and reset properly.
            };
            
            return MediaPlayerDictionary[id];
        }


        public static void InitializeMediaController(VideoView vv, int id)
        {
            if (!MainPlaybackSticky.MediaControllerDictionary.ContainsKey(id))
            {
                MainPlaybackSticky.MediaControllerDictionary.Add(id, ExtStickyServ.InitializeMediaController(Android.App.Application.Context));
            }
            vv.SetMediaController(MainPlaybackSticky.MediaControllerDictionary[id]);
            MainPlaybackSticky.MediaControllerDictionary[id].SetAnchorView((View)ViewHelpers.Main.ContentRelativeLayout.Parent);

            try
            {
                MainPlaybackSticky.MediaControllerDictionary[id].Enabled = true;
                MainPlaybackSticky.MediaControllerDictionary[id].Show(10000);
                //MainPlaybackSticky.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Show();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        public ExtMediaController InitializeMediaController(Context ctx)
        {
            var mc = new ExtMediaController(ctx);
            
            mc.SetMediaPlayer(this);
            return mc;
        }
        #endregion

        #region playback methods
        public void Play(int id = -1, PlayerType playerType = PlayerType.None)
        {

            if ((PlaystateManagement.MediaPlayerIsStreaming && PlaystateManagement.MediaPlayerIsQueued)
                || playerType == PlayerType.NativeMediaPlayer)
            {
                Task.Factory.StartNew(() =>
                {
                    if (MediaPlayerDictionary[id] != null)
                    {
                        MediaPlayerDictionary[id].Start();
                        return;
                    }
                    if (MediaPlayerDictionary[id].IsPlaying)
                    {
                        PlaystateChanged.Invoke(new PlaystateEventArgs(-1, false, false, false, id, false, false, false, id));
                    }
                });
            }
            else if (PlaystateManagement.PlayerTypeQueued() == PlayerType.WebViewPlayer)
            {
                Task.Factory.StartNew(() =>
                {
                    if (PlaystateManagement.WebViewPlayerNumberIsStreaming != -1)
                        StartVideoInBkgrd(PlaystateManagement.WebViewPlayerNumberIsStreaming);
                    else if (PlaystateManagement.MediaPlayerNumberIsQueued != -1)
                        StartVideoInBkgrd(PlaystateManagement.MediaPlayerNumberIsQueued);
                    else { StartVideoInBkgrd(); }
                });
            }
        }

        public static void SkipToPrev(int id = -1)
        {
            if (PlaystateManagement.MediaPlayerIsStreaming || PlaystateManagement.PlayerTypeQueued() == PlayerType.NativeMediaPlayer)
            {

            }
            if (!PlaystateManagement.MediaPlayerIsStreaming || PlaystateManagement.PlayerTypeQueued() == PlaystateManagement.PlayerType.WebViewPlayer)
            {
                try {
                    if (id == -1)
                    {
                        if (PlaystateManagement.GetWebViewPlayerById().CanGoBack())
                            PlaystateManagement.GetWebViewPlayerById().GoBack();
                    }
                    else
                    {
                        if (PlaystateManagement.GetWebViewPlayerById(id).CanGoBack())
                            PlaystateManagement.GetWebViewPlayerById(id).GoBack();
                    }
                }catch { }
            }
            else { }
        }

        /// <summary>
        /// skips to the next video on a WebView tab
        /// </summary>
        /// <param name="tab"></param>
        public static void SendWebViewNextVideoCommand(int id = -1)
        {
            if (id == -1) { id = PlaystateManagement.WebViewPlayerNumberIsStreaming; }
            if (id != -1) { PlaystateManagement.GetWebViewPlayerById(id).LoadUrl(JavascriptCommands._jsNextVideoByASpa); }
        }

        public static void SkipToNext(VideoCard vc)
        {

            if (!PlaystateManagement.MediaPlayerIsStreaming || PlaystateManagement.PlayerTypeQueued() == PlaystateManagement.PlayerType.WebViewPlayer)
            {
                SendWebViewNextVideoCommand();
            }
        }

        /// <summary>
        /// pause the video
        /// </summary>
        public void Pause() {
            AppIsMovingIntoBackgroundAndStreaming = false;
            PlaystateManagement.SendPauseVideoCommand();
        }

        public static void Stop(int id = -1, PlayerType playerType = PlayerType.None)
        {
            if (playerType == PlayerType.NativeMediaPlayer)
            {
                if (MediaPlayerDictionary[id] == null)
                    return;

                if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].IsPlaying)
                    MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Stop();

                MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Reset();
            }
        }

        public static bool OnVideoFinished(bool overide, int tab)
        {
            if (AppSettings.AutoPlay && tab != -1)
            {
                return overide;
            }

            return overide;
        }

        #endregion

        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        private static void ReleaseWifiLock()
        {
            if (WifiLock == null)
                return;

            WifiLock.Release();
            WifiLock = null;
        }

        protected void OnNewIntent(Intent intent)
        {
            string url = "";
            if (intent != null)
            {
                try { url = intent.Extras.GetString("URL"); }
                catch {   }
            }
            if (url == "" || url == null) { return; }
            try
            {
                PlaystateManagement.GetWebViewPlayerById(-1, MainActivity.ViewPager.CurrentItem).LoadUrl(url);
            }
            catch {   }
        }

        #region service methods
        /// <summary>
        /// When we start on the foreground we will present a notification to the user
        /// When they press the notification it will take them to the main page so they can control the music
        /// </summary>
        public static void StartForeground(Notification startNote, bool createNewNotification = false, bool overridePlaystate = false)
        {
            try
            {
                if (createNewNotification)
                {
                    AppState.ForeNote = null;
                    ExtStickyServ.StopForeground(true);
                    StartForegroundNotificationId = StartForegroundNotificationId++;
                    ExtStickyServ.StartForeground(StartForegroundNotificationId, startNote);
                }
                else
                {
                    ExtStickyServ.StartForeground(StartForegroundNotificationId, startNote);
                    ServiceIsRunningInForeground = true;
                }
                if (overridePlaystate)
                {
                    StartVideoInBkgrd(GetWebViewPlayerById(-1, MainActivity.ViewPager.CurrentItem).Id);
                }
                AppState.ForeNote = startNote;
            }
            catch (Exception ex)
            {

            }
        }

        #region StickyServiceMethods
        public MainPlaybackSticky(Context applicationContext)
        {

        }
        public MainPlaybackSticky()
        {

        }

        public MainPlaybackSticky GetStickyNotificationService()
        {
            return this;
        }

        public override void OnCreate()
        {
            PlaystateChanged += OnPlaystateChanged;
            ExtStickyServ = this;
            base.OnCreate();
            //Find our audio and notificaton managers
            AudioMan = (AudioManager)GetSystemService(AudioService);
            WifiManager = (WifiManager)GetSystemService(WifiService);
            
            
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


        public static void ExternalStopForeground()
        {
            try
            {
                //WifiLock?.Release();
                ExtStickyServ.StopForeground(StopForegroundFlags.Remove);
            }
            catch { }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            try
            {
                WifiLock?.Release();
            }
            catch { }
        }
        #endregion

        public static void LoadVideoFromUrl(Intent i = null, int tab = -1, string url = null)
        {
            string u = "";
            if (i != null)
            {
                try { u = i.GetStringExtra("URL"); }
                catch{ }
                if (u == null || u== "") { return; }
                else { url = u; }
            }
            if (!PlaystateManagement.MediaPlayerIsStreaming)
            {
                if (tab == -1) { tab = MainActivity.ViewPager.CurrentItem; }
                CommonFrag.GetFragmentById(-1, null, tab).Wv.LoadUrl(url);
            }
            else{ }
        }

        public bool ToggleNotificationShouldPlayInBkgrd(bool bkgrd = true)
        {
            NotificationShouldPlayInBkgrd = bkgrd;
            return NotificationShouldPlayInBkgrd;
        }
        
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            switch (intent?.Action)
            {
                case ActionPlay: Play(); break;
                case ActionStop: Stop(); break;
                case ActionPause: Pause(); break;
                case ActionNext: SkipToNext(null); break;
                case ActionPrevious: SkipToPrev(); break;
                case ActionLoadUrl: LoadVideoFromUrl(intent); break;
                case ActionBkgrdNote: ToggleNotificationShouldPlayInBkgrd(true); break;
                case ActionResumeNote: ToggleNotificationShouldPlayInBkgrd(false); break;
            }    

            try{ WifiManager = (WifiManager)GetSystemService(Context.WifiService); }
            catch{  }
            ExtStickyServ = this;
            try
            {
                Pm = (PowerManager)GetSystemService(Context.PowerService);
                PowerManager.WakeLock _wl = Pm.NewWakeLock(WakeLockFlags.Partial, "BitChute Wakelock");
                _wl.Acquire();
            }
            catch (Exception ex){ Console.WriteLine(ex.Message);}
            return StartCommandResult.Sticky;
        }
        public static volatile NotificationTimerTask _extTimerTask = new NotificationTimerTask();

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        public static void AquireWifiLock()
        {
            if (WifiLock == null)
            {
                WifiLock = WifiManager.CreateWifiLock(Android.Net.WifiMode.Full, "bitchute_wifi_lock");
            }
            WifiLock.Acquire();
        }

        #endregion

        static int NotificationLoopStartTimesInvoked = 0;
        /// <summary>
        /// starts/restarts the notifications, 
        /// takes a ms int as the delay for starting,
        /// if this is called with no delay TheFragment5 sometimes
        /// is null or has issues when it's methods are called
        /// immediately after the app initially loads.
        /// 
        /// Once the notifications are started this loop returns
        /// and invokes a long running TimerTask
        /// The reason for this is because the loop is
        /// stopping after a while so I am moving to a timer
        /// system for the long running task to see if that
        /// will prevent the loop from breaking.
        /// </summary>
        public static async void StartNotificationLoop(int delay, List<ExtNotifications.CustomNotification> initialNotifications = null, bool afterLogin = false)
        {
            NotificationLoopStartTimesInvoked++; 
            //wait on a delay so that the cookie is ready when we make
            //httprequest for the notifications
            await Task.Delay(delay);

            //use a while loop to start the notifications
            //they move over to a service timer eventually to prevent the loop from breaking
            while (AppSettings.Notifying)
            {
                if (!ExtNotifications.NotificationHttpRequestInProgress && !_notificationStackExecutionInProgress && AppState.UserIsLoggedIn)
                {
                    if (initialNotifications == null)
                    {
                        _notificationStackExecutionInProgress = true;
                        var noteText = await ExtWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
                        var noteList = await ExtNotifications.DecodeHtmlNotifications(noteText);
                        if (noteList.Count > 0)
                        {
                            ExtNotifications.SendNotifications(noteList);
                            NotificationsHaveBeenSent = true;
                        }
                        _notificationStackExecutionInProgress = false;
                    }
                    else
                    {
                        ExtNotifications.SendNotifications(initialNotifications);
                    }
                }
                if (NotificationsHaveBeenSent)
                {
                    //check to make sure the timer isn't already started or the app will crash
                    if (!MainPlaybackSticky._notificationLongTimerSet)
                    {
                        try
                        {
                            //after the initial notifications are sent, start the long running service timer task
                            _timer.ScheduleAtFixedRate(_extTimerTask, 500000, 780000); // 780000
                            _notificationLongTimerSet = true;
                            return;
                        }
                        catch { }
                    }
                }
                if (NotificationLoopStartTimesInvoked > 1) { NotificationLoopStartTimesInvoked--; break; }
                else if (!AppState.UserIsLoggedIn) { await Task.Delay(220000); }
                //user is logged in but has not yet received a notification
                else {  await Task.Delay(220000); }
            }
        }


        /// <summary>
        /// Timer task for background notifications
        /// has to be within the service so that it's more persistent
        /// </summary>
        public class NotificationTimerTask : Java.Util.TimerTask
        {
            public async override void Run()
            {
                if (AppSettings.Notifying)
                {
                    try
                    {
                        if (!ExtNotifications.NotificationHttpRequestInProgress && !_notificationStackExecutionInProgress)
                        {
                            _notificationStackExecutionInProgress = true;
                            var noteText = await ExtWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
                            var noteList = await ExtNotifications.DecodeHtmlNotifications(noteText);
                            ExtNotifications.SendNotifications(noteList);
                            _notificationStackExecutionInProgress = false;
                        }
                    }
                    catch {   }
                }
            }
        }

        /// <summary>
        /// returns true when the app detects that it's running
        /// in background
        /// </summary>
        /// <returns>is the app in background</returns>
        public static bool IsInBkGrd()
        {
            ActivityManager.GetMyMemoryState(_dProcess);

            if (_dProcess.Importance == Importance.Foreground)
            {
                AppState.Bkgrd = false;
                return false;
            }
            else  { AppState.Bkgrd = true; return true; }
        }

        public void OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain: break;
                case AudioFocus.Loss: break;
                case AudioFocus.LossTransientCanDuck: break;
            }
        }

        public int AudioSessionId { get { return 6; } }

        public int BufferPercentage
        {
            get
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    return 100;
                }
                return 0;
            }
        }

        public int CurrentPosition
        {
            get
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    return MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].CurrentPosition;
                }
                return 0;
            }
            set
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].SeekTo(value);
                }
            }
        }

        public int Duration {
            get
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    return MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].Duration;
                }
                return 0;
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
                {
                    return MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].IsPlaying;
                }
                else { return false; }
            }
        }

        public bool CanPause()
        {
            if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
            {
                return MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].IsPlaying;
            }
            return false;
        }

        public bool CanSeekBackward()
        {
            if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
            {
                if (MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].CurrentPosition > 0)
                    return true;
                else { return false; }
            }
            return false;
        }

        public bool CanSeekForward()
        {
            if (MediaPlayerDictionary.ContainsKey(PlaystateManagement.MediaPlayerNumberIsStreaming))
            {
                if (MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].Duration >
                    MediaPlayerDictionary[PlaystateManagement.MediaPlayerNumberIsStreaming].CurrentPosition)
                { return true; }
                else { return false; }
            }
            return false;
        }

        void ExtMediaController.IMediaPlayerControl.Pause()
        {
            Pause();
        }

        public void SeekTo(int pos)
        {
            CurrentPosition = pos;
        }

         void ExtMediaController.IMediaPlayerControl.Start()
         {
            Play();
         }

        public static bool AppIsMovingIntoBackgroundAndStreaming = false;

        /// <summary>
        /// starts the video in background
        /// </summary>
        /// <param name="tab"></param>
        public static async void StartVideoInBkgrd(int webViewId = -1, int mediaPlayerId = -1)
        {
            if (PlaystateManagement.WebViewPlayerIsStreaming || PlaystateManagement.PlayerTypeQueued() == PlayerType.WebViewPlayer)
            {
                if (webViewId == -1) { webViewId = PlaystateManagement.WebViewPlayerNumberIsStreaming; }
                PlaystateManagement.WebViewPlayerIsStreaming = true;
                await Task.Delay(10);
                ViewHelpers.DoActionOnUiThread(() =>
                {
                    PlaystateManagement.WebViewIdDictionary[webViewId].LoadUrl(JavascriptCommands._jsPlayVideo);
                });
                await Task.Delay(20);
                ViewHelpers.DoActionOnUiThread(() =>
                {
                    PlaystateManagement.WebViewIdDictionary[webViewId].LoadUrl(JavascriptCommands._jsPlayVideo);
                });
                VerifyInBackground(webViewId);
            }
        }

        static async void VerifyInBackground(int webViewId = -1, int backgroundTimeOut = 3000)
        {
            int backgroundTimer = 0;
            while (IsInBkGrd() && AppIsMovingIntoBackgroundAndStreaming && backgroundTimer <= 3000)
            {
                await Task.Delay(100);
                ViewHelpers.DoActionOnUiThread(() =>
                {
                    PlaystateManagement.WebViewIdDictionary[webViewId].LoadUrl(JavascriptCommands._jsPlayVideo);
                });
            }
            await Task.Delay(30);
            ViewHelpers.DoActionOnUiThread(() =>
            {
                PlaystateManagement.WebViewIdDictionary[webViewId].LoadUrl(JavascriptCommands._jsPlayVideo);
            });
        }

        public void OnSeekComplete(MediaPlayer mp)
        {

        }

        public class ServiceWebView : Android.Webkit.WebView
        {
            public override string Url => base.Url;
            private int _tabKey = -1;
            public int TabKey { get { return _tabKey; } }
            private static int _latestTabKey = -1;
            public string RootUrl;

            public static bool PlayerBufferingDetectedOnMinimize = false;
            public static List<int> WebViewPlayersWhereBufferingDetectedOnMinimized = new List<int>();
            public static List<int> AllWebViewPlayersWithPlayingState = new List<int>();
            public bool PlaybackShouldResumeOnMinimize = false;
            public bool PlayerIsLoadingOnMinimize = false;
            public bool ViewIsNoLongerVisible = false;
            public bool StickyBackgroundRequested = false;

            public override void OnWindowFocusChanged(bool hasWindowFocus)
            {
                base.OnWindowFocusChanged(hasWindowFocus);
                if (hasWindowFocus)
                {
                    //TotalWebViewsMovingIntoBackground = 0;
                    //ClearWebViewMinimizedState();
                }
                else if (!hasWindowFocus)
                {
                    if ((PlaybackShouldResumeOnMinimize && AppSettings.AutoPlayOnMinimized != "off") && !MainActivity.UserRequestedStickyBackground)
                    {
                        if (WebViewPlayersWhereBufferingDetectedOnMinimized.Count == 0 || AllWebViewPlayersWithPlayingState.Count < 2)
                        {
                            AwaitMinimizedInBackground();
                            this.LoadUrl(JavascriptCommands._jsPlayVideo);
                            PlaybackShouldResumeOnMinimize = false;
                        }
                        else if (WebViewPlayersWhereBufferingDetectedOnMinimized.Count > 0)
                        {
                            if (AllWebViewPlayersWithPlayingState.Count > 1)
                            {
                                if (WebViewPlayersWhereBufferingDetectedOnMinimized.Contains(this.Id))
                                {
                                    this.LoadUrl(JavascriptCommands._jsPauseVideo);
                                    PlaybackShouldResumeOnMinimize = false;
                                }
                            }
                            else
                            {
                                AwaitMinimizedInBackground();
                                this.LoadUrl(JavascriptCommands._jsPlayVideo);
                                PlaybackShouldResumeOnMinimize = false;
                            }
                        }
                    }
                    else if (MainActivity.UserRequestedStickyBackground && StickyBackgroundRequested)
                    {
                        AwaitMinimizedInBackground();
                        this.LoadUrl(JavascriptCommands._jsPlayVideo);
                        PlaybackShouldResumeOnMinimize = false;
                        StickyBackgroundRequested = false;
                    }
                    TotalWebViewsMovingIntoBackground--;
                }
            }
            public static int TotalWebViewsMovingIntoBackground;

            public async void AwaitMinimizedInBackground()
            {
                while (TotalWebViewsMovingIntoBackground > 0)
                {
                    await Task.Delay(50);

                        this.LoadUrl(JavascriptCommands._jsPlayVideo);
                }
                await Task.Delay(50);
                PlaystateManagement.WebViewPlayerNumberIsStreaming = this.Id;
                PlaystateManagement.WebViewPlayerIsStreaming = true;
                if (AppState.ForeNote == null)
                {
                    MainPlaybackSticky.StartForeground(BitChute.ExtNotifications.BuildPlayControlNotification());
                }
                ViewHelpers.DoActionOnUiThread(() =>
                {
                    this.LoadUrl(JavascriptCommands._jsPlayVideo);
                });
                await Task.Delay(100);
                ViewHelpers.DoActionOnUiThread(() =>
                {
                    this.LoadUrl(JavascriptCommands._jsPlayVideo);
                });
            }

            public static void ClearWebViewMinimizedState()
            {
                MainPlaybackSticky.ServiceWebView.TotalWebViewsMovingIntoBackground = 0;
                WebViewPlayersWhereBufferingDetectedOnMinimized.Clear();
                AllWebViewPlayersWithPlayingState.Clear();
            }

            public override void GoBack()
            {
                base.GoBack();
                BitChute.Web.ViewClients.RunBaseCommands((Android.Webkit.WebView)this);
            }
            
            public bool WvRl = true;
            /// <summary>
            /// one press refreshes the page; two presses pops back to the root
            /// </summary>
            public void Pop2Root()
            {
                if (WvRl)
                {
                    this.Reload();
                    WvRl = false;
                }
                else { this.LoadUrl(RootUrl); }


            }

            public async void LoadUrlWithDelay(string url, int delay = 1)
            {
                await Task.Delay(delay);
                this.LoadUrl(url);
            }

            public override void LoadUrl(string url)
            {
                base.LoadUrl(url);
            }

            public bool WvRling = false;
            /// <summary>
            /// this is to allow faster phones and connections the ability to Pop2Root
            /// used to be set without delay inside OnPageFinished but I don't think 
            /// that would work on faster phones
            /// </summary>
            public async void SetReload()
            {
                if (!WvRling)
                {
                    WvRling = true;
                    await Task.Delay(AppSettings.TabDelay);
                    WvRl = true;
                    WvRling = false;
                }
            }

            public ServiceWebView(Context context) : base(context) { }

            public ServiceWebView(Context context, IAttributeSet attrs) : base(context, attrs)
            {
                this.SetBackgroundColor(Android.Graphics.Color.Black);
                PlaystateManagement.WebViewIdDictionary.Add(this.Id, this);
                _latestTabKey++;
                this._tabKey = _latestTabKey;
                PlaystateManagement.WebViewTabDictionary.Add(this._tabKey, this);
                AppState.WebViewAgentString = this.Settings.UserAgentString;
                this.Settings.SetSupportMultipleWindows(true);

            }

            

            public ServiceWebView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
            {
            }

            public ServiceWebView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
            {
            }

            protected ServiceWebView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }
        }
    }
}

