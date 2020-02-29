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
using static BitChute.Models.CreatorModel;
using static BitChute.Models.VideoModel;

namespace BitChute.Models
{
    public class SubscriptionModel 
    {
        //a list of ALL creators user is subscribed to
        public List<Creator> subscriptionListLight;

        public List<Creator> GetLightSubscriptionList()
        {
            subscriptionListLight = new List<Creator>();
            return subscriptionListLight;
        }

        /// <summary>
        /// only the first 25 recent posts from users they're sub'd to
        /// </summary>
        public List<CreatorPackage> subscriptionListFullPackage;

        
        //
        public class PostedContent
        {

        }
        

        /// <summary>
        /// more resource intensive class for creators who have recently posted
        /// ,, this one contains video information for the creator's recently
        /// posted videos only
        /// </summary>
        public class CreatorPackage
        {
            public string CreatorName { get; set; }
            public string CreatorLinkToChannel { get; set; }
            public string CreatorDescription { get; set; }

            private List<VideoCard> recentVideos;

            public List<VideoCard> GetRecentVideos()
            {
                return recentVideos;
            }

            private void SetRecentVideos(List<VideoCard> value)
            {
                recentVideos = value;
            }
        }
    }
}