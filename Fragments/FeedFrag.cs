using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using BitChute.Classes;
using static BitChute.Services.ExtSticky;
using BitChute.Web;

namespace BitChute.Fragments
{
    public class FeedFrag : Fragment
    {
        string _title;
        string _icon;
        public static ServiceWebView Wv;
        readonly ViewClients.Feed _wvc = new ViewClients.Feed();
        public static int TNo = 2;
        public static string RootUrl = "https://www.bitchute.com/";

        public static FeedFrag NewInstance(string title, string icon)
        {
            var fragment = new FeedFrag();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
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
            var _view = inflater.Inflate(Resource.Layout.Tab2FragLayout, container, false);
            Wv = (ServiceWebView)_view.FindViewById<ServiceWebView>(Resource.Id.webView3);
            Wv.SetWebViewClient(_wvc);
            Wv.Settings.MediaPlaybackRequiresUserGesture = false;
            Wv.Settings.JavaScriptEnabled = true;
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            Wv.Settings.DisplayZoomControls = false;
            //LoadUrlWithDelay(RootUrl, 2000);
            return _view;
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
            if (Wv.CanGoBack())
                Wv.GoBack();
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
            if (delayed){ await Task.Delay(5000); }
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
