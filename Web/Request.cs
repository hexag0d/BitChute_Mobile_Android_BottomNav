using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HtmlAgilityPack;
using Java.IO;
using Java.Net;
using static BitChute.Models.VideoModel;
using static BitChute.Web.ExtWebInterface;

namespace BitChute.Web
{
    public class Request
    {
        public class UrlStrings
        {
            public static string SubscriptionFullFeed = "https://www.bitchute.com/";
        }

        public static System.Net.Http.HttpClient ImageHttpClient;
        public static System.Net.Http.HttpClientHandler HttpClientHandler;

        public static async Task<Bitmap> GetBitmapDrawable(string url)
        {
            //HttpURLConnection connection = (HttpURLConnection)new URL(url).OpenConnection();

            try {

                var stream = await ImageHttpClient.GetStreamAsync(url);
                //var request = ExtWebInterface.HttpClient.GetAsync(url).Result;
                //var stream = await request.Content.ReadAsStreamAsync();
                //connection.Connect();
                //var stream = connection.InputStream;
                //var bmpdrawable = Drawable.CreateFromResourceStreamAsync(stream)

                return BitmapFactory.DecodeStream(stream);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static async Task<List<VideoCard>> GetVideoCardList()
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
                        foreach (HtmlNode videoCardNode in node.SelectNodes(".//div[@class='video-card']"))
                        {
                            VideoCard vc = new VideoCard();
                            //vc.ThumbnailUri = Android.Net.Uri.Parse(videoCardNode.SelectSingleNode(".//img").GetAttributeValue("data-src", ""));
                            vc.ThumbnailBitmap = await GetBitmapDrawable(videoCardNode.SelectSingleNode(".//img").GetAttributeValue("data-src", ""));
                            int nodeNumber = -1;
                            foreach (HtmlNode aNode in videoCardNode.SelectNodes(".//a[@class='spa']"))
                            {
                                nodeNumber++;
                                if (nodeNumber == 0) { vc.Link = aNode.GetAttributeValue("href", ""); }
                                else if (nodeNumber == 1) { vc.Title = aNode.InnerText; }
                                else if (nodeNumber == 2) { vc.CreatorName = aNode.InnerText; }
                            }
                            videoCards.Add(vc);
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
    }
}