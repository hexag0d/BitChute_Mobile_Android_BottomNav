using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BitChute.Fragments;
using StartServices.Servicesclass;
using static BitChute.Models.CreatorModel;
using static BitChute.Models.VideoModel;

namespace BitChute.Classes
{
    /// <summary>
    /// class that loads the videos into the detail view and media player
    /// </summary>
    public class VideoDetailLoader : Activity, ISurfaceHolderCallback, MediaPlayer.IOnPreparedListener
    {
        public static Dictionary<int, VideoView> videoViewDictionary = new Dictionary<int, VideoView>();
        public static Dictionary<int, TextView> titleTextViewDictionary = new Dictionary<int, TextView>();
        public static Dictionary<int, MediaController> mediaControllerDictionary = new Dictionary<int, MediaController>();

        public VideoDetailLoader()
        {

        }

        public static void OnRotation(LinearLayout.LayoutParams layoutParams)
        {
            foreach (var vv in videoViewDictionary)
            {
                vv.Value.LayoutParameters = layoutParams;
            }
            if (AppState.Display._horizontal)
            {
                foreach (var tv in titleTextViewDictionary)
                {
                    tv.Value.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                foreach (var tv in titleTextViewDictionary)
                {
                    tv.Value.Visibility = ViewStates.Visible;
                }
            }
        }

        //public IntPtr Handle => throw new NotImplementedException();

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
        
        public void LoadVideoFromDetail(View v, VideoDetail vi)
        {

            //var videoCreator = v.FindViewById<TextView>(Resource.Id.)
            //var videoDescription = v.FindViewById<VideoView>(Resource.Id.videoDetailDescription)
        }


        public void LoadVideoFromCard(View v, CreatorCard cc, VideoCard vc)
        {
            if (!videoViewDictionary.ContainsKey(MainActivity._viewPager.CurrentItem))
            {
                videoViewDictionary.Add(MainActivity._viewPager.CurrentItem, (VideoView)v.FindViewById<VideoView>(Resource.Id.videoView));
            }
            if (!titleTextViewDictionary.ContainsKey(MainActivity._viewPager.CurrentItem))
            {
                titleTextViewDictionary.Add(MainActivity._viewPager.CurrentItem, (TextView)v.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView));
            }
            if (!mediaControllerDictionary.ContainsKey(MainActivity._viewPager.CurrentItem))
            {
                mediaControllerDictionary.Add(MainActivity._viewPager.CurrentItem, new MediaController(Application.Context));
            }
            ISurfaceHolder holder = videoViewDictionary[MainActivity._viewPager.CurrentItem].Holder;

            ////holder.SetType(SurfaceType.PushBuffers);
            holder.AddCallback(this);

            var descriptor = MainActivity._assets.OpenFd("sample.mp4");
            Android.Net.Uri uri = null;

            if (MainActivity._viewPager.CurrentItem == 2)
            {
                //  854 x 480 .mp4 h264 file but I can't put it on github
                //  put a similar file in your resources folder to test
                string path = "android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd;

                uri = Android.Net.Uri.Parse(path);
            }


            ExtStickyService._player.SetOnPreparedListener(this);

            mediaControllerDictionary[MainActivity._viewPager.CurrentItem].SetAnchorView(videoViewDictionary[MainActivity._viewPager.CurrentItem]);
            mediaControllerDictionary[MainActivity._viewPager.CurrentItem].SetMediaPlayer(videoViewDictionary[MainActivity._viewPager.CurrentItem]);

            ExtStickyService._player.Looping = true;

            if (MainActivity._viewPager.CurrentItem == 1)
            {
                ExtStickyService._player.SetDataSource(descriptor.FileDescriptor, descriptor.StartOffset, descriptor.Length);
            }
            if (MainActivity._viewPager.CurrentItem == 2)
            {
                ExtStickyService._player.SetDataSource(Android.App.Application.Context, uri);
            }
            ExtStickyService._player.Prepare();

            ExtStickyService._player.Start();

            videoViewDictionary[MainActivity._viewPager.CurrentItem].Start();


            var videoTitle = v.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView);
            var videoCreatorName = v.FindViewById<TextView>(Resource.Id.videoDetailCreatorName);
            var imageView = v.FindViewById<ImageView>(Resource.Id.creatorAvatarImageView);

            if (cc != null)
            {
                videoTitle.Text = cc.Caption;
                videoCreatorName.Text = cc.Creator.Name;
                imageView.SetImageDrawable(MainActivity.UniversalGetDrawable(cc.PhotoID));
            }
            else
            {
                videoTitle.Text = vc.Caption;
                videoCreatorName.Text = vc.Creator.Name;
                imageView.SetImageDrawable(MainActivity.UniversalGetDrawable(vc.PhotoID));
            }
            videoViewDictionary[MainActivity._viewPager.CurrentItem].LayoutParameters = new LinearLayout.LayoutParams(AppState.Display.ScreenWidth, (int)(AppState.Display.ScreenWidth * (.5625)));

            switch (MainActivity._viewPager.CurrentItem)
            {
                case 0:
                    break;
                case 1:
                    SubscriptionFragment.SwapView(v);
                    break;
                case 2:
                    FeedFragment.SwapView(v);
                    break;
                case 3:
                    break;

                case 4:
                    break;
            }

            bool playing = videoViewDictionary[MainActivity._viewPager.CurrentItem].IsPlaying;
        }


        public void Dispose()
        {
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

            var aspect = (float)width / (float)height;
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            ExtStickyService._player.SetDisplay(holder);
            videoViewDictionary[MainActivity._viewPager.CurrentItem].SetMediaController(mediaControllerDictionary[MainActivity._viewPager.CurrentItem]);
            mediaControllerDictionary[MainActivity._viewPager.CurrentItem].SetMediaPlayer(videoViewDictionary[MainActivity._viewPager.CurrentItem]);
            //mediaControllerDictionary[MainActivity._viewPager.CurrentItem].Show();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
        }

        public void OnPrepared(MediaPlayer mp)
        {

        }
    }
}