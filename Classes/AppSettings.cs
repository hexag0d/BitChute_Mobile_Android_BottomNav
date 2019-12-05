using Android.Content;

namespace BottomNavigationViewPager.Classes
{
    class AppSettings
    {
        public static Android.Content.ISharedPreferences _prefs;

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
        /// gets the url string; input examples include: "tab4overridestring" and "tab5overridestring"
        /// </summary>
        /// <param name="tabPref"></param>
        /// <returns></returns>
        public static string GetTabOverrideUrlPref(string tabPref)
        {
            GetAppSharedPrefs();
            switch (tabPref)
            {
                case "tab4overridestring":
                    return Https.URLs.GetUrlStringFromPref(_prefs.GetString(tabPref, Https.URLs._myChannel)); 
                case "tab5overridestring":
                    return Https.URLs.GetUrlStringFromPref(_prefs.GetString(tabPref, Https.URLs._settings)); 
            }
            return Https.URLs._homepage;
        }

        /// <summary>
        /// the ms delay for setting a pop back to root for each tab
        /// </summary>
        public static int _tabDelay = 800;

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
        public static bool _hideHorizontalNavbar = true;

        public static bool _hideVerticalNavbar = false;

        public static bool _notifying = true;

    }
}