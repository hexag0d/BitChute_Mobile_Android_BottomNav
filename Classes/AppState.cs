namespace BitChute
{
    class AppState
    {
        public class Display
        {

            public static Android.Graphics.Color DarkGrey = new Android.Graphics.Color(20, 20, 20);

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

        /// <summary>
        /// global bool state: 
        /// returns/should be set to false if this app is in the foreground
        /// returns/should be set to true when the app goes background
        /// 
        /// it doesn't override the OS setting; it keeps the status for you
        /// </summary>
        public static bool Bkgrd = true;

        public static bool UserIsLoggedIn = false;

        public static bool NotificationStartedApp = false;

        public static Android.App.Notification ForeNote { get; set; }

        /// <summary>
        /// this string is used in the app settings fragment to notify user 
        /// of version they're running
        /// </summary>
        public static string AppVersion = "28.6.4.6.API8.OpenGL.VideoProcessing.CleanupPt2";
    }
}