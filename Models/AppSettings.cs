using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace BitChute.Classes
{
    class AppSettings
    {
        public static Android.Content.ISharedPreferences _prefs;

        public static bool ZoomControl { get; set; }
        public static bool Tab1FeaturedOn { get; set; }
        public static bool FanMode { get; set; }
        public static bool Tab3Hide { get; set; }
        public static bool SettingsTabOverride { get; set; }
        public static bool AutoPlay = true;

        public static string Tab3OverridePreference { get; set; }
        public static string Tab4OverridePreference { get; set; }
        
        /// <summary>
        /// the ms delay for setting a pop back to root for each tab
        /// </summary>
        public static int TabDelay = 800;
        
        /// <summary>
        /// this int controls the delay in ms of notifications being 
        /// parsed and then sent out.  It should be set to a high int
        /// so as to not overload bitchute.com with httprequests
        /// </summary>
        public static int NotificationDelay = 120000;

        /// <summary>
        /// this bool should be set to/returns whether or not the navbar
        /// will be hidden by default when the device is held horizontally
        /// </summary>
        public static bool _hideHorizontalNavbar = true;

        public static bool _hideVerticalNavbar = false;

        public static bool _notifying = false;
        
        /// <summary>
        /// gets the app preferences object from android
        /// </summary>
        /// <returns></returns>
        public static Android.Content.ISharedPreferences GetAppSharedPrefs()
        {
            _prefs = Android.App.Application.Context.GetSharedPreferences("BitChute", FileCreationMode.Private);
            return _prefs;
        }

        /// <summary>
        /// Gets the android stored prefs for the app and 
        /// stores them as local variables in the AppSettings class
        /// </summary>
        public static void LoadAllPrefsFromSettings()
        {
            GetAppSharedPrefs();
           // _notifying = _prefs.GetBoolean("notificationson", true);
            Tab4OverridePreference = _prefs.GetString("tab4overridestring", "MyChannel");
            Tab4OverridePreference = _prefs.GetString("tab5overridestring", "Settings");
            ZoomControl = _prefs.GetBoolean("zoomcontrol", false);
            FanMode = _prefs.GetBoolean("fanmode", false);
            Tab3Hide = _prefs.GetBoolean("tab3hide", true);
            Tab1FeaturedOn = _prefs.GetBoolean("t1featured", true);
            SettingsTabOverride = _prefs.GetBoolean("settingstaboverride", false);
            _hideHorizontalNavbar = _prefs.GetBoolean("hidehoriztonalnavbar", true);
            _hideVerticalNavbar = _prefs.GetBoolean("hideverticalnavbar", false);

            return;
        }
        
        /// <summary>
        /// gets the url string; input examples include: "tab4overridestring" and "tab5overridestring"
        /// </summary>
        /// <param name="tabPref"></param>
        /// <returns></returns>
        public static string GetTabOverrideUrlPref(string tabPref)
        {
            _prefs = GetAppSharedPrefs();

            switch (tabPref)
            {
                case "tab4overridestring":
                    string t4url = Https.Urls.GetUrlStringFromPref(_prefs.GetString("tab4overridestring", "MyChannel"));
                    return t4url;
                case "tab5overridestring":
                    string t5url = Https.Urls.GetUrlStringFromPref(_prefs.GetString("tab5overridestring", "Settings"));
                    return t5url;
            }
            return Https.Urls._homepage;
        }

        public class Debug
        {
            /// <summary>
            /// for testing without connecting to internet, timer for
            /// </summary>
            public static int DummyDelay1ms = 1;
            public static int DummyDelay3000ms = 3000;

            /// <summary>
            /// set this to simulate a delayed response from server
            /// </summary>
            public static int DummyDelay = 1;
        }

        public class Themes
        {
            public static void InitializeTheme(string theme)
            {
                if (theme == "dark")
                {
                    SelectedTheme.CommentBackground = Colors.DarkGrey;
                }
            }

            public class SelectedTheme
            {
                public static Android.Graphics.Color CommentBackground { get; set; }
            }

            public static Color GetCommentBackground()
            {
                return Colors.DarkGrey;
            }

            public class Colors
            {
                public static Android.Graphics.Color DarkGrey = new Android.Graphics.Color(20, 20, 20);
            }
        }
    }
}