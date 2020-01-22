namespace BottomNavigationViewPager.Classes
{
    class AppState
    {
        public class Display
        {
            /// <summary>
            /// returns true when the app detects that device
            /// has been rotated horizontally
            /// </summary>
            public static bool _horizontal = false;
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
        public static string _appVersion = "22.6.1";

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