using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using BitChute;
using static BitChute.Services.MainPlaybackSticky;
using BitChute.Web;
using static BitChute.ViewHelpers.Tab2;
using static BitChute.Web.ViewClients;
using BitChute.ViewHolders;
using Android.Widget;
using Android.Support.V7.Widget;

namespace BitChute.Fragments
{
    public class FeedFragNative : CommonWebViewFrag
    {
        string _title;
        string _icon;
        public static ServiceWebView Wv;
        public static object WebViewClient;
        public static int TNo = 2;

        public static FeedFragNative NewInstance(string title, string icon, string rootUrl = null)
        {
            if (AppSettings.UserWasLoggedInLastAppClose) { WebViewClient = new Feed(); }
            else { WebViewClient = new LoginWebViewClient(); }
            var fragment = new FeedFragNative();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            if (rootUrl == null) rootUrl = "https://www.bitchute.com/";
            fragment.RootUrl = rootUrl;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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
            try
            {
                if (FragmentContainerLayout == null)
                    FragmentContainerLayout = inflater.Inflate(Resource.Layout.Tab2FragLayout, container, false);
                if (TabFragmentRelativeLayout == null)  {
                    TabFragmentRelativeLayout = FragmentContainerLayout.FindViewById<RelativeLayout>(Resource.Id.feedTabRelativeLayout);
                }
                if (FeedView == null)
                {
                    FeedView = inflater.Inflate(Resource.Layout.FeedLayout, container, false);
                }
                if (WebViewFragmentLayout == null)
                {
                    WebViewFragmentLayout = inflater.Inflate(Resource.Layout.Tab2WebView, container, false);
                }
                Wv = (ServiceWebView)FragmentContainerLayout.FindViewById<ServiceWebView>(Resource.Id.webView2);
                Wv.RootUrl = RootUrl;
                if (WebViewClient.GetType() == typeof(LoginWebViewClient)) { Wv.SetWebViewClient((LoginWebViewClient)WebViewClient); }
                else { Wv.SetWebViewClient((Feed)WebViewClient); }
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                Wv.Settings.JavaScriptEnabled = true;
                if (AppSettings.ZoomControl)
                {
                    Wv.Settings.BuiltInZoomControls = true;
                    Wv.Settings.DisplayZoomControls = false;
                }
                Wv.Settings.DisplayZoomControls = false;
                SwapFeedView();
                this.GetRecyclerAdapter(container);
                return FragmentContainerLayout;
            }
            catch { }
            return null;
        }

        public static RecyclerView _feedRecyclerView;
        public static LinearLayoutManager _layoutManager;

        public void GetRecyclerAdapter(ViewGroup container)
        {
            _feedRecyclerView = FragmentContainerLayout.FindViewById<RecyclerView>(Resource.Id.feedRecyclerView);
            _feedRecyclerView.NestedScrollingEnabled = false;
            _layoutManager = new LinearLayoutManager(container.Context);

            _feedRecyclerView.SetLayoutManager(_layoutManager);
            //var _rootAdapter = GetFeedAdapter();
           // _rootAdapter.ItemClick += RootVideoAdapter_ItemClick;
           // _recycleView.SetAdapter(_rootAdapter);
            
        }

        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void RootVideoAdapter_ItemClick(object sender, int e)
        {
            
        }


        static bool FeedVisible;
        /// <summary>
        /// swaps the view for the test login layout
        /// </summary>
        /// <param name="v"></param>
        public static void SwapFeedView()
        {
            if (!FeedVisible)
            {
               TabFragmentRelativeLayout.RemoveAllViews();
                TabFragmentRelativeLayout.AddView(FeedView);
            }
            else
            {
                TabFragmentRelativeLayout.RemoveAllViews();
                TabFragmentRelativeLayout.AddView(WebViewFragmentLayout);
            }
            FeedVisible = !FeedVisible;
        }

        public static async void GetSubscriptionFeed()
        {
            var vcList = await BitChute.Web.Request.GetVideoCardList();
            var frva = new FeedRecyclerViewAdapter(vcList);

            frva.ItemClick += RootVideoAdapter_ItemClick;
             _feedRecyclerView.SetAdapter(frva);
        }

        public static FeedRecyclerViewAdapter GetFeedAdapter()
        {
            return new FeedRecyclerViewAdapter(new List<Models.VideoModel.VideoCard>());
        }

        public async void LoadUrlWithDelay(string url, int delay)
        {
            await Task.Delay(delay);
            Wv.LoadUrl(url);
        }

        public void OnSettingsChanged(List<object> settings)
        {
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            else { Wv.Settings.BuiltInZoomControls = false; }
            if (AppSettings.Tab3Hide)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideCarousel);
            }
            else { Wv.LoadUrl(JavascriptCommands._jsShowCarousel); }
        }

        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack()) Wv.GoBack();
            BitChute.Web.ViewClients.RunBaseCommands(Wv);
        }

        public static bool WvRl = true;
        public void Pop2Root()
        {
            if (WvRl) { Wv.Reload(); WvRl = false; }
            else { Wv.LoadUrl(@"https://www.bitchute.com/"); }
        }

        public static bool WvRling = false;
        /// <summary>
        /// this is to allow faster phones and connections the ability to Pop2Root
        /// used to be set without delay inside OnPageFinished but I don't think 
        /// that would work on faster phones
        /// </summary>
        public static async void SetReload()
        {
            if (!WvRling)
            {
                WvRling = true;
                await Task.Delay(AppSettings.TabDelay);
                WvRl = true;
                WvRling = false;
            }
        }

        public void LoadCustomUrl(string url) { Wv.LoadUrl(url); }
        public static async void HidePageTitle(int delay)
        {
            if (delay != 0) { await Task.Delay(delay); }
            if (Wv.Url != "https://www.bitchute.com/" && AppState.Display.Horizontal)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideTitle);
                Wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
                Wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                Wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
            }
        }

        private static async void HideWatchLabel(int delay)
        {
            await Task.Delay(delay);
            Wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }

        public static async void ExpandVideoCards(bool delayed = false)
        {
            if (delayed) { await Task.Delay(5000); }
            Wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            Wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);
        }

        public static async void SelectSubscribedTab(int delay)
        {
            await Task.Delay(delay);
            Wv.LoadUrl(JavascriptCommands._jsSelectSubscribed);
        }
    }
}
