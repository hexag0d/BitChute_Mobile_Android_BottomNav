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

            public VideoCard(int id, string title, string creatorName, string videoId, Creator creator)
            {
                PhotoID = id;
                //title
                Title = title;
                //description
                CreatorName = creatorName;

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

            public VideoCard(int id, string title, string creatorName, string videoId, Creator creator, Android.Net.Uri vidUri)
            {
                PhotoID = id;
                //title
                Title = title;
                //description
                CreatorName = creatorName;

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

            public Creator Creator {get;}
            public int PhotoID { get; }
            //Title of Video
            public string Title { get; }
            //Description
            public string CreatorName { get; }
            public string Link { get; }
            public string VideoId { get; set; }

            public Android.Net.Uri VideoUri { get; set; }

            public int Index { get; }
            public Drawable ThumbnailDrawable { get; set; }
            public Bitmap ThumbnailBitmap { get; set; }
        }

        /// <summary>
        /// class that contains video information for the user to select
        /// should not contain video data, this class is for the metadata of the video
        /// </summary>
        public class VideoCardNoCreator
        {
            public static List<VideoCardNoCreator> GetVideoCardNoCreatorList()
            {
                return VideoCardNoCreatorList;
            }

            static List<VideoCardNoCreator> VideoCardNoCreatorList = new List<VideoCardNoCreator>{
new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", Resource.Drawable._testThumb360_0, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", Resource.Drawable._testThumb360_1, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", Resource.Drawable._testThumb360_2, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", Resource.Drawable._testThumb360_3, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", Resource.Drawable._testThumb360_4, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", Resource.Drawable._testThumb360_0, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", Resource.Drawable._testThumb360_1, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", Resource.Drawable._testThumb360_2, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", Resource.Drawable._testThumb360_3, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", Resource.Drawable._testThumb360_4, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", Resource.Drawable._testThumb360_0, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", Resource.Drawable._testThumb360_1, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", Resource.Drawable._testThumb360_2, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", Resource.Drawable._testThumb360_3, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", Resource.Drawable._testThumb360_4, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", Resource.Drawable._testThumb360_0, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", Resource.Drawable._testThumb360_1, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", Resource.Drawable._testThumb360_2, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", Resource.Drawable._testThumb360_3, null, null ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", Resource.Drawable._testThumb360_4, null, null ),
  };


            public static List<VideoCardNoCreator> GetVideoCardNoCreatorListSamePerson(Creator creator)
            {
                
                var list = new List<VideoCardNoCreator>{
new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "","videoID1", Resource.Drawable._testThumb360_0, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "","videoID2", Resource.Drawable._testThumb360_1, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "","videoID3", Resource.Drawable._testThumb360_2, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "","videoID4", Resource.Drawable._testThumb360_3, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "","videoID5", Resource.Drawable._testThumb360_4, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "","videoID1", Resource.Drawable._testThumb360_0, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "","videoID2", Resource.Drawable._testThumb360_1, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "","videoID3", Resource.Drawable._testThumb360_2, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "","videoID4", Resource.Drawable._testThumb360_3, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "","videoID5", Resource.Drawable._testThumb360_4, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "","videoID1", Resource.Drawable._testThumb360_0, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "","videoID2", Resource.Drawable._testThumb360_1, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "","videoID3", Resource.Drawable._testThumb360_2, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "","videoID4", Resource.Drawable._testThumb360_3, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "","videoID5", Resource.Drawable._testThumb360_4, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_0, "Video 1", "","videoID1", Resource.Drawable._testThumb360_0, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_1, "Video 2", "","videoID2", Resource.Drawable._testThumb360_1, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_2, "Video 3", "","videoID3", Resource.Drawable._testThumb360_2, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_3, "Video 4", "","videoID4", Resource.Drawable._testThumb360_3, null, creator.Name ),
new VideoCardNoCreator (Resource.Drawable._testThumb360_4, "Video 5", "","videoID5", Resource.Drawable._testThumb360_4, null, creator.Name ),
  };
                return list;
            }

            public VideoCardNoCreator()
            {
            }

            /// <summary>
            /// this type of VideoCard contains no Creator object
            /// so it's used for situations where a Creator
            /// is the container
            /// 
            /// Feel free to clean up these classes or redo them
            /// I'm still trying to work out the most efficient way to do this
            /// 
            /// I could use one class for all of them, maybe will eventually
            /// </summary>
            /// <param name="id"></param>
            /// <param name="title"></param>
            /// <param name="caption2"></param>
            /// <param name="link"></param>
            /// <param name="drawableID"></param>
            /// <param name="thumbbmp"></param>
            public VideoCardNoCreator(int id, string title, string caption2, 
                string link, int drawableID , Bitmap thumbbmp, string creatorName )
            {
                //int for the drawable but I don't know if this is going to work when the app is on an API
                PhotoID = id;
                //title
                Title = title;
                //description
                CreatorName = creatorName;
                Link = link;

                Uri = SampleUri;

                if (drawableID != 0)
                {
                    ThumbnailDrawable = MainActivity.UniversalGetDrawable(drawableID);
                }
                if (thumbbmp != null)
                {
                    ThumbnailBitmap = thumbbmp;
                }
            }

            public VideoCardNoCreator(int id, string title, string caption2,
    string link, int drawableID, Bitmap thumbbmp, string creatorName, Android.Net.Uri uri)
            {
                //int for the drawable but I don't know if this is going to work when the app is on an API
                PhotoID = id;
                //title
                Title = title;
                //description
                CreatorName = creatorName;
                Link = link;
                uri = SampleUri; //TODO : remove the sample
                Uri = uri;

                if (drawableID != 0)
                {
                    ThumbnailDrawable = MainActivity.UniversalGetDrawable(drawableID);
                }
                if (thumbbmp != null)
                {
                    ThumbnailBitmap = thumbbmp;
                }
            }
            //video thumbnail resource int
            public int PhotoID { get; set; }
            //Title of Video
            public string Title { get; set; }
            //Description
            public string Caption2 { get; set; }
            public string Link { get; set; }
            public int Index { get; set; }
            public string CreatorName { get; set; }

            public Android.Net.Uri Uri { get; set; }

            //I've added these two members because I don't know exactly how the JSON caching is going to work
            public Drawable ThumbnailDrawable { get; set; }

            public Bitmap ThumbnailBitmap { get; set; }

            public bool Equals(VideoCardNoCreator other)
            {
                if (this.PhotoID == other.PhotoID && this.Title == other.Title
                    && this.Caption2 == other.Caption2 && this.CreatorName == other.CreatorName 
                    && this.ThumbnailBitmap == other.ThumbnailBitmap 
                    && this.ThumbnailDrawable == other.ThumbnailDrawable)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        public class VideoCardSet
        {
            public List<VideoCard> GetVideoCardSet(List<VideoDetail> videoInfoSet)
            {
                return RealThumbSample;
            }
            // Built-in photo collection - this could be replaced with
            // a photo database:

            static List<VideoCard> BigList = new List<VideoCard>{
new VideoCard (Resource.Drawable._i50, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago50", "postedvideoNAMEdBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago50","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i51, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i52, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago52", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i53, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago53", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i54, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago54", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i55, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago55", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i56, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago56", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i57, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago57", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i58, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago58", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i59, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago59", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i60, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago60", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i61, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago61", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i62, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago62", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i63, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago63", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i64, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago64", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i65, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago65", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i66, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago66", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i67, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago67", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i68, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago68", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i69, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago69", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i70, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago70", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i71, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago71", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i72, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago72", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i73, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago73", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i74, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago74", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i75, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago75", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i76, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago76", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i77, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago77", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i78, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago78", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i79, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago79", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i80, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago80", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i81, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago81", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i82, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago82", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i83, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago83", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i84, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago84", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i85, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago85", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i86, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago86", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i87, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago87", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i88, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago88", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i89, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago89", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i90, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago90", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i91, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago91", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i92, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago92", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i93, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago93", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i94, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago94", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i95, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago95", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i96, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago96", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i97, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago97", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
new VideoCard (Resource.Drawable._i98, "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago98", "postedvideotitledBITCHUTEGETSTHEAPIxxxxxxxxxxminutesago51","videoID", GetSampleCreator()),
            };

            static List<VideoCard> mBuiltInPhotos = new List<VideoCard>{
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 1", "Creator 1","videoID1", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 2", "Creator 2","videoID2", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb3, "Video 3", "Creator 3","videoID3", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb4, "Video 4", "Creator 4","videoID4", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 5", "Creator 5","videoID5", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 6", "Creator 6","videoID6", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 1", "Creator 1","videoID1", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 2", "Creator 2","videoID2", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb3, "Video 3", "Creator 3","videoID3", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb4, "Video 4", "Creator 4","videoID4", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 5", "Creator 5","videoID5", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 6", "Creator 6","videoID6", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 1", "Creator 1","videoID1", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 2", "Creator 2","videoID2", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb3, "Video 3", "Creator 3","videoID3", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb4, "Video 4", "Creator 4","videoID4", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb, "Video 5", "Creator 5","videoID5", GetSampleCreator()),
new VideoCard (Resource.Drawable._sampleVideoThumb2, "Video 6", "Creator 6","videoID6", GetSampleCreator()),
  };

            static List<VideoCard> RealThumbSample = new List<VideoCard>{
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_0, "Video 1", "Creator 1","videoID1", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_1, "Video 2", "Creator 2","videoID2", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_2, "Video 3", "Creator 3","videoID3", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_3, "Video 4", "Creator 4","videoID4", GetSampleCreator()),
new VideoCard (Resource.Drawable._testThumb360_4, "Video 5", "Creator 5","videoID5", GetSampleCreator()),
  };
            
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