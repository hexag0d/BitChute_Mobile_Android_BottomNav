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
using Android.Util;

using Android.Telephony;
using BitChute.Classes;
using BitChute;
using System.Threading.Tasks;
using BitChute.Fragments;
using Android.Net.Wifi;
using Android.Media;
using Android.Support.V4.App;
using BitChute.Models;
using static BitChute.Models.VideoModel;

using BitChute;

namespace StartServices.Servicesclass
{
    [Service(Exported = true)]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop, ActionTogglePlayback, ActionNext, ActionPrevious })]
    public class ExtStickyService : Service, AudioManager.IOnAudioFocusChangeListener,
        MediaController.IMediaPlayerControl
    {
        #region members

        //Actions
        public const string ActionPlay = "com.xamarin.action.PLAY";
        public const string ActionPause = "com.xamarin.action.PAUSE";
        public const string ActionStop = "com.xamarin.action.STOP";
        public const string ActionTogglePlayback = "com.xamarin.action.TOGGLEPLAYBACK";
        public const string ActionNext = "com.xamarin.action.NEXT";
        public const string ActionPrevious = "com.xamarin.action.PREVIOUS";

        private static bool _serviceIsLooping = false;
        public static MainActivity Main;

        private static BitChute.Fragments.TheFragment4.ExtWebInterface _extWebInterface =
            BitChute.Fragments.TheFragment4._extWebInterface;

        private static Java.Util.Timer _timer = new Java.Util.Timer();
        private static ExtTimerTask _timerTask = new ExtTimerTask();

        public static ExtStickyService ExtStickyServ;
        public static PowerManager Pm;

        public static WifiManager WifiManager;
        public static WifiManager.WifiLock wifiLock;
        public static AudioManager AudioMan;
        

        private static bool _backgroundTimeout = false;
        public static bool NotificationsHaveBeenSent = false;
        private static ExtNotifications _extNotifications = new ExtNotifications();
        private static TheFragment4 _fm5;
        private static ActivityManager.RunningAppProcessInfo _myProcess = new ActivityManager.RunningAppProcessInfo();
        private static int _startForegroundNotificationId = 6666;

        private static bool _notificationStackExecutionInProgress = false;
        private static bool _notificationLongTimerSet = false;

        public static Dictionary<int, MediaPlayer> MediaPlayerDictionary = new Dictionary<int, MediaPlayer>();
        public static Dictionary<int, ExtMediaController> MediaControllerDictionary
                     = new Dictionary<int, ExtMediaController>();

        public static int PlayerNumberHasFocus = 0;

        private static bool _paused;
        //private static VideoDetailLoader _vidLoader = new VideoDetailLoader();

        #endregion


        /// <summary>
        /// initializes the mediaplayer object on tab of your choice.  
        /// if the mediaplayer is already instantiated then it gets reset for new playback
        /// </summary>
        /// <param name="mp"></param>
        /// <returns></returns>
        public static MediaPlayer InitializePlayer(int tab, Android.Net.Uri uri, Context ctx)
        {
            if (ctx == null)
            {
                ctx = Android.App.Application.Context;
            }
            bool tbo = false;
            if (tab == -1)
            {
                tab = MainActivity.ViewPager.CurrentItem;
            }
            if (tab == -2)
            {
                tbo = true;
                tab = -1;
            }

            // we might be able to eventually just use one media player but I think the buffering will be better
            // with a few of them, plus this way you can queue up videos and instantly switch
            if (!ExtStickyService.MediaPlayerDictionary.ContainsKey(tab))
            {
                ExtStickyService.MediaPlayerDictionary.Add(tab, new MediaPlayer());
            }
            else if (MediaPlayerDictionary[tab] == null)
            {
                MediaPlayerDictionary[tab] = new MediaPlayer();
            }
            //I tried to figure out how to switch the data source on an existing media player and eventually gave up lol
            else
            {
                MediaPlayerDictionary[tab].Reset();
                MediaPlayerDictionary[tab].Release();

                //this is odd, have to set the media player back to null and re-instantiate every time the video loads
                //I couldn't get it working without doing this
                MediaPlayerDictionary[tab] = null;
                MediaPlayerDictionary[tab] = new MediaPlayer();
            }

            if (uri != null)
            {
                MediaPlayerDictionary[tab].SetDataSource(ctx, uri);
            }

            if (tab != 1)
                AppState.MediaPlayback.MediaPlayerNumberIsStreaming = tab;

            //Wake mode will be partial to keep the CPU still running under lock screen
            MediaPlayerDictionary[tab].SetWakeMode(Android.App.Application.Context, WakeLockFlags.Partial);

            //When we have prepared the song start playback
            MediaPlayerDictionary[tab].Prepared += (sender, args) => ExtStickyServ.Play();

            //When we have reached the end of the song stop ourselves, however you could signal next track here.
            MediaPlayerDictionary[tab].Completion += (sender, args) => OnVideoFinished(false, tab);

            MediaPlayerDictionary[tab].Error += (sender, args) =>
            {
                //playback error
                Console.WriteLine("Error in playback resetting: " + args.What);
                Stop();//this will clean up and reset properly.
            };

            if (!tbo)
                ExtStickyService.MediaPlayerDictionary[tab].Prepare();

            return MediaPlayerDictionary[tab];
        }

        public ExtMediaController InitializeMediaController(Context ctx)
        {
            var mc = new ExtMediaController(ctx);
            mc.SetMediaPlayer(this);
            return mc;
        }

        private async void Play()
        {
            if (AppState.MediaPlayback.MediaPlayerIsStreaming)
            {
                await Task.Run(() =>
                {
                    if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem] != null)
                    {
                    //We are simply paused so just start again
                    MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Start();
                    //StartForeground();
                    return;
                    }

                    try
                    {
                        AquireWifiLock();
                    }
                    catch (Exception ex)
                    {
                    //unable to start playback log error
                    Console.WriteLine("Unable to start playback: " + ex);
                    }

                    if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].IsPlaying)
                    {
                        AppState.MediaPlayback.MediaPlayerNumberIsStreaming = MainActivity.ViewPager.CurrentItem;
                    }
                });
            }
            else
            {
                StartVideoInBkgrd(MainActivity.ViewPager.CurrentItem);
            }
        }

        public static void SkipToPrev()
        {
            ExtStickyServ.CurrentPosition = 0;
        }

        /// <summary>
        /// skips to the next video on a WebView tab
        /// </summary>
        /// <param name="tab"></param>
        public static void SendWebViewNextVideoCommand(int tab)
        {
            switch (tab)
            {
                case 0: TheFragment0.Wv.LoadUrl(JavascriptCommands._jsNextVideoByASpa); break;
                case 1: TheFragment1.Wv.LoadUrl(JavascriptCommands._jsNextVideoByASpa); break;
                case 2: TheFragment2.Wv.LoadUrl(JavascriptCommands._jsNextVideoByASpa); break;
                case 3: TheFragment3.Wv.LoadUrl(JavascriptCommands._jsNextVideoByASpa); break;
                case 4: TheFragment4.Wv.LoadUrl(JavascriptCommands._jsNextVideoByASpa); break;
            }
        }

        public static void SkipToNext(VideoCard vc)
        {
            if (!AppState.MediaPlayback.MediaPlayerIsStreaming)
            {
                SendWebViewNextVideoCommand(MainActivity.ViewPager.CurrentItem);
            }
            else
            {
                if (vc == null)
                {
                    //TabStates.Tab1.VideoCardLoader = TabStates.Main.NextUp.NextUpVideoCard;
                    //    _vidLoader.LoadVideoFromCard(CustomViewHelpers.Main.GetDefaultVideoDetailView(
                    //        MainActivity.ViewPager.CurrentItem), null, TabStates.Main.NextUp.NextUpVideoCard, 
                    //        MainActivity.ViewPager.CurrentItem);
                }
            }
        }

        private void Pause()
        {
            if (!AppState.MediaPlayback.MediaPlayerIsStreaming)
            {
                HeadphoneIntent.ControlIntentReceiver.SendPauseVideoCommand();
            }
            else
            {
                if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem] == null)
                    return;

                if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].IsPlaying)
                    MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Pause();
            }

            //StopForeground(false);
            //_paused = true;
        }

        public static void Stop()
        {
            if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem] == null)
                return;

            if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].IsPlaying)
                MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Stop();

            MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Reset();
            _paused = false;
            ExtStickyServ.StopForeground(true);
            ReleaseWifiLock();
            //AppState.MediaPlayback.MediaPlayerNumberIsStreaming = -1;
        }

        public static bool OnVideoFinished(bool overide, int tab)
        {
            if (AppSettings.AutoPlay && tab != -1)
            {
                //_vidLoader.LoadVideoFromCard(ViewHelpers.Main.GetDefaultVideoDetailView(tab), null, TabStates.Main.NextUp.NextUpVideoCard, tab);
                return overide;
            }

            return overide;
        }


        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        private static void ReleaseWifiLock()
        {
            if (wifiLock == null)
                return;

            wifiLock.Release();
            wifiLock = null;
        }

        /// <summary>
        /// When we start on the foreground we will present a notification to the user
        /// When they press the notification it will take them to the main page so they can control the music
        /// </summary>
        public static void StartForeground(Notification startNote)
        {
            try
            {
                ExtStickyServ.StartForeground(MainActivity.NOTIFICATION_ID, startNote);
                MainActivity.NOTIFICATION_ID++;
            }
            catch
            {

            }
        }

        #region StickyServiceMethods
        public ExtStickyService(Context applicationContext)
        {

        }
        public ExtStickyService()
        {

        }

        public ExtStickyService GetStickyNotificationService()
        {
            return this;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            //Find our audio and notificaton managers
            AudioMan = (AudioManager)GetSystemService(AudioService);
            WifiManager = (WifiManager)GetSystemService(WifiService);
            ExtStickyServ = this;
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            switch (intent.Action)
            {
                case ActionPlay: Play(); break;
                case ActionStop: Stop(); break;
                case ActionPause: Pause(); break;
                case ActionNext: SkipToNext(null); break;
            }

            WifiManager = (WifiManager)GetSystemService(Context.WifiService);
            Main = MainActivity.Main;
            ExtStickyServ = this;

            try
            {
                Pm = (PowerManager)GetSystemService(Context.PowerService);
                PowerManager.WakeLock _wl = Pm.NewWakeLock(WakeLockFlags.Partial, "BitChute Wakelock");
                _wl.Acquire();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return StartCommandResult.Sticky;
        }
        //public static Timer timer;
        //private TimerTask timerTask;
        public static volatile ExtTimerTask _extTimerTask = new ExtTimerTask();



        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        public static void AquireWifiLock()
        {
            if (wifiLock == null)
            {
                wifiLock = WifiManager.CreateWifiLock(Android.Net.WifiMode.Full, "bitchute_wifi_lock");
            }
            wifiLock.Acquire();
        }

        #endregion



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
        public static async void StartNotificationLoop(int delay)
        {
            //wait on a delay so that the cookie is ready when we make
            //httprequest for the notifications
            await Task.Delay(delay);

            if (_fm5 == null)
            {
                await Task.Run(() => _fm5 = MainActivity.Fm4);
            }

            //use a while loop to start the notifications
            //they move over to a service timer eventually to prevent the loop from breaking
            while (AppSettings.Notifying)
            {
                if (!TheFragment4._notificationHttpRequestInProgress && !_notificationStackExecutionInProgress)
                {
                    _notificationStackExecutionInProgress = true;
                    await _extWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
                    await _extNotifications.DecodeHtmlNotifications(TheFragment4.ExtWebInterface._htmlCode);
                    _fm5.SendNotifications(ExtNotifications._customNoteList);
                    _notificationStackExecutionInProgress = false;
                }
                if (ExtStickyService.NotificationsHaveBeenSent)
                {
                    //check to make sure the timer isn't already started or the app will crash
                    if (!ExtStickyService._notificationLongTimerSet)
                    {
                        //after the initial notifications are sent, start the long running service timer task
                        _timer.ScheduleAtFixedRate(ExtStickyService._extTimerTask, 500000, 780000); // 780000
                        _notificationLongTimerSet = true;
                    }
                    return;
                }
                else if (!AppState.UserIsLoggedIn)
                {
                    await Task.Delay(380000);
                }
                //user is logged in but has not yet received a notification
                else
                {
                    await Task.Delay(380000);
                }
            }
        }

        public override void OnDestroy()
        {
            try
            {
                base.OnDestroy();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Timer task for background notifications
        /// has to be within the service so that it's more persistent
        /// </summary>
        public class ExtTimerTask : Java.Util.TimerTask
        {
            public async override void Run()
            {
                if (AppSettings.Notifying)
                {
                    if (_fm5 == null)
                    {
                        await Task.Run(() => _fm5 = MainActivity.Fm4);
                    }
                    try
                    {
                        if (!TheFragment4._notificationHttpRequestInProgress && !_notificationStackExecutionInProgress)
                        {
                            _notificationStackExecutionInProgress = true;
                            await _extWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
                            await _extNotifications.DecodeHtmlNotifications(TheFragment4.ExtWebInterface._htmlCode);
                            _fm5.SendNotifications(ExtNotifications._customNoteList);
                            _notificationStackExecutionInProgress = false;
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        public static bool DummyLoop()
        {

            var dummyVar = true;
            return dummyVar;
        }


        /// <summary>
        /// returns true when the app detects that it's running
        /// in background
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsInBkGrd()
        {
            ActivityManager.GetMyMemoryState(_myProcess);

            if (_myProcess.Importance == Importance.Foreground)
            {
                AppState.Bkgrd = false;
                return false;
            }
            else
            {
                AppState.Bkgrd = true;
                return true;
            }
        }

        public void OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
        {

        }

        public int AudioSessionId { get { return 6; } }

        public int BufferPercentage
        {
            get
            {
                return 100;
            }
        }

        public int CurrentPosition
        {
            get
            {
                return MediaPlayerDictionary[AppState.MediaPlayback.MediaPlayerNumberIsStreaming].CurrentPosition;
            }
            set
            {
                MediaPlayerDictionary[AppState.MediaPlayback.MediaPlayerNumberIsStreaming].SeekTo(value);
            }
        }

        public int Duration { get { return MediaPlayerDictionary[AppState.MediaPlayback.MediaPlayerNumberIsStreaming].Duration; } }

        public bool IsPlaying
        {
            get
            {
                return MediaPlayerDictionary[AppState.MediaPlayback.MediaPlayerNumberIsStreaming].IsPlaying;
            }
        }

        public bool CanPause()
        {
            return MediaPlayerDictionary[AppState.MediaPlayback.MediaPlayerNumberIsStreaming].IsPlaying;
        }

        public bool CanSeekBackward()
        {
            return true;
        }

        public bool CanSeekForward()
        {
            return true;
        }

        void MediaController.IMediaPlayerControl.Pause()
        {
            Pause();
        }

        public void SeekTo(int pos)
        {
            CurrentPosition = pos;
        }

        void MediaController.IMediaPlayerControl.Start()
        {
            Play();
        }

        /// <summary>
        /// starts the video in background
        /// </summary>
        /// <param name="tab"></param>
        public static async void StartVideoInBkgrd(int tab)
        {
            await Task.Delay(1);
            switch (tab)
            {
                case 0:
                    TheFragment0.Wv.LoadUrl(JavascriptCommands._jsPlayVideo);
                    break;
                case 1:
                    TheFragment1.Wv.LoadUrl(JavascriptCommands._jsPlayVideo);
                    break;
                case 2:
                    TheFragment2.Wv.LoadUrl(JavascriptCommands._jsPlayVideo);
                    break;
                case 3:
                    TheFragment3.Wv.LoadUrl(JavascriptCommands._jsPlayVideo);
                    break;
                case 4:
                    TheFragment4.Wv.LoadUrl(JavascriptCommands._jsPlayVideo);
                    break;
            }
        }

        public class ServiceWebView : Android.Webkit.WebView
        {
            public override string Url => base.Url;
            public ExtStickyService _serviceContext;

            //public override void OnWindowFocusChanged(bool hasWindowFocus)
            //{
            //    base.OnWindowFocusChanged(hasWindowFocus);
                //StartVideoInBkgrd(MainActivity.ViewPager.CurrentItem);
                //if (MainActivity._backgroundRequested)
                //{
                //    try
                //    {
                //        Pm = (PowerManager)ExtStickyServ.GetSystemService(Context.PowerService);
                //        PowerManager.WakeLock _wl = Pm.NewWakeLock(WakeLockFlags.Partial, "My Tag");
                //        _wl.Acquire();
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex.Message);
                //    }
                //    try
                //    {
                //        if (wifiLock == null)
                //        {
                //            wifiLock = WifiManager.CreateWifiLock(Android.Net.WifiMode.Full, "bitchute_wifi_lock");
                //        }
                //        wifiLock.Acquire();
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex.Message);
                //    }
                //    while (ExtStickyService.IsInBkGrd())
                //    {
                //        var dontSleep = DummyLoop();
                //        System.Threading.Thread.Sleep(3600);
                //    }
                //    try
                //    {
                //        Pm = (PowerManager)ExtStickyServ.GetSystemService(Context.PowerService);
                //        PowerManager.WakeLock _wl = Pm.NewWakeLock(WakeLockFlags.Partial, "My Tag");
                //        _wl.Release();
                //    }
                //    catch
                //    {

                //    }
                //}
                //MainActivity._backgroundRequested = false;
            //}

            public ServiceWebView(Context context) : base(context)
            {
            }

            public ServiceWebView(Context context, IAttributeSet attrs) : base(context, attrs)
            {
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

