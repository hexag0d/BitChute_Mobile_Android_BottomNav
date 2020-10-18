using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BitChute.Web
{
    public class ExtWebInterface
    {
        public static System.Net.CookieContainer CookieCon = new CookieContainer();
        public static string NotificationRawText;
        public static string HtmlCode = "";
        public static CookieCollection Cookies = new CookieCollection();
        private static Dictionary<string, string> _cookieDictionary;
        public static Dictionary<string, string> CookieDictionary {
            get
            {
                if (_cookieDictionary == null)
                {
                    if (CookieHeader != null)
                    {
                        _cookieDictionary = new Dictionary<string, string>();
                        _cookieDictionary.Add("Cookie", CookieHeader);
                    }
                }
                return _cookieDictionary;
            }
            set { _cookieDictionary = value; }
        }

        private static string _cookieHeader;
        public static string CookieHeader{get{return _cookieHeader;}set{_cookieHeader=value;}}


        /// <summary>
        /// returns html source of url requested
        /// </summary>
        /// <param name="url">use the string you want to get html source from</param>
        /// <returns></returns>
        public static async Task<string> GetNotificationText(string url)
        {
            await Task.Run(() =>
            {
                HtmlCode = "";
                HttpClientHandler handler = new HttpClientHandler() { UseCookies = false };

                if (!ExtNotifications.NotificationHttpRequestInProgress)
                {
                    try
                    {
                        Uri _notificationURI = new Uri("https://bitchute.com/notifications/");
                        var _cookieHeader = CookieCon.GetCookieHeader(_notificationURI);

                        using (HttpClient _client = new HttpClient(handler))
                        {
                            _client.DefaultRequestHeaders.Add("Cookie", ExtWebInterface.CookieHeader);
                            ExtNotifications.NotificationHttpRequestInProgress = true;

                            var getRequest = _client.GetAsync("https://bitchute.com/notifications/").Result;
                            var resultContent = getRequest.Content.ReadAsStringAsync().Result;
                            HtmlCode = resultContent;
                            ExtNotifications.NotificationHttpRequestInProgress = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

            return HtmlCode;
        }

        /// <summary>
        /// returns html source of url requested
        /// </summary>
        /// <param name="url">use the string you want to get html source from</param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<string> GetHtmlTextFromUrl(string url)
        {
            string _htmlCode = "";
            await System.Threading.Tasks.Task.Run(() =>
            {
                HttpClientHandler handler = new HttpClientHandler() { UseCookies = false };
                try
                {
                    using (HttpClient _client = new HttpClient(handler))
                    {
                        var getRequest = _client.GetAsync(url).Result;
                        var resultContent = getRequest.Content.ReadAsStringAsync().Result;
                        _htmlCode = resultContent;
                        //_notificationHttpRequestInProgress = false;
                    }
                }
                catch 
                {
                }
            });

            return _htmlCode;
        }
    }
}
