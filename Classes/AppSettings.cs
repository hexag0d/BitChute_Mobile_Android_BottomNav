using Android.Content;
using Android.Graphics.Drawables;

namespace BitChute.Classes
{
    class AppSettings
    {
        public static bool AutoPlay = true;

        //english for now
        public static string AppLanguage = "en";

        public static Android.Content.ISharedPreferences Prefs;
        public static Android.Content.ISharedPreferencesEditor PrefEditor;

        public static bool ZoomControl { get; set; }
        public static bool Tab1FeaturedOn { get; set; }
        public static bool FanMode { get; set; }
        public static bool Tab3Hide { get; set; }
        public static bool SettingsTabOverride { get; set; }

        /// <summary>
        /// any || feed
        /// </summary>
        public static string BackgroundKey { get; set; }

        /// <summary>
        /// any || feed
        /// </summary>
        public static string AutoPlayOnMinimized { get; set; }

        private static string _dlFabShowSetting = "onpress";
        /// <summary>
        /// onpress || never || always
        /// </summary>
        public static string DlFabShowSetting
        {
            get
            {
                return _dlFabShowSetting;
            }
            set
            {
                if (_dlFabShowSetting != value)
                {
                    if (value == "never" || 
                        (value == "onpress" && !TabStates.Tab3.VideoDownloaderViewEnabled))
                    {
                        ViewHelpers.Main.DownloadFAB?.Hide();
                    }
                    else
                    {
                        ViewHelpers.Main.DownloadFAB?.Show();
                    }
                    _dlFabShowSetting = value;
                }
            }
        }
        public static string Tab4OverridePreference { get; set; }
        public static string Tab5OverridePreference { get; set; }
        
        /// <summary>
        /// the ms delay for setting a pop back to root for each tab
        /// </summary>
        public static int TabDelay = 3000;

        /// <summary>
        /// the ms delay for fixing link overflow on mobile
        /// </summary>
        public static int LinkOverflowFixDelay = 6000;

        public static int AdBlockDelay = 10000;

        public static int ScrollToTopDelay = 30;

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
        public static bool HideHorizontalNavBar = true;

        public static bool HideVerticalNavBar = false;

        public static bool Notifying = true;
        
        /// <summary>
        /// gets the app preferences object from android
        /// </summary>
        /// <returns></returns>
        public static Android.Content.ISharedPreferences GetAppSharedPrefs()
        {
            Prefs = Android.App.Application.Context.GetSharedPreferences("BitChute", FileCreationMode.Private);
            return Prefs;
        }

        /// <summary>
        /// Loads the stored android preferences for app
        /// and puts them into the static AppSettings class
        /// </summary>
        public static void LoadAllPrefsFromSettings()
        {
            GetAppSharedPrefs();
            Notifying = Prefs.GetBoolean("notificationson", true);
            Tab4OverridePreference = Prefs.GetString("tab4overridestring", "MyChannel");
            Tab5OverridePreference = Prefs.GetString("tab5overridestring", "Settings");
            ZoomControl = Prefs.GetBoolean("zoomcontrol", false);
            FanMode = Prefs.GetBoolean("fanmode", false);
            Tab3Hide = Prefs.GetBoolean("tab3hide", true);
            Tab1FeaturedOn = Prefs.GetBoolean("t1featured", true);
            SettingsTabOverride = Prefs.GetBoolean("settingstaboverride", false);
            HideHorizontalNavBar = Prefs.GetBoolean("hidehoriztonalnavbar", true);
            HideVerticalNavBar = Prefs.GetBoolean("hideverticalnavbar", false);
            DlFabShowSetting = Prefs.GetString("dlfabshowsetting", "onpress");
            AutoPlayOnMinimized = Prefs.GetString("autoplayonminimized", "feed");
            BackgroundKey = Prefs.GetString("backgroundkey", "feed");
            return;
        }
        
        /// <summary>
        /// gets the url string; input examples include: "tab4overridestring" and "tab5overridestring"
        /// </summary>
        /// <param name="tabPref"></param>
        /// <returns></returns>
        public static string GetTabOverrideUrlPref(string tabPref)
        {
            Prefs = GetAppSharedPrefs();

            switch (tabPref)
            {
                case "tab4overridestring":
                    string t4url = Https.URLs.GetUrlStringFromPref(Prefs.GetString("tab4overridestring", "MyChannel"));
                    return t4url;
                case "tab5overridestring":
                    string t5url = Https.URLs.GetUrlStringFromPref(Prefs.GetString("tab5overridestring", "Settings"));
                    return t5url;
            }
            return Https.URLs._homepage;
        }
    }
}