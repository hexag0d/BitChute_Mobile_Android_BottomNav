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

namespace StartServices.Servicesclass
{
    [Service(Exported = true)]
    public class CustomStickyService : Service
    {
        public static bool _serviceIsLooping = false;
        public static MainActivity _main;

        public int counter = 0;
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
            //Log.Info("HERE", "Oncreate method SensorService");
            //Toast.MakeText(this, "Oncreate method SimpleStartedService", ToastLength.Long).Show();
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
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

        public async void StickyLoop()
        {
            while (Globals._bkgrd)
            {
                _main.SetWebViewVisibility();
                _serviceIsLooping = true;
                await Task.Delay(60000);
                bool loopme = true;
            }
        }

        public void ServiceViewOverride()
        {
            _main.SetVisible(true);
            _main.SetWebViewVisibility();
        }

        public void StartTimer()
        {
            //set a new Timer
            timer = new Timer();
            //initialize the TimerTask's job
            InitializeTimerTask();

            if (timer != null)
            timer.Schedule(one, 20000, 20000); //
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
                StopTimerTask();
                base.OnDestroy();

            }
            catch (Exception ex)
            {
            }
        }
        public void StopTimerTask()
        {
            try
            {
                //stop the timer, if it's not already null
                if (timer != null)
                {
                    timer.Cancel();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public class SampleOne : TimerTask
        {
            public override void Run()
            {

                    var _dummy = false;


            }
        }
    }
}  