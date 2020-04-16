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
            //var videoCreator = v.FindViewById<TextView>(Resource.Id.)
            //var videoDescription = v.FindViewById<VideoView>(Resource.Id.videoDetailDescription)
        }

        private static bool _vidBack;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="cc"></param>
        /// <param name="vc"></param>
        /// <param name="vcnc"></param>
        /// <param name="tab">this is the tab to load the video on; set to -1 for selected tab</param>
        public void LoadVideoFromCard(View v, CreatorCard cc, VideoCard vc, int tab)
        {
            if (cc == null && vc == null)
            {
                return; //nothing to load....
            }

            //IF the tab is -1 then use current tab
            //eventually we want to be able to dynamically load videos on any tab from any tab
            if (tab == -1)
            {
                tab = MainActivity.ViewPager.CurrentItem;
            }


            //ISurfaceHolder holder = CustomViewHelpers.Common.Video.GetVideoView(tab).Holder;
            ISurfaceHolder holder = CustomViewHelpers.Tab1.VideoView.Holder;

            ////holder.SetType(SurfaceType.PushBuffers);
            holder.AddCallback(this);
            //var descriptor = MainActivity._assets.OpenFd("sample.mp4");
            Android.Net.Uri uri;

            if (cc != null)
            uri = cc.LatestVideoUri;
            else if (vc != null)
            uri = vc.VideoUri;
            else
            {
                uri = Android.Net.Uri.Parse("android.resource://" + "com.xamarin.example.BitChute" + "/" + Resource.Raw.mylastvidd);
            }

            ExtStickyService.InitializePlayer(tab, uri, Android.App.Application.Context);

            // we might be able to eventually just use one media player but I think the buffering will be better
            // with a few of them, plus this way you can queue up videos and instantly switch

           CustomViewHelpers.Tab1.VideoView.LayoutParameters = new LinearLayout.LayoutParams(AppState.Display.ScreenWidth, (int)(AppState.Display.ScreenWidth * (.5625)));

            if (vc != null)
            {
                GetSetVideoDetailViewComplete(tab, vc);
            }
            else if (cc != null)
            {
                GetSetVideoDetailViewComplete(tab, cc.Creator.RecentVideoCards[0]);
            }
            TabStates.MediaTabHasFocus(tab);
            
        }
        
        /// <summary>
        /// this method will set the controls on the video detail page 
        /// for example, gets the comments, like counts, and related videos
        /// we can call this async so that the video loads first and then the externals
        /// </summary>
        public async void GetSetVideoDetailViewComplete(int tab, VideoCard vc)
        {
            await Task.Run(() =>
            {
               switch (tab)
               {
                   case 0:
                       break;
                   case 1:
                       TabStates.Tab1.MainVideoDetail = BitChuteAPI.Inbound.GetFullVideoDetail(vc).Result;
                       TabStates.Tab1.CommentSystem.MainCommentList = BitChuteAPI.Inbound.GetVideoComments(vc.VideoId).Result;
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
            //var aspect = (float)width / (float)height;
            ExtStickyService.MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].SetDisplay(holder);
            //mediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Show();
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
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