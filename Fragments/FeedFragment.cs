using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Classes;
using Java.IO;
using StartServices.Servicesclass;
using static Android.Views.View;
using static BitChute.MainActivity;
using static BitChute.Models.VideoModel;
using static StartServices.Servicesclass.ExtStickyService;

namespace BitChute.Fragments
{
    public class FeedFragment : Fragment
    {
        string _title;
        string _icon;

        public static Android.Support.V7.Widget.RecyclerView _recycleView;
        public static Android.Support.V7.Widget.RecyclerView.LayoutManager _layoutManager;

        //video cards for the root view
        public static VideoCardSet _feedVideoCardSetRoot;
        //video cards for the channel view
        public static VideoCardSet _videoCardSetChannel;
        //video cards for the related videos
        public static VideoCardSet _videoCardSetRelatedVideos;
        
        public static FeedRecyclerViewAdapter _rootAdapter;

        public static View _videoDetailView;
        public static LinearLayout _subscriptionRecyclerView;
        public static LinearLayout _tab2ParentLayout;

        public static ViewGroup _viewGroup;
        public static LayoutInflater _inflater;
        public static LinearLayoutManager _layoutMan;

        bool tabLoaded = false;

        public static FeedFragment NewInstance(string title, string icon)
        {
            var fragment = new FeedFragment();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            return fragment;
        }

        public static void GetFeedList()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _feedVideoCardSetRoot = new VideoCardSet();
            if (Arguments != null)
            {
                if (Arguments.ContainsKey("title"))
                    _title = (string)Arguments.Get("title");

                if (Arguments.ContainsKey("icon"))
                    _icon = (string)Arguments.Get("icon");
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _view = inflater.Inflate(Resource.Layout.Tab2FragmentLayout, container, false);
            _recycleView = _view.FindViewById<RecyclerView>(Resource.Id.feedRecyclerView);
            _recycleView.NestedScrollingEnabled = false;
            _layoutManager = new LinearLayoutManager(container.Context);
            _recycleView.SetLayoutManager(_layoutManager);
            _rootAdapter = new FeedRecyclerViewAdapter(_feedVideoCardSetRoot);
            _rootAdapter.ItemClick += RootVideoAdapter_ItemClick;
            // _adapter.ItemClick += new EventHandler<int>((sender, e) => MAdapter_ItemClick(sender, e, _videoCard));
            _recycleView.SetAdapter(_rootAdapter);
            _tab2ParentLayout = _view.FindViewById<LinearLayout>(Resource.Id.tab2ParentFragmentLayout);
            _videoDetailView = LayoutInflater.Inflate(Resource.Layout.Tab2VideoDetail, container, false);

            _viewGroup = container;
            _inflater = inflater;

            if (!tabLoaded)
            {
                tabLoaded = true;
            }
            if (AppSettings._zoomControl)
            {
            }
            CustomSetTouchListener(AppState.Display._horizontal);
            return _view;
        }

        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootVideoAdapter_ItemClick(object sender, int e)
        {
            NavigateToNewPageFromVideoCard(_videoDetailView, _feedVideoCardSetRoot[e]);
        }

        /// <summary>
        /// Navigates the selected tab to a new page
        /// </summary>
        /// <param name="view">the view you want to swap</param>
        /// <param name="type"></param>
        /// <param name=""></param>
        public static void NavigateToNewPageFromVideoCard(View view, VideoCard videoCard)
        {
            VideoDetailLoader.LoadVideoFromVideoCard(view, videoCard);
            SwapView(view);
        }

        /// <summary>
        /// removes the child views and swaps for the View arg
        /// </summary>
        /// <param name="view"></param>
        public static void SwapView(View view)
        {
            _tab2ParentLayout.RemoveAllViews();
            _tab2ParentLayout.AddView(view);
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

        public void CustomSetTouchListener(bool landscape)
        {
            if (landscape)
            {
            }
            else
            {
            }
        }

        public static MainActivity _main = MainActivity._main;

        public class ExtTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                MainActivity.CustomOnTouch();
                CustomOnTouch();
                return false;
            }
        }

        private static int _scrollY = 0;



        private static async void CustomOnTouch()
        {
            if (AppState.Display._horizontal)
            {
            }
        }

        public void WebViewGoBack()
        {
        }

        public void Pop2Root()
        {

        }

        public static bool _wvRling = false;

        /// <summary>
        /// this is to allow faster phones and connections the ability to Pop2Root
        /// used to be set without delay inside OnPageFinished but I don't think 
        /// that would work on faster phones
        /// </summary>
        public static async void SetReload()
        {

        }


        public void LoadCustomUrl(string url)
        {
        }

        public static async void HidePageTitle(int delay)
        {

        }

        private static async void HideWatchLabel(int delay)
        {
            await Task.Delay(delay);
        }
    }
}
