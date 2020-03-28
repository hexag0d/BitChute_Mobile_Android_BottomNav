using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static BitChute.Models.CommentModel;

namespace BitChute.Classes
{
    class CustomDiscusAPI
    {
        public class Inbound
        {
            public static async Task<List<Comment>> GetCommentList(string videoId)
            {
                //make the api call
                await Task.Delay(6000);
                return SampleCommentList.GetSampleCommentList();
            }
        }

        public class Outbound
        {
            /// <summary>
            /// returns a Task<string> response
            /// </summary>
            /// <param name="apiFormattedString">use BitChute API formatted string</param>
            /// <returns></returns>
            public async Task<string> MakeAPICall(string apiFormattedString)
            {
                string response = "";

                //if the API request string isn't using https then return
                //we don't want to expose the cookie header to snooper attacks
                if (apiFormattedString.Substring(0, 5) != "https" || apiFormattedString.Substring(0, 5) != "HTTPS" ||
                    apiFormattedString.Substring(0, 5) != "Https")
                {
                    response = "cannot make non https requests, use https";
                    return response;
                }

                await Task.Run(() =>
                {
                    HttpClientHandler handler = new HttpClientHandler() { UseCookies = false };

                    try
                    {
                        //https://disqus.com/api/{version}/{resource}.{output_type}
                        Uri _URI = new Uri("https://discus.com/api");

                        using (HttpClient _client = new HttpClient(handler))
                        {
                            _client.DefaultRequestHeaders.Add("Cookie", Https._cookieString);
                            var getRequest = _client.GetAsync(apiFormattedString).Result;
                            var resultContent = getRequest.Content.ReadAsStringAsync().Result;
                            response = resultContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                return response;
            }
        }
    }
}


/*
 $ curl -0 -L "https://disqus.com/api/3.0/trends/listThreads.json?api_key=API_PUBLIC_KEY_HERE"
{
"code": 0,
"response": [
{
"thread": {
"forum": {
  "id": "cucirca",
  "name": "Tv Shows",
  "founder": 996907,
  "favicon": {
    "permalink": "https://disqus.com/api/forums/favicons/cucirca.jpg",
    "cache": "https://a.disquscdn.com/uploads/forums/20/8466/favicon.png"
  }
},
"author": 996907,
"title": "Watch True Blood Online",
"link": "http://www.cucirca.com/2009/05/27/watch-true-blood-online/",
"closed": false,
"id": 40385200,
"createdAt": "2009-10-19T04:51:26"
},
"comments": 14605,
"score": "2.9408284023668639",
"link": "http://www.cucirca.com/2009/05/27/watch-true-blood-online/",
"likes": 62,
"commentLikes": 9884
}
]
}
     */
