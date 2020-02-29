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

namespace BottomNavigationViewPager.Models
{
    public class VideoModel 
    {
        public static Context context;
        public VideoModel(Context contextt)
        {
            context = contextt;
        }

        public class VideoInfo 
        {
            public VideoInfo videoInfo;

            public VideoInfo BuildVideoInfo(Drawable thumbnail, string creatorName, string videoTitle, 
                string videoLocation, string videoDescription)
            {
                VideoInfo newVid = new VideoInfo();

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
            public string VideoDescription { get; set; }
            
            public Drawable ThumbnailDrawable { get; set; }
            public Bitmap ThumbnailBitmap { get; set; }
        }

        public static VideoInfo GetSample()
        {
            VideoInfo vi = new VideoInfo();
            vi.CreatorName = "Creator Name Here";
            vi.VideoTitle = "Video Title";
            vi.ThumbnailDrawable = MainActivity.UniversalGetDrawable("nothing");

            return vi;
        }
    }
}