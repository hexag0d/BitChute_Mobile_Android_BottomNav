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

namespace BitChute.Fragments 
{
    public class SubscriptionFragment : Fragment
    {
        string _title;
        string _icon;
        
        public static Android.Support.V7.Widget.RecyclerView _recycleView;
        public static Android.Support.V7.Widget.RecyclerView.LayoutManager _layoutManager;

        //video cards for the root view
        public static SubscriptionCardSet _creatorCardSetRoot;
        //video cards for the channel view
        public static VideoCardSet _videoCardSetChannel;
        //video cards for the related videos
        public static VideoCardSet _videoCardSetRelatedVideos;
        
        public static SubscriptionRecyclerViewAdapter _rootAdapter;

        public static View _videoDetailView;
        public static LinearLayout _subscriptionRecyclerView;
        public static LinearLayout _tab1ParentLayout;

        public static ViewGroup _viewGroup;
        public static LayoutInflater _inflater;
        public static LinearLayoutManager _layoutMan;

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
            var _view = inflater.Inflate(Resource.Layout.Tab1FragmentLayout, container, false);
            _recycleView = _view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _videoDetailView = inflater.Inflate(Resource.Layout.Tab1VideoDetail, container, false);
            _layoutManager = new LinearLayoutManager(container.Context);
            _recycleView.SetLayoutManager(_layoutManager);
            _rootAdapter = new SubscriptionRecyclerViewAdapter(_creatorCardSetRoot);
            _rootAdapter.ItemClick += RootVideoAdapter_ItemClick;
           // _adapter.ItemClick += new EventHandler<int>((sender, e) => MAdapter_ItemClick(sender, e, _videoCard));
            _recycleView.SetAdapter(_rootAdapter);
            _tab1ParentLayout = _view.FindViewById<LinearLayout>(Resource.Id.tab1ParentFragmentLayout);
            
            _viewGroup = container;
            _inflater = inflater;
            return _view;
        }
        
        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootVideoAdapter_ItemClick(object sender, int e)
        {
            NavigateToNewPageFromVideoCard(_videoDetailView, null, _creatorCardSetRoot[e]);
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
            _tab1ParentLayout.RemoveAllViews();
            _tab1ParentLayout.AddView(view);
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
            SwapView(_videoDetailView);


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