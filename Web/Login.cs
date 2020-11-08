using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HtmlAgilityPack;
using BitChute.Web.Ui;
using BitChute.Web.Auth;
using BitChute.Fragments;

namespace BitChute.Web
{
    public class Login
    {
        public delegate void LoginEventDelegate(LoginEventArgs args);
        public static event LoginEventDelegate OnLogin;
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

        private static string _cookieStringHeader;
        public static string CookieStringHeader {
            get { return _cookieStringHeader; }
            set { _cookieStringHeader = value; }
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
                // csrfmiddlewaretoken = xxxxx & username = xxxxxx & password = xxxxxxxx & one_time_code =
                string serialized = $"csrfmiddlewaretoken={toke.Csrfmiddleware}&username={toke.Username}" +
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

        public static async void MakeLoginRequest(string username, string password, string csrfmiddlewaretoken = null, bool getNewTokens = false)
        {
            if (GetLatestCsrfToken().Value != null) { 
            var response = await DoLogin(AuthToken.GetToken(GetLatestCsrfToken().Value, username, password));
            }
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
                    cookieNew.Domain = "www.bitchute.com";
                    //.Add(cookieNew);
                    CookieContainer.Add(cookieNew);
                    //_cookieCollection.Add(new Cookie // setting the domain crashes the app, 
                    //  nothing calls parent method atm
                    //{
                    //    Name = cookieSplit[0],
                    //    Value = cookieSplit[1],
                    //    Domain = "www.bitchute.com"
                    //});
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

        public static FormUrlEncodedContent GetLoginKeys(AuthToken toke = null, string csrf = null, string username = null, string password = null, string onetimecode = "")
        {
            try
            {
                if (toke == null)
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                 new KeyValuePair<string, string>("csrfmiddlewaretoken", csrf),
                 new KeyValuePair<string, string>("username", username),
                 new KeyValuePair<string, string>("password", password),
                 new KeyValuePair<string, string>("one_time_code", onetimecode)
            });
                    return formContent;
                }
                else
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                 new KeyValuePair<string, string>("csrfmiddlewaretoken", toke.Csrfmiddleware),
                 new KeyValuePair<string, string>("username", toke.Username),
                 new KeyValuePair<string, string>("password", toke.Password),
                 new KeyValuePair<string, string>("one_time_code", toke.One_time_code)
                });
                    return formContent;
                }
            }
            catch { return null; }
        }

        public static string GetRequestHeader(HttpResponseHeaders headers)
        {
            string cookieHeader = "";
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
                            }
                            if (token.Contains("__cfduid"))
                            {
                                cookieHeader += token + ";";
                            }
                            if (token.Contains("sessionid"))
                            {
                                cookieHeader += token + ";";
                            }
                        }
                    }
                }
            }
            cookieHeader += "preferences={%22theme%22:%22night%22%2C%22autoplay%22:true}; ";
            return cookieHeader;
        }

        public static async Task<string> GetInitialCSRFToken(string html)
        {
            string csrfToken = "";

            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                if (doc != null)
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//link[@href]"))
                    {
                        if (node.OuterHtml.Contains("/common.css"))
                        {
                            CssHelper.CommonCssUrl = await Task.FromResult("https://www.bitchute.com" + node.Attributes["href"].Value);
                        }
                        if (node.OuterHtml.Contains("/search.css"))
                        {
                            CssHelper.SearchCssUrl = await Task.FromResult("https://www.bitchute.com" + node.Attributes["href"].Value);
                        }
                    }
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//input[@name='csrfmiddlewaretoken']"))
                    {
                        csrfToken = Login.GetLatestCsrfToken(node.Attributes["value"].Value).Value;
                    }
                }
                //CssHelper.GetCommonCss(CssHelper.CommonCssUrl, true, true);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return csrfToken;
        }

        static bool FullHeadersAdded = false;

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static async Task<string> DoLogin(AuthToken authToken)
        {
            bool loginSuccess = false;
            bool responseCodeSuccess = false;
            if (OnLogin == null) {
                OnLogin += OnPostLogin;
                //OnLogin += BitChute.Web.ViewClients.Run_OnLogin;
                //OnLogin += HomePageFrag.OnPostLoginAttempt;
            }
            try
            {
                if (!FullHeadersAdded)
                {
                    ExtWebInterface.HttpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                    ExtWebInterface.HttpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                    ExtWebInterface.HttpClient.DefaultRequestHeaders.Add("Referer", "https://www.bitchute.com/");
                    FullHeadersAdded = true;
                }
            }
            catch
            {

            }
            try { 
                var formdata = GetLoginKeys(authToken);
                var requestContent = await formdata.ReadAsStringAsync();
                var response = await ExtWebInterface.HttpClient.PostAsync("https://www.bitchute.com/accounts/login/", formdata);
                var header = await ExtWebInterface.GetRequestHeader(response.Headers, true);
                responseCodeSuccess = response.IsSuccessStatusCode;
                if (String.IsNullOrWhiteSpace(AppSettings.SessionState.SessionId))
                {
                    loginSuccess = false;
                }
                else
                {
                    loginSuccess = true;
                    AppState.UserIsLoggedIn = true;
                }
            }
            catch (Exception ex) { }
            try
            {
                if (loginSuccess && responseCodeSuccess)
                {
                    
                    OnLogin.Invoke(new LoginEventArgs(false, true, true, false));

                }
                else
                {
                    OnLogin.Invoke(new LoginEventArgs(false, true, false, true));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        public static void OnPostLogin(LoginEventArgs e)
        {

        }
    }
}