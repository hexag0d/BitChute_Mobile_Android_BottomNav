using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BitChute.Classes
{
    public class ExtWebInterface
    {
        public static System.Net.CookieContainer CookieCon = new CookieContainer();
        public static string NotificationRawText;
        public static string HtmlCode = "";

        public static string CookieHeader;

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
                    //Uri _notificationURI = new Uri("https://bitchute.com/notifications/");
                    //var _cookieHeader = _cookieCon.GetCookieHeader(_notificationURI);
                    var check = CookieHeader;
                    using (HttpClient _client = new HttpClient(handler))
                    {
                        _client.DefaultRequestHeaders.Add("Cookie", check);
                        //_notificationHttpRequestInProgress = true;

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
