namespace BottomNavigationViewPager.Classes
{
    class AppState
    {
        public class Display
        {
            public static Android.Graphics.Color DarkGrey = new Android.Graphics.Color(20, 20, 20);
            /// <summary>
            /// returns true when the app detects that device
            /// has been rotated horizontally
            /// </summary>
            public static bool Horizontal = false;
        }

        /// <summary>
        /// global bool state: 
        /// returns/should be set to false if this app is in the foreground
        /// returns/should be set to true when the app goes background
        /// 
        /// it doesn't override the OS setting; it keeps the status for you
        /// </summary>
        public static bool Bkgrd = true;

        public static bool UserIsLoggedIn = false;

        /// <summary>
        /// this string is sent into the app settings fragment to notify user 
        /// of version they're running
        /// </summary>
        public static string AppVersion = "25.1.API8Retainer_VideoDownloader";
    }
}