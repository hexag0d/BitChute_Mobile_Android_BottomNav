﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
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
        
        public class SubscriptionCardSet
        {
            static List<CreatorCard> SubscriptionCreatorCardList = new List<CreatorCard>{
new CreatorCard (Resource.Drawable._i50, "Video Title Here", "warski","videoID", GetSampleCreator(Resource.Drawable._i50)),
new CreatorCard (Resource.Drawable._i51, "Video Title Here", "styx","videoID", GetSampleCreator(Resource.Drawable._i51)),
new CreatorCard (Resource.Drawable._i52, "Video Title Here", "burmas","videoID", GetSampleCreator(Resource.Drawable._i52)),
new CreatorCard (Resource.Drawable._i53, "Video Title Here", "hotep","videoID", GetSampleCreator(Resource.Drawable._i53)),
new CreatorCard (Resource.Drawable._i54, "Video Title Here", "junsonchan","videoID", GetSampleCreator(Resource.Drawable._i54)),
new CreatorCard (Resource.Drawable._i55, "Video Title Here", "joeysalads","videoID", GetSampleCreator(Resource.Drawable._i55)),
new CreatorCard (Resource.Drawable._i56, "pos", "goodman","videoID", GetSampleCreator(Resource.Drawable._i56)),
new CreatorCard (Resource.Drawable._i57, "d", "ralph","videoID", GetSampleCreator(Resource.Drawable._i57)),
new CreatorCard (Resource.Drawable._i58, "d", "duckhk","videoID", GetSampleCreator(Resource.Drawable._i58)),
new CreatorCard (Resource.Drawable._i59, "video name", "hardbastard","videoID", GetSampleCreator(Resource.Drawable._i59)),
new CreatorCard (Resource.Drawable._i60, "vid name", "cernovich","videoID", GetSampleCreator(Resource.Drawable._i60)),
new CreatorCard (Resource.Drawable._i61, "a random vid", "amazingatheist","videoID", GetSampleCreator(Resource.Drawable._i61)),
new CreatorCard (Resource.Drawable._i62, "question eerythang", "corbettreport","videoID", GetSampleCreator(Resource.Drawable._i62)),
new CreatorCard (Resource.Drawable._i63, "random animal crossing", "hog","videoID", GetSampleCreator(Resource.Drawable._i63)),
new CreatorCard (Resource.Drawable._i64, "RF RANT", "razor","videoID", GetSampleCreator(Resource.Drawable._i64)),
new CreatorCard (Resource.Drawable._i65, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago65", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(Resource.Drawable._i65)),
new CreatorCard (Resource.Drawable._i66, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago66", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(Resource.Drawable._i66)),
new CreatorCard (Resource.Drawable._i67, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago67", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(Resource.Drawable._i67)),
new CreatorCard (Resource.Drawable._i68, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago68", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(Resource.Drawable._i68)),
new CreatorCard (Resource.Drawable._i69, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago69", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(Resource.Drawable._i69)),
new CreatorCard (Resource.Drawable._i70, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago70", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i71, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago71", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i72, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago72", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i73, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago73", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i74, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago74", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i75, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago75", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i76, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago76", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i77, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago77", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i78, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago78", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i79, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago79", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i80, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago80", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i81, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago81", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i82, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago82", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i83, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago83", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i84, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago84", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i85, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago85", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i86, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago86", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i87, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago87", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i88, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago88", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i89, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago89", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i90, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago90", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i91, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago91", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i92, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago92", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i93, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago93", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i94, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago94", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i95, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago95", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i96, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago96", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i97, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago97", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new CreatorCard (Resource.Drawable._i98, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago98", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
            };


            private List<CreatorCard> _creatorCardList = new List<CreatorCard>();

            public SubscriptionCardSet()
            {
                _creatorCardList = SubscriptionCreatorCardList;
            }

            public int NumPhotos
            {
                get { return _creatorCardList.Count; }
            }

            // Indexer (read only) for accessing a photo:
            public CreatorCard this[int i]
            {
                get { return _creatorCardList[i]; }
            }
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