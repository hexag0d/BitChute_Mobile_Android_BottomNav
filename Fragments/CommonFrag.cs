using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BitChute.Services;
using BitChute.ViewModel;
using BitChute.Web.Auth;
using static BitChute.Models.VideoModel;
using static BitChute.Services.MainPlaybackSticky;

namespace BitChute.Fragments
{
    public class CommonFrag : Android.Support.V4.App.Fragment // @TODO consolidate webview fragment shared methods, members
    {
        private int _tabId;
        public int TabId { get { return _tabId; } set { _tabId = value; } }
        public delegate void LoginEventDelegate(object sender, LoginEventArgs args);
        public static event LoginEventDelegate OnLogin;
        public delegate void LogoutEventDelegate(LogoutEventArgs args);
        public static event LogoutEventDelegate OnLogout;
        private int _id;
        public int Id { get { return _id; }set { _id = value; } }
        private VideoView _videoView;
        public VideoView VideoView { get { return _videoView; } set { _videoView = value; } }
        private View _webViewFragment;
        public View WebViewFragmentLayout { get { return _webViewFragment; } set { _webViewFragment = value; } }
        private View _feedView;
        public View FeedView { get { return _feedView; } set { _feedView = value; } }
        private View _loginLayout;
        public View LoginLayout { get { return _loginLayout; } set { _loginLayout = value; } }
        private RelativeLayout _tabFragmentRelativeLayout;
        public RelativeLayout TabFragmentRelativeLayout { get { return _tabFragmentRelativeLayout; } set { _tabFragmentRelativeLayout = value; } }
        private View _fragmentContainerLayout;
        public View FragmentContainerLayout { get { return _fragmentContainerLayout; } set { _fragmentContainerLayout = value; }}

        private Button _loginButton;
        public Button LoginButton { get { return _loginButton; } set { _loginButton = value; } }
        private Button _continueWithoutLoginButton;
        public Button ContinueWithoutLoginButton { get { return _continueWithoutLoginButton; } set { _continueWithoutLoginButton = value; } }
        private Button _logoutButton;
        public Button LogoutButton { get { return _logoutButton; } set { _logoutButton = value; } }
        private Button _logoutAndHardFlushCacheButton;
        public Button LogoutAndHardFlushCacheButton { get { return _logoutAndHardFlushCacheButton; } set { _logoutAndHardFlushCacheButton = value; } }
        private Button _registerNewAccountButton;
        public Button RegisterNewAccountButton { get { return _registerNewAccountButton; } set { _registerNewAccountButton = value; } }
        private Button _forgotPasswordButton;
        public Button ForgotPasswordButton { get { return _forgotPasswordButton; } set { _forgotPasswordButton = value; } }
        private EditText _usernameTextBox;
        public EditText UserNameTextBox { get { return _usernameTextBox; } set { _usernameTextBox = value; } }
        private EditText _passwordTextBox;
        public EditText PasswordTextBox { get { return _passwordTextBox; } set { _passwordTextBox = value; } }
        private TextView _loginErrorTextView;
        public TextView LoginErrorTextView { get { return _loginErrorTextView; } set { _loginErrorTextView = value; } }

        private RecyclerView _feedRecyclerView;
        public RecyclerView FeedRecyclerView { get { return _feedRecyclerView; } set { _feedRecyclerView = value; } }
        private LinearLayoutManager _layoutManager;
        public LinearLayoutManager LayoutManager { get { return _layoutManager; } set { _layoutManager = value; } }
        private RelativeLayout _relativeLayout;
        public RelativeLayout RelativeLayout { get { return _relativeLayout; }set { _relativeLayout = value; } }
        private LinearLayout _linearLayout;
        public LinearLayout LinearLayout { get { return _linearLayout; }set { _linearLayout = value; } }
        private View _videoDetailView;
        public View VideoDetailView { get { return _videoDetailView; } set { _videoDetailView = value; } }
        private ServiceWebView _wv;
        public ServiceWebView Wv { get { return _wv; } set { _wv = value; } }
        private VideoDetailLoader _videoDetail;
        public VideoDetailLoader VideoDetail { get { return _videoDetail; } set { _videoDetail = value; } }
        private static Dictionary<int, CommonFrag> _fragmentByTab;
        public static Dictionary<int, CommonFrag> FragmentByTab { get { return _fragmentByTab; } }
        public static Dictionary<int, CommonFrag> FragmentDictionary { get { return _fragmentDictionary; } }
        private static Dictionary<int, CommonFrag> _fragmentDictionary;
        public static CommonFrag GetFragmentById(int id = -1, object frag = null, int tabId = -1)
        {
            if (_fragmentByTab == null) { _fragmentByTab = new Dictionary<int, CommonFrag>(); }
            if (tabId != -1)
            {
                if (!FragmentByTab.Keys.Contains(tabId))
                {
                    if (frag != null)
                    {
                        FragmentByTab.Add(tabId, (CommonFrag)frag);
                    }
                }
                else
                {
                    return FragmentByTab[tabId];
                }
            }
            if (_fragmentDictionary == null) { _fragmentDictionary = new Dictionary<int, CommonFrag>(); }
            if ( !_fragmentDictionary.Keys.Contains(id))
            {
                if (id != -1)
                {
                    if (frag != null)
                    {

                        _fragmentDictionary.Add(id, (CommonFrag)frag);
                        return _fragmentDictionary[id];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (frag != null && id != -1)
                {
                    _fragmentDictionary.Remove(id);
                    _fragmentDictionary.Add(id, (CommonFrag)frag);
                }
                else if (frag == null && id != -1) {
                    return _fragmentDictionary[id];
                }
                else if(frag==null && id == -1){
                    return null;
                }
            }
            return null;
        }
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            
            System.Random random = new Random();
            this.Id = new System.Random().Next(999999999);
            base.OnCreate(savedInstanceState);
        }
        

        private async void HideWatchLabel()
        {
            await Task.Delay(4000);
            if (Wv.Url != "https://www.bitchute.com/")
                Wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }

        /// <summary>
        /// swaps the view for the test login layout
        /// </summary>
        /// <param name="v"></param>
        public virtual void SwapFragView(bool forceRemoveLoginLayout = false, bool forceWebViewLayout = false, bool forceShowLoginView = false)
        {
        }

        public void MakeADonationViaWebView()
        {
            if (!(this.TabId >= MainActivity.ViewPager.ChildCount))
            {
                CommonFrag.GetFragmentById(-1, null, this.TabId + 1).Wv.LoadUrlWithDelay(Https.URLs.MakeADonation);
                MainActivity.ViewPager.CurrentItem = this.TabId + 1;
            }
            else
            {
                CommonFrag.GetFragmentById(-1, null, 0).Wv.LoadUrlWithDelay(Https.URLs.MakeADonation);
                MainActivity.ViewPager.CurrentItem = 0;
            }
        }

        public void SetWebViewVis() { Wv.Visibility = ViewStates.Visible; }
        public async void ExpandVideoCards(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(5000);
            }
            Wv.LoadUrl(JavascriptCommands._jsBorderBoxAll);
            Wv.LoadUrl(JavascriptCommands._jsRemoveMaxWidthAll);
        }

        private async void ExpandFeaturedChannels(bool delayed)
        {
            if (delayed)
            {
                await Task.Delay(3000);
            }
            Wv.LoadUrl(JavascriptCommands._jsFeaturedRemoveMaxWidth);
            Wv.LoadUrl(JavascriptCommands._jsExpandFeatured);
        }

        private async void ExpandPage(bool delayed)
        {
            if (delayed) { await Task.Delay(3000); }
        }

        int _autoInt = 0;
        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings.LinkOverflowFixDelay);
            Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
            Wv.LoadUrl(JavascriptCommands._jsDisableTooltips);
            Wv.LoadUrl(JavascriptCommands._jsHideTooltips);
        }
        
        public void LoadCustomUrl(string url) { Wv.LoadUrl(url); }
        public async void HidePageTitle()
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

        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RootVideoAdapter_ItemClick(object sender, KeyValuePair<int, VideoCard> e)
        {
            var vdl = VideoDetailLoader.GetVideoDetailById(e.Value.FragmentId);
            var f = GetFragmentById(e.Value.FragmentId);
            var vdv = f.VideoDetailView;
            VideoDetailLoader.GetVideoDetailById(e.Value.FragmentId)
                .LoadVideoDetailFromVideoCard(vdv, e.Value, null, null, f.TabFragmentRelativeLayout, e.Value.FragmentId);
            //VideoDetailLoader.GetVideoDetailById(e.Value.FragmentId)
            //    .LoadVideoDetailFromVideoCard(GetFragmentById(e.Value.FragmentId).VideoDetailView, 
            //    e.Value, null, null, GetFragmentById(e.Value.FragmentId).TabFragmentRelativeLayout);
        }

        public void VideoView_OnClick(object sender, EventArgs e)
        {
            if (!MainPlaybackSticky.MediaControllerDictionary.ContainsKey(MainActivity.ViewPager.CurrentItem))
            {
                MainPlaybackSticky.MediaControllerDictionary.Add(MainActivity.ViewPager.CurrentItem, ExtStickyServ.InitializeMediaController(this.Context));
            }
            MainPlaybackSticky.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetAnchorView((View)FragmentContainerLayout.Parent);

            try
            {
                MainPlaybackSticky.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Enabled = true;
                MainPlaybackSticky.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Show(6000);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        private string _rootUrl;
        public string RootUrl
        {
            get { return _rootUrl; }
            set
            {
                _rootUrl = value;
                if (Wv != null)
                {
                    Wv.RootUrl = value;
                }
            }
        }

        public bool WvRl = true;
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
        
        public bool WvRling = false;
        /// <summary>
        /// this is to allow faster phones and connections the ability to Pop2Root
        /// used to be set without delay inside OnPageFinished but I don't think 
        /// that would work on faster phones
        /// </summary>
        public async void SetReload()
        {
            if (!WvRling)
            {
                WvRling = true;
                await Task.Delay(AppSettings.TabDelay);
                WvRl = true;
                WvRling = false;
            }
        }
        
        public async void LoadUrlWithDelay(string url, int delay)
        {
            await Task.Delay(delay); Wv.LoadUrl(url);
        }
        
        public void WebViewGoBack()
        {
            if (Wv.CanGoBack()) Wv.GoBack();
            // BitChute.Web.ViewClients.RunBaseCommands(Wv, 2000);
        }
    }
}