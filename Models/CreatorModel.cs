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
using static BottomNavigationViewPager.Models.VideoModel;

namespace BottomNavigationViewPager.Models
{
    public class CreatorModel
    {
        /// <summary>
        /// lighter weight class that only contains the creator and link
        /// for getting the entire list of subs, doesn't contain video info
        /// </summary>
        public class Creator
        {
            public Creator()
            {
            }

            public Creator(Drawable creatorAvatar, Bitmap creatorAvatarBitmap, string name, string linkToChannel,
                List<VideoCard> recentVideoCardList, int thumbnail)
            {
                Name = name;
                LinkToChannel = linkToChannel;
                MostRecentVideoLink = recentVideoCardList.First().Link;
                MostRecentVideoTitle = recentVideoCardList.First().Title;
                RecentVideoCards = recentVideoCardList;
                CreatorThumbnailBitmap = creatorAvatarBitmap;
            }


            public Creator(Drawable creatorAvatar, Bitmap creatorAvatarBitmap, string name, string linkToChannel,
                List<VideoCard> recentVideoCardList, int thumbnail, int subcount)
            {
                Name = name;
                LinkToChannel = linkToChannel;
                MostRecentVideoLink = recentVideoCardList.First().Link;
                MostRecentVideoTitle = recentVideoCardList.First().Title;
                RecentVideoCards = recentVideoCardList;
                CreatorThumbnailBitmap = creatorAvatarBitmap;
            }

            public Creator(Drawable creatorAvatar, Bitmap creatorAvatarBitmap, string name, string linkToChannel,
                List<VideoCard> recentVideoCardList)
            {
                Name = name;
                LinkToChannel = linkToChannel;
                MostRecentVideoLink = recentVideoCardList.First().Link;
                MostRecentVideoTitle = recentVideoCardList.First().Title;
                RecentVideoCards = recentVideoCardList;
                CreatorThumbnailDrawable = creatorAvatar;
                CreatorThumbnailBitmap = creatorAvatarBitmap;
                //MostRecentVideoUri = Android.Net.Uri.Parse("android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd);
                //MostRecentVideoUri = Android.Net.Uri.Parse(MostRecentVideoLink); //TODO uncomment
            }

            public string Name { get; set; }
            public string LinkToChannel { get; set; }
            public string MostRecentVideoLink { get; set; }
            public string MostRecentVideoTitle { get; set; }
            public Android.Net.Uri MostRecentVideoUri { get; set; }
            public int SubCount { get; set; }

            public Drawable CreatorThumbnailDrawable { get; set; }
            public Bitmap CreatorThumbnailBitmap { get; set; }

            public string CreatorDescription { get; set; }

            public List<VideoCard> RecentVideoCards;

        }
    }
}