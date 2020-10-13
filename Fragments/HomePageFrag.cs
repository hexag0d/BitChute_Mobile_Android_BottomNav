using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using BitChute;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BitChute.Web.ViewClients;
using static BitChute.Services.MainPlaybackSticky;
using static BitChute.ViewHelpers.Tab0;

namespace BitChute.Fragments
{
    public class HomePageFrag : CommonWebViewFrag
    {
        string _title;
        string _icon;
        public static ServiceWebView Wv;
        public static string RootUrl = "https://www.bitchute.com/";
        public static string LastLoadedUrl = "";
        Home Wvc = new Home();
        public static int TNo = 0;
        public static HomePageFrag NewInstance(string title, string icon)
        {
            var fragment = new HomePageFrag();
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
                if (Arguments.ContainsKey("title")) { _title = (string)Arguments.Get("title"); }
                if (Arguments.ContainsKey("icon"))
                    _icon = (string)Arguments.Get("icon");
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {

                if (FragmentContainerLayout == null)
                { FragmentContainerLayout = inflater.Inflate(Resource.Layout.Tab0FragLayout, container, false); }
                
                Wv = FragmentContainerLayout.FindViewById<ServiceWebView>(Resource.Id.webView1);
                Wv.SetWebViewClient(Wvc);

                SetAutoPlayWithDelay(1); 
                Wv.Settings.JavaScriptEnabled = true;
                Wv.Settings.DisplayZoomControls = false;
                if (AppSettings.ZoomControl)
                {
                    Wv.Settings.BuiltInZoomControls = true;
                    Wv.Settings.DisplayZoomControls = false;
                }
                return FragmentContainerLayout;
            }
            catch { }
            return null;
        }

        public static async void SetAutoPlayWithDelay(int delay)
        {
            await Task.Delay(delay);
            Wv.Settings.MediaPlaybackRequiresUserGesture = false;
        }

        public static void SwapView(View v) { }
        public override void OnResume()
        {
            base.OnResume();
        }

        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack()) Wv.GoBack();
            BitChute.Web.ViewClients.RunBaseCommands(Wv);
        }

        public void OnSettingsChanged(List<object> settings)
        {
            Wv.Settings.SetSupportZoom(AppSettings.ZoomControl);
            if (!AppSettings.Tab1FeaturedOn) { Wv.LoadUrl(JavascriptCommands._jsHideCarousel); }
            else { Wv.LoadUrl(JavascriptCommands._jsShowCarousel); }
            if (AppSettings.ZoomControl) { Wv.Settings.BuiltInZoomControls = true; }
            else { Wv.Settings.BuiltInZoomControls = false; }
        }
        
        public static bool WvRl = true;
        /// <summary>
        /// one press refreshes the page; two presses pops back to the root
        /// </summary>
        public void Pop2Root()
        {
            if (WvRl)
            {
                Wv.Reload();
                WvRl = false;
            }
            else { Wv.LoadUrl(RootUrl); }
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

        static int _autoInt = 0;
        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings.LinkOverflowFixDelay);
            Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
            Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
            Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
        }

        public void LoadCustomUrl(string url) { Wv.LoadUrl(url); }
        public static async void HidePageTitle()
        {
            await Task.Delay(5000);

            if (Wv.Url != "https://www.bitchute.com/" && AppState.Display.Horizontal)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideTitle);
                Wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
                Wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                Wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
            }
        }

        private static async void HideWatchLabel()
        {
            await Task.Delay(4000);
            if (Wv.Url != "https://www.bitchute.com/")
                Wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }

        public void SetWebViewVis() { Wv.Visibility = ViewStates.Visible; }
        public static async void ExpandVideoCards(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(5000);
            }
            Wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            Wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);
        }

        private static async void ExpandFeaturedChannels(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(3000);
            }
            Wv.LoadUrl(JavascriptCommands._jsFeaturedRemoveMaxWidth);
            Wv.LoadUrl(JavascriptCommands._jsExpandFeatured);
        }

        private static async void ExpandPage(bool delayed)
        {
            if (delayed) { await Task.Delay(3000); }
        }
    }
}
