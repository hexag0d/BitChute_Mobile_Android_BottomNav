using Android.Content;
using Android.Graphics.Drawables;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BitChute
{
    class AppSettings
    {
        public static bool AutoPlay = true;

        //english for now
        public static string AppLanguage = "en";

        public static Android.Content.ISharedPreferences Prefs;
        public static Android.Content.ISharedPreferencesEditor PrefEditor;

        private static bool _firstTimeAppLoad;
        public static bool FirstTimeAppLoad
        {
            get
            {
                return _firstTimeAppLoad;
            }   
            set
            {
                _firstTimeAppLoad = value;
                SendPrefSettingToAndroid("firsttimeappload", value);
            }
        }


        public static bool ZoomControl { get; set; }
        public static bool Tab0FeaturedOn { get; set; }
        public static bool Tab3OverrideEnabled { get; set; }
        public static bool Tab2Hide { get; set; }
        public static bool Tab4OverrideEnabled { get; set; }
        public static bool VideoPreProcessingApproved { get; set; }
        private static bool _userWasLoggedInLastAppClose = false;
        public static bool UserWasLoggedInLastAppClose {
            get { return _userWasLoggedInLastAppClose; }
            set { _userWasLoggedInLastAppClose = value;
                SendPrefSettingToAndroid("userwasloggedinlastappclose", value);
            } }
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
                        (value == "onpress" && !BitChute.Fragments.MyChannelFrag.VideoDownloaderViewEnabled))
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
            if (!AppSettingsLoadingFromAndroid)
            {
                try
                {
                    switch (newSet.GetType().ToString().ToLower())
                    {
                        case "system.boolean": PrefEditor.PutBoolean(setting, Convert.ToBoolean(newSet)); break;
                        case "system.string": PrefEditor.PutString(setting, newSet.ToString()); break;
                    }
                    PrefEditor.Commit();
                }
                catch { }
            }
        }

        public static string Tab3OverridePreference { get; set; }
        public static string Tab4OverridePreference { get; set; }
        
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
            PrefEditor = Prefs.Edit();
            return Prefs;
        }

        public static bool AppSettingsLoadingFromAndroid = false;

        public class SessionState
        {
            private static string _csrfMiddlewareToken;
            public static string CsrfMiddleWareToken {
                get { return _csrfMiddlewareToken; }
                set { _csrfMiddlewareToken = value; }
            }
            private static string _csrfToken;
            public static string CsrfToken {
                get { return RemoveExpiredCookie(_csrfToken); }
                set { _csrfToken = value;
                    if (!AppSettingsLoadingFromAndroid)
                        SendPrefSettingToAndroid("csrftoken", value);
                }
            }
            private static string _cfduid;
            public static string Cfduid {
                get { return RemoveExpiredCookie(_cfduid); }
                set { _cfduid = value;
                    if (!AppSettingsLoadingFromAndroid)
                        SendPrefSettingToAndroid("cfduid", value);
                }
            }
            private static string _sessionId;
            public static string SessionId
            {
                get { return RemoveExpiredCookie(_sessionId); }
                set { _sessionId = value;
                    if (!AppSettingsLoadingFromAndroid)
                    {
                        SendPrefSettingToAndroid("sessionid", SessionId);
                        if (String.IsNullOrWhiteSpace(_sessionId)) { AppState.UserIsLoggedIn = false; }
                    }
                }
            }
            public static string RemoveExpiredCookie(string cookie)
            {
                foreach (var split in cookie.Split(';'))
                {
                    if (split.Contains("expires="))
                    {
                        if (DateTime.Now < DateTime.Parse(split.Split('=').Last()))
                        {
                            return cookie;
                        }
                        else { return ""; }
                    }
                }
                return cookie;
            }

            public static void SaveSessionState()
            {
                SendPrefSettingToAndroid("csrftoken", CsrfToken);
                SendPrefSettingToAndroid("cfduid", Cfduid);
                SendPrefSettingToAndroid("sessionid", SessionId);
            }
        }

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
                    Tab3OverridePreference = Prefs.GetString("tab3overridestring", "MyChannel");
                    Tab4OverridePreference = Prefs.GetString("tab4overridestring", "Settings");
                    ZoomControl = Prefs.GetBoolean("zoomcontrol", false);
                    Tab3OverrideEnabled = Prefs.GetBoolean("tab3overrideenabled", false);
                    Tab2Hide = Prefs.GetBoolean("tab2hide", true);
                    Tab0FeaturedOn = Prefs.GetBoolean("t1featured", true);
                    Tab4OverrideEnabled = Prefs.GetBoolean("tab4overrideenabled", false);
                    HideHorizontalNavBar = Prefs.GetBoolean("hidehoriztonalnavbar", true);
                    HideVerticalNavBar = Prefs.GetBoolean("hideverticalnavbar", false);
                    DlFabShowSetting = Prefs.GetString("dlfabshowsetting", "onpress");
                    AutoPlayOnMinimized = Prefs.GetString("autoplayonminimized", "any");
                    BackgroundKey = Prefs.GetString("backgroundkey", "any");
                    SearchFeatureOverride = Prefs.GetBoolean("searchfeatureoverride", false); // @TODO set to false
                    SearchOverrideSource = Prefs.GetString("searchoverridesource", "DuckDuckGo");
                    SessionState.CsrfToken = Prefs.GetString("csrftoken", "");
                    SessionState.Cfduid = Prefs.GetString("cfduid", "");
                    SessionState.SessionId = Prefs.GetString("sessionid", "");
                    UserWasLoggedInLastAppClose = Prefs.GetBoolean("userwasloggedinlastappclose", false);
                    FirstTimeAppLoad = await Task.FromResult<bool>(Prefs.GetBoolean("firsttimeappload", true));
                }
                catch { }
            
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
                case "tab3overridestring":
                    string t4url = Https.URLs.GetUrlStringFromPref(Prefs.GetString("tab4overridestring", "MyChannel"));
                    return t4url;
                case "tab4overridestring":
                    string t5url = Https.URLs.GetUrlStringFromPref(Prefs.GetString("tab5overridestring", "Settings"));
                    return t5url;
            }
            return Https.URLs._homepage;
        }

        public class Debug
        {
            public static bool LoadWebViewsOnStart = true;
        }

        public class Logging
        {
            public static bool SendToConsole = false;
        }
    }
}