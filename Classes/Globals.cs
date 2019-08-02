using System.Collections.Generic;
using Android.Content;

namespace BottomNavigationViewPager.Classes
{
    public class Globals
    {
        //   Android.App.ActivityManager _am = (Android.App.ActivityManager)Android.App.Application
        //         .Context.GetSystemService(Context.ActivityService);


        public static bool _setWebView { get; set; }

        

        public static Android.App.ActivityManager _am = (Android.App.ActivityManager)Android.App.Application
             .Context.GetSystemService(Context.ActivityService);


        /// <summary>
        /// global bool setting: 
        /// returns/should be set to false if this app is in the foreground
        /// returns/should be set to true when the app goes background
        /// 
        /// it doesn't override the OS setting; it keeps the status for you
        /// </summary>
        public static bool _bkgrd = true;

        /// <summary>
        /// returns false when the ActivityManager contains
        /// an entry for this app running in foreground: 
        /// importance is present in package.name with OS focus
        /// 
        /// requires a modified android manifest for get_task ALLOWED
        /// </summary>
        /// <returns>bool</returns>
        public bool IsInBkGrd()
        {
            var _ctx = Android.App.Application.Context;

            var runningAppProcesses = _am.RunningAppProcesses;

            List<Android.App.ActivityManager.RunningAppProcessInfo> list
                = new List<Android.App.ActivityManager.RunningAppProcessInfo>();

            list.AddRange(_am.RunningAppProcesses);

            foreach (var _process in list)
            {
                if (_process.Importance == Android.App.ActivityManager.RunningAppProcessInfo.ImportanceForeground)
                {
                    foreach (var _pkg in _process.PkgList)
                    {
                        if (_pkg == _ctx.PackageName)
                        {
                            _bkgrd = false;
                        }

                        else
                        {
                            _bkgrd = true;
                        }
                    }
                }
            }
            return _bkgrd;
        }

    }
}