namespace BitChute.Classes
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

        public class MediaPlayback
        {
            /// <summary>
            /// there are 5 different media players and this keeps track of which one is playing
            /// 
            /// returns -1 if none are playing otherwise it's 0-4 coinciding with the tab played from
            /// </summary>
            public static int MediaPlayerNumberIsStreaming { get; set; }

            /// <summary>
            /// this bool is set true when a native media player is currently streaming
            /// it should be set to false when WebView is streaming audio
            /// </summary>
            public static bool MediaPlayerIsStreaming = false;

            public static bool UserRequestedBackgroundPlayback = false;
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

        public static Android.App.Notification ForeNote = new Android.App.Notification();

        /// <summary>
        /// this string is sent into the app settings fragment to notify user 
        /// of version they're running
        /// </summary>
        public static string AppVersion = "27.2.1.API8Retainer.MuxerEncoding";
    }
}