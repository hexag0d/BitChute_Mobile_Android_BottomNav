using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BitChute.Fragments;

namespace BitChute.Classes
{
    class ExtendedWebInterface
    {
        public class ExtWebInterface
        {
            public static string _notificationRawText;
            public static CookieContainer _cookieCon = new CookieContainer();
            public static string _htmlCode = "";

            public static string _endPoint = "https://bitchute.com/";

            /// <summary>
            /// returns html source of url requested
            /// </summary>
            /// <param name="url">use the string you want to get html source from</param>
            /// <returns></returns>
            public async Task<string> Make(string url)
            {
                await Task.Run(() =>
                {
                    _htmlCode = "";
                    HttpClientHandler handler = new HttpClientHandler() { UseCookies = false };

                    try
                    {
                        Uri _notificationURI = new Uri("https://bitchute.com/");

                        var _cookieHeader = _cookieCon.GetCookieHeader(_notificationURI);

                        using (HttpClient _client = new HttpClient(handler))
                        {
                            _client.DefaultRequestHeaders.Add("Cookie", TheFragment5.GetCookieHeader());
                            var getRequest = _client.GetAsync("https://bitchute.com/notifications/").Result;
                            var resultContent = getRequest.Content.ReadAsStringAsync().Result;
                            _htmlCode = resultContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                return _htmlCode;
            }
        }
    }
}