using Android.OS;
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
        public static ServiceWebView Wv;
        public static bool ShowLoginOnStartup;

        public static string LastLoadedUrl = "";
        static object WebViewClient;
        public static int TNo = 0;
        public static HomePageFrag NewInstance(string title, string icon, string rootUrl = null)
        {
            //if (AppSettings.UserWasLoggedInLastAppClose || AppState.UserIsLoggedIn) {  WebViewClient = new Home(); }
            //else { WebViewClient = new LoginWebViewClient(); }
            WebViewClient = new Home();
            var fragment = new HomePageFrag();
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
                if (Arguments.ContainsKey("title")) { _title = (string)Arguments.Get("title"); }
                if (Arguments.ContainsKey("icon"))
                    _icon = (string)Arguments.Get("icon");
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


                GetFragmentById(this.Id, this);

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
                SwapLoginView();
            }
            else
            {
                SwapLoginView(false, true);
            }
            return ViewHelpers.Tab0.FragmentContainerLayout;
        }

        public void RegisterNewAccountButton_OnClick(object sender, EventArgs e)
        {
            SwapLoginView(true);
            Wv.LoadUrl("https://www.bitchute.com/accounts/register/");
        }


        public void ForgotPasswordButton_OnClick(object sender, EventArgs e)
        {
            SwapLoginView(true);
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
            SwapLoginView(true);
        }

        public void OnPostLoginAttempt(LoginEventArgs e)
        {
            if (e.LoginSuccess)
            {
                MainActivity.Fm0.SwapLoginView(true);
                ContinueWithoutLoginButton.Visibility = ViewStates.Gone;
                return;
            }
            else if (e.LoginAttemptFailure)
            {
                LoginErrorTextView.Text = "Login failed";
                LoginErrorTextView.Visibility = ViewStates.Visible;
            }
        }
        
        public static async void SetAutoPlayWithDelay(int delay)
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
        public void SwapLoginView(bool forceRemoveLoginLayout = false, bool forceWebViewLayout = false, bool forceShowLoginView = false)
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

        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack()) Wv.GoBack();
            //BitChute.Web.ViewClients.RunBaseCommands(Wv);
        }

        public void OnSettingsChanged(List<object> settings)
        {
            Wv.Settings.SetSupportZoom(AppSettings.ZoomControl);
            if (!AppSettings.Tab0FeaturedOn) { Wv.LoadUrl(JavascriptCommands._jsHideCarousel); }
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
