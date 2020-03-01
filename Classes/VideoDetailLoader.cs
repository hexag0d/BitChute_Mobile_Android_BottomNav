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
    public class VideoDetailLoader : Activity, ISurfaceHolderCallback
    {
        public VideoDetailLoader()
        {
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
            var videoView = (VideoView)v.FindViewById<VideoView>(Resource.Id.videoView);
            
            ISurfaceHolder holder = videoView.Holder;

            //holder.SetType(SurfaceType.PushBuffers);
            holder.AddCallback(this);

            var descriptor = MainActivity._assets.OpenFd("sample.mp4");
            var mediaPlayer = ExtStickyService._player;
            mediaPlayer.SetDataSource(descriptor.FileDescriptor, descriptor.StartOffset, descriptor.Length);
            mediaPlayer.Prepare();
            mediaPlayer.Start();
            
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

        public void LoadVideoFromVideoCard(View v, VideoCard vc)
        {
            var videoView = v.FindViewById<VideoView>(Resource.Id.videoView);

            ISurfaceHolder holder = videoView.Holder;

            //holder.SetType(SurfaceType.PushBuffers);
            holder.AddCallback(this);

            var descriptor = MainActivity._assets.OpenFd("sample.mp4");
            var mediaPlayer = ExtStickyService._player;
            mediaPlayer.SetDataSource(descriptor.FileDescriptor, descriptor.StartOffset, descriptor.Length);
            mediaPlayer.Prepare();
            mediaPlayer.Start();

            var videoTitle = v.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView);
            var videoCreatorName = v.FindViewById<TextView>(Resource.Id.videoDetailCreatorName);
            var imageView = v.FindViewById<ImageView>(Resource.Id.creatorAvatarImageView);

            videoTitle.Text = vc.Caption;
            videoCreatorName.Text = vc.Creator.Name;

            //if the drawable resource isn't null then set 
            if (vc.ThumbnailDrawable != null)
            {
                imageView.SetImageDrawable(vc.ThumbnailDrawable);
            }
            else
            {
                if (vc.ThumbnailBitmap != null)
                {
                    imageView.SetImageBitmap(vc.ThumbnailBitmap);
                }
            }
        }

        public void LoadVideoFromCreatorCard(View v, CreatorCard cc)
        {
            var videoView = v.FindViewById<VideoView>(Resource.Id.subsVideoView);


            ISurfaceHolder holder = videoView.Holder;

            ////holder.SetType(SurfaceType.PushBuffers);
            holder.AddCallback(this);

            var descriptor = MainActivity._assets.OpenFd("sample.mp4");

            MediaController mc = new MediaController(Android.App.Application.Context);

            
            mc.SetAnchorView(videoView);
            mc.SetMediaPlayer(videoView);

            ExtStickyService._player.Looping = true;

            ExtStickyService._player.SetDataSource(descriptor.FileDescriptor, descriptor.StartOffset, descriptor.Length);
            ExtStickyService._player.Prepare();

            ExtStickyService._player.Start();

            videoView.Start();


            var videoTitle = v.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView);
            var videoCreatorName = v.FindViewById<TextView>(Resource.Id.videoDetailCreatorName);
            var imageView = v.FindViewById<ImageView>(Resource.Id.creatorAvatarImageView);

            videoTitle.Text = cc.Caption;
            videoCreatorName.Text = cc.Creator.Name;


                imageView.SetImageDrawable(MainActivity.UniversalGetDrawable(cc.PhotoID));
                //imageView.SetImageDrawable(cc.Creator.CreatorThumbnailDrawable);
            

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

            bool playing = videoView.IsPlaying;
        }


        public void Dispose()
        {
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            ExtStickyService._player.SetDisplay(holder);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
        }
    }
}