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


namespace StartServices.Servicesclass
{
    [Service(Exported = true)]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop })]
    public class ExtStickyService : Service, AudioManager.IOnAudioFocusChangeListener, MediaController.IMediaPlayerControl
    {
        #region members

        //Actions
        public const string ActionPlay = "com.xamarin.action.PLAY";
        public const string ActionPause = "com.xamarin.action.PAUSE";
        public const string ActionStop = "com.xamarin.action.STOP";

        private static bool _serviceIsLooping = false;
        public static MainActivity Main;

        private static BitChute.Fragments.SettingsFragment.ExtWebInterface _extWebInterface =
            BitChute.Fragments.SettingsFragment._extWebInterface;

        private static Java.Util.Timer _timer = new Java.Util.Timer();
        private static ExtTimerTask _timerTask = new ExtTimerTask();

        public static ExtStickyService ExtStickyServ;
        private static PowerManager _pm;

        private static WifiManager wifiManager;
        private static WifiManager.WifiLock wifiLock;
        private static AudioManager _audioManager;

        private static bool _backgroundTimeout = false;
        public static bool NotificationsHaveBeenSent = false;
        private static ExtNotifications _extNotifications = new ExtNotifications();
        private static SettingsFragment _fm5;
        private static ActivityManager.RunningAppProcessInfo _myProcess = new ActivityManager.RunningAppProcessInfo();
        private static int _startForegroundNotificationId = 6666;

        private static bool _notificationStackExecutionInProgress = false;
        private static bool _notificationLongTimerSet = false;

        public static Dictionary<int, MediaPlayer> MediaPlayerDictionary = new Dictionary<int, MediaPlayer>();
        public static int PlayerNumberHasFocus = 0;

        private static bool _paused;

        #endregion


        /// <summary>
        /// initializes the mediaplayer object on tab of your choice.  
        /// if the mediaplayer is already instantiated then it gets reset for new playback
        /// </summary>
        /// <param name="mp"></param>
        /// <returns></returns>
        public static MediaPlayer InitializePlayer(int tab)
        {
            if (tab == null)
            {
                tab = MainActivity.ViewPager.CurrentItem;
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

            //Wake mode will be partial to keep the CPU still running under lock screen
            MediaPlayerDictionary[tab].SetWakeMode(Android.App.Application.Context, WakeLockFlags.Partial);

            //When we have prepared the song start playback
            MediaPlayerDictionary[tab].Prepared += (sender, args) => MediaPlayerDictionary[tab].Start();

            AppState.MediaPlayback.MediaPlayerNumberIsStreaming = tab;

            //When we have reached the end of the song stop ourselves, however you could signal next track here.
            MediaPlayerDictionary[tab].Completion += (sender, args) => Stop();

            MediaPlayerDictionary[tab].Error += (sender, args) =>
            {
                //playback error
                Console.WriteLine("Error in playback resetting: " + args.What);
                Stop();//this will clean up and reset properly.
            };

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
            await Task.Run(() =>
            {
                if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem] != null)
                {
                    //We are simply paused so just start again
                    MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Start();
                    StartForeground();
                    return;
                }

                if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem] == null)
                {
                    InitializePlayer(MainActivity.ViewPager.CurrentItem);
                }

                if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].IsPlaying)
                    return;

                try
                {
                    MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].PrepareAsync();
                    AquireWifiLock();
                    StartForeground();
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

        private void Pause()
        {
            if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem] == null)
                return;

            if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].IsPlaying)
                MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Pause();

            //StopForeground(false);
            _paused = true;
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
            AppState.MediaPlayback.MediaPlayerNumberIsStreaming = -1;
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
        public void StartForeground()
        {

            var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0,
                            new Intent(ApplicationContext, typeof(MainActivity)),
                            PendingIntentFlags.UpdateCurrent);

            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Android.App.Application.Context, MainActivity.CHANNEL_ID)
                            .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                            .SetContentTitle("BitChute streaming in background") // Set the title
                            .SetSmallIcon(Resource.Drawable.bitchute_notification)
                            .SetPriority(NotificationCompat.PriorityLow);

            StartForeground(-6666, builder.Build());
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
            _audioManager = (AudioManager)GetSystemService(AudioService);
            wifiManager = (WifiManager)GetSystemService(WifiService);
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
            }

            wifiManager = (WifiManager)GetSystemService(Context.WifiService);
            Main = MainActivity.Main;
            ExtStickyServ = this;

            try
            {
                _pm = (PowerManager)GetSystemService(Context.PowerService);
                PowerManager.WakeLock _wl = _pm.NewWakeLock(WakeLockFlags.Partial, "My Tag");
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
                wifiLock = wifiManager.CreateWifiLock(Android.Net.WifiMode.Full, "bitchute_wifi_lock");
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
            while (AppSettings._notifying)
            {
                if (!SettingsFragment._notificationHttpRequestInProgress && !_notificationStackExecutionInProgress)
                {
                    _notificationStackExecutionInProgress = true;
                    await _extWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
                    await _extNotifications.DecodeHtmlNotifications(SettingsFragment.ExtWebInterface._htmlCode);
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
                else if (!AppState._userIsLoggedIn)
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
                if (AppSettings._notifying)
                {
                    if (_fm5 == null)
                    {
                        await Task.Run(() => _fm5 = MainActivity.Fm4);
                    }
                    try
                    {
                        if (!SettingsFragment._notificationHttpRequestInProgress && !_notificationStackExecutionInProgress)
                        {
                            _notificationStackExecutionInProgress = true;
                            await _extWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
                            await _extNotifications.DecodeHtmlNotifications(SettingsFragment.ExtWebInterface._htmlCode);
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
                AppState._bkgrd = false;
                return false;
            }
            else
            {
                AppState._bkgrd = true;
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

            }
        }

        public int Duration { get { return 6; } }

        public bool IsPlaying
        {
            get
            {
                return MediaPlayerDictionary[AppState.MediaPlayback.MediaPlayerNumberIsStreaming].IsPlaying;
            }
        }

        public bool CanPause()
        {
            return true;
            //try
            //{
            //    return MediaPlayerDictionary[AppState.MediaPlayback.MediaPlayerNumberIsStreaming].IsPlaying;
            //}
            //catch
            //{
            //    return true;
            //}
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

        public class ServiceWebView : Android.Webkit.WebView
        {
            public override string Url => base.Url;
            public ExtStickyService _serviceContext;
            public override void OnWindowFocusChanged(bool hasWindowFocus)
            {
                if (MainActivity._backgroundRequested)
                {
                    try
                    {
                        _pm = (PowerManager)ExtStickyServ.GetSystemService(Context.PowerService);
                        PowerManager.WakeLock _wl = _pm.NewWakeLock(WakeLockFlags.Partial, "My Tag");
                        _wl.Acquire();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    try
                    {
                        if (wifiLock == null)
                        {
                            wifiLock = wifiManager.CreateWifiLock(Android.Net.WifiMode.Full, "bitchute_wifi_lock");
                        }
                        wifiLock.Acquire();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    while (ExtStickyService.IsInBkGrd())
                    {
                        var dontSleep = DummyLoop();
                        System.Threading.Thread.Sleep(3600);
                    }
                    try
                    {
                        _pm = (PowerManager)ExtStickyServ.GetSystemService(Context.PowerService);
                        PowerManager.WakeLock _wl = _pm.NewWakeLock(WakeLockFlags.Partial, "My Tag");
                        _wl.Release();
                    }
                    catch
                    {

                    }
                }
                MainActivity._backgroundRequested = false;
                base.OnWindowFocusChanged(hasWindowFocus);
            }

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

