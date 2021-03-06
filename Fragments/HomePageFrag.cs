﻿using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using BitChute;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BitChute.Web.ViewClients;
using static BitChute.Services.MainPlaybackSticky;
using static BitChute.ViewHelpers.Tab0;
using static BitChute.ViewHelpers.Main;
using Android.Widget;
using BitChute.Web;
using System;
using BitChute.Web.Auth;

namespace BitChute.Fragments
{
    public class HomePageFrag : CommonFrag
    {
        string _title;
        string _icon;
        public static bool ShowLoginOnStartup;

        public static string LastLoadedUrl = "";
        static object WebViewClient;
        public static int TNo = 0;
        public static HomePageFrag NewInstance(string title, string icon, string rootUrl = null, int tabId = -1)
        {
            //if (AppSettings.UserWasLoggedInLastAppClose || AppState.UserIsLoggedIn) {  WebViewClient = new Home(); }
            //else { WebViewClient = new LoginWebViewClient(); }
            WebViewClient = new Home();
            var fragment = new HomePageFrag();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            fragment.Arguments.PutInt("tabId", tabId);
            if (rootUrl == null) rootUrl = "https://www.bitchute.com/";
            fragment.RootUrl = rootUrl;
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
                if (Arguments.ContainsKey("tabId"))
                    TabId = (int)Arguments.Get("tabId");
            }
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if (ViewHelpers.Tab0.FragmentContainerLayout == null)
                {
                    ViewHelpers.Tab0.FragmentContainerLayout = 
                        inflater.Inflate(Resource.Layout.Tab0FragLayout, container, false);
                }
                if (ViewHelpers.Tab0.TabFragmentRelativeLayout == null)
                {
                    ViewHelpers.Tab0.TabFragmentRelativeLayout =
                        ViewHelpers.Tab0.FragmentContainerLayout.FindViewById<RelativeLayout>(Resource.Id.tab0relativeLayout);
                }
                if (WebViewFragmentLayout == null)
                {
                    WebViewFragmentLayout = inflater.Inflate(Resource.Layout.Tab0WebView, container, false);
                }

                Wv = WebViewFragmentLayout.FindViewById<ServiceWebView>(Resource.Id.webView0Swapable);
                //Wv.LoadUrl("file:///android_asset/html/splash.html");
                Wv.RootUrl = RootUrl;
                Wv.SetWebViewClient((Home)WebViewClient);

                //LoginLayout = inflater.Inflate(Resource.Layout.Login, container, false);
                SetAutoPlayWithDelay(1);
                Wv.Settings.JavaScriptEnabled = true;
                Wv.Settings.DisplayZoomControls = false;
                if (AppSettings.ZoomControl)
                {
                    Wv.Settings.BuiltInZoomControls = true;
                    Wv.Settings.DisplayZoomControls = false;
                }


                GetFragmentById(this.Id, this, TabId);

            }

            catch { }
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
            }
            catch { }
            if (ShowLoginOnStartup)
            {
                //SwapMainActivityLoginView(true);
                SwapFragView();
            }
            else
            {
                SwapFragView(false, true);
            }
            return ViewHelpers.Tab0.FragmentContainerLayout;
        }

        public void RegisterNewAccountButton_OnClick(object sender, EventArgs e)
        {
            SwapFragView(true);
            Wv.LoadUrl("https://www.bitchute.com/accounts/register/");
        }


        public void ForgotPasswordButton_OnClick(object sender, EventArgs e)
        {
            SwapFragView(true);
            Wv.LoadUrl("https://www.bitchute.com/accounts/reset/");
        }

        public void LoginButton_OnClick(object sender, EventArgs e)
        {
            BitChute.Web.Login.MakeLoginRequest(
                UserNameTextBox.Text,
                PasswordTextBox.Text);
            UserNameTextBox.Text = "";
            PasswordTextBox.Text = "";
        }

        public void ContinueWithoutLogin_OnClick(object sender, EventArgs e)
        {
            if (MainActivity.ViewPager.CurrentItem == 0)
            SwapFragView(true);
        }

        public void OnPostLoginAttempt(LoginEventArgs e)
        {
            if (e.LoginSuccess)
            {
                MainActivity.Fm0.SwapFragView(true);
                ContinueWithoutLoginButton.Visibility = ViewStates.Gone;
                return;
            }
            else if (e.LoginAttemptFailure)
            {
                LoginErrorTextView.Text = "Login failed";
                LoginErrorTextView.Visibility = ViewStates.Visible;
            }
        }
        
        public async void SetAutoPlayWithDelay(int delay)
        {
            await Task.Delay(delay);
            Wv.Settings.MediaPlaybackRequiresUserGesture = false;
        }

        public void SwapMainActivityLoginView(bool showLoginScreen)
        {
            ContentRelativeLayout.AddView(ViewHelpers.Main.LoginLayout);
        }

        static bool LoginVisible;
        /// <summary>
        /// swaps the view for the test login layout
        /// </summary>
        /// <param name="v"></param>
        public override void SwapFragView(bool forceRemoveLoginLayout = false, bool forceWebViewLayout = false, bool forceShowLoginView = false)
        {
            if (forceRemoveLoginLayout)
            {
                ViewHelpers.Tab0.TabFragmentRelativeLayout.RemoveAllViews();
                ViewHelpers.Tab0.TabFragmentRelativeLayout.AddView(WebViewFragmentLayout);
                LoginVisible = false;
                return;
            }
            if (forceShowLoginView)
            {
                ViewHelpers.Tab0.TabFragmentRelativeLayout.RemoveAllViews();
                ViewHelpers.Tab0.TabFragmentRelativeLayout.AddView(LoginLayout);
                LoginVisible = true;
                return;
            }
            else
            {
                if (forceWebViewLayout)
                {
                    ViewHelpers.Tab0.TabFragmentRelativeLayout.RemoveAllViews();
                    ViewHelpers.Tab0.TabFragmentRelativeLayout.AddView(WebViewFragmentLayout);
                }
                else if (!LoginVisible)
                {
                    ViewHelpers.Tab0.TabFragmentRelativeLayout.RemoveAllViews();
                    ViewHelpers.Tab0.TabFragmentRelativeLayout.AddView(LoginLayout);
                }
                else
                {
                    ViewHelpers.Tab0.TabFragmentRelativeLayout.RemoveAllViews();
                    ViewHelpers.Tab0.TabFragmentRelativeLayout.AddView(WebViewFragmentLayout);
                }
            }
        }


        public override void OnResume()
        {
            base.OnResume();
        }

        public void OnSettingsChanged(List<object> settings)
        {
            Wv.Settings.SetSupportZoom(AppSettings.ZoomControl);
            if (!AppSettings.Tab0FeaturedOn) { Wv.LoadUrl(JavascriptCommands._jsHideCarousel); }
            else { Wv.LoadUrl(JavascriptCommands._jsShowCarousel); }
            if (AppSettings.ZoomControl) { Wv.Settings.BuiltInZoomControls = true; }
            else { Wv.Settings.BuiltInZoomControls = false; }
        }
        
        //public static bool WvRl = true;
        ///// <summary>
        ///// one press refreshes the page; two presses pops back to the root
        ///// </summary>
        //public void Pop2Root()
        //{
        //    if (WvRl)
        //    {
        //        Wv.Reload();
        //        WvRl = false;
        //    }
        //    else { Wv.LoadUrl(RootUrl); }
        //}

        //public static bool WvRling = false;
        ///// <summary>
        ///// this is to allow faster phones and connections the ability to Pop2Root
        ///// used to be set without delay inside OnPageFinished but I don't think 
        ///// that would work on faster phones
        ///// </summary>
        //public static async void SetReload()
        //{
        //    if (!WvRling)
        //    {
        //        WvRling = true;
        //        await Task.Delay(AppSettings.TabDelay);
        //        WvRl = true;
        //        WvRling = false;
        //    }
        //}
    }
}
