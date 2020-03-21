﻿using Android.Util;

namespace BitChute.Classes
{
    class AppState
    {
        public class Display
        {
            /// <summary>
            ///  true when the app detects that device
            /// has been rotated horizontally
            /// </summary>
            public static bool Horizontal = false;

            public static void OrientationChanged(bool horizontal)
            {
                Horizontal = horizontal;
            }

            /// <summary>
            ///  the screen height in pixels
            /// </summary>
            public static int ScreenHeight;

            /// <summary>
            ///  the screen width in pixels
            /// </summary>
            public static int ScreenWidth;

            public static Android.Widget.LinearLayout.LayoutParams GetCurrentVideoContainerLayout()
            {
                Android.Widget.LinearLayout.LayoutParams vidParams =
                         new Android.Widget.LinearLayout.LayoutParams(AppState.Display.ScreenWidth,
                                 (int)(AppState.Display.ScreenWidth * (.5625)));

                return vidParams;
            }

            /// <summary>
            /// use this LinearLayout.LayoutParams to scale videos to the screen
            /// </summary>
            Android.Widget.LinearLayout.LayoutParams videoContainerLinearParams =
               new Android.Widget.LinearLayout.LayoutParams(AppState.Display.ScreenWidth,
               (int)(AppState.Display.ScreenWidth * (.5625)));
        }

        public class MediaPlayback
        {
            /// <summary>
            /// there are 5 different media players and this keeps track of which one is playing
            /// 
            /// returns -1 if none are playing otherwise it's 0-4 coinciding with the tab played from
            /// </summary>
            public static int MediaPlayerNumberIsStreaming;
        }

        /// <summary>
        /// global bool state: 
        /// returns/should be set to false if this app is in the foreground
        /// returns/should be set to true when the app goes background
        /// 
        /// it doesn't override the OS setting; it keeps the status for you
        /// </summary>
        public static bool _bkgrd = true;

        public static bool _userIsLoggedIn = false;

        /// <summary>
        /// this string is sent into the app settings fragment to notify user 
        /// of version they're running
        /// </summary>
        public static string _appVersion = "25.1.API_Implementation";

        /// <summary>
        /// tab 4 string should be set to strings like
        /// "Subs" "Home" or "Feed"
        /// </summary>
        public static string _t4Is { get; set; }

        /// <summary>
        /// tab 5 string should be set to strings like
        /// "Subs" "Home" or "Feed"
        /// </summary>
        public static string _t5Is { get; set; }
    }
}