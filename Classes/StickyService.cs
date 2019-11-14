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
using Android.Support.V4.App;

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
        public int counter = 0;

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
            _service = this;

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
        public void AquireWifiLock()
        {
            if (wifiLock == null)
            {
                wifiLock = wifiManager.CreateWifiLock(Android.Net.WifiMode.Full, "bitchute_wifi_lock");
            }
            wifiLock.Acquire();
        }

        public static bool _backgroundTimeout = false;
        public static bool _notificationsHaveBeenSent = false;
        public static ExtNotifications _extNotifications = new ExtNotifications();
        public static TheFragment5 _fm5;

        private static ActivityManager.RunningAppProcessInfo myProcess = new ActivityManager.RunningAppProcessInfo();

        /// <summary>
        /// returns true when the app detects that it's running
        /// in background
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsInBkGrd()
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

        private int _notificationMsElapsed = 0;
        public static bool _notificationLongTimerSet = false;

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
                if (ExtStickyService._notificationsHaveBeenSent)
                {
                    //check to make sure the timer isn't already started or the app will crash
                    if (!ExtStickyService._notificationLongTimerSet)
                    {
                        //after the initial notifications are sent, start the long running service timer task
                        _timer.ScheduleAtFixedRate(ExtStickyService._extTimerTask, 180000, 780000);
                        _notificationLongTimerSet = true;
                    }
                    return;
                }
                else
                {
                    await Task.Delay(180000);
                }
            }
        }

        private static bool _notificationStackExecutionInProgress = false;
        
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

        public static Notification GetStartServiceNotification()
        {

            var _ctx = Android.App.Application.Context;
            Notification note = new Notification();

            var resultIntent = new Intent(_ctx, typeof(MainActivity));
            var valuesForActivity = new Bundle();
            valuesForActivity.PutInt(MainActivity.COUNT_KEY, 1);
            MainActivity.NOTIFICATION_ID += 6;
            var resultPendingIntent = PendingIntent.GetActivity(_ctx, MainActivity.NOTIFICATION_ID, resultIntent, PendingIntentFlags.UpdateCurrent);


            var builder = new Android.Support.V4.App.NotificationCompat.Builder(_ctx, MainActivity.CHANNEL_ID)
                    .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                    .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                    .SetContentTitle("BitChute Entering Background Mode") // Set the title
                    .SetNumber(1) // Display the count in the Content Info
                    .SetSmallIcon(2130837590) // This is the icon to display
                    .SetContentText("now Notifying");

            var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(_ctx);
            note = builder.Build();
            return note;
        }

        public static int _startForegroundNotificationId = 6666;
        
        public class ServiceWebView : Android.Webkit.WebView
        {

            public override string Url => base.Url;

            public ExtStickyService _serviceContext;


            public override void OnWindowFocusChanged(bool hasWindowFocus)
            {

                if (MainActivity._backgroundRequested)
                {

                    while (ExtStickyService.IsInBkGrd())
                    {
                        System.Threading.Thread.Sleep(3600);
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