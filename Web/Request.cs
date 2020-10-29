using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Support.V7.Widget;
using BitChute.Fragments;
using HtmlAgilityPack;
using static BitChute.Models.VideoModel;

namespace BitChute.Web
{
    public class Request
    {
        public class UrlStrings
        {
            public static string Domain = "https://www.bitchute.com";
            public static string SubscriptionFullFeed = "https://www.bitchute.com/";
        }

        public static System.Net.Http.HttpClient ImageHttpClient;
        public static System.Net.Http.HttpClientHandler HttpClientHandler;

        public static async Task<Bitmap> GetBitmapDrawable(string url)
        {
            try {
                var stream = await ImageHttpClient.GetStreamAsync(url);
                return BitmapFactory.DecodeStream(stream);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static void SendVideoCardListToFeedRecycler(VideoCard vc, Android.Support.V7.Widget.RecyclerView recyclerView, int id)
        {
            if (GetFeedRecyclerViewAdapter == null)
            {
                List<VideoCard> vcl = new List<VideoCard>();
                vcl.Add(vc);
                GetFeedRecyclerViewAdapter = new FeedRecyclerViewAdapter(vcl);
                GetFeedRecyclerViewAdapter.ItemClick += CommonFrag.GetFragmentById(id).RootVideoAdapter_ItemClick;
                recyclerView.SetAdapter(GetFeedRecyclerViewAdapter);
            }
            else
            {
                GetFeedRecyclerViewAdapter.UpdateDataSet(vc);
            }
        }

        private static FeedRecyclerViewAdapter _feedRecyclerAdapter;
        public static FeedRecyclerViewAdapter GetFeedRecyclerViewAdapter { get { return _feedRecyclerAdapter; } set { _feedRecyclerAdapter = value; } }

        public static void SendVideoCardListToFeedRecycler(List<VideoCard> vcl, Android.Support.V7.Widget.RecyclerView recyclerView, int id = -1)
        {
            if (GetFeedRecyclerViewAdapter == null)
            {
                GetFeedRecyclerViewAdapter = new FeedRecyclerViewAdapter(vcl);
                GetFeedRecyclerViewAdapter.ItemClick += CommonFrag.GetFragmentById(id).RootVideoAdapter_ItemClick;
                recyclerView.SetAdapter(GetFeedRecyclerViewAdapter);
            }
            else
            {
                GetFeedRecyclerViewAdapter.UpdateDataSet(vcl, true);
            }
        }

        private static int _numberOfCardsInQueue;
        private static List<VideoCard> _videoCardList = new List<VideoCard>();
        public static async void AddSingleVideoToList(VideoCard vc, RecyclerView recyclerView, int fragid)
        {
            vc.ThumbnailBitmap = await GetBitmapDrawable(vc.ThumbnailPath);

            SendVideoCardListToFeedRecycler(vc, recyclerView, fragid);
        }

        public static async Task<List<VideoCard>> GetVideoCardList(int fragId, RecyclerView recyclerView = null)
        {
            HttpClientHandler = new System.Net.Http.HttpClientHandler() { UseCookies = false };
            ImageHttpClient = new System.Net.Http.HttpClient(HttpClientHandler);

            List<VideoCard> videoCards = new List<VideoCard>();

            var response = await ExtWebInterface.HttpClient.GetAsync(UrlStrings.SubscriptionFullFeed);

            var html = await response.Content.ReadAsStringAsync();
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                if (doc != null)
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@id='listing-subscribed']"))
                    {
                        int vcNum = -1;
                        foreach (HtmlNode videoCardNode in node.SelectNodes(".//div[@class='video-card']"))
                        {
                            vcNum++;
                            VideoCard vc = new VideoCard();
                            //vc.ThumbnailUri = Android.Net.Uri.Parse(videoCardNode.SelectSingleNode(".//img").GetAttributeValue("data-src", ""));
                            vc.ThumbnailPath = videoCardNode.SelectSingleNode(".//img").GetAttributeValue("data-src", "");
                            vc.FragmentId = fragId;
                            //vc.ThumbnailBitmap = await GetBitmapDrawable(videoCardNode.SelectSingleNode(".//img").GetAttributeValue("data-src", ""));
                            int nodeNumber = -1;
                            foreach (HtmlNode aNode in videoCardNode.SelectNodes(".//a[@class='spa']"))
                            {
                                nodeNumber++;
                                if (nodeNumber == 0) { vc.Link = aNode.GetAttributeValue("href", ""); }
                                else if (nodeNumber == 1) { vc.Title = aNode.InnerText; }
                                else if (nodeNumber == 2) { vc.CreatorName = aNode.InnerText; }
                            }
                            if (vcNum >= 10)
                            {

                            }
                            AddSingleVideoToList(vc, recyclerView, fragId);
                            //videoCards.Add(vc);
                        }
                        //lstRecords.Add(record);
                    }
                    return videoCards;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public static string GetFullRequest(string afterRoot)
        {
            if (!afterRoot.StartsWith("/")) { afterRoot = afterRoot.Insert(0, @"/"); }
            return $"{UrlStrings.Domain}{afterRoot}";
        }
    }
}