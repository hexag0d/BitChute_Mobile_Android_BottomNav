using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BitChute.Services;
using BitChute.Web;
using HtmlAgilityPack;
using Java.IO;
using Java.Net;
using static BitChute.Models.VideoModel;
using static BitChute.Web.ExtWebInterface;

namespace BitChute.ViewModel
{
    public class VideoDetailLoader : Activity, ISurfaceHolderCallback, MediaPlayer.IOnPreparedListener
    {
        private int _id;
        public int Id { get { return _id; } set { _id = value; } }
        private LinearLayout _linearContainer;
        public LinearLayout LinearContainer { get { return _linearContainer; } set { _linearContainer = value; } }
        private RelativeLayout _relativeContainer;
        public RelativeLayout RelativeContainer { get { return _relativeContainer; } set { _relativeContainer = value; } }

        private static Dictionary<int, VideoDetailLoader> _videoDetailDictionary;
        public static Dictionary<int, VideoDetailLoader> GetVideoDetailDictionary(int id = -1, VideoDetailLoader videoDetail = null)
        {
            if (_videoDetailDictionary == null) { _videoDetailDictionary = new Dictionary<int, VideoDetailLoader>(); }
            if (id != -1 && videoDetail != null)
            {
                if (_videoDetailDictionary.ContainsKey(id))
                {
                    _videoDetailDictionary.Remove(id);
                }
                _videoDetailDictionary.Add(id, videoDetail);
            }
            else if (id != -1 && videoDetail == null) { return _videoDetailDictionary; }
            return _videoDetailDictionary;
        }

        public static VideoDetailLoader GetVideoDetailById(int id)
        {
            if (GetVideoDetailDictionary().ContainsKey(id))
            {
                return GetVideoDetailDictionary().GetValueOrDefault(id);
            }
            return null;
        }

        public VideoDetailLoader()
        {
            GetVideoDetailDictionary(this.Id, this);
        }

        public VideoDetailLoader(RelativeLayout relativeLayout = null, LinearLayout linearLayout = null, int id = -1)
        {
            if (relativeLayout != null)
                RelativeContainer = relativeLayout;
            if (linearLayout != null)
                LinearContainer = linearLayout;
            if (id == -1) { id = this.Id; }
            this.Id = id;
            GetVideoDetailDictionary(id, this);
        }

        public class UrlStrings
        {
            public static string Domain = "https://www.bitchute.com";
            public static string SubscriptionFullFeed = "https://www.bitchute.com/";
        }

        public static void SetLayoutParameters(VideoView vv)
        {
            vv.LayoutParameters = new LinearLayout.LayoutParams(AppState.Display.ScreenWidth, (int)(AppState.Display.ScreenWidth * (.5625)));
        }

        public static void SwapView(View toSwap, LinearLayout container = null, RelativeLayout relcontainer = null)
        {
            if (relcontainer != null) { relcontainer.RemoveAllViews(); relcontainer.AddView(toSwap); return; }
            if (container != null) { container.RemoveAllViews(); container.AddView(toSwap); }
        }

        public async Task<VideoCard> LoadVideoDetailFromVideoCard(View view, VideoCard vc = null, string url = null, 
            LinearLayout container = null, RelativeLayout relContainer = null, int id = -1) {
            
            if (vc != null)
            {
                view.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView).Text = vc?.Title;
                view.FindViewById<TextView>(Resource.Id.videoDetailCreatorName).Text = vc?.CreatorName;
            }
            var response = await ExtWebInterface.HttpClient.GetAsync(Request.GetFullRequest(vc.Link));
            if (relContainer != null) { SwapView(view, null, relContainer); }
            else if (container != null) { SwapView(view, container, null); }
            var html = await response.Content.ReadAsStringAsync();
            
            try
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                if (doc != null)
                {
                    var vidurl = doc.DocumentNode.SelectSingleNode("//source").GetAttributeValue("src", "");
                    var link = Android.Net.Uri.Parse(doc.DocumentNode.SelectSingleNode("//source").GetAttributeValue("src", ""));
                    //var link = doc.DocumentNode.SelectSingleNode("//source")
                    //MainPlaybackSticky.InitializePlayer(this.Id, link, MainActivity.GetMainContext());
                    Fragments.CommonFrag.GetFragmentById(id).VideoView = view.FindViewById<VideoView>(Resource.Id.videoView);
                    Fragments.CommonFrag.GetFragmentById(id).VideoView.Click += Fragments.CommonFrag.GetFragmentById(id).VideoView_OnClick;
                    MainPlaybackSticky.InitializePlayer(id, null, MainActivity.GetMainContext(), vidurl);
                    SetLayoutParameters(view.FindViewById<VideoView>(Resource.Id.videoView));
                    ISurfaceHolder holder = view.FindViewById<VideoView>(Resource.Id.videoView).Holder;
                    holder.AddCallback(this);
                    MainPlaybackSticky.InitializeMediaController(view.FindViewById<VideoView>(Resource.Id.videoView), vc.FragmentId);
                    foreach (var node in doc.DocumentNode.SelectNodes("//div[@class='container']"))
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            MainPlaybackSticky.MediaPlayerDictionary[this.Id].SetDisplay(holder);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
        }

        public void OnPrepared(MediaPlayer mp)
        {

        }
    }
}