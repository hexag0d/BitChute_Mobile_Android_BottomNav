using System;
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

        public static bool _vidLoadTest;


        public void LoadVideoFromDetail(View v, VideoDetail vi)
        {
            //var videoCreator = v.FindViewById<TextView>(Resource.Id.)
            //var videoDescription = v.FindViewById<VideoView>(Resource.Id.videoDetailDescription)
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="cc"></param>
        /// <param name="vc"></param>
        /// <param name="vcnc"></param>
        /// <param name="tab">this is the tab to load the video on; set to -1 for default selected tab</param>
        public void LoadVideoFromCard(View v, CreatorCard cc, VideoCard vc, VideoCardNoCreator vcnc, int tab)
        {
            //IF the tab is -1 then use current tab
            //eventually we want to be able to dynamically load videos on any tab from any tab
            if (tab == -1)
            {
                tab = MainActivity.ViewPager.CurrentItem;
            }


            if (cc == null && vc == null && vcnc == null)
            {
                return; //nothing to load....
            }

            if (_vidLoadTest)
            {
                ExtStickyService.MediaPlayerDictionary[tab].Reset();
                ExtStickyService.MediaPlayerDictionary[tab].Release();
                ExtStickyService.MediaPlayerDictionary[tab] = null;
                ExtStickyService.InitializePlayer(tab);
            }
            //next we initialize the media player
            if (!videoViewDictionary.ContainsKey(MainActivity.ViewPager.CurrentItem))
            {
                videoViewDictionary.Add(MainActivity.ViewPager.CurrentItem, (VideoView)v.FindViewById<VideoView>(Resource.Id.videoView));
            }
            if (!mediaControllerDictionary.ContainsKey(MainActivity.ViewPager.CurrentItem))
            {
                mediaControllerDictionary.Add(MainActivity.ViewPager.CurrentItem, new MediaController(Application.Context));
            }
            
            // we might be able to eventually just use one media player but I think the buffering will be better
            // with a few of them, plus this way you can queue up videos and instantly switch
            if (!ExtStickyService.MediaPlayerDictionary.ContainsKey(tab))
            {
                ExtStickyService.InitializePlayer(tab);
            }
            
            ISurfaceHolder holder = videoViewDictionary[MainActivity.ViewPager.CurrentItem].Holder;

            ////holder.SetType(SurfaceType.PushBuffers);
            holder.AddCallback(this);

            //var descriptor = MainActivity._assets.OpenFd("sample.mp4");

            Android.Net.Uri uri;

            if (cc != null)
            uri = cc.LatestVideoUri;
            if (vcnc != null)
            uri = vcnc.Uri;
            if (vc != null)
            uri = vc.VideoUri;
            else
            {
                uri = Android.Net.Uri.Parse("android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd);
                //uri = Android.Net.Uri.Parse("android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.test2);
            }

            if (_vidLoadTest)
            {
                uri = Android.Net.Uri.Parse("android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.test2);
            }

            //ExtStickyService.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].SetOnPreparedListener(this);

            mediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetAnchorView(videoViewDictionary[MainActivity.ViewPager.CurrentItem]);
            mediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetMediaPlayer(videoViewDictionary[MainActivity.ViewPager.CurrentItem]);

            //ExtStickyService.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Looping = true;

            if (tab == 1)
            {
                //ExtStickyService.MediaPlayerDictionary[MainActivity._viewPager.CurrentItem].SetDataSource(descriptor.FileDescriptor, descriptor.StartOffset, descriptor.Length);
                ExtStickyService.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].SetDataSource(Android.App.Application.Context, uri);
            }
            if (MainActivity.ViewPager.CurrentItem == 2)
            {
                ExtStickyService.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].SetDataSource(Android.App.Application.Context, uri);
            }
            
            ExtStickyService.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Prepare();
            //ExtStickyService.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].Start();

            videoViewDictionary[MainActivity.ViewPager.CurrentItem].Start();
            videoViewDictionary[MainActivity.ViewPager.CurrentItem].LayoutParameters = new LinearLayout.LayoutParams(AppState.Display.ScreenWidth, (int)(AppState.Display.ScreenWidth * (.5625)));

            bool playing = videoViewDictionary[MainActivity.ViewPager.CurrentItem].IsPlaying;

            if (vc != null)
            {
                GetSetVideoDetailViewComplete(MainActivity.ViewPager.CurrentItem, vc.VideoId);
            }
            else
            {
                if (vcnc != null)
                {
                    GetSetVideoDetailViewComplete(MainActivity.ViewPager.CurrentItem, vcnc.Link);
                }
                else
                {
                    if (cc != null)
                    {
                        GetSetVideoDetailViewComplete(MainActivity.ViewPager.CurrentItem, cc.LatestVidLinkString);
                    }
                }
            }

            _vidLoadTest = true;
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
                       TabStates.Tab1.CommentSystem.MainCommentList = BitChuteAPI.Inbound.GetVideoComments(videoLink).Result;
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
            ExtStickyService.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].SetDisplay(holder);
            videoViewDictionary[MainActivity.ViewPager.CurrentItem].SetMediaController(mediaControllerDictionary[MainActivity.ViewPager.CurrentItem]);
            mediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetMediaPlayer(videoViewDictionary[MainActivity.ViewPager.CurrentItem]);
            videoViewDictionary[MainActivity.ViewPager.CurrentItem].Start();
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