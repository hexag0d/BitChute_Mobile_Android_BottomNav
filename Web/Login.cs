using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections;
using Json.Net;

namespace BitChute.Web
{
    public class Login
    {
        private static bool _userIsLoggingIn = false;
        public static bool UserIsLoggingIn
        {
            get { return _userIsLoggingIn; }
            set { _userIsLoggingIn = value; }
        }

        private static HttpResponseHeaders _initialResponseHeader;
        public static HttpResponseHeaders InitialResponseHeader
        {
            get { return _initialResponseHeader; }
            set { _initialResponseHeader = value; }
        }

        public static string CsrfKey = "csrfmiddlewaretoken";
        private static KeyValuePair<string, string> _csrfPair;
        public static KeyValuePair<string, string> GetLatestCsrfToken(string token = null)
        {
            if (token == null && _csrfPair.Value != null) { return _csrfPair; }
            else if (token != null) {
                _csrfPair = new KeyValuePair<string, string>(CsrfKey, token);
                return _csrfPair;
            }
            else { return _csrfPair; }
        }
        
        public class AuthToken
        {
            public string Csrfmiddleware;
            public string Username;
            public string Password;
            public string One_time_code;
            public AuthToken() { }
            public static AuthToken GetToken(string csrftoken, string username, string password, string one_time_code = "")
            {
                var toke = new AuthToken()
                {
                    Csrfmiddleware = csrftoken,
                    Username = username,
                    Password = password,
                    One_time_code = one_time_code
                };
                return toke;
            }

            public static string GetSerializedToken(AuthToken toke)
            {
                if (toke.Csrfmiddleware == null || toke.Csrfmiddleware == "") { toke.Csrfmiddleware = GetLatestCsrfToken().Value; }
                // csrfmiddlewaretoken = xxxxx & username = hexagod_deep_house_mixes & password = xxxxxxxx & one_time_code =
                string serialized =  $"csrfmiddlewaretoken={toke.Csrfmiddleware}&username={toke.Username}" +
                    $"&password={toke.Password}&one_time_code={toke.One_time_code}";
                return serialized;
            }

            public static Dictionary<string, string> GetAuthTokenDictionary(AuthToken toke)
            {
                var dictionary = new Dictionary<string, string>();
                dictionary.Add("username", toke.Username);
                dictionary.Add("password", toke.Password);
                dictionary.Add(CsrfKey, toke.Csrfmiddleware);
                dictionary.Add("one_time_code", "");
                return dictionary;
            }
        }
        
        public static async void MakeLoginRequest(string username, string password)
        {
            var jt = AuthToken.GetToken(GetLatestCsrfToken().Value, username, password);
            var rq = ExtWebInterface.RequestHeaders;
            var rs = ExtWebInterface.ResponseHeaders;
            var response = await DoLogin("https://www.bitchute.com/accounts/login/", jt,  rs, rq);
        }

        public static CookieContainer CookieContainer = new CookieContainer();
        private static CookieCollection _cookieCollection;

        public static async Task<CookieCollection> GetCookieCollection(HttpResponseHeaders responseHeader)
        {
            if (_cookieCollection == null) { _cookieCollection = new System.Net.CookieCollection(); }
            var setCookie = responseHeader.GetValues("Set-Cookie");
            foreach (var cookiePair in setCookie)
            {
                var cookies = cookiePair.Split(';');
                foreach (var cookie in cookies)
                {
                    var cookieSplit = cookie.Split('=');
                    var cookieName = cookieSplit[0];
                    var cookieValue = cookieSplit[1];
                    var cookieNew = new Cookie(cookieName, cookieValue);
                    cookieNew.Domain = "bitchute.com";
                    //.Add(cookieNew);
                    CookieContainer.Add(cookieNew);
                    _cookieCollection.Add(new Cookie
                    {
                        Name = cookieSplit[0],
                        Value = cookieSplit[1],
                        //Domain = "https://www.bitchute.com/"
                    });
                }
            }

            return _cookieCollection;
        }

        public static string RemoveDuplicateCookies(List<string> cookies, string strCookies = null)
        {
            try
            {
                int position = -1;
                List<int> positionsToRemove = new List<int>();
                foreach (var cookie in cookies)
                {
                    position++;
                    if (!positionsToRemove.Contains(position))
                    {
                        for (int i = 0; i < cookies.Count(); i++)
                        {
                            if ((cookie?.Trim() == cookies[i]?.Trim()) && position != i)
                            {
                                positionsToRemove.Add(i);
                            }
                        }
                    }
                }
                foreach (var ptr in positionsToRemove)
                {
                    cookies.RemoveAt(ptr);
                }
            }
            catch { }
            return String.Join(";", cookies);
        }
        
        public static Task<CookieContainer> GetCookieContainer(HttpResponseHeaders responseHeader)
        {
            try
            {
                if (_cookieCollection == null) { _cookieCollection = new System.Net.CookieCollection(); }
                var setCookie = responseHeader.GetValues("Set-Cookie");
                foreach (var cookiePair in setCookie)
                {
                    Dictionary<string, string> pieces = new Dictionary<string, string>();
                    Cookie cookie = new Cookie();
                    var parts = cookiePair.Split(';').ToList<string>();
                    foreach (var pair in parts)
                    {
                        var pairTrimmed = "";
                        if (pair.StartsWith(" ")) { pairTrimmed = pair.Remove(0, 1); }
                        else { pairTrimmed = pair; }
                        if (pairTrimmed.Contains('='))
                        {
                            var cookiePieces = pairTrimmed.Split("=");
                            if (pairTrimmed.StartsWith("__cfduid"))
                            {
                                cookie.Name = "__cfduid";
                                cookie.Value = cookiePieces[1];
                            }
                            else if (pairTrimmed.StartsWith("csrftoken"))
                            {
                                cookie.Name = "csrftoken";
                                cookie.Value = cookiePieces[1];
                                cookie.Domain = "www.bitchute.com";
                            }
                            else if (pairTrimmed.StartsWith("expires"))
                            {
                                cookie.Expires = Convert.ToDateTime(cookiePieces[1]);
                            }
                            else if (pairTrimmed.StartsWith("path"))
                            {
                                cookie.Path = @"/";
                            }
                            else if (pairTrimmed.StartsWith("domain"))
                            {
                                cookie.Domain = cookiePieces[1];
                            }
                            else if (pairTrimmed.StartsWith("SameSite"))
                            {
                                
                            }
                        }
                        else
                        {
                            if (pairTrimmed.StartsWith("HttpOnly"))
                            {
                                cookie.HttpOnly = true;
                            }
                            else if (pairTrimmed.StartsWith("Secure"))
                            {
                                cookie.Secure = true;
                            }
                        }
                    }
                    if (cookie.Domain == null) { cookie.Domain = "www.bitchute.com"; }
                    CookieContainer.Add(cookie);
                    if (cookie.Name == "csrftoken") GetLatestCsrfToken(cookie.Value);
                }
                
            }
            catch (Exception ex)
            {

            }
            return Task.FromResult(CookieContainer);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static async Task<string> DoLogin(string url, AuthToken serializedPayload, HttpResponseHeaders responseHeader, HttpRequestHeaders requestHeader)
        {
            CookieContainer cookieContainer = new CookieContainer();
            HttpClientHandler handler1 = new HttpClientHandler() { UseCookies = true, CookieContainer = cookieContainer };

            try
            {
                using (HttpClient _client = new HttpClient(handler1))
                {
                    var getRequest = _client.GetAsync("https://www.bitchute.com/").Result;
                    var content = getRequest.Content;
                    var resultContent = content.ReadAsStringAsync().Result;
                    cookieContainer = await GetCookieContainer(getRequest.Headers);
                }
                if (GetLatestCsrfToken().Value == "" || GetLatestCsrfToken().Value == null)
                {
                    serializedPayload.Csrfmiddleware = GetLatestCsrfToken().Value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var cookieHeader = CookieContainer.GetCookieHeader(new Uri("https://www.bitchute.com"));
            cookieHeader = RemoveDuplicateCookies(cookieHeader.Split(";").ToList<string>());
            try
            {
                    WebClient wc = new WebClient();
                wc.Headers.Add(@"Accept: */*");
                wc.Headers.Add(@"Origin: https://www.bitchute.com");
                wc.Headers.Add(@"Content-Type: application/x-www-form-urlencoded; charset=UTF-8");
                wc.Headers.Add(@"Accept-Language: en-US,en;q=0.5");
                wc.Headers.Add($"User-Agent: {AppState.WebViewAgentString}");
                wc.Headers.Add(@"X-Requested-With: XMLHttpRequest");
                wc.Headers.Add($"Cookie: {cookieHeader}");
                var body = AuthToken.GetSerializedToken(serializedPayload);
                var response = await wc.UploadStringTaskAsync(new Uri(url), AuthToken.GetSerializedToken(serializedPayload));
                
            }
            catch (Exception ex) {  Console.WriteLine(ex.Message); }
            return "";
        }
    }
}