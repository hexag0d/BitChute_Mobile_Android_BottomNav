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
using BitChute;
using Newtonsoft.Json.Linq;
using static BitChute.Models.CommentModel;
using static BitChute.Models.SubscriptionModel;

namespace BitChute.Classes
{
    public class BitChuteAPI
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
            if (apiFormattedString.Substring(0,5) != "https" || apiFormattedString.Substring(0, 5) != "HTTPS" ||
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
                    Uri _URI = new Uri("https://bitchute.com/");
                    
                    using (HttpClient _client = new HttpClient(handler))
                    {
                        _client.DefaultRequestHeaders.Add("Cookie", Https._cookieString);
                        var getRequest = _client.GetAsync("https://bitchute.com/notifications/").Result;
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

        /// <summary>
        /// Gets the currently logged in user's subscription list
        /// This should return a formatted list of creators who have recently posted
        /// OR it can also return a COMPLETE list of every subscription, without the recently
        /// posted videos, just the alphabetical or most recently posted
        /// 
        /// if it's more efficient for the bitchute server, I can also
        /// have the end user's device handle the sorting.. have to ask rich on that one
        /// </summary>
        /// <param name="getPackages"></param>
        /// <returns></returns>
        public List<CreatorPackage> GetSubscriptionsFromAPI(bool getPackages)
        {
            List<CreatorPackage> creatorPackageList = new List<CreatorPackage>();

            //string json = "{'results':[{'SwiftCode':'','City':'','BankName':'Deutsche    Bank','Bankkey':'10020030','Bankcountry':'DE'},{'SwiftCode':'','City':'10891    Berlin','BankName':'Commerzbank Berlin (West)','Bankkey':'10040000','Bankcountry':'DE'}]}";
            string jsonReturn;
            if (getPackages)
            {
                jsonReturn = MakeAPICall("apiFormattedStringHere").Result;
            }
            else
            {
                jsonReturn = MakeAPICall("getEntireSubscribedList").Result;
            }
            var resultObjects = AllChildren(JObject.Parse(jsonReturn))
                .First(c => c.Type == JTokenType.Array && c.Path.Contains("results"))
                .Children<JObject>();

            foreach (JObject result in resultObjects)
            {
                CreatorPackage cp = new CreatorPackage();
                cp = result.ToObject<CreatorPackage>();

                creatorPackageList.Add(cp);
                foreach (JProperty property in result.Properties())
                {
                    
                }
            }

            return creatorPackageList;
        }

        // recursively yield all children of json
        private static IEnumerable<JToken> AllChildren(JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;
                foreach (var cc in AllChildren(c))
                {
                    yield return cc;
                }
            }
        }

        public class FormattingHelpers
        {

        }

        public class DispatchToUi
        {
            public static Handler APIHandler = new Handler();
            

        }

        public class Inbound
        {
            public static async Task<Tuple<int, int>> GetVideoLikeCount(string videoId)
            {
                await Task.Delay(3000);
                Tuple<int, int> tuple = new Tuple<int, int>(6,6);
                return tuple;
            }
            public async static Task<List<Comment>> GetVideoComments (string videoId)
            {
                await Task.Delay(3000);
                return SampleCommentList.GetSampleCommentList();
            }
        }

        public class Outbound
        {
            public static async void SendVideoLike(string videoId, bool liked)
            {
                await Task.Run(() =>
                {
                    if (liked)
                    {

                    }
                    else
                    {

                    }
                });
            }

            public static async void SendVideoDislike (string videoId, bool disliked)
            {
                await Task.Run(() =>
                {
                    if (disliked)
                    {

                    }
                    else
                    {

                    }
                });
            }

            /// <summary>
            /// sends video comment to the video id
            /// </summary>
            /// <param name="videoId">the id of video to post comment</param>
            /// <param name="commentId">the id of comment, if it's a reply</param>
            /// <param name="commentContents">object list representing multimedia replies such as youtube videos, images, other
            /// potential replies.  I'd like to make it so that bitchute videos can also be embedded as replies</param>
            public static async void SendVideoComment (string videoId, string commentId,
                List<object> commentContents)
            {
                await Task.Run(() =>
                {
                    //if commentId is null then this isn't a reply to a comment
                    if (commentId == null)
                    {

                    }
                    //commentId was passed as param so this is a reply to a comment
                    else
                    {

                    }
                });
            }
        }
    }
}