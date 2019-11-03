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
using Java.Util;
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
            //_context=applicationContext;
            //Log.Info("HERE", "here I am!");
        }
        public CustomStickyService()
        {
            //Log.Info("HERE", "here I am contructor!");
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
        public static Timer timer;
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



        public async void StickyLoop()
        {
            AquireWifiLock();
            while (Globals.AppState._bkgrd)
            {
                //_main.SetWebViewVisibility();
                _serviceIsLooping = true;
                await Task.Delay(60000);
                //bool loopme = true;
            }
        }

        public static bool _foregroundNotify = true;
        public static bool _backgroundNotify = false;

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
            var _ctx = Android.App.Application.Context;

            //var runningAppProcesses = _am.RunningAppProcesses;

            ActivityManager.RunningAppProcessInfo myProcess = new ActivityManager.RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(myProcess);

            //List<Android.App.ActivityManager.RunningAppProcessInfo> list
            //    = new List<Android.App.ActivityManager.RunningAppProcessInfo>();

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

            //if (_am != null && list != null)
            //{
            //    try
            //    {
            //        list.AddRange(_am.RunningAppProcesses);
            //    }
            //    catch
            //    {

            //    }
            //}
            //else
            //{
            //    try
            //    {
            //        _main = MainActivity._main;
            //        _am = _main.CustomGetActivityManager();
            //        list.AddRange(_am.RunningAppProcesses);
            //    }
            //    catch
            //    {

            //    }
            //    Globals.AppState._bkgrd = true;
//                return _bkgrd;
//            }

//            if (list != null)
//            {
//                foreach (var _process in list)
//                {
//                    //this needs to be replaced with a newer enum
//#pragma warning disable CS0618 // Type or member is obsolete
//                    //if (_process.Importance == Android.App.ActivityManager.RunningAppProcessInfo.ImportanceForeground)
//#pragma warning restore CS0618 // Type or member is obsolete

//                    {
//                        foreach (var _pkg in _process.PkgList)
//                        {
//                            if (_pkg == _ctx.PackageName)
//                            {
//                                _bkgrd = false;
//                            }

//                            else
//                            {
//                                _bkgrd = true;
//                            }
//                        }
//                    }
//                }
//            }
//            return _bkgrd;
        }

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
                    await Task.Run (() => _fm5 = MainActivity._fm5);
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
        
        //public async void SendBackgroundNotification()
        //{
        //    _notesSent = false;
        //    if (_backgroundNotify)
        //    {
        //        await Task.Run(() =>
        //        {
        //            try
        //            {
        //                if (!TheFragment5._notificationHttpRequestInProgress)
        //                {
        //                    _extWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
        //                    TheFragment5._notificationHttpRequestInProgress = true;
        //                    Globals.AppState._backgroundTimeOut = true;
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex.Message);
        //            }
        //        });
        //    }
        //    return;
        //}
        
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
        public class SampleOne : TimerTask
        {
            public override void Run()
            {
                

            }
        }
    }
}  