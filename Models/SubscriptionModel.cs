using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.ViewModels;
using static BottomNavigationViewPager.Models.VideoModel;

namespace BottomNavigationViewPager.Models
{
    public class SubscriptionModel : MvxViewModel
    {
        //a list of ALL creators user is subscribed to
        public List<Creator> subscriptionListLight;

        /// <summary>
        /// only the first 25 recent posts from users they're sub'd tp
        /// </summary>
        public List<CreatorPackage> subscriptionListFullPackage;

        
        //
        public class PostedContent
        {

        }
        
        /// <summary>
        /// lighter weight class that only contains the creator and link
        /// for getting the entire list of subs, doesn't contain video info
        /// </summary>
        public class Creator
        {
            string creatorName { get; set; }
            string creatorLinkToChannel { get; set; }
        }
        
        /// <summary>
        /// more resource intensive class for creators who have recently posted
        /// ,, this one contains video information for the creator's recently
        /// posted videos only
        /// </summary>
        public class CreatorPackage
        {
            public string _creatorName { get; set; }
            public string _creatorLinkToChannel { get; set; }
            public string _creatorDescription { get; set; }

            private List<VideoInfo> recentVideos;

            public List<VideoInfo> GetRecentVideos()
            {
                return recentVideos;
            }

            private void SetRecentVideos(List<VideoInfo> value)
            {
                recentVideos = value;
            }
        }
    }
}