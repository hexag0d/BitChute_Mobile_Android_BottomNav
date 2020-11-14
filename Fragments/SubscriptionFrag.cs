using Android.OS;
using Android.Views;
using Android.Webkit;
using BitChute;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BitChute.Services.MainPlaybackSticky;
using BitChute.Web;
using static BitChute.ViewHelpers.Tab1;
using static BitChute.Web.ViewClients;
using Android.Widget;
using System;

namespace BitChute.Fragments
{
    public class SubscriptionFrag : CommonFrag
    {
        string _title;
        string _icon;
        public static object WebViewClient;
        public static int TNo = 1;

        public static SubscriptionFrag NewInstance(string title, string icon, string rootUrl = null, int tabId = -1)
        {
            //if (AppSettings.UserWasLoggedInLastAppClose) { WebViewClient = new Subs(); }
            //else { WebViewClient = new LoginWebViewClient(); }
            WebViewClient = new Subs();
            var fragment = new SubscriptionFrag();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            fragment.Arguments.PutInt("tabId", tabId);
            if (rootUrl == null) rootUrl = "https://www.bitchute.com/subscriptions/";
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
                if (Arguments.ContainsKey("tabId"))
                    TabId = (int)Arguments.Get("tabId");
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if (FragmentContainerLayout == null)
                {
                    FragmentContainerLayout = inflater.Inflate(Resource.Layout.Tab1FragLayout, container, false); 
                }
                if (TabFragmentRelativeLayout == null)
                {
                    TabFragmentRelativeLayout =
                        FragmentContainerLayout.FindViewById<RelativeLayout>(Resource.Id.tab1relativeLayout);
                }
                if (WebViewFragmentLayout == null)
                {
                    WebViewFragmentLayout = inflater.Inflate(Resource.Layout.Tab1WebView, container, false);
                }
                Wv = WebViewFragmentLayout.FindViewById<ServiceWebView>(Resource.Id.webView1Swapable);
                Wv.SetWebViewClient((Subs)WebViewClient);
                Wv.RootUrl = RootUrl;
                Wv.Settings.JavaScriptEnabled = true;
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                Wv.Settings.DisplayZoomControls = false;
                if (AppSettings.ZoomControl)
                {
                    Wv.Settings.BuiltInZoomControls = true;
                    Wv.Settings.DisplayZoomControls = false;
                }
                GetFragmentById(this.Id, this, TabId);

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
            try
            {
                if (AppState.UserIsLoggedIn)
                {
                    SwapFragView(false, true);
                }
            }
            catch { }
            return FragmentContainerLayout;
        }


        public void RegisterNewAccountButton_OnClick(object sender, EventArgs e)
        {
            Wv.LoadUrl("https://www.bitchute.com/accounts/register/");

            SwapFragView(true);
        }



        public void ForgotPasswordButton_OnClick(object sender, EventArgs e)
        {
            Wv.LoadUrl("https://www.bitchute.com/accounts/reset/");

            SwapFragView(true);
        }

        public void ContinueWithoutLogin_OnClick(object sender, EventArgs e)
        {
              SwapFragView(true);
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
        public override void SwapFragView(bool forceRemoveLoginLayout = false, bool forceWebViewLayout = false, bool forceShowLoginView = false)
        {
            try
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
            catch { }
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

        public static string GetHtmlText()
        {
            string innerHtml = "";
            return innerHtml;
        }
    }
}
