using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BottomNavigationViewPager.Classes;
using Java.IO;
using BitChute;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static BottomNavigationViewPager.MainActivity;
using static StartServices.Servicesclass.ExtStickyService;
using BottomNavigationViewPager.Models;

namespace BottomNavigationViewPager.Fragments
{
    public class SubscriptionFragment : Fragment
    {
        string _title;
        string _icon;
        
        public static Android.Support.V7.Widget.RecyclerView _recycleView;
        public static Android.Support.V7.Widget.RecyclerView.LayoutManager _layoutManager;
        public static VideoCardSet _videoCardSet;
        public static ImageRecyclerViewAdapter _adapter;

        public static View _videoDetailView;
        public static LinearLayout _subscriptionRecyclerView;
        public static LinearLayout _tab2ParentLayout;

        public static ViewGroup _viewGroup;
        public static LayoutInflater _inflater;
        public static LinearLayoutManager _layoutMan;

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

            //GetSubscriptionList();
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
            var _view = inflater.Inflate(Resource.Layout.TheFragmentLayout2, container, false);
            _recycleView = _view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _videoDetailView = inflater.Inflate(Resource.Layout.VideoDetail, container, false);
            _layoutManager = new LinearLayoutManager(container.Context);
            _recycleView.SetLayoutManager(_layoutManager);
            _adapter = new ImageRecyclerViewAdapter(_videoCardSet);
            _adapter.ItemClick += MAdapter_ItemClick;
            // Plug the adapter into the RecyclerView:
            _recycleView.SetAdapter(_adapter);
            _tab2ParentLayout = _view.FindViewById<LinearLayout>(Resource.Id.tab2ParentFragmentLayout);
            _videoDetailView = LayoutInflater.Inflate(Resource.Layout.VideoDetail, container, false);
            
            _viewGroup = container;
            _inflater = inflater;
            return _view;
        }
        
        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MAdapter_ItemClick(object sender, int e)
        {
            SwapView(_videoDetailView);
            var check = e;
        }

        /// <summary>
        /// Navigates the selected tab to a new page
        /// </summary>
        /// <param name="view">the view you want to swap</param>
        /// <param name="type"></param>
        /// <param name=""></param>
        public static void NavigateToNewPage(View view, string linkId)
        {
            // the link will most likely be tagged with an identifier
            //this will tell the app what type of layout to load the page link in
            switch (linkId.Substring(0, 2))
            {
                //for example, link id starts with vd for videodetail
                case "vd":
                    // pass the link details to the video detail view
                    SwapView(_videoDetailView);
                    break;

            }
        }

        public static void SwapView(View view)
        {
            _tab2ParentLayout.RemoveAllViews();
            VideoDetailLoader.LoadVideoFromDetail(_videoDetailView, VideoModel.GetSample());
            _tab2ParentLayout.AddView(view);
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

        private static async void CustomOnTouch()
        {

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
