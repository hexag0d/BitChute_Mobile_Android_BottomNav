﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using BitChute.Models;
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
            if (AppState.Display.Horizontal)
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

        /// <summary>
        /// Loads a video into the media player and changes the view
        /// CreatorCard or VideoCard can be null but not both at the same time
        /// If both args are provided the VideoCard will be used for metadata
        /// </summary>
        /// <param name="v"></param>
        /// <param name="cc"></param>
        /// <param name="vc"></param>
        public void LoadVideoFromCard(View v, CreatorCard cc, VideoCard vc, VideoCardNoCreator vcnc)
        {
            ////first we set the view members we already have metadata for
            //var videoTitle = v.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView);
            //var videoCreatorName = v.FindViewById<TextView>(Resource.Id.videoDetailCreatorName);
            //var imageView = v.FindViewById<ImageView>(Resource.Id.creatorAvatarImageView);

            //if (cc != null)
            //{
            //    videoTitle.Text = cc.Title;
            //    videoCreatorName.Text = cc.Creator.Name;
            //    imageView.SetImageDrawable(MainActivity.UniversalGetDrawable(cc.PhotoID));
            //}
            //else if (vc != null)
            //{
            //    videoTitle.Text = vc.Title;
            //    videoCreatorName.Text = vc.Creator.Name;
            //    imageView.SetImageDrawable(MainActivity.UniversalGetDrawable(vc.PhotoID));
            //}
            //else if (vcnc != null)
            //{
            //    videoTitle.Text = vcnc.Title;
            //    videoCreatorName.Text = vcnc.CreatorName;
            //    imageView.SetImageDrawable(MainActivity.UniversalGetDrawable(vcnc.PhotoID));
            //}
            //the rest of the video metadata should be set async as the video is loading
            //this will give a snappy UI while also allowing for data to be
            //loaded as the video loads
            //we want the video loading to be next, however, get the data source stream set asap
            
            //next we initialize the media player

            if (!videoViewDictionary.ContainsKey(MainActivity._viewPager.CurrentItem))
            {
                videoViewDictionary.Add(MainActivity._viewPager.CurrentItem, (VideoView)v.FindViewById<VideoView>(Resource.Id.videoView));
            }
            //if (!titleTextViewDictionary.ContainsKey(MainActivity._viewPager.CurrentItem))
            //{
            //    titleTextViewDictionary.Add(MainActivity._viewPager.CurrentItem, (TextView)v.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView));
            //}
            if (!mediaControllerDictionary.ContainsKey(MainActivity._viewPager.CurrentItem))
            {
                mediaControllerDictionary.Add(MainActivity._viewPager.CurrentItem, new MediaController(Application.Context));
            }
            // we might be able to eventually just use one media player but I think the buffering will be better
            // with a few of them, plus this way you can queue up videos and instantly switch
            if (!ExtStickyService.MediaPlayerDictionary.ContainsKey(MainActivity._viewPager.CurrentItem))
            {
                ExtStickyService.MediaPlayerDictionary.Add(MainActivity._viewPager.CurrentItem, new MediaPlayer());
            }

            ISurfaceHolder holder = videoViewDictionary[MainActivity._viewPager.CurrentItem].Holder;

            ////holder.SetType(SurfaceType.PushBuffers);
            holder.AddCallback(this);

            var descriptor = MainActivity._assets.OpenFd("sample.mp4");
            Android.Net.Uri uri = null;

            if (MainActivity._viewPager.CurrentItem == 2)
            {
                string path = "android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd;

                uri = Android.Net.Uri.Parse(path);
            }
            
            ExtStickyService.MediaPlayerDictionary[MainActivity._viewPager.CurrentItem].SetOnPreparedListener(this);

            mediaControllerDictionary[MainActivity._viewPager.CurrentItem].SetAnchorView(videoViewDictionary[MainActivity._viewPager.CurrentItem]);
            mediaControllerDictionary[MainActivity._viewPager.CurrentItem].SetMediaPlayer(videoViewDictionary[MainActivity._viewPager.CurrentItem]);

            ExtStickyService.MediaPlayerDictionary[MainActivity._viewPager.CurrentItem].Looping = true;

            if (MainActivity._viewPager.CurrentItem == 1)
            {
                //ExtStickyService.MediaPlayerDictionary[MainActivity._viewPager.CurrentItem].SetDataSource(descriptor.FileDescriptor, descriptor.StartOffset, descriptor.Length);
                ExtStickyService.MediaPlayerDictionary[MainActivity._viewPager.CurrentItem].SetDataSource(Android.App.Application.Context, SampleUri);
            }
            if (MainActivity._viewPager.CurrentItem == 2)
            {
                ExtStickyService.MediaPlayerDictionary[MainActivity._viewPager.CurrentItem].SetDataSource(Android.App.Application.Context, uri);
            }
            ExtStickyService.MediaPlayerDictionary[MainActivity._viewPager.CurrentItem].Prepare();

            ExtStickyService.MediaPlayerDictionary[MainActivity._viewPager.CurrentItem].Start();

            videoViewDictionary[MainActivity._viewPager.CurrentItem].Start();

            videoViewDictionary[MainActivity._viewPager.CurrentItem].LayoutParameters = new LinearLayout.LayoutParams(AppState.Display.ScreenWidth, (int)(AppState.Display.ScreenWidth * (.5625)));

            bool playing = videoViewDictionary[MainActivity._viewPager.CurrentItem].IsPlaying;

            if (vc != null)
            {
                GetSetVideoDetailViewComplete(MainActivity._viewPager.CurrentItem, vc.VideoId);
            }
            else
            {
                GetSetVideoDetailViewComplete(MainActivity._viewPager.CurrentItem, "aVideoId");
            }
        }

      

        /// <summary>
        /// this method will set the controls on the video detail page 
        /// for example, gets the comments, like counts, and related videos
        /// we can call this async so that the video loads first and then the externals
        /// </summary>
        public async void GetSetVideoDetailViewComplete(int tab, string videoLink)
        {
            await Task.Run(() =>
            {
               switch (tab)
               {
                   case 0:
                       break;
                   case 1:
                       TabStates.Tab1.MainVideoDetail = BitChuteAPI.Inbound.GetFullVideoDetail(videoLink).Result;
                       break;
                   case 2:
                       break;
                   case 3:
                       break;
                   case 4:
                       break;
               }
            });
        }

        
        
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

            var aspect = (float)width / (float)height;
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            ExtStickyService.MediaPlayerDictionary[MainActivity._viewPager.CurrentItem].SetDisplay(holder);
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