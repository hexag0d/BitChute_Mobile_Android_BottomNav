using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static BitChute.Models.CreatorModel;

namespace BitChute.Models
{
    public class VideoModel
    {
        /// <summary>
        /// class that contains video information for the user to select
        /// should not contain video data, this class is for the metadata of the video
        /// </summary>
        public class VideoCard
        {
            public VideoCard()
            {
            }

            public VideoCard(int id, string title, string videoId, Creator creator)
            {
                Thumbnail = id;
                //title
                Title = title;
                //description

                Creator = creator;

                // TODO: need to make this more dynamic
                // essentially, we need to take the linkId from
                // bitchute JSON return and convert into URI for each video
                //string vidpath = "android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd;
                

                VideoId = videoId;
            }

            public VideoCard(int thumbnail, string title, string creatorName, string videoId, Creator creator, System.Uri vidUri)
            {
                Thumbnail = thumbnail;
                //title
                Title = title;
                //description
                Creator = creator;
                VideoUri = vidUri;
                // TODO: need to make this more dynamic
                // essentially, we need to take the linkId from
                // bitchute JSON return and convert into URI for each video
               // string vidpath = "android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd;
                //VideoUri = new System.Uri(vidpath);
                //Link = link;
                VideoId = videoId;
            }

            public VideoCard(int thumbnail, string title, string videoId, Creator creator, System.Uri vidUri)
            {
                Thumbnail = thumbnail;
                //title
                Title = title;
                //description
                Creator = creator;
                VideoUri = vidUri;
                // TODO: need to make this more dynamic
                // essentially, we need to take the linkId from
                // bitchute JSON return and convert into URI for each video
                //Link = link;
                VideoId = videoId;
            }

            public Creator Creator { get; }
            public int Thumbnail { get; }
            //Title of Video
            public string Title { get; set; }
            //Description
            public string Link { get; set; }
            public string VideoId { get; set; }
            public System.Uri VideoUri { get; set; }
            public Android.Net.Uri VideoUriLocal { get; set; }
            public int Index { get; }
            public Drawable ThumbnailDrawable { get; set; }
            public Bitmap ThumbnailBitmap { get; set; }
            public Android.Net.Uri ThumbnailUri { get; set; }
            public string CreatorName { get; set; }
        }
    }
}