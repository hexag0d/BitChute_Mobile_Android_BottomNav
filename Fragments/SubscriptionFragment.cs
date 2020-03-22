using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Classes;
using Java.IO;
using BitChute;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static BitChute.MainActivity;
using static StartServices.Servicesclass.ExtStickyService;
using BitChute.Models;
using static BitChute.Models.VideoModel;

using static BitChute.Models.SubscriptionModel;
using static BitChute.Models.CreatorModel;
using static BitChute.Models.CommentModel;

namespace BitChute.Fragments 
{
    public class SubscriptionFragment : Fragment
    {
        string _title;
        string _icon;

        //video cards for the root view
        public static SubscriptionCardSet CreatorCardRootSet;
        //video cards for the channel view
        public static List<VideoModel.VideoCardNoCreator> CreatorDetailVideoCardSet;
            


        //video cards for the related videos
        public static VideoCardSet _videoCardSetRelatedVideos;
        
        public static VideoDetailLoader _vidLoader = new VideoDetailLoader();

        public static Handler Tab1Handler = new Handler();

        public static SubscriptionFragment NewInstance(string title, string icon)
        {
            var fragment = new SubscriptionFragment();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource  

            if (Arguments != null)
            {
                if (Arguments.ContainsKey("title"))
                    _title = (string)Arguments.Get("title");

                if (Arguments.ContainsKey("icon"))
                    _icon = (string)Arguments.Get("icon");
            }
            GetSubscriptionList();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            CustomViewHelpers.Tab1.Tab1FragmentLayout = inflater.Inflate(Resource.Layout.Tab1FragmentLayout, container, false);
            CustomViewHelpers.Tab1.RootRecyclerView = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<RecyclerView>(Resource.Id.subscriptionRootRecyclerView);

            CustomViewHelpers.Tab1.CreatorDetailView = inflater.Inflate(Resource.Layout.Tab1CreatorDetail, container, false);
            CustomViewHelpers.Tab1.CreatorDetailRecyclerView = CustomViewHelpers.Tab1.CreatorDetailView.FindViewById<RecyclerView>(Resource.Id.creatorDetailRecyclerView);
            CustomViewHelpers.Tab1.CreatorDetailLayoutManager = new LinearLayoutManager(CustomViewHelpers.Tab1.CreatorDetailView.Context);

            CustomViewHelpers.Tab1.CreatorDetailRecyclerView.SetAdapter(CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter);
            CustomViewHelpers.Tab1.VideoDetailView = inflater.Inflate(Resource.Layout.Tab1VideoDetail, container, false);
            CustomViewHelpers.Tab1.RootLayoutManager = new LinearLayoutManager(container.Context);
            CustomViewHelpers.Tab1.CreatorDetailLayoutManager = new LinearLayoutManager(CustomViewHelpers.Tab1.CreatorDetailView.Context);
            CustomViewHelpers.Tab1.VideoLayoutManager = new LinearLayoutManager(CustomViewHelpers.Tab1.VideoDetailView.Context);
            CustomViewHelpers.Tab1.RootRecyclerView.SetLayoutManager(CustomViewHelpers.Tab1.RootLayoutManager);
            CustomViewHelpers.Tab1.CreatorDetailRecyclerView.SetLayoutManager(CustomViewHelpers.Tab1.CreatorDetailLayoutManager);

            CustomViewHelpers.Tab1.CreatorDetailAvatarImageView = CustomViewHelpers.Tab1.CreatorDetailView.FindViewById<ImageView>(Resource.Id.creatorDetailAvatarImageView);
            CustomViewHelpers.Tab1.CreatorNameTextView = CustomViewHelpers.Tab1.CreatorDetailView.FindViewById<TextView>(Resource.Id.creatorDetailNameTextView);

            CustomViewHelpers.Tab1.CommentRecyclerView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<RecyclerView>(Resource.Id.commentRecyclerView);
            CustomViewHelpers.Tab1.CommentRecyclerView.SetLayoutManager(CustomViewHelpers.Tab1.VideoLayoutManager);
            CustomViewHelpers.Tab1.SubscriptionViewAdapter = new SubscriptionRecyclerViewAdapter(CreatorCardRootSet);
            CustomViewHelpers.Tab1.RootRecyclerView.SetAdapter(CustomViewHelpers.Tab1.SubscriptionViewAdapter);
            CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter = new Adapters.CommentRecyclerViewAdapter(SampleCommentList.GetSampleCommentList());
            CustomViewHelpers.Tab1.SubscriptionViewAdapter.ItemClick += RootVideoAdapter_ItemClick;
            //CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter.ItemClick += CreatorDetailAdapter_ItemClickInt;

            CustomViewHelpers.Tab1.Tab1ParentLayout = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<LinearLayout>(Resource.Id.tab1ParentFragmentLayout);
            CustomViewHelpers.Tab1.Container = container;
            CustomViewHelpers.Tab1.LayoutInflater = inflater;
            CustomViewHelpers.Tab1.VideoTitle = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView);
            CustomViewHelpers.Tab1.LikeButtonImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.likeButtonImageView);
            CustomViewHelpers.Tab1.DislikeButtonImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.dislikeButtonImageView);
            CustomViewHelpers.Tab1.SubscribeButton = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<Button>(Resource.Id.creatorSubscribeButton);
            CustomViewHelpers.Tab1.FavoriteImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.favoriteImageView);
            CustomViewHelpers.Tab1.LikeCountTextView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<TextView>(Resource.Id.likeCountTextView);
            CustomViewHelpers.Tab1.DislikeCountTextView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<TextView>(Resource.Id.dislikeCountTextView);

            // _adapter.ItemClick += new EventHandler<int>((sender, e) => MAdapter_ItemClick(sender, e, _videoCard));
            return CustomViewHelpers.Tab1.Tab1FragmentLayout;
        }

        public static async Task<SubscriptionCardSet> GetSubscriptionList()
        {
            await Task.Run(() => {
                CreatorCardRootSet = BitChuteAPI.Inbound.GetSubscriptionList().Result;
                });
            return CreatorCardRootSet;
        }
        
        public static Task<bool> ReceiveVideoList(List<VideoCardNoCreator> vcl)
        {
            Tab1Handler.Post(() =>
            {
                CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter =
                             new Adapters.CreatorDetailRecyclerViewAdapter(vcl);

                CustomViewHelpers.Tab1.CreatorDetailRecyclerView.SetAdapter(
                    CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter);
            });
            return Task.FromResult<bool>(true);
        }

        public static Task<bool> ReceiveVideoComments(List<Comment> commentList)
        {
            Tab1Handler.Post(() =>
            {
                CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter =
                             new Adapters.CommentRecyclerViewAdapter(commentList);

                CustomViewHelpers.Tab1.CommentRecyclerView.SetAdapter(
                    CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter);
            });
            return Task.FromResult<bool>(true);
        }

        public static Task<bool> ReceiveVideoLikeDislikeCount(int likes, int dislikes)
        {
            Tab1Handler.Post(() =>
            {
                CustomViewHelpers.Tab1.LikeCountTextView.Text = likes.ToString();
                CustomViewHelpers.Tab1.DislikeCountTextView.Text = dislikes.ToString();
            });
            return Task.FromResult<bool>(true);
        }

        public static bool VideoDetailChanged(List<Comment> comments, int likes, int dislikes)
        {
            if (likes != 0)
            {
                CustomViewHelpers.Tab1.LikeCountTextView.Text = likes.ToString();
            }
            if (dislikes != 0)
            {
                CustomViewHelpers.Tab1.DislikeCountTextView.Text = dislikes.ToString();
            }
            if (comments != null)
            {
                CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter =
                    new Adapters.CommentRecyclerViewAdapter(comments);
                CustomViewHelpers.Tab1.CommentRecyclerView.SetAdapter(
                    CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter);
            }
            return false;
        }

        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootVideoAdapter_ItemClick(object sender, int e)
        {
            NavigateToCreatorPage(CreatorCardRootSet[e]);
        }

        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CreatorDetailAdapter_ItemClick(object sender, int i)
        {
            NavigateToNewPageFromVideoCard(null, CreatorDetailVideoCardSet[i], null);
        }

        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatorDetailAdapter_ItemClickInt(object sender, int pos)
        {
            NavigateToNewPageFromVideoCard(null, CreatorDetailVideoCardSet[pos], null);
        }

        public static void NavigateToCreatorPage(CreatorCard cc)
        {
            CreatorDetailLoader.LoadCreatorPage(CustomViewHelpers.Tab1.Tab1FragmentLayout, cc);
            //CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter = new Adapters.CreatorDetailRecyclerViewAdapter()
        }

        private void CommentSystemViewAdapter_ItemClick(object sender, int e)
        {
        }
        
        /// <summary>
        /// Navigates the selected tab to a new page
        /// </summary>
        /// <param name="view">the view you want to swap</param>
        /// <param name="videoCard">the videocard you want to load</param>
        /// <param name="creatorCard">optionally use a creatorcard for more detail</param>
        public static void NavigateToNewPageFromVideoCard(VideoCard vc, VideoCardNoCreator vcnc, CreatorCard cc)
        {
            _vidLoader.LoadVideoFromCard(CustomViewHelpers.Tab1.VideoDetailView, cc, vc, vcnc);
        }

        /// <summary>
        /// removes the child views and swaps for the View arg
        /// </summary>
        /// <param name="view"></param>
        public static void SwapView(View view)
        {
            CustomViewHelpers.Tab1.Tab1ParentLayout.RemoveAllViews();
            CustomViewHelpers.Tab1.Tab1ParentLayout.AddView(view);
        }

        /// <summary>
        /// Navigates the selected tab to a new page
        /// </summary>
        /// <param name="view">the view you want to swap</param>
        /// <param name="type"></param>
        /// <param name=""></param>
        public static void NavigateToNewPage(View view, string linkId)
        {
            // VideoDetailLoader.LoadVideoFromDetail()
            SwapView(CustomViewHelpers.Tab1.VideoDetailView);


            //// the link will most likely be tagged with an identifier
            ////this will tell the app what type of layout to load the page link in
            //switch (linkId.Substring(0, 2))
            //{
            //    //for example, link id starts with vd for videodetail
            //    case "vd":
            //        // pass the link details to the video detail view
            //        SwapView(_videoDetailView);
            //        break;

            //}
        }

        public void CustomSetTouchListener(bool landscape)
        {
        }
        
        public class ExtTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                MainActivity.CustomOnTouch();
                CustomOnTouch();
                return false;
            }
        }

        public void OnSettingsChanged()
        {
            if (AppSettings._zoomControl)
            {

            }
            else
            {

            }
        }
        
        public void SubsTabGoBack()
        {
        }
        
        /// <summary>
        /// pops back to the root of the subscription tab
        /// </summary>
        public void Pop2Root()
        {

        }
    }
}
///// <summary>
///// click event for the adapter
///// </summary>
///// <param name="sender"></param>
///// <param name="e"></param>
//private void MAdapter_ItemClick(object sender, int e, VideoCard vc)
//{
//    SwapView(_videoDetailView);
//    var check = e;
//}