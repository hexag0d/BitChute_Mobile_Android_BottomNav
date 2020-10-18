using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute;

using BitChute.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BitChute.Services.MainPlaybackSticky;
using BitChute.Web;
using static BitChute.ViewHelpers.Tab3;
using static BitChute.Web.ViewClients;

namespace BitChute.Fragments
{
    public class MyChannelFrag : CommonWebViewFrag
    {
        string _title;
        string _icon;
        public static ServiceWebView Wv;
        public static object WebViewClient;
        
        public static int TNo = 3;
        public static bool WvRling;


        public static MyChannelFrag NewInstance(string title, string icon, string rootUrl = null)
        {
            if (AppSettings.UserWasLoggedInLastAppClose) { WebViewClient = new MyChannel(); }
            else { WebViewClient = new LoginWebViewClient(); }
            var fragment = new MyChannelFrag();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            if (rootUrl == null) rootUrl = "https://www.bitchute.com/profile/";
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

        public static bool WvRl = true;
        public void Pop2Root()
        {
            if (WvRl) { Wv.Reload(); WvRl = false; }
            else { Wv.LoadUrl(RootUrl); }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if (FragmentContainerLayout == null)
                ViewHelpers.Tab3.FragmentContainerLayout = inflater.Inflate(Resource.Layout.Tab3FragLayout, container, false);
                if (WebViewFragmentLayout == null)
                ViewHelpers.Tab3.WebViewFragmentLayout = inflater.Inflate(Resource.Layout.Tab3WebView, container, false);
                if (DownloaderLayout == null)
                ViewHelpers.Tab3.DownloaderLayout = inflater.Inflate(Resource.Layout.DownloaderLayout, container, false);
                ViewHelpers.Tab3.TabFragmentLinearLayout = (LinearLayout)ViewHelpers.Tab3.FragmentContainerLayout.FindViewById<LinearLayout>(Resource.Id.tab3LinearLayout);
                ViewHelpers.Tab3.DownloadButton = ViewHelpers.Tab3.DownloaderLayout.FindViewById<Button>(Resource.Id.downloadButton);
                ViewHelpers.Tab3.DownloadLinkEditText = ViewHelpers.Tab3.DownloaderLayout.FindViewById<EditText>(Resource.Id.downloadByUrlEditText);
                ViewHelpers.Tab3.DownloadFileNameEditText = ViewHelpers.Tab3.DownloaderLayout.FindViewById<EditText>(Resource.Id.downloadByUrlFileNameEditText);
                ViewHelpers.Tab3.DownloadProgressBar = ViewHelpers.Tab3.DownloaderLayout.FindViewById<ProgressBar>(Resource.Id.downloadProgressBar);
                ViewHelpers.Tab3.DownloadProgressTextView = ViewHelpers.Tab3.DownloaderLayout.FindViewById<TextView>(Resource.Id.progressTextView);
                Wv = (ServiceWebView)ViewHelpers.Tab3.WebViewFragmentLayout.FindViewById<ServiceWebView>(Resource.Id.webView3Swapable);
                Wv.RootUrl = RootUrl;
                ViewHelpers.Container = container;
                ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(ViewHelpers.Tab3.WebViewFragmentLayout);
                ViewHelpers.Tab3.AutoFillVideoTitleText = ViewHelpers.Tab3.DownloaderLayout.FindViewById<CheckBox>(Resource.Id.autoFillTitleCheckBox);
                ViewHelpers.Tab3.GetDownloadFilesButton = ViewHelpers.Tab3.DownloaderLayout.FindViewById<Button>(Resource.Id.getVideoFileDownloadButton);
                ViewHelpers.Tab3.AutoFillVideoTitleText = ViewHelpers.Tab3.DownloaderLayout.FindViewById<CheckBox>(Resource.Id.autoFillTitleCheckBox);
                ViewHelpers.Tab3.DownloadButton.Click += VideoDownloader.VideoDownloadButton_OnClick;
                ViewHelpers.Tab3.GetDownloadFilesButton.Click += FileBrowser.FileBrowserButton_OnClick;
                ViewHelpers.Tab3.FileLayoutManager = new Android.Support.V7.Widget.LinearLayoutManager(Android.App.Application.Context);
                ViewHelpers.Tab3.FileRecyclerView = ViewHelpers.Tab3.DownloaderLayout.FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.fileRecyclerView);
                ViewHelpers.Tab3.CancelDownloadButton = ViewHelpers.Tab3.DownloaderLayout.FindViewById<Button>(Resource.Id.cancelDownloadButton);
                ViewHelpers.Tab3.CancelDownloadButton.Click += VideoDownloader.CancelDownloadButton_OnClick;
                ViewHelpers.Main.DownloadFAB.Clickable = true;
                ViewHelpers.Main.DownloadFAB.Click += VideoDownloader.DownloadFAB_OnClick;
                if (AppSettings.FanMode) { RootUrl = AppSettings.GetTabOverrideUrlPref("tab4overridestring"); }
                if (WebViewClient.GetType() == typeof(LoginWebViewClient)) { Wv.SetWebViewClient((LoginWebViewClient)WebViewClient); }
                else { Wv.SetWebViewClient((MyChannel)WebViewClient); }
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                Wv.Settings.DisplayZoomControls = false;
                Wv.Settings.JavaScriptEnabled = true;
                if (AppSettings.ZoomControl)
                {
                    Wv.Settings.BuiltInZoomControls = true;
                    Wv.Settings.DisplayZoomControls = false;
                }
            }
            catch (Exception ex) { }
            return ViewHelpers.Tab3.TabFragmentLinearLayout;
        }

        /// <summary>
        /// swaps the view for the downloader layout
        /// </summary>
        /// <param name="v">nullable, the view to swap for</param>
        public static void SwapDownloaderView(bool showDownloadView)
        {
            if (showDownloadView)
            {
                ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
                ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(ViewHelpers.Tab3.DownloaderLayout);
            }
            else
            {
                ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
                ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(ViewHelpers.Tab3.WebViewFragmentLayout);
            }
        }

        public static async void ExpandVideoCards(bool delayed = false)
        {
            if (delayed) { await Task.Delay(4000); }
            Wv.LoadUrl(JavascriptCommands._jsExpandSubs);
            Wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            Wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);
        }

        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack()) Wv.GoBack();
            BitChute.Web.ViewClients.RunBaseCommands(Wv, 2000);
        }

        /// <summary>
        /// swaps the view for this tab
        /// </summary>
        /// <param name="v">nullable, the view to swap for</param>
        public static void SwapView(View v)
        {
            ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
            try { ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(v); }
            catch { }
        }

        public static async void LoadUrlWithDelay(string url, int delay)
        {
            await Task.Delay(delay); Wv.LoadUrl(url);
        }

        public void OnSettingsChanged(List<object> settings)
        {
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            else { Wv.Settings.BuiltInZoomControls = false; }
        }
    }
}
