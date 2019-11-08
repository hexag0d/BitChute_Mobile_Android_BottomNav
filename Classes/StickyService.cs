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
    public class CustomStickyService : Service
    {
        public static bool _serviceIsLooping = false;
        public static MainActivity _main;
        public static ExtNotifications _extNotes = MainActivity.notifications;

        public static BottomNavigationViewPager.Fragments.TheFragment5.ExtWebInterface _extWebInterface =
            BottomNavigationViewPager.Fragments.TheFragment5._extWebInterface;

        public int counter = 0;

        //private static AudioManager _am;
        private static WifiManager wifiManager;
        private static WifiManager.WifiLock wifiLock;

        public CustomStickyService(Context applicationContext)
        {

        }
        public CustomStickyService()
        {

        }
        public override void OnCreate()
        {
            base.OnCreate();
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            wifiManager = (WifiManager)GetSystemService(Context.WifiService);
            //_am = (AudioManager)GetSystemService(Context.AudioService);
            _main = MainActivity._main;

            try
            {
                PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
                PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "My Tag");
                wl.Acquire();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return StartCommandResult.Sticky;
        }
        //public static Timer timer;
        //private TimerTask timerTask;
        private SampleOne one;

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

        public void StartStickyTimer()
        {

        }
        
        
        public static bool _backgroundTimeout = false;
        public static bool _notificationsHaveBeenSent = false;
        public static ExtNotifications _extNotifications = new ExtNotifications();
        public static TheFragment5 _fm5;
        
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
            ActivityManager.RunningAppProcessInfo myProcess = new ActivityManager.RunningAppProcessInfo();
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

        /// <summary>
        /// starts/restarts the notifications, 
        /// takes a ms int as the delay for starting,
        /// if this is called with no delay TheFragment5 sometimes
        /// is null or has issues when it's methods are called
        /// immediately after the app initially loads.
        /// </summary>
        public async void StartNotificationLoop(int delay)
        {
            bool _notificationStackExecutionInProgress = false;
            await Task.Delay(delay);

            if (_fm5 == null)
            {
                await Task.Run(() => _fm5 = MainActivity._fm5);
            }

            while (Globals.AppSettings._notifying)
            {
                //var _afs = GetAudioFocusState();

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
                    await Task.Delay(Globals.AppSettings._notificationDelay);
                    _notificationMsElapsed += Globals.AppSettings._notificationDelay;
                }
                else
                {
                    await Task.Delay(30000);
                    _notificationMsElapsed += 30000;
                }

                if (_notificationMsElapsed >= 888888)
                {
                    _notificationStackExecutionInProgress = false;
                    TheFragment5._notificationHttpRequestInProgress = false;
                }
            }
        }

        private static bool _notificationStackExecutionInProgress = false;

        private async void OnTimerElapsed()
        {
            await Task.Delay(30000);

            if (_fm5 == null)
            {
                await Task.Run(() => _fm5 = MainActivity._fm5);
            }

            while (Globals.AppSettings._notifying)
            {
                //var _afs = GetAudioFocusState();

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
                    await Task.Delay(Globals.AppSettings._notificationDelay);
                }
                else
                {
                    await Task.Delay(30000);
                }
            }
        }
        
        public void ServiceViewOverride()
        {
            _main.SetVisible(true);
            _main.SetWebViewVisibility();
        }
        public void InitializeTimerTask()
        {
            one = new SampleOne();
            one.Run();
            
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
        public class SampleOne : Java.Util.TimerTask
        {
            public async override void Run()
            {
                bool _notificationStackExecutionInProgress = false;

                if (_fm5 == null)
                {
                    await Task.Run(() => _fm5 = MainActivity._fm5);
                }

                while (Globals.AppSettings._notifying)
                {
                    //var _afs = GetAudioFocusState();

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
                        await Task.Delay(Globals.AppSettings._notificationDelay);
                    }
                    else
                    {
                        await Task.Delay(30000);
                    }
                }

            }
        }
    }
}  