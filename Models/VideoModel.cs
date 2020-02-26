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
using MvvmCross.ViewModels;

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
                videoInfo.thumbnail = thumbnail;
                videoInfo.creatorName = creatorName;
                videoInfo.videoTitle = videoTitle;
                videoInfo.videoLocation = videoLocation;
                videoInfo.videoDescription = videoDescription;

                return videoInfo;
            }

            public string creatorName { get; set; }
            public string videoTitle { get; set; }
            public string videoLocation { get; set; }
            public string videoDescription { get; set; }

            //Bitmap thumbnail { get; set; }
            public Drawable thumbnail { get; set; }
        }

        public static VideoInfo GetSample()
        {
            VideoInfo vi = new VideoInfo();
            vi.creatorName = "Creator Name Here";
            vi.videoTitle = "Video Title";
            vi.thumbnail = MainActivity.UniversalGetDrawable("nothing");

            return vi;
        }
    }
}