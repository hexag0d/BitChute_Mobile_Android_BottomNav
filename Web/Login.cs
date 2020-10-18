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

        public static void LoginButton_OnClick(object sender, EventArgs e)
        {
            MakeLoginRequest();
        }

        public static void MakeLoginRequest()
        {
            var rq = ExtWebInterface.RequestHeaders;
            var rs = ExtWebInterface.ResponseHeaders;
            TryLoginText("https://www.bitchute.com/accounts/login/", rs, rq);
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
        public static async Task<string> TryLoginText(string url, HttpResponseHeaders responseHeader, HttpRequestHeaders requestHeader)
        {

            var cc = await GetCookieContainer(responseHeader);

            HttpClientHandler handler1 = new HttpClientHandler() { UseCookies = true, CookieContainer = cc };

            try
            {
                using (HttpClient _client = new HttpClient(handler1))
                {

                    var getRequest = _client.GetAsync("https://www.bitchute.com/notifications/").Result;
                    var content = getRequest.Content;
                    var resultContent = content.ReadAsStringAsync().Result;
                    cc = await GetCookieContainer(getRequest.Headers);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            HttpClientHandler handler2 = new HttpClientHandler() { UseCookies = true, CookieContainer = cc };

            try
            {
                using (HttpClient _client = new HttpClient(handler2))
                {

                    var getRequest = _client.GetAsync("https://www.bitchute.com/notifications/").Result;
                    var content = getRequest.Content;
                    var resultContent = content.ReadAsStringAsync().Result;
                    cc = await GetCookieContainer(getRequest.Headers);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            HttpClientHandler handler = new HttpClientHandler() { UseCookies = true, CookieContainer = cc };

            try
            {

                using (HttpClient _client = new HttpClient(handler))
                {



                    var getRequest = _client.GetAsync(url).Result;
                    var content = getRequest.Content;
                    var resultContent = getRequest.Content.ReadAsStringAsync().Result;


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "";
        }

    }
}