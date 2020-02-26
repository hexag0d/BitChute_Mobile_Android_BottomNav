using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BottomNavigationViewPager.Classes;
using Java.IO;
using RecyclerViewer;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XamarinRecycleView.Adapter;
using static BottomNavigationViewPager.MainActivity;
using static StartServices.Servicesclass.ExtStickyService;

namespace BottomNavigationViewPager.Fragments
{
    public class SubscriptionFragment : Fragment
    {
        string _title;
        string _icon;
        
        static Android.Support.V7.Widget.RecyclerView mRecycleView;
        static Android.Support.V7.Widget.RecyclerView.LayoutManager mLayoutManager;
        VideoCardSet mPhotoAlbum;
        PhotoAlbumAdapter mAdapter;

        public static View _videoDetailView;
        public static LinearLayout _subscriptionRecyclerView;
        public static LinearLayout _tab2ParentLayout;

        public static ViewGroup _viewGroup;
        public static LayoutInflater _inflater;
        public static LinearLayoutManager _layoutMan;

        public static ServiceWebView _wv;
        readonly ExtWebViewClient _wvc = new ExtWebViewClient();

        public static string _url = "https://bitchute.com/subscriptions/";

        bool tabLoaded = false;

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

            _wv = _view.FindViewById<ServiceWebView>(Resource.Id.webView2);

            var mRecyclerView = _view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            _videoDetailView = inflater.Inflate(Resource.Layout.VideoDetail, container, false);
            //............................................................
            // Layout Manager Setup:

            // Use the built-in linear layout manager:
            mLayoutManager = new LinearLayoutManager(container.Context);

            // Or use the built-in grid layout manager (two horizontal rows):
            // mLayoutManager = new GridLayoutManager
            //        (this, 2, GridLayoutManager.Horizontal, false);

            // Plug the layout manager into the RecyclerView:
            mRecyclerView.SetLayoutManager(mLayoutManager);

            //............................................................
            // Adapter Setup:

            // Create an adapter for the RecyclerView, and pass it the
            // data set (the photo album) to manage:
            mAdapter = new PhotoAlbumAdapter(mPhotoAlbum);

            // Register the item click handler (below) with the adapter:
            mAdapter.ItemClick += MAdapter_ItemClick;

            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);

            _tab2ParentLayout = _view.FindViewById<LinearLayout>(Resource.Id.tab2ParentFragmentLayout);

            _videoDetailView = LayoutInflater.Inflate(Resource.Layout.VideoDetail, container, false);
            _viewGroup = container;
            _inflater = inflater;
            return _view;
        }
        
        private void MAdapter_ItemClick(object sender, int e)
        {
            bool test = true;
            if (test)
                SwapView(true);
            else
                SwapView(false);

            //var file = new File(Android.Net.Uri.Parse("file:///Assets/sample.mp4").ToString());
            //var uri = file.AbsolutePath;

            var check = e;
        }

        public static void SwapView(bool swap)
        {
            _tab2ParentLayout.RemoveAllViews();
            _tab2ParentLayout.AddView(_videoDetailView);

            if (swap)
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
                _wv.SetOnTouchListener(new ExtTouchListener());
            }
            else
            {
                _wv.SetOnTouchListener(null);
            }
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
            _scrollY += _wv.ScrollY;
            if (AppState.Display._horizontal)
            {
                await Task.Delay(500);
                if (_scrollY >= 4000)
                {
                    ExpandVideoCards(false);
                    _scrollY = 0;
                }
            }
        }

        public void OnSettingsChanged(List<object> settings)
        {
            if (AppSettings._zoomControl)
            {
                _wv.Settings.BuiltInZoomControls = true;
                _wv.Settings.DisplayZoomControls = false;
            }
            else
            {
                _wv.Settings.BuiltInZoomControls = false;
            }
        }

        private static int _scrollY = 0;

        //public class ExtScrollListener : Java.Lang.Object, View.IOnScrollChangeListener
        //{
        //    public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        //    {
        //        OnScrollChanged(scrollY);
        //    }
        //}

        //public static async void OnScrollChanged(int scrollY)
        //{
        //    await Task.Delay(60);
        //    _scrollY += scrollY;
        //    if (AppState.Display._horizontal)
        //    {
        //        if (_scrollY >= 4000)
        //        {
        //            ExpandVideoCards(false);
        //            _scrollY = 0;
        //        }
        //    }
        //}

        public void WebViewGoBack()
        {
            if (_wv.CanGoBack())
                _wv.GoBack();
        }

        static bool _wvRl = true;

        public void Pop2Root()
        {
            if (_wvRl)
            {
                _wv.Reload();
                _wvRl = false;
            }
            else
            {
                _wv.LoadUrl(@"https://bitchute.com/subscriptions/");
            }
        }

        public void SetWebViewVis()
        {
            _wv.Visibility = ViewStates.Visible;
        }

        public static bool _wvRling = false;

        /// <summary>
        /// this is to allow faster phones and connections the ability to Pop2Root
        /// used to be set without delay inside OnPageFinished but I don't think 
        /// that would work on faster phones
        /// </summary>
        public static async void SetReload()
        {
            if (!_wvRling)
            {
                _wvRling = true;
                await Task.Delay(AppSettings._tabDelay);
                _wvRl = true;
                _wvRling = false;
            }
        }

        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings._linkOverflowFixDelay);

            _wv.LoadUrl(JavascriptCommands._jsLinkFixer);
            _wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
            _wv.LoadUrl(JavascriptCommands._jsHideTooltips);
        }

        public void LoadCustomUrl(string url)
        {
            _wv.LoadUrl(url);
        }

        public static async void HidePageTitle(int delay)
        {
            await Task.Delay(delay);

            if (_wv.Url != "https://www.bitchute.com/" && AppState.Display._horizontal)
            {
                _wv.LoadUrl(JavascriptCommands._jsHideTitle);
                _wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
                _wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                _wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
            }
        }

        public static async void HideWatchTab(int delay)
        {
            if (delay != 0)
            {
                await Task.Delay(delay);
            }
            if (_wv.Url != "https://www.bitchute.com/")
            {
                _wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
            }
        }

        public static async void ExpandVideoCards(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(4000);
            }
            _wv.LoadUrl(JavascriptCommands._jsExpandSubs);
            _wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            _wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);

        }

        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView view, string url)
            {
                _wv.LoadUrl(JavascriptCommands._jsHideBanner);
                _wv.LoadUrl(JavascriptCommands._jsHideBuff);
                _wv.LoadUrl(JavascriptCommands._jsHideNavTabsList);

                if (AppState.Display._horizontal)
                {
                    HidePageTitle(5000);
                }
                SetReload();
                HideLinkOverflow();
                HideWatchTab(5000);
                ExpandVideoCards(true);
                _wv.LoadUrl(JavascriptCommands._jsLinkFixer);
                _wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
                _wv.LoadUrl(JavascriptCommands._jsHideTooltips);
                base.OnPageFinished(view, url);
            }
        }
    }
}
