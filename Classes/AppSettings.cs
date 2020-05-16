using Android.Content;
using Android.Graphics.Drawables;

namespace BottomNavigationViewPager.Classes
{
    class AppSettings
    {
        //english for now
        public static string AppLanguage = "en";

        public static Android.Content.ISharedPreferences _prefs;

        public static bool _zoomControl { get; set; }
        public static bool _tab1FeaturedOn { get; set; }
        public static bool _fanMode { get; set; }
        public static bool _tab3Hide { get; set; }
        public static bool _settingsTabOverride { get; set; }
        
        public static string _tab4OverridePreference { get; set; }
        public static string _tab5OverridePreference { get; set; }
        
        /// <summary>
        /// the ms delay for setting a pop back to root for each tab
        /// </summary>
        public static int _tabDelay = 3000;

        /// <summary>
        /// the ms delay for fixing link overflow on mobile
        /// </summary>
        public static int _linkOverflowFixDelay = 6000;

        /// <summary>
        /// this int controls the delay in ms of notifications being 
        /// parsed and then sent out.  It should be set to a high int
        /// so as to not overload bitchute.com with httprequests
        /// </summary>
        public static int _notificationDelay = 120000;

        /// <summary>
        /// this bool should be set to/returns whether or not the navbar
        /// will be hidden by default when the device is held horizontally
        /// </summary>
        public static bool HideHorizontalNavBar = true;

        public static bool HideVerticalNavBar = false;

        public static bool _notifying = true;
        
        /// <summary>
        /// gets the app preferences object from android
        /// </summary>
        /// <returns></returns>
        public static Android.Content.ISharedPreferences GetAppSharedPrefs()
        {
            _prefs = Android.App.Application.Context.GetSharedPreferences("BitChute", FileCreationMode.Private);
            return _prefs;
        }

        public static void LoadAllPrefsFromSettings()
        {
            GetAppSharedPrefs();
            _notifying = _prefs.GetBoolean("notificationson", true);
            _tab4OverridePreference = _prefs.GetString("tab4overridestring", "MyChannel");
            _tab5OverridePreference = _prefs.GetString("tab5overridestring", "Settings");
            _zoomControl = _prefs.GetBoolean("zoomcontrol", false);
            _fanMode = _prefs.GetBoolean("fanmode", false);
            _tab3Hide = _prefs.GetBoolean("tab3hide", true);
            _tab1FeaturedOn = _prefs.GetBoolean("t1featured", true);
            _settingsTabOverride = _prefs.GetBoolean("settingstaboverride", false);
            HideHorizontalNavBar = _prefs.GetBoolean("hidehoriztonalnavbar", true);
            HideVerticalNavBar = _prefs.GetBoolean("hideverticalnavbar", false);

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
                    string t4url = Https.URLs.GetUrlStringFromPref(_prefs.GetString("tab4overridestring", "MyChannel"));
                    return t4url;
                case "tab5overridestring":
                    string t5url = Https.URLs.GetUrlStringFromPref(_prefs.GetString("tab5overridestring", "Settings"));
                    return t5url;
            }
            return Https.URLs._homepage;
        }
    }
}