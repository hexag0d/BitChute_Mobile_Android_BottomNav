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
using static BitChute.Models.VideoModel;

namespace BitChute.Models
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
                List<VideoCardNoCreator> recentVideoCardList, int thumbnail)
            {
                Name = name;
                LinkToChannel = linkToChannel;
                MostRecentVideoLink = recentVideoCardList.First().Link;
                MostRecentVideoTitle = recentVideoCardList.First().Title;
                RecentVideoCards = recentVideoCardList;
                creatorAvatar = MainActivity.UniversalGetDrawable(thumbnail);
                CreatorThumbnailBitmap = creatorAvatarBitmap;
            }

            public Creator(Drawable creatorAvatar, Bitmap creatorAvatarBitmap, string name, string linkToChannel,
                List<VideoCardNoCreator> recentVideoCardList)
            {
                Name = name;
                LinkToChannel = linkToChannel;
                MostRecentVideoLink = recentVideoCardList.First().Link;
                MostRecentVideoTitle = recentVideoCardList.First().Title;
                RecentVideoCards = recentVideoCardList;
                CreatorThumbnailDrawable = creatorAvatar;
                CreatorThumbnailBitmap = creatorAvatarBitmap;
            }

            public string Name { get; set; }
            public string LinkToChannel { get; set; }
            public string MostRecentVideoLink { get; set; }
            public string MostRecentVideoTitle { get; set; }

            public Drawable CreatorThumbnailDrawable { get; set; }
            public Bitmap CreatorThumbnailBitmap { get; set; }

            public string CreatorDescription { get; set; }

            public List<VideoCardNoCreator> RecentVideoCards;
            
        }
        
        public static List<VideoCardNoCreator> CacheVideoCards = VideoCardNoCreator.GetVideoCardNoCreatorList();

        public static List<VideoCardNoCreator> Get3RandomCards()
        {
            Random rnd = new Random();
            rnd.Next(19);

            var lis = new List<VideoCardNoCreator>();
            lis.Add(CacheVideoCards[rnd.Next(19)]);
            lis.Add(CacheVideoCards[rnd.Next(19)]);
            lis.Add(CacheVideoCards[rnd.Next(19)]);
            return lis;
        }

        /// <summary>
        /// just testing this out... obviously this isn't how we do it lol
        /// eventually something similar to this model will be populated from JSON parsed returns
        /// so we can use this to build out the app until the API goes live
        /// </summary>
        /// <returns></returns>
        public static List<Creator> GetCreatorList()
        {
            List<Creator> creatorList = new List<Creator>();
            creatorList.Add(new Creator(null, null, "warski", @"\channel\warski", Get3RandomCards(), Resource.Drawable._i50));
            creatorList.Add(new Creator(null, null, "styx", @"\channel\styx", Get3RandomCards(), Resource.Drawable._i51));
            creatorList.Add(new Creator(null, null, "burmas", @"\channel\burmas", Get3RandomCards(), Resource.Drawable._i52));
            creatorList.Add(new Creator(null, null, "hotep", @"\channel\warski", Get3RandomCards(), Resource.Drawable._i53));
            creatorList.Add(new Creator(null, null, "hotep", @"\channel\hotep", Get3RandomCards(), Resource.Drawable._i54));
            creatorList.Add(new Creator(null, null, "jchan", @"\channel\jchan", Get3RandomCards(), Resource.Drawable._i55));
            creatorList.Add(new Creator(null, null, "goodman", @"\channel\goodman", Get3RandomCards(), Resource.Drawable._i56));
            creatorList.Add(new Creator(null, null, "ralph", @"\channel\ralph", Get3RandomCards(), Resource.Drawable._i57));
            creatorList.Add(new Creator(null, null, "duck", @"\channel\duck", Get3RandomCards(), Resource.Drawable._i58));
            creatorList.Add(new Creator(null, null, "hb", @"\channel\hb", Get3RandomCards(), Resource.Drawable._i59));
            return creatorList;
        }

        /// <summary>
        /// Class that contains a creator card
        /// </summary>
        public class CreatorCard
        {
            public CreatorCard()
            {

            }

            public CreatorCard(Creator creator)
            {
            } 

            public CreatorCard(int id, string title, string creatorName, string link, Creator creator)
            {
                PhotoID = id;
                //title
                Title = title;
                //description
                CreatorName = creatorName;
                Creator = creator;
                Link = link;
            }

            public Creator Creator { get; }

            public int PhotoID { get; }

            //Title of Video
            public string Title { get; }

            //Description
            public string CreatorName { get; }

            public string Link { get; }

            public int Index { get; }
        }
        
        public static Creator GetSampleCreator()
        {
            Creator sample = new Creator();
            sample.Name = "A Random Creator";
            sample.LinkToChannel = "/channel/aRandomNameHere";
            return sample;
        }
    }
}