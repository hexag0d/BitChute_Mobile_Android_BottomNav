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
using Android.Widget;
using System;

namespace BitChute.Fragments
{
    public class FeedFrag : CommonFrag
    {
        string _title;
        string _icon;
        public static ServiceWebView Wv;
        public static Feed WebViewClient;
        public static int TNo = 2;

        public static FeedFrag NewInstance(string title, string icon, string rootUrl = null)
        {
            //if (AppSettings.UserWasLoggedInLastAppClose) { WebViewClient = new Feed(); }
            //else { WebViewClient = new LoginWebViewClient(); }
            WebViewClient = new Feed();
            var fragment = new FeedFrag();
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
                {
                    FragmentContainerLayout = inflater.Inflate(Resource.Layout.Tab2FragLayout, container, false);
                }
                if (TabFragmentRelativeLayout == null)
                {
                    TabFragmentRelativeLayout =
                        FragmentContainerLayout.FindViewById<RelativeLayout>(Resource.Id.tab2relativeLayout);
                }
                if (WebViewFragmentLayout == null)
                {
                    WebViewFragmentLayout = inflater.Inflate(Resource.Layout.Tab2WebView, container, false);
                }
                Wv = (ServiceWebView)WebViewFragmentLayout.FindViewById<ServiceWebView>(Resource.Id.webView2Swapable);
                Wv.RootUrl = RootUrl;
                Wv.SetWebViewClient(WebViewClient);
                Wv.Settings.JavaScriptEnabled = true;
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                if (AppSettings.ZoomControl)
                {
                    Wv.Settings.BuiltInZoomControls = true;
                    Wv.Settings.DisplayZoomControls = false;
                }
                else
                {
                    Wv.Settings.DisplayZoomControls = false;
                }
                
                
            }catch { }
            try
            {
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
                ContinueWithoutLoginButton.Click += ContinueWithoutLogin_OnClick;
                RegisterNewAccountButton.Click += RegisterNewAccountButton_OnClick;
            }
            catch
            {

            }
            try
            {
                SwapLoginView(false, true);
            }
            catch { }
            return FragmentContainerLayout;
        }


        public void RegisterNewAccountButton_OnClick(object sender, EventArgs e)
        {
            SwapLoginView(true);
            Wv.LoadUrl("https://www.bitchute.com/accounts/register/");
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
                TabFragmentRelativeLayout.RemoveAllViews();
                TabFragmentRelativeLayout.AddView(WebViewFragmentLayout);
                LoginVisible = false;
                return;
            }
            if (forceShowLoginView)
            {
                TabFragmentRelativeLayout.RemoveAllViews();
                TabFragmentRelativeLayout.AddView(LoginLayout);
                LoginVisible = true;
                return;
            }
            else
            {
                if (forceWebViewLayout)
                {
                    TabFragmentRelativeLayout.RemoveAllViews();
                    TabFragmentRelativeLayout.AddView(WebViewFragmentLayout);
                }
                else if (!LoginVisible)
                {
                    TabFragmentRelativeLayout.RemoveAllViews();
                    TabFragmentRelativeLayout.AddView(LoginLayout);
                }
                else
                {
                    TabFragmentRelativeLayout.RemoveAllViews();
                    TabFragmentRelativeLayout.AddView(WebViewFragmentLayout);
                }
            }
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
            if (AppSettings.Tab2Hide)
            {
                Wv.LoadUrl(JavascriptCommands._jsHideCarousel);
            }
            else { Wv.LoadUrl(JavascriptCommands._jsShowCarousel); }
        }
        
        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack()) Wv.GoBack();
            //BitChute.Web.ViewClients.RunBaseCommands(Wv);
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
