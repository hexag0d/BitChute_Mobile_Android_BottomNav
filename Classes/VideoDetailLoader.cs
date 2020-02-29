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
        public static void InitializeVideo(int tab)
        {
            switch (tab)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }
        public static void LoadVideoFromDetail(View v, VideoInfo vi)
        {
            var videoView = v.FindViewById<VideoView>(Resource.Id.videoView);
            var videoTitle = v.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView);
            var videoCreatorName = v.FindViewById<TextView>(Resource.Id.videoDetailCreatorName);
            var imageView = v.FindViewById<ImageView>(Resource.Id.creatorAvatarImageView);

            videoTitle.Text = vi.VideoTitle;
            videoCreatorName.Text = vi.CreatorName;

            //if the drawable resource isn't null then set 
            if (vi.ThumbnailDrawable != null)
            {
                imageView.SetImageDrawable(vi.ThumbnailDrawable);
            }
            else
            {
                if (vi.ThumbnailBitmap != null)
                {
                    imageView.SetImageBitmap(vi.ThumbnailBitmap);
                }
            }
            //var videoCreator = v.FindViewById<TextView>(Resource.Id.)
            //var videoDescription = v.FindViewById<VideoView>(Resource.Id.videoDetailDescription)
        }
    }
}