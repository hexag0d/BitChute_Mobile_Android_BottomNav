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
        public static SubscriptionCardSet _creatorCardSetRoot;
        //video cards for the channel view
        public static VideoCardSet _videoCardSetChannel;
        //video cards for the related videos
        public static VideoCardSet _videoCardSetRelatedVideos;
        
        public static SubscriptionRecyclerViewAdapter _rootAdapter;
        
        public static VideoDetailLoader _vidLoader = new VideoDetailLoader();

        public static SubscriptionFragment NewInstance(string title, string icon) {
            var fragment = new SubscriptionFragment();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            return fragment;
        }
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _creatorCardSetRoot = new SubscriptionCardSet();
            // Set our view from the "main" layout resource  

            if (Arguments != null)
            {
                if(Arguments.ContainsKey("title"))
                    _title = (string)Arguments.Get("title");

                if (Arguments.ContainsKey("icon"))
                    _icon = (string)Arguments.Get("icon");
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            CustomViewHelpers.Tab1.Tab1FragmentLayout = inflater.Inflate(Resource.Layout.Tab1FragmentLayout, container, false);
            CustomViewHelpers.Tab1.RootRecyclerView = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<RecyclerView>(Resource.Id.subscriptionRootRecyclerView);
            CustomViewHelpers.Tab1.VideoDetailView = inflater.Inflate(Resource.Layout.Tab1VideoDetail, container, false);
            CustomViewHelpers.Tab1.LayoutManager = new LinearLayoutManager(container.Context);
            CustomViewHelpers.Tab1.RootRecyclerView.SetLayoutManager(CustomViewHelpers.Tab1.LayoutManager);
            CustomViewHelpers.Tab1.SubscriptionViewAdapter = new SubscriptionRecyclerViewAdapter(_creatorCardSetRoot);
            CustomViewHelpers.Tab1.RootRecyclerView.SetAdapter(CustomViewHelpers.Tab1.SubscriptionViewAdapter);
            CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter = new Adapters.CommentRecyclerViewAdapter(SampleCommentList.GetSampleCommentList());
            CustomViewHelpers.Tab1.SubscriptionViewAdapter.ItemClick += RootVideoAdapter_ItemClick;
            CustomViewHelpers.Tab1.Tab1ParentLayout = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<LinearLayout>(Resource.Id.tab1ParentFragmentLayout);
            CustomViewHelpers.Tab1.Container = container;
            CustomViewHelpers.Tab1.LayoutInflater = inflater;
            CustomViewHelpers.Tab1.VideoTitle = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView);
            CustomViewHelpers.Tab1.LikeButtonImageView = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<ImageView>(Resource.Id.likeButtonImageView);
            CustomViewHelpers.Tab1.DislikeButtonImageView = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<ImageView>(Resource.Id.dislikeButtonImageView);
            CustomViewHelpers.Tab1.SubscribeButton = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<Button>(Resource.Id.creatorSubscribeButton);
            CustomViewHelpers.Tab1.FavoriteImageView = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<ImageView>(Resource.Id.favoriteImageView);

            // _adapter.ItemClick += new EventHandler<int>((sender, e) => MAdapter_ItemClick(sender, e, _videoCard));
            return CustomViewHelpers.Tab1.Tab1FragmentLayout;
        }
        


        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootVideoAdapter_ItemClick(object sender, int e)
        {
            NavigateToNewPageFromVideoCard(CustomViewHelpers.Tab1.VideoDetailView, null, _creatorCardSetRoot[e]);
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
        public static void NavigateToNewPageFromVideoCard(View view, VideoCard videoCard, CreatorCard creatorCard)
        {
            _vidLoader.LoadVideoFromCard(view, creatorCard, videoCard, null);
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