using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BitChute.Classes
{
    public class ExtWebInterfaceGeneral
    {
        public static System.Net.CookieContainer CookieCon = new CookieContainer();
        

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
                    var check = BitChute.Fragments.TheFragment4.CookieHeader;
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
