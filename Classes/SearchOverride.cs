using Android.OS;
using Android.Widget;
using BitChute.Fragments;
using System;
using System.Collections.Generic;
using System.Net;
using static BitChute.Classes.ViewHelpers;

namespace BitChute.Classes
{
    public class SearchOverride
    {
        public static bool SearchOverrideInProg = false;

        public class SearchEngine
        {
            public static string RootUrl = "https://duckduckgo.com/"; //@TODO add other search engines
            public static string DisplayName { get; set; }
            
            public SearchEngine() { }

            public static string GetRoolUrl(string engine)
            {
                string root = "";
                switch (engine)
                {
                    case "DuckDuckGo": root = "https://duckduckgo.com/"; break;
                    case "Google": root = "https://google.com/"; break;
                    case "JoshWhoSearch": root = "https://joshwhosearch.com/"; break;
                    case "StartPage": root = "https://startpage.com/"; break;
                    case "Bing": root = "https://bing.com/"; break;
                }
                RootUrl = root;
                return root;
            }

            public static string GetQueryParams(string engine)
            {
                string _params = "";
                switch (engine)
                {
                    case "DuckDuckGo": _params = "&kae=t&kp=-2&kav=1"; break; // set to darkmode, safesearch off, infinite scroll ON
                    case "Google": _params = ""; break; //@TODO add the rest of these
                    case "JoshWhoSearch": _params = ""; break;
                    case "StartPage": _params = ""; break;
                    case "Bing": _params = ""; break;
                }
                return _params;
            }

            public static string GetApiFormattedString(string searchQuery, string engine)
            {
                string fs = "";
                string formattedQ = "";
                string queryOperator = "";
                string qp = GetQueryParams(engine);
                RootUrl = GetRoolUrl(engine);
                switch (engine)
                {
                    case "DuckDuckGo":
                        queryOperator = @"?q=site%3Abitchute.com+";
                        formattedQ = searchQuery.Replace(@" ", "+");
                        fs = $"{RootUrl}{queryOperator}{formattedQ}{qp}";
                        break;
                    case "Google":
                        queryOperator = @"search?q=site:bitchute.com+";
                        formattedQ = searchQuery.Replace(@" ", "+");
                        fs = $"{RootUrl}{queryOperator}{formattedQ}{qp}";
                        break;
                    case "JoshWhoSearch":
                        break;
                    case "StartPage":
                        break;
                    case "Bing": break;
                }
                return fs;
            }
        }

        public static string StripBitChuteQuery(string query)
        {
            return query.Replace(@"https://www.bitchute.com/search?q=", "").Replace(@"&sort=date_created+desc", "");
        }


        public static string[] SearchEngineNames = new string[] 
        { "DuckDuckGo", "Google", "JoshWhoSearch", "StartPage", "Bing" };
                             
        //@TODO build out search engine class
                                     //I'm kind of short on time ATM so I'm just going to
                                       //build this with strings for now
        
        public List<SearchEngine> GetAvailableSearchEngines()
        { 
            List<SearchEngine> sel = new List<SearchEngine>();

            return sel;
        } 

        /// <summary>
        /// Takes bitchute search api query and turns it into an
        /// api formatted string for whatever search engine you want
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tab"></param>
        public static string ReRouteSearch(string query)
        {
            var stripped = StripBitChuteQuery(query);
            var fq = SearchEngine.GetApiFormattedString(stripped, AppSettings.SearchOverrideSource);
            return fq;
        }

        public class UI
        {
            public static void PopulateSpinner()
            {
                var _searchAdapter = new ArrayAdapter<string>(Android.App.Application.Context,
                    Resource.Id.searchOverrideSourceSpinner, SearchEngineNames);

                Tab4.SearchOverrideSourceSpinner.Adapter = _searchAdapter;

                switch (AppSettings.SearchOverrideSource)
                {
                    case "DuckDuckGo": Tab4.SearchOverrideSourceSpinner.SetSelection(0); break;
                    case "Google": Tab4.SearchOverrideSourceSpinner.SetSelection(1); break;
                }

                ViewHelpers.Tab4.SearchOverrideSourceSpinner.ItemSelected += OnSearchSpinnerChanged;
            }

            public static void OnSearchSpinnerChanged(object sender, EventArgs e)
            {
                AppSettings.SearchOverrideSource = Tab4.SearchOverrideSourceSpinner.SelectedItem.ToString();
               
            }
        }
    }
}