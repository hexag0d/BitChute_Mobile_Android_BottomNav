using Android.Content;
using Android.Graphics.Drawables;
using System;
using System.Threading.Tasks;

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
        private static bool _searchFeatureOverride { get; set; }
        public static bool SearchFeatureOverride
        {
            get { return _searchFeatureOverride; }
            set
            {
                if (!AppSettings.AppSettingsLoadingFromAndroid)
                {
                    AppSettings.SendPrefSettingToAndroid("searchfeatureoverride", value);
                }
                _searchFeatureOverride = value;
            }
        }
        static string _searchOverrideSource { get; set; }
        public static string SearchOverrideSource
        {
            get { return _searchOverrideSource; }
            set
            {
                if (!AppSettings.AppSettingsLoadingFromAndroid)
                {
                    AppSettings.SendPrefSettingToAndroid("searchoverridesource", value);
                }
                _searchOverrideSource = value;
            }
        }

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
        public static void SendPrefSettingToAndroid(string setting, object newSet)
        {
            try
            {
                var yo = newSet.GetType().ToString().ToLower();
                switch (newSet.GetType().ToString().ToLower())
                {
                    case "system.boolean": PrefEditor.PutBoolean(setting, Convert.ToBoolean(newSet)); break;
                    case "system.string": PrefEditor.PutString(setting, newSet.ToString()); break;
                }
                PrefEditor.Commit();
            }
            catch { }
        }

        public static string Tab4OverridePreference { get; set; }
        public static string Tab5OverridePreference { get; set; }
        
        /// <summary>
        /// the ms delay for setting a pop back to root for each tab
        /// </summary>
        public static int TabDelay = 4000;

        /// <summary>
        /// the number of seconds that the navbar will disappear after
        /// </summary>
        public static int NavRemovalDelay = 11;

        /// <summary>
        /// the ms delay for fixing link overflow on mobile
        /// </summary>
        public static int LinkOverflowFixDelay = 6000;

        public static int AdBlockDelay = 10000;

        public static int ScrollToTopDelay = 30;

        public static int HidePageTitleDelay = 5000;

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

        public static bool AppSettingsLoadingFromAndroid = false;

        /// <summary>
        /// Loads the stored android preferences for app
        /// and puts them into the static AppSettings class
        /// </summary>
        public static async Task<bool> LoadAllPrefsFromSettings()
        {
            try
            {
                AppSettingsLoadingFromAndroid = true;
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
                SearchFeatureOverride = Prefs.GetBoolean("searchfeatureoverride", false); // @TODO set to false
                SearchOverrideSource = Prefs.GetString("searchoverridesource", "DuckDuckGo");
            }
            catch { return false; }
            await Task.Delay(2000);
            AppSettingsLoadingFromAndroid = false;
            return true;
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

        public class Logging
        {
            public static bool SendToConsole = false;
        }
    }
}