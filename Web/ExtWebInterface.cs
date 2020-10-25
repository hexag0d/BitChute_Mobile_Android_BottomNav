using Android.Webkit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static BitChute.Web.Login;

namespace BitChute.Web
{
    public class ExtWebInterface
    {
        private static HttpClient _httpClient;
        public static HttpClient HttpClient {
            get {
                if (_httpClient == null) {
                    _httpClient = new HttpClient(HttpClientHandler);
                    _httpClient.DefaultRequestHeaders.Add("Host", "www.bitchute.com");
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0");
                    _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
                   
                    //_httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                    _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    _httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                    _httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                    _httpClient.DefaultRequestHeaders.Add("TE", "Trailers");

                }
                return _httpClient;
            }
            set { _httpClient = value; } }
        private static HttpClientHandler _httpClientHandler;
        public static HttpClientHandler HttpClientHandler {
            get {
                if (_httpClientHandler == null) { _httpClientHandler = new HttpClientHandler() { UseCookies = false }; }
                return _httpClientHandler;
            }
            set { _httpClientHandler = value; }
        }
        private static System.Net.CookieContainer _cookieCon = new CookieContainer();
        public static System.Net.CookieContainer CookieCon { get { return _cookieCon; } set { _cookieCon = value; } }
        public static string NotificationRawText;
        public static string HtmlCode = "";
        public static HttpRequestHeaders RequestHeaders;
        private static HttpResponseHeaders _responseHeaders;
        public static HttpResponseHeaders ResponseHeaders {
            get { return _responseHeaders; }
            set { _responseHeaders = value;

            }
        }
        private static string _defaultRequestHeaderString;
        public static string DefaultRequestHeaderString {
            get { return _defaultRequestHeaderString; }
            set { _defaultRequestHeaderString = value; }
        }

        public static string GetRequestHeader(HttpResponseHeaders headers = null, bool fromSettings = false)
        {
            string cookieHeader = "";
            if (!fromSettings)
            {
                foreach (var header in headers)
                {
                    if (header.Key.ToLower() == "set-cookie")
                    {
                        foreach (var cookiePair in header.Value)
                        {
                            var tokens = cookiePair.ToString().Split(';');
                            foreach (var token in tokens)
                            {
                                if (token.Contains("csrftoken"))
                                {
                                    cookieHeader += token + ";";
                                    AppSettings.SessionState.CsrfToken = token;
                                }
                                if (token.Contains("__cfduid"))
                                {
                                    cookieHeader += token + ";";
                                    AppSettings.SessionState.Cfuid = token;
                                }
                                if (token.Contains("sessionid"))
                                {
                                    cookieHeader += token + ";";
                                    AppSettings.SessionState.SessionId = token;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                cookieHeader += AppSettings.SessionState.CsrfToken + ";";
                cookieHeader += AppSettings.SessionState.Cfuid + ";";
                cookieHeader += AppSettings.SessionState.SessionId + ";";
            }
            cookieHeader += "preferences={%22theme%22:%22night%22%2C%22autoplay%22:true}; ";
            DefaultRequestHeaderString = cookieHeader;
            return cookieHeader;
        }
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

        public static string GetCookieString()
        {
            string cookieString = "";
            foreach (var cookie in CookieStore?.Values)
            {
                cookieString += cookie;
            }
            return cookieString;
        }

        public static Dictionary<string, string> GetCookieDictionary()
        {
            var cookieDict = new Dictionary<string, string>();

            cookieDict.Add("Host", "www.bitchute.com");
            cookieDict.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0");
            cookieDict.Add("Accept-Language", "en-US,en;q=0.5");
            cookieDict.Add("Accept", "video/webm,video/ogg,video/*;q=0.9,application/ogg;q=0.7,audio/*;q=0.6,*/*;q=0.5");
            cookieDict.Add("Connection", "keep-alive");
            cookieDict.Add("Upgrade-Insecure-Requests", "1");
            cookieDict.Add("Cache-Control", "max-age=0");
            cookieDict.Add("TE", "Trailers");
            string cookieHeader = "";
            if (!String.IsNullOrWhiteSpace(AppSettings.SessionState.CsrfToken))
            {
                cookieHeader = $"csrftoken={AppSettings.SessionState.CsrfToken};";
            }
            if (!String.IsNullOrWhiteSpace(AppSettings.SessionState.Cfuid))
            {
                cookieHeader += $"__cfduid={AppSettings.SessionState.Cfuid};";
            }
            if (!String.IsNullOrWhiteSpace(AppSettings.SessionState.SessionId)) {
                 cookieHeader += $"sessionid={AppSettings.SessionState.SessionId};";
            }
                
            cookieHeader += "preferences={%22theme%22:%22night%22%2C%22autoplay%22:true}; ";

            cookieDict.Add("Cookie", cookieHeader);
            return cookieDict;
        }

        public static Dictionary<string, string> CookieStore = new Dictionary<string, string>();
        
        public static Dictionary<string, string> CopyCookies(HttpResponseMessage result, Uri uri = null, string domain = null)
        {
            if (domain != null) { uri = new Uri(domain); }
            foreach (var header in result.Headers)
            {
                if (header.Key.ToLower() == "set-cookie")
                {
                    foreach (var value in header.Value)
                    {
                        try
                        {
                            CookieStore.Add($"{uri.Scheme}://{uri.Host}", value);
                        }
                        catch { }
                    }
                }
            }
            //foreach (var cookie in GetAllCookies(_cookieContainer))
            //    CookieManager.Instance.SetCookie(cookie.Domain, cookie.ToString());
            return CookieStore;
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
                if (!ExtNotifications.NotificationHttpRequestInProgress)
                {
                    try
                    {
                        Uri _notificationURI = new Uri("https://bitchute.com/notifications/");
                        var _cookieHeader = CookieCon.GetCookieHeader(_notificationURI);

                            _httpClient.DefaultRequestHeaders.Add("Cookie", ExtWebInterface.CookieHeader);
                            ExtNotifications.NotificationHttpRequestInProgress = true;

                            var getRequest = _httpClient.GetAsync("https://bitchute.com/notifications/").Result;

                            CopyCookies(getRequest, new Uri("https://bitchute.com/"));
                            var resultContent = getRequest.Content.ReadAsStringAsync().Result;
                            HtmlCode = resultContent;
                            ExtNotifications.NotificationHttpRequestInProgress = false;
                        
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
        public static async System.Threading.Tasks.Task<string> GetHtmlTextFromUrl(string url, bool getInitialHeaders = false)
        {
            string _htmlCode = "";

            try
            {
                HttpResponseMessage getRequest = new HttpResponseMessage();
                if (getInitialHeaders)
                {
                    getRequest = await HttpClient.GetAsync(url);
                    ExtWebInterface.HttpClient.DefaultRequestHeaders.Add("Cookie", GetRequestHeader(getRequest.Headers));
                }

                if (!getInitialHeaders)
                {
                    ExtWebInterface.HttpClient.DefaultRequestHeaders.Add("Cookie", GetRequestHeader(null, true));
                    getRequest = await HttpClient.GetAsync(url);
                }
                return await getRequest.Content.ReadAsStringAsync();
            }
            catch { }
            return _htmlCode;
        }
    }
}
