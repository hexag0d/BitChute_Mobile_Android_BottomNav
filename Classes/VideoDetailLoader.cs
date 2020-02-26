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
using static BottomNavigationViewPager.Models.VideoModel;

namespace BottomNavigationViewPager.Classes
{
    /// <summary>
    /// class that loads the videos into the detail view and media player
    /// </summary>
    public class VideoDetailLoader
    {
        public static void LoadVideoFromDetail(View v, VideoInfo vi)
        {
            var videoView = v.FindViewById<VideoView>(Resource.Id.videoView);
            var videoTitle = v.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView);
            var videoCreatorName = v.FindViewById<TextView>(Resource.Id.videoDetailCreatorName);

            videoTitle.Text = vi.videoTitle;
            videoCreatorName.Text = vi.creatorName;
            //var videoCreator = v.FindViewById<TextView>(Resource.Id.)
            //var videoDescription = v.FindViewById<VideoView>(Resource.Id.videoDetailDescription)
        }
    }
}