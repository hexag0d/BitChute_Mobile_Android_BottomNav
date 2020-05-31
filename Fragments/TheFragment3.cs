
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Classes;
using BitChute;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static StartServices.Servicesclass.ExtStickyService;


namespace BitChute.Fragments
{
    public class TheFragment3 : Fragment
    {
        string _title;
        string _icon;

        public static ServiceWebView Wv;
        readonly ExtWebViewClient _wvc = new ExtWebViewClient();

        public static string RootUrl = "https://www.bitchute.com/profile";

        bool tabLoaded = false;

        public static TheFragment3 NewInstance(string title, string icon) {
            var fragment = new TheFragment3();
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
                if(Arguments.ContainsKey("title"))
                    _title = (string)Arguments.Get("title");

                if (Arguments.ContainsKey("icon"))
                    _icon = (string)Arguments.Get("icon");
            }
        }

        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack())
                Wv.GoBack();
        }

        static bool _wvRl = true;

        public void Pop2Root()
        {
            if (_wvRl)
            {
                Wv.Reload();
                _wvRl = false;
            }
            else
            {
                Wv.LoadUrl(RootUrl);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewHelpers.Tab3.FragmentContainerLayout = inflater.Inflate(Resource.Layout.TheFragmentLayout4, container, false);
            ViewHelpers.Tab3.WebViewFragmentLayout = inflater.Inflate(Resource.Layout.Tab3WebView, container, false);
            ViewHelpers.Tab3.DownloaderLayout = inflater.Inflate(Resource.Layout.DownloaderLayout, container, false);
            ViewHelpers.Tab3.TabFragmentLinearLayout = (LinearLayout)ViewHelpers.Tab3.FragmentContainerLayout.FindViewById<LinearLayout>(Resource.Id.tab3LinearLayout);
            ViewHelpers.Tab3.DownloadButton = ViewHelpers.Tab3.DownloaderLayout.FindViewById<Button>(Resource.Id.downloadButton);
            ViewHelpers.Tab3.DownloadLinkEditText = ViewHelpers.Tab3.DownloaderLayout.FindViewById<EditText>(Resource.Id.downloadByUrlEditText);
            ViewHelpers.Tab3.DownloadFileNameEditText = ViewHelpers.Tab3.DownloaderLayout.FindViewById<EditText>(Resource.Id.downloadByUrlFileNameEditText);
            ViewHelpers.Tab3.DownloadProgressBar = ViewHelpers.Tab3.DownloaderLayout.FindViewById<ProgressBar>(Resource.Id.downloadProgressBar);
            ViewHelpers.Tab3.DownloadProgressTextView = ViewHelpers.Tab3.DownloaderLayout.FindViewById<TextView>(Resource.Id.progressTextView);
            Wv = (ServiceWebView)ViewHelpers.Tab3.WebViewFragmentLayout.FindViewById<ServiceWebView>(Resource.Id.webView3Swapable);
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
            if (AppSettings.FanMode)
            {
                //get the url string from prefs
                RootUrl = AppSettings.GetTabOverrideUrlPref("tab4overridestring");
            }

                Wv.SetWebViewClient(new ExtWebViewClient());
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                Wv.Settings.DisplayZoomControls = false;
                Wv.Settings.JavaScriptEnabled = true;
                //_wv.Settings.AllowFileAccess = true;
                //_wv.Settings.AllowContentAccess = true;
                tabLoaded = true;
            
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            CustomSetTouchListener(AppState.Display.Horizontal);
            //_wv.SetOnScrollChangeListener(new ExtScrollListener());
            LoadUrlWithDelay(RootUrl, 4000);
            //ViewHelpers.Main.DownloadFAB.LayoutParameters = 
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

        /// <summary>
        /// swaps the view for this tab
        /// </summary>
        /// <param name="v">nullable, the view to swap for</param>
        public static void SwapView(View v)
        {
            ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
            try
            {
                ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(v);
            }
            catch
            {
            }
        }

        public void CustomSetTouchListener(bool landscape)
        {
            if (landscape)
            {
                Wv.SetOnTouchListener(new ExtTouchListener());
            }
            else
            {
                Wv.SetOnTouchListener(null);
            }
        }

        public static async void LoadUrlWithDelay(string url, int delay)
        {
            await Task.Delay(delay);
            Wv.LoadUrl(url);
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
            _scrollY += Wv.ScrollY;
            if (AppState.Display.Horizontal)
            {
                await Task.Delay(500);
                if (_scrollY >= 4000)
                {
                    ExpandVideoCards(false);
                    _scrollY = 0;
                }
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
        //        await Task.Delay(500);
        //        if (_scrollY >= 4000)
        //        {
        //            ExpandVideoCards(false);
        //            _scrollY = 0;
        //        }
        //    }
        //}

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
                await Task.Delay(AppSettings.TabDelay);
                _wvRl = true;
                _wvRling = false;
            }
        }

        public void OnSettingsChanged(List<object> settings)
        {
            if (AppSettings.FanMode && AppSettings.Tab4OverridePreference == "Feed")
            {
                Wv.LoadUrl(JavascriptCommands._jsHideCarousel);
                Wv.LoadUrl(JavascriptCommands._jsHideTab1);
                Wv.LoadUrl(JavascriptCommands._jsHideTab2);
                Wv.LoadUrl(JavascriptCommands._jsSelectTab3);
                Wv.LoadUrl(JavascriptCommands._jsHideLabel);
            }

            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            else
            {
                Wv.Settings.BuiltInZoomControls = false;
            }
        }

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
        
        public void LoadCustomUrl(string url)
        {
            Wv.LoadUrl(url);
        }

        public static async void HidePageTitle(int delay)
        {
            await Task.Delay(delay);

            if (Wv.Url != "https://www.bitchute.com/" && AppState.Display.Horizontal)
            {
                Wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                Wv.LoadUrl(JavascriptCommands._jsPageBarDelete);
                Wv.LoadUrl(JavascriptCommands._jsHideTitle);
                Wv.LoadUrl(JavascriptCommands._jsHideWatchTab);
            }
        }

        public void SetWebViewVis()
        {
            Wv.Visibility = ViewStates.Visible;
        }

        public static async void ExpandVideoCards(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(4000);
            }
            if (AppSettings.Tab5OverridePreference == "Subs" || AppSettings.FanMode)
            {
                Wv.LoadUrl(JavascriptCommands._jsExpandSubs);
            }
            Wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            Wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);
        }
        
        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                Wv.ScrollTo(0, 0);
                Wv.LoadUrl(JavascriptCommands._jsHideBanner);
                Wv.LoadUrl(JavascriptCommands._jsHideBuff);

                if (TabStates.T4Is == "Feed")
                {
                    Wv.LoadUrl(JavascriptCommands._jsHideCarousel);
                    Wv.LoadUrl(JavascriptCommands._jsHideTab1);
                    Wv.LoadUrl(JavascriptCommands._jsHideTab2);
                    Wv.LoadUrl(JavascriptCommands._jsSelectTab3);
                    Wv.LoadUrl(JavascriptCommands._jsHideTrending);
                    //_wv.LoadUrl(JavascriptCommands._jsHideLabel);
                }

                if (!AppSettings.Tab1FeaturedOn)
                {
                    Wv.LoadUrl(JavascriptCommands._jsHideCarousel);
                }

                if (AppState.Display.Horizontal)
                {
                    HidePageTitle(5000);
                }
                
                Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
                HideLinkOverflow();
                SetReload();
                Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
                Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
            }
        }
    }
}
