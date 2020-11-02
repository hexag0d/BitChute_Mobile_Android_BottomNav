using Android.Graphics.Drawables;
using Android.Views;
using Android.Webkit;
using System;
using System.Collections.Generic;
using static BitChute.App.TabStates.TabFragPackage;
using static BitChute.Services.MainPlaybackSticky;
using static BitChute.Web.ViewClients;

namespace BitChute.App
{
    public class TabStates
    {
        /// <summary>
        /// returns a Drawable icon to be used for tab detail changing
        /// takes string arguments like "Home" "Subs" "Feed" "MyChannel" "Settings"
        /// </summary>
        /// <param name="tabString"></param>
        /// <returns></returns>
        public static Drawable GetTabIconFromString(string tabString)
        {
            Drawable icon;

            switch (tabString)
            {
                case "Home":
                    icon =  MainActivity.Main.GetDrawable(Resource.Drawable.tab_home);
                    return icon;
                case "Subs":
                    icon =  MainActivity.Main.GetDrawable(Resource.Drawable.tab_subs);
                    return icon;
                case "Feed":
                    icon = MainActivity.Main.GetDrawable(Resource.Drawable.tab_playlists);
                    return icon;
                case "MyChannel":
                    icon =  MainActivity.Main.GetDrawable(Resource.Drawable.tab_mychannel);
                    return icon;
                case "Settings":
                    icon =  MainActivity.Main.GetDrawable(Resource.Drawable.tab_settings);
                    return icon;
                case "Playlists":
                    icon =  MainActivity.Main.GetDrawable(Resource.Drawable.tab_playlists);
                    return icon;
                case "WatchLater":
                    icon =  MainActivity.Main.GetDrawable(Resource.Drawable.tab_playlists);
                    return icon;
                case "Explore":
                    icon =  MainActivity.Main.GetDrawable(Resource.Drawable.tab_home);
                    return icon;
            }
            return  MainActivity.Main.GetDrawable(Resource.Drawable.tab_home);
        }

        public static List<TabFragPackage> GetTabFragPackages(bool fromSettingsWithOverride = false)
        {
            List<TabFragPackage> tabfrags = new List<TabFragPackage>();
            TabFragPackage tab3 = null;
            TabFragPackage tab4 = null;
            var tab0 = new TabFragPackage(TabFragPackage.FragmentType.Home);
            var tab1 = new TabFragPackage(TabFragPackage.FragmentType.Subs);
            var tab2 = new TabFragPackage(TabFragPackage.FragmentType.Feed);
            if (!fromSettingsWithOverride)
            {
                tab3 = new TabFragPackage(TabFragPackage.FragmentType.MyChannel);
                tab4 = new TabFragPackage(TabFragPackage.FragmentType.Settings);
            }
            if (fromSettingsWithOverride)
            {
                tab3 = new TabFragPackage(AppSettings.Tab3OverridePreference);
                tab4 = new TabFragPackage(AppSettings.Tab4OverridePreference);
            }
            tabfrags.Add(tab0);
            tabfrags.Add(tab1);
            tabfrags.Add(tab2);
            tabfrags.Add(tab3);
            tabfrags.Add(tab4);
            return tabfrags;
        }

        public static object GetWebViewClientByType(FragmentType fragtype = FragmentType.None, string type = null)
        {
            if (fragtype != FragmentType.None) { type = fragtype.ToString(); }
            switch (type)
            {
                case "Home": return new Home();
                case "Subs": return new Subs();
                case "Feed": return new Feed();
                case "MyChannel": return new MyChannel();
                case "Settings": return new Settings();
            }
            return new Subs();
        }


        public class TabFragPackage
        {
            public Drawable Icon { get; set; }
            public string Title { get; set; }
            public string RootUrl { get; set; }
            public object WebViewClient{get; set;}

            public enum FragmentType
            {
                Home,
                Subs,
                Feed,
                MyChannel,
                Settings,
                Playlists,
                WatchLater,
                None
            }

            public TabFragPackage()
            {

            }

            public TabFragPackage(FragmentType fragType)
            {
                Icon = GetTabIconFromString(fragType.ToString());
                Title = fragType.ToString();
                RootUrl = Https.URLs.GetUrlStringFromPref(fragType.ToString());
            }

            public TabFragPackage(string fragType, bool withWebViewClient = false)
            {
                Icon = GetTabIconFromString(fragType);
                Title = fragType;
                RootUrl = Https.URLs.GetUrlStringFromPref(fragType);
                if (withWebViewClient)
                {
                    WebViewClient = GetWebViewClientByType(FragmentType.None, fragType);
                }
            }
        }
    }
}