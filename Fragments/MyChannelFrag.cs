﻿using Android.Graphics.Drawables;
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
using BitChute.App;

namespace BitChute.Fragments
{
    public class MyChannelFrag : CommonFrag
    {
        string _title;
        string _icon;
        public static ServiceWebView Wv;
        public static object WebViewClient;
        
        public static int TNo = 3;
        public static bool WvRling;


        public static MyChannelFrag NewInstance(string title, string icon, string tabOverridePref = null)
        {
            string rootUrl = "";
            var fragment = new MyChannelFrag();
            fragment.Arguments = new Bundle();
            if (tabOverridePref != null && AppSettings.Tab3OverrideEnabled)
            {
                var tabFragPackage = new TabStates.TabFragPackage(tabOverridePref, true);
                WebViewClient = tabFragPackage.WebViewClient;
                rootUrl = tabFragPackage.RootUrl;
                title = tabFragPackage.Title;
                icon = tabFragPackage.Icon.ToString();
            }
            else
            {
                WebViewClient = new MyChannel();
                rootUrl = "https://www.bitchute.com/profile/";
            }
            fragment.RootUrl = rootUrl;
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
                if (AppSettings.Tab3OverrideEnabled) { RootUrl = AppSettings.GetTabOverrideUrlPref("tab3overridestring"); }
                BitChute.Web.ViewClients.SetWebViewClientFromObject(Wv, WebViewClient);
                Wv.SetWebChromeClient(new ExtWebChromeClient.ExtendedChromeClient(MainActivity.Main));
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                Wv.Settings.DisplayZoomControls = false;
                Wv.Settings.JavaScriptEnabled = true;


                if (AppSettings.ZoomControl)
                {
                    Wv.Settings.BuiltInZoomControls = true;
                    Wv.Settings.DisplayZoomControls = false;
                }
                this.Id = new System.Random().Next(777);
                GetFragmentById(this.Id, this);
            }
            catch (Exception ex) { }
            try
            {
                LoginLayout = inflater.Inflate(Resource.Layout.Login, container, false);
                LoginButton = LoginLayout.FindViewById<Button>(Resource.Id.loginButton);
                UserNameTextBox = LoginLayout.FindViewById<EditText>(Resource.Id.userNameEditText);
                PasswordTextBox = LoginLayout.FindViewById<EditText>(Resource.Id.passwordEditText);
                ContinueWithoutLoginButton = LoginLayout.FindViewById<Button>(Resource.Id.continueWithoutLoginButton);
                RegisterNewAccountButton = LoginLayout.FindViewById<Button>(Resource.Id.registerNewAccountButton);
                ForgotPasswordButton = LoginLayout.FindViewById<Button>(Resource.Id.forgotPasswordButton);
                ContinueWithoutLoginButton = LoginLayout.FindViewById<Button>(Resource.Id.continueWithoutLoginButton);
                LoginErrorTextView = LoginLayout.FindViewById<TextView>(Resource.Id.loginFailedTextView);
                LoginButton.Click += LoginButton_OnClick;
                ForgotPasswordButton.Click += ForgotPasswordButton_OnClick;
                ContinueWithoutLoginButton.Click += ContinueWithoutLogin_OnClick;
                RegisterNewAccountButton.Click += RegisterNewAccountButton_OnClick;
                ContinueWithoutLoginButton.Visibility = ViewStates.Gone;
            }
            catch { }
            return ViewHelpers.Tab3.TabFragmentLinearLayout;
        }


        public void RegisterNewAccountButton_OnClick(object sender, EventArgs e)
        {
            Wv.LoadUrl("https://www.bitchute.com/accounts/register/");

            SwapLoginView(true);
        }



        public void ForgotPasswordButton_OnClick(object sender, EventArgs e)
        {
            Wv.LoadUrl("https://www.bitchute.com/accounts/reset/");

            SwapLoginView(true);
        }


        public void ContinueWithoutLogin_OnClick(object sender, EventArgs e)
        {
            SwapLoginView(true);
        }

        public void LoginButton_OnClick(object sender, EventArgs e)
        {
            BitChute.Web.Login.MakeLoginRequest(
                UserNameTextBox.Text,
                PasswordTextBox.Text);
            UserNameTextBox.Text = "";
            PasswordTextBox.Text = "";
        }


        static bool LoginVisible;
        /// <summary>
        /// swaps the view for the test login layout
        /// </summary>
        /// <param name="v"></param>
        public void SwapLoginView(bool forceRemoveLoginLayout = false, bool forceWebViewLayout = false, bool forceShowLoginView = false)
        {
            if (forceRemoveLoginLayout)
            {
                ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
                ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(ViewHelpers.Tab3.WebViewFragmentLayout);
                LoginVisible = false;
                return;
            }
            if (forceShowLoginView)
            {
                ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
                ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(LoginLayout);
                LoginVisible = true;
                return;
            }
            else
            {
                if (forceWebViewLayout)
                {
                    ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
                    ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(ViewHelpers.Tab3.WebViewFragmentLayout);
                }
                else if (!LoginVisible)
                {
                    ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
                    ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(LoginLayout);
                }
                else
                {
                    ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
                    ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(ViewHelpers.Tab3.WebViewFragmentLayout);
                }
            }
            if (VideoDownloaderViewEnabled)
            {
                VideoDownloaderViewEnabled = false;
            }
        }

        private static bool _videoDownloaderViewEnabled = false;
        public static bool VideoDownloaderViewEnabled
        {
            get
            {
                return _videoDownloaderViewEnabled;
            }
            set
            {

                    _videoDownloaderViewEnabled = value;
            }
        }

        /// <summary>
        /// swaps the view for the downloader layout
        /// </summary>
        /// <param name="v">nullable, the view to swap for</param>
        public static void SwapDownloaderView()
        {
            if (!VideoDownloaderViewEnabled)
            {
                ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
                ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(ViewHelpers.Tab3.DownloaderLayout);
                VideoDownloaderViewEnabled = true;
                if (AppSettings.DlFabShowSetting == "onpress")
                {
                    ViewHelpers.Main.DownloadFAB.Show();
                    ViewHelpers.Main.DownloadFAB.Visibility = ViewStates.Visible;
                    ViewHelpers.Main.DownloadFAB.SetY(0);
                }
            }
            else
            {
                if (AppSettings.DlFabShowSetting == "onpress")
                {
                    ViewHelpers.Main.DownloadFAB.Hide();
                    ViewHelpers.Main.DownloadFAB.Visibility = ViewStates.Gone;
                    ViewHelpers.Main.DownloadFAB.SetY(5000);
                }
                ViewHelpers.Tab3.TabFragmentLinearLayout.RemoveAllViews();
                if (AppState.UserIsLoggedIn)
                {
                    ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(ViewHelpers.Tab3.WebViewFragmentLayout);
                    VideoDownloaderViewEnabled = false;
                }
                else
                {
                    ViewHelpers.Tab3.TabFragmentLinearLayout.AddView(MainActivity.Fm3.LoginLayout);
                    VideoDownloaderViewEnabled = false;
                }
            }
            if (MainActivity.ViewPager.CurrentItem != 3)
            {
                MainActivity.ViewPager.CurrentItem = 3;
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
            //BitChute.Web.ViewClients.RunBaseCommands(Wv, 2000);
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
