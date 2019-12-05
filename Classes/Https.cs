namespace BottomNavigationViewPager.Classes
{
    class Https
    {
        /// <summary>
        /// this is a string containing cookies for the app.
        /// </summary>
        public static string _cookieString { get; set; }

        /// <summary>
        /// contains url strings
        /// </summary>
        public class URLs
        {
            public static string _homepage = "https://www.bitchute.com/";
            public static string _subspage = "https://www.bitchute.com/subscriptions/";
            public static string _explore = "https://www.bitchute.com/channels/";
            public static string _settings = "https://www.bitchute.com/settings/";
            public static string _myChannel = "https://www.bitchute.com/profile";
            public static string _watchLater = "https://www.bitchute.com/playlist/watch-later/";
            public static string _playlists = "https://www.bitchute.com/playlists/";

            /// <summary>
            /// gets the url string based upon user preference
            /// </summary>
            /// <param name="pref">use strings like "MyChannel" or "Home"</param>
            /// <returns></returns>
            public static string GetUrlStringFromPref(string pref)
            {
                switch (pref)
                {
                    case "Home":
                        return _homepage;
                    case "Subs":
                        return _subspage;
                    case "Explore":
                        return _explore;
                    case "Settings":
                        return _settings;
                    case "MyChannel":
                        return _myChannel;
                    case "WatchLater":
                        return _watchLater;
                    case "Playlists":
                        return _playlists;
                }
                return _homepage;                                     
            }
        }
    }
}