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
        public List<PostedContent> subscriptionList;

        //
        public class PostedContent
        {

        }
        
        //class specific to the recent content posted by a creator
        public class CreatorPackage
        {
            public string _creatorName { get; set; }
            public string _creatorPage { get; set; }
            public string _creatorDescription { get; set; }
            public class VideosPosted
            {
                List<VideoInfo> RecentPosts;
            }
        }
    }
}