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
            public string Name { get; set; }
            public string LinkToChannel { get; set; }
            public string MostRecentVideoLink { get; set; }

            public Drawable CreatorThumbnailDrawable { get; set; }
            public Bitmap CreatorThumbnailBitmap { get; set; }

            public class CreatorDetail
            {
                string CreatorDescription { get; set; }
                //VideoCardSet RecentVideos { get; set; }

                public class CreatorVideos
                {

                }
            }
        }

        /// <summary>
        /// Class that contains a creator card
        /// </summary>
        public class CreatorCard
        {
            public CreatorCard()
            {

            }

            public CreatorCard(int id, string caption, string caption2, string link, Creator creator)
            {
                PhotoID = id;
                //title
                Caption = caption;
                //description
                Caption2 = caption2;

                Creator = creator;

                Link = link;
            }

            public Creator Creator { get; }

            public int PhotoID { get; }

            //Title of Video
            public string Caption { get; }

            //Description
            public string Caption2 { get; }

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