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
using BitChute.Models;
using Newtonsoft.Json.Linq;
using static BitChute.Models.CommentModel;
using static BitChute.Models.CreatorModel;
using static BitChute.Models.SubscriptionModel;
using static BitChute.Models.VideoModel;

namespace BitChute.Classes
{
    public class BitChuteAPI
    {
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

            string jsonReturn;
            if (getPackages)
            {
                jsonReturn = Outbound.MakeAPICall("apiFormattedStringHere").Result;
            }
            else
            {
                jsonReturn = Outbound.MakeAPICall("getEntireSubscribedList").Result;
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
            /// <summary>
            /// gets everything other than the comments
            /// 
            /// The comments may take longer to return, so I'm keeping them separate for now
            /// </summary>
            /// <param name="videoId"></param>
            /// <returns></returns>
            public static async Task<VideoDetail> GetFullVideoDetail(string videoId)
            {
                int viewCount = 6;
                int likeCount = 6;
                int dislikeCount = 6;
                int subscriberCount = 6666;
                string videoDescription = "This is a sample video description.  I'm sure it's a very good video";//TODO: turn this into an object array

                VideoDetail vd = new VideoDetail()
                {
                    ViewCount = viewCount,
                    LikeCount = likeCount,
                    DislikeCount = dislikeCount,
                    VideoDescription = videoDescription
                };
                
                await Task.Delay(AppSettings.Debug.DummyDelay);

                return vd;
            }

            public static async  Task<SubscriptionCardSet> GetSubscriptionList()
            {
                await Task.Delay(AppSettings.Debug.DummyDelay);
                return new SubscriptionCardSet();
            }
            public async static Task<List<VideoCard>> GetCreatorRecentVideos(Creator c)
            {
                await Task.Delay(AppSettings.Debug.DummyDelay);
                return VideoCardSet.GetSampleVideoCardListOneCreator(c);
            }

            /// <summary>
            /// Gets the video like, dislike and view counts
            /// </summary>
            /// <param name="videoId">the video id string</param>
            /// <returns> int like, int dislike, int view</returns>
            public static async Task<Tuple<int, int, int>> GetVideoLikeAndViewCount(string videoId)
            {
                await Task.Delay(AppSettings.Debug.DummyDelay);
                
                Tuple<int, int, int> tuple = new Tuple<int, int, int>(6,6,6);
                return tuple;
            }
            public async static Task<List<Comment>> GetVideoComments (string videoId)
            {
                await Task.Delay(AppSettings.Debug.DummyDelay);
                return SampleCommentList.GetSampleCommentList();
            }

            public static async Task<int> GetCreatorCurrentSubCount(string creatorId)
            {
                await Task.Delay(AppSettings.Debug.DummyDelay);
                return 6666;
            }
        }

        public class Outbound
        {
            public static async Task<bool> SendComment (string videoId, string commentId, string comment)
            {
                await Task.Run(() =>
                {

                });
                return true;
            }

            public static async void SendVideoView(string videoId)
            {
                await Task.Run(() =>
                {

                });
            }

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

            /// <summary>
            /// returns a Task<string> response
            /// </summary>
            /// <param name="apiFormattedString">use BitChute API formatted string</param>
            /// <returns></returns>
            public static async Task<string> MakeAPICall(string apiFormattedString)
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

        }
    }
}