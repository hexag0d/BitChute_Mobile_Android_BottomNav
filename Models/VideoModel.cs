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
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using static BitChute.Models.CommentModel;
using static BitChute.Models.CreatorModel;

namespace BitChute.Models
{
    /// <summary>
    /// contains the VideoModel which is really more of metadata
    /// since we're probably parsing links and then streaming p2p or https
    /// </summary>
    public class VideoModel 
    {
        public static Context context;
        public VideoModel(Context contextt)
        {
            context = contextt;
        }
        
        public static Android.Net.Uri SampleUri = Android.Net.Uri.Parse("android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd);
        public static Android.Net.Uri SampleUri2 = Android.Net.Uri.Parse("android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.test2);

        /// <summary>
        /// VideoDetail class contains the details for videos that would not be efficient to get all at once
        /// </summary>
        public class VideoDetail 
        {
            public VideoDetail videoInfo;

            public VideoDetail()
            {

            }

            public VideoDetail BuildVideoInfo(Drawable thumbnail, string creatorName, string videoTitle, 
                string videoLocation, string videoDescription)
            {
                VideoDetail newVid = new VideoDetail();

                videoInfo.ThumbnailDrawable = thumbnail;
                videoInfo.CreatorName = creatorName;
                videoInfo.VideoTitle = videoTitle;
                videoInfo.VideoLocation = videoLocation;
                videoInfo.VideoDescription = videoDescription;

                return newVid;
            }

            public string CreatorName { get; set; }
            public string VideoTitle { get; set; }
            public string VideoLocation { get; set; }
            public string VideoDescription { get; set; } //TODO: switch this to an object array
            public string VideoId { get; set; }
            public int ViewCount { get; set; }
            public int LikeCount { get; set; }
            public int DislikeCount { get; set; }
            public List<Comment> Comments { get; set; }

            public List<VideoCard> RelatedVideos { get; set; }

            public Uri VideoUri { get; set; }
            public Drawable ThumbnailDrawable { get; set; }
            public Bitmap ThumbnailBitmap { get; set; }
        }

        public static VideoDetail GetSample()
        {
            VideoDetail vi = new VideoDetail();
            vi.CreatorName = "Creator Name Here";
            vi.VideoTitle = "Video Title";
            vi.ViewCount = 777;
            vi.LikeCount = 6;
            vi.DislikeCount = 6;
            vi.VideoDescription = @"This is a sample description.  Waddap dough!  
                                    Thanks 4 watching from https://soundcloud.com/vybemasterz";
            //vi.ThumbnailDrawable = MainActivity.UniversalGetDrawable();

            return vi;
        }


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

                //  854 x 480 .mp4 h264 file but I can't commit it on github
                //  put a similar file in your resources/raw/ folder to test
                //  if it's too big the apk will fail to deploy but this is just for testing

                // TODO: need to make this more dynamic
                // essentially, we need to take the linkId from
                // bitchute JSON return and convert into URI for each video
                string vidpath = "android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd;

                VideoUri = Android.Net.Uri.Parse(vidpath);
                //Link = link;

                VideoId = videoId;
            }

            public VideoCard(int thumbnail, string title, string creatorName, string videoId, Creator creator, Android.Net.Uri vidUri)
            {
                Thumbnail = thumbnail;
                //title
                Title = title;
                //description

                Creator = creator;

                VideoUri = vidUri;
                //  854 x 480 .mp4 h264 file but I can't commit it on github
                //  put a similar file in your resources/raw/ folder to test
                //  if it's too big the apk will fail to deploy but this is just for testing

                // TODO: need to make this more dynamic
                // essentially, we need to take the linkId from
                // bitchute JSON return and convert into URI for each video
                string vidpath = "android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd;

                VideoUri = Android.Net.Uri.Parse(vidpath);
                //Link = link;

                VideoId = videoId;
            }


            public VideoCard(int thumbnail, string title, string videoId, Creator creator, Android.Net.Uri vidUri)
            {
                Thumbnail = thumbnail;
                //title
                Title = title;
                //description

                Creator = creator;

                VideoUri = vidUri;
                //  854 x 480 .mp4 h264 file but I can't commit it on github
                //  put a similar file in your resources/raw/ folder to test
                //  if it's too big the apk will fail to deploy but this is just for testing

                // TODO: need to make this more dynamic
                // essentially, we need to take the linkId from
                // bitchute JSON return and convert into URI for each video
                
                //Link = link;

                VideoId = videoId;
            }

            public Creator Creator {get;}
            public int Thumbnail { get; }
            //Title of Video
            public string Title { get; }
            //Description
            public string Link { get; }
            public string VideoId { get; set; }

            public Android.Net.Uri VideoUri { get; set; }

            public int Index { get; }
            public Drawable ThumbnailDrawable { get; set; }
            public Bitmap ThumbnailBitmap { get; set; }
        }



        public class VideoCardSet
        {
            public List<VideoCard> GetVideoCardSet(List<VideoDetail> videoInfoSet)
            {
                return RealThumbSample;
            }
            // Built-in photo collection - this could be replaced with
            // a photo database:

            public static Android.Net.Uri sampleUri = Android.Net.Uri.Parse("android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.test2);

            static List<VideoCard> BigList = new List<VideoCard>{
new VideoCard (Resource.Drawable._i50, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago50", "postedvideoNAMEdBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago50","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i51, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i52, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago52", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i53, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago53", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i54, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago54", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i55, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago55", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i56, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago56", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i57, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago57", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i58, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago58", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i59, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago59", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i60, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago60", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i61, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago61", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i62, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago62", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i63, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago63", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i64, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago64", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i65, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago65", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i66, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago66", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i67, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago67", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i68, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago68", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i69, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago69", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i70, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago70", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i71, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago71", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i72, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago72", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i73, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago73", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i74, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago74", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i75, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago75", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i76, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago76", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i77, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago77", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i78, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago78", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i79, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago79", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i80, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago80", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i81, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago81", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i82, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago82", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i83, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago83", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i84, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago84", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i85, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago85", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i86, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago86", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i87, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago87", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i88, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago88", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i89, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago89", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i90, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago90", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i91, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago91", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i92, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago92", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i93, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago93", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i94, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago94", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i95, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago95", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i96, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago96", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i97, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago97", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._i98, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago98", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator(), sampleUri),
            };

            static List<VideoCard> mBuiltInPhotos = new List<VideoCard>{
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 1", "Creator 1","videoID1", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 2", "Creator 2","videoID2", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb3, "Video 3", "Creator 3","videoID3", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb4, "Video 4", "Creator 4","videoID4", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 5", "Creator 5","videoID5", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 6", "Creator 6","videoID6", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 1", "Creator 1","videoID1", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 2", "Creator 2","videoID2", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb3, "Video 3", "Creator 3","videoID3", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb4, "Video 4", "Creator 4","videoID4", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 5", "Creator 5","videoID5", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 6", "Creator 6","videoID6", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 1", "Creator 1","videoID1", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 2", "Creator 2","videoID2", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb3, "Video 3", "Creator 3","videoID3", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb4, "Video 4", "Creator 4","videoID4", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 5", "Creator 5","videoID5", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 6", "Creator 6","videoID6", GetSampleCreator(), sampleUri),
  };

            static List<VideoCard> RealThumbSample = new List<VideoCard>{
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", GetSampleCreator(), sampleUri),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", GetSampleCreator(), sampleUri),
  };


            public static List<VideoCard> GetSampleVideoCardListOneCreator(Creator creator)
            {
                List<VideoCard> sampleList = new List<VideoCard>{
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "videoID1", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "videoID2", creator, SampleUri2),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "videoID3", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "videoID4", creator, SampleUri2),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "videoID5", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "videoID1", creator, SampleUri2),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "videoID2", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "videoID3", creator, SampleUri2),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "videoID4", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "videoID5", creator, SampleUri2),
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "videoID1", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "videoID2", creator, SampleUri2),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "videoID3", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "videoID4", creator, SampleUri2),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "videoID5", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "videoID1", creator, SampleUri2),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "videoID2", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "videoID3", creator, SampleUri2),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "videoID4", creator, SampleUri),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "videoID5", creator, SampleUri2),
  };
                return sampleList;
            }

            public static List<VideoCard> GetSampleRelatedVids(Creator c)
            {
                List<VideoCard> videoCards = new List<VideoCard>
                {
                    new VideoCard(Resource.Drawable._testThumb360_3, "Sample related video 1", "/video/bajsdijasd", c, SampleUri),
                    new VideoCard(Resource.Drawable._testThumb360_1, "Sample related video 2", "/video/sampleurl", c, SampleUri2),
                    new VideoCard(Resource.Drawable._testThumb360_2, "Sample related video 3", "/video/youououo", c, SampleUri)
                };
                return videoCards;
            } 

            private List<VideoCard> mPhotos = new List<VideoCard>();
            
            public VideoCardSet()
            {
                mPhotos = RealThumbSample;
            }
            
            public int NumPhotos
            {
                get { return mPhotos.Count; }
            }

            // Indexer (read only) for accessing a photo:
            public VideoCard this[int i]
            {
                get { return mPhotos[i]; }
            }
        }


    }
}