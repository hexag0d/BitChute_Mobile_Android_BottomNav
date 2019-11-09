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
using BottomNavigationViewPager.Classes;
using BottomNavigationViewPager;
using System.Threading.Tasks;
using BottomNavigationViewPager.Fragments;
using Android.Net.Wifi;
using Android.Media;

namespace StartServices.Servicesclass
{
    [Service(Exported = true)]
    public class ExtStickyService : Service
    {
        public static bool _serviceIsLooping = false;
        public static MainActivity _main;
        public static ExtNotifications _extNotes = MainActivity.notifications;

        public static BottomNavigationViewPager.Fragments.TheFragment5.ExtWebInterface _extWebInterface =
            BottomNavigationViewPager.Fragments.TheFragment5._extWebInterface;

        public static Java.Util.Timer _timer = new Java.Util.Timer();
        public static ExtTimerTask _timerTask = new ExtTimerTask();
        public static ExtStickyService _service;

        private static PowerManager _pm;
        private static PowerManager.WakeLock _wl;
        public int counter = 0;

        //private static AudioManager _am;
        private static WifiManager wifiManager;
        private static WifiManager.WifiLock wifiLock;

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
            _service = this;
            base.OnCreate();
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            wifiManager = (WifiManager)GetSystemService(Context.WifiService);
            _main = MainActivity._main;
            
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
        private static volatile ExtTimerTask _extTimerTask = new ExtTimerTask();

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        public void AquireWifiLock()
        {
            if (wifiLock == null)
            {
                wifiLock = wifiManager.CreateWifiLock(Android.Net.WifiMode.Full, "bitchute_wifi_lock");
            }
            wifiLock.Acquire();
        }

        //public static List<bool> _recentAudioFocusStates = new List<bool>();
        //public static int _numberOfAudioChecks = 0;

        //public bool GetAudioFocusState()
        //{

        //    _numberOfAudioChecks++;

        //    if (_numberOfAudioChecks <= 13)
        //    {
        //        bool _currentAudioActive = true;

        //        try
        //        {
        //            _currentAudioActive = _am.IsMusicActive;
        //        }
        //        catch
        //        {

        //        }
        //        _recentAudioFocusStates.Add(_currentAudioActive);
        //        return true;
        //    }
        //    else
        //    {
        //        foreach (var state in _recentAudioFocusStates)
        //        {
        //            if (state)
        //            {
        //                return true;
        //            }
        //        }
        //        _recentAudioFocusStates.Clear();
        //        _numberOfAudioChecks = 0;
        //        return false;
        //    }
        //}
        
        public static bool _backgroundTimeout = false;
        public static bool _notificationsHaveBeenSent = false;
        public static ExtNotifications _extNotifications = new ExtNotifications();
        public static TheFragment5 _fm5;
        
        private static ActivityManager.RunningAppProcessInfo myProcess = new ActivityManager.RunningAppProcessInfo();

        /// <summary>
        /// returns false when the ActivityManager contains
        /// an entry for this app running in foreground: 
        /// importance is present in package.name with OS focus
        /// 
        /// requires a modified an  droid manifest for get_task ALLOWED
        /// </summary>
        /// <returns>bool</returns>
        public bool IsInBkGrd()
        {
            ActivityManager.GetMyMemoryState(myProcess);
            
            if (myProcess.Importance == Importance.Foreground)
            {
                Globals.AppState._bkgrd = false;
                return false;
            }
            else
            {
                Globals.AppState._bkgrd = true;
                return true;
            }
        }

        public void DummyService()
        {
            bool _stayAwake = true;
        }

        private int _notificationMsElapsed = 0;
        private static bool _notificationLongTimerSet = false;

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
        public async void StartNotificationLoop(int delay)
        {
            bool _notificationStackExecutionInProgress = false;
            //wait on a delay so that the cookie is ready when we make
            //httprequest for the notifications
            await Task.Delay(delay);

            if (_fm5 == null)
            {
                await Task.Run(() => _fm5 = MainActivity._fm5);
            }

            //use a while loop to start the notifications
            //they move over to a service timer eventually to prevent the loop from breaking
            while (Globals.AppSettings._notifying)
            {
                if (!TheFragment5._notificationHttpRequestInProgress && !_notificationStackExecutionInProgress)
                {
                    _notificationStackExecutionInProgress = true;
                    await _extWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
                    await _extNotifications.DecodeHtmlNotifications(TheFragment5.ExtWebInterface._htmlCode);
                    _fm5.SendNotifications(ExtNotifications._customNoteList);
                    _notificationStackExecutionInProgress = false;
                }

                if (_notificationsHaveBeenSent)
                {
                    //check to make sure the timer isn't already started or the app will crash
                    if (!_notificationLongTimerSet)
                    {
                        //after the initial notifications are sent, start the long running service timer task
                        _timer.ScheduleAtFixedRate(_extTimerTask, 180000, 660000);
                        _notificationLongTimerSet = true;
                    }
                    return;
                }
                else
                {
                    await Task.Delay(30000);
                    _notificationMsElapsed += 30000;
                }
            }
        }
        
        private static bool _notificationStackExecutionInProgress = false;
        
        public void ServiceViewOverride()
        {
            _main.SetVisible(true);
            _main.SetWebViewVisibility();
        }

        public override void OnDestroy()
        {
            try
            {
                //Intent ll24 = new Intent(this, typeof(SensorRestarterBroadcastReceiver));
                //SendBroadcast(ll24);
                base.OnDestroy();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        

        public class ExtTimerTask : Java.Util.TimerTask
        {
            public async override void Run()
            {
                if (Globals.AppSettings._notifying)
                {
                    if (_fm5 == null)
                    {
                        await Task.Run(() => _fm5 = MainActivity._fm5);
                    }
                    try
                    {
                        if (!TheFragment5._notificationHttpRequestInProgress && !_notificationStackExecutionInProgress)
                        {
                            _notificationStackExecutionInProgress = true;
                            await _extWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
                            await _extNotifications.DecodeHtmlNotifications(TheFragment5.ExtWebInterface._htmlCode);
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


        public class ServiceWebView : Android.Webkit.WebView
        {
            public override void OnWindowFocusChanged(bool hasWindowFocus)
            {
                Globals.AppState._bkgrd = true;

                if (MainActivity._backgroundRequested)
                {
                    try
                    {
                        _pm = (PowerManager)_service.GetSystemService(Context.PowerService);
                        PowerManager.WakeLock _wl = _pm.NewWakeLock(WakeLockFlags.Partial, "My Tag");
                        _service.AquireWifiLock();
                        _wl.Acquire();
                    }
                    catch
                    {

                    }
                    while (_service.IsInBkGrd())
                    {
                        //var afs = _service.GetAudioFocusState();

                        _service.DummyService();
                        System.Threading.Thread.Sleep(3600);
                    }
                }
                MainActivity._backgroundRequested = false;
                if (!ExtStickyService._notificationsHaveBeenSent)
                {
                    try
                    {
                        _service.StartNotificationLoop(30000);
                    }
                    catch
                    {

                    }
                }
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

            public ServiceWebView(Context context, IAttributeSet attrs, int defStyleAttr, bool privateBrowsing) : base(context, attrs, defStyleAttr, privateBrowsing)
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