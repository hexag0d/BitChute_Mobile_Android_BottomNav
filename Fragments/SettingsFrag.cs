using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using BitChute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static BitChute.AppSettings;
using static BitChute.ExtWebChromeClient;
using static BitChute.ExtNotifications;
using static BitChute.Services.MainPlaybackSticky;
using static BitChute.ViewHelpers;
using static BitChute.ViewHelpers.VideoEncoder;
using static BitChute.ViewHelpers.Tab4;

using System.Linq;
using MediaCodecHelper;
using static BitChute.Web.ViewClients;
using BitChute.App;

namespace BitChute.Fragments
{
    public class SettingsFrag : CommonFrag
    {
        string _title;
        string _icon;
        //public static ServiceWebView Wv;
        public static object Wvc;

        public static SettingsFrag NewInstance(string title, string icon, string tabOverridePref = null, int tabId = -1)
        {
            var fragment = new SettingsFrag();
            fragment.Arguments = new Bundle();
            var tabFragPackage = new TabStates.TabFragPackage();
            if (tabOverridePref != null && AppSettings.Tab4OverrideEnabled) { tabFragPackage = new TabStates.TabFragPackage(tabOverridePref, true); }
            else { tabFragPackage = new TabStates.TabFragPackage(TabStates.TabFragPackage.FragmentType.Settings); }
            title = tabFragPackage.Title;
            icon = tabFragPackage.Icon.ToString();
            Wvc = tabFragPackage.WebViewClient;
            fragment.RootUrl = tabFragPackage.RootUrl;
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            fragment.Arguments.PutInt("tabId", tabId);
            return fragment;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            _tabOverrideStringList.Add("Home");
            _tabOverrideStringList.Add("Subs");
            _tabOverrideStringList.Add("Feed");
            _tabOverrideStringList.Add("Explore");
            _tabOverrideStringList.Add("Settings");
            _tabOverrideStringList.Add("MyChannel");
            _tabOverrideStringList.Add("WatchLater");
            _tabOverrideStringList.Add("Playlists");
            _tabOverrideStringList.Add("Downloader");
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
            //MainActivity.Main.FinalizeStartUp();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.Id = new System.Random().Next(99999999);
            try
            {
                if (FragmentContainerLayout == null)
                    FragmentContainerLayout = inflater.Inflate(Resource.Layout.Tab4FragLayout, container, false);
                if (WebViewFragmentLayout == null)
                    WebViewFragmentLayout = inflater.Inflate(Resource.Layout.Tab4WebView, container, false);
                if (InternalTabbedLayout == null)
                    InternalTabbedLayout = inflater.Inflate(Resource.Layout.InternalEncoderTabLayout, container, false);
                if (SettingsTabLayout == null)
                    SettingsTabLayout = inflater.Inflate(Resource.Layout.SettingsTabLayout, container, false);
                if (InternalTabbedLayout != null)
                {
                    InternalTabbedLayout.FindViewById<Button>(Resource.Id.loginViewSwapButton)
                        .Click += LoginButtonShow_OnClick;
                    InternalTabbedLayout.FindViewById<Button>(Resource.Id.settingsViewSwapButton)
                        .Click += SettingsButton_OnClick;
                }
                TabFragmentLinearLayout = (LinearLayout)FragmentContainerLayout.FindViewById<LinearLayout>(Resource.Id.tab4LinearLayout);
                TabFragmentLinearLayout.RemoveAllViews();
                TabFragmentLinearLayout.AddView(ViewHelpers.Tab4.InternalTabbedLayout);
                Wv = (ServiceWebView)WebViewFragmentLayout.FindViewById<ServiceWebView>(Resource.Id.webView4Swapable);
                Wv.RootUrl = RootUrl;
                BitChute.Web.ViewClients.SetWebViewClientFromObject(Wv, Wvc);
                Wv.SetWebChromeClient(new ExtendedChromeClient(MainActivity.Instance));
                Wv.Settings.JavaScriptEnabled = true;
                Wv.Settings.DisplayZoomControls = false;
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;
                
                if (VideoEncoderLayout == null)
                    ViewHelpers.VideoEncoder.VideoEncoderLayout = inflater.Inflate(Resource.Layout.VideoEncodingLayout, container, false);
                ViewHelpers.VideoEncoder.EncoderBitRateEditText = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<EditText>(Resource.Id.videoEncoderBitRateEditText);
                ViewHelpers.VideoEncoder.EncoderWidthEditText = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<EditText>(Resource.Id.videoEncoderWidthEditText);
                ViewHelpers.VideoEncoder.EncoderHeightEditText = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<EditText>(Resource.Id.videoEncoderHeightEditText);
                EncoderFpsEditText = VideoEncoderLayout.FindViewById<EditText>(Resource.Id.videoEncoderFpsEditText);
                ViewHelpers.VideoEncoder.StartEncodingButton = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<Button>(Resource.Id.encodingStartButton);
                ViewHelpers.VideoEncoder.EncodingStatusTextView = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<TextView>(Resource.Id.encoderStatusTextView);
                ViewHelpers.VideoEncoder.AudioEncodingStatusTextView = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<TextView>(Resource.Id.audioEncoderStatusTextView);
                ViewHelpers.VideoEncoder.StartEncodingButton.Click += StartEncodingButton_OnClick;
                ViewHelpers.VideoEncoder.EncoderOutputFileEditText = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<EditText>(Resource.Id.encoderOutputFileEditText);
                ViewHelpers.VideoEncoder.EncodeProgressBar = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<ProgressBar>(Resource.Id.encoderProgressBar);
                ViewHelpers.VideoEncoder.AudioEncodeProgressBar = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<ProgressBar>(Resource.Id.audioEncoderProgressBar);
                ViewHelpers.VideoEncoder.EncoderSourceEditText = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<EditText>(Resource.Id.encoderSourceFileEditText);
                ViewHelpers.VideoEncoder.PickSourceButton = ViewHelpers.VideoEncoder.VideoEncoderLayout.FindViewById<Button>(Resource.Id.encodingPickAVideoButton);
                ViewHelpers.VideoEncoder.PickSourceButton.Click += EncoderSourceButton_OnClick;
                Tab4.ShowEncoderViewButton = InternalTabbedLayout.FindViewById<Button>(Resource.Id.encoderViewSwapButton);
                Tab4.ShowWebViewButton = InternalTabbedLayout.FindViewById<Button>(Resource.Id.webViewSwapButton);
                Tab4.ShowEncoderViewButton.Click += ShowEncoderView_OnClick;
                Tab4.ShowWebViewButton.Click += ShowWebView_OnClick;
                ViewHelpers.Tab4.SettingsTabLayout.FindViewById<Button>(Resource.Id.goButton).Click += GoButton_OnClick;
                JavascriptInjectionTextBox = ViewHelpers.Tab4.SettingsTabLayout.FindViewById<EditText>(Resource.Id.javascriptDebugInjectionTextBox);
            }
            catch (Exception ex)
            {

            }
            try
            {
                EncoderFlexLinearLayout = InternalTabbedLayout.FindViewById<LinearLayout>(Resource.Id.encoderFlexLinearLayout);
                ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(WebViewFragmentLayout);
            }
            catch (Exception ex) { }
            try {
                AppSettings.Prefs = Android.App.Application.Context.GetSharedPreferences("BitChute", FileCreationMode.Private);
                AppSettings.PrefEditor = AppSettings.Prefs.Edit();
                _zcoffrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._zoomControlOffBtn);
                _zconrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._zoomControlOnBtn);
                _fmoffrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._zoomControlOffBtn);
                _fmonrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._fanModeOnBtn);
                _t3hoffrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._tab3HideOverrideOff);
                _t3honrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._tab3HideOverrideOn);
                _t1foffrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._tab1FeaturedCreatorsOff);
                _t1fonrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._tab1FeaturedCreatorsOn);
                _stoverrideoffrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._stOverrideOffRb);
                _stoverrideonrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._stOverrideOnRb);
                _tab3OverrideSpinner = SettingsTabLayout.FindViewById<Spinner>(Resource.Id.tab4OverrideSpinner);
                _tab4OverrideSpinner = SettingsTabLayout.FindViewById<Spinner>(Resource.Id.tab5OverrideSpinner);
                _notificationonrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._notificationsOnRb);
                _notificationoffrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._notificationsOffRb);
            }
            catch (Exception ex) { }
            try {
                _hidehorizontalnavbaronrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._hideNavBarHorizontalOn);
                _hidehorizontalnavbaroffrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._hideNavBarHorizontalOff);
                _hideverticalnavbaronrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.verticalNavbarRbOn);
                _hideverticalnavbaroffrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.verticalNavbarRbOff);
                _showdlbuttononpress = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.showDlFabOnPress);
                _showdlbuttonalways = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.alwaysShowDlFab);
                _showdlbuttonnever = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.dlFabOff);
                _versionTextView = SettingsTabLayout.FindViewById<TextView>(Resource.Id.versionTextView);
                _zcoffrb.CheckedChange += ExtSettingChanged;
                _fmonrb.CheckedChange += ExtSettingChanged;
                _fmonrb.CheckedChange += OnFanModeRbCheckChanged;
                _t3hoffrb.CheckedChange += ExtSettingChanged;
                _t1foffrb.CheckedChange += ExtSettingChanged;
                _stoverrideonrb.CheckedChange += ExtSettingChanged;
                _stoverrideonrb.CheckedChange += OnSettingsRbCheckChanged;
                _notificationonrb.CheckedChange += ExtSettingChanged;

                _hidehorizontalnavbaronrb.CheckedChange += OnHorizontalNavbarRbChecked;
                _hideverticalnavbaronrb.CheckedChange += OnVerticalNavbarRbChecked;
                _showdlbuttonalways.CheckedChange += ExtSettingChanged;
                _showdlbuttonnever.CheckedChange += ExtSettingChanged;
                _showdlbuttononpress.CheckedChange += ExtSettingChanged;

                //Tab4.SearchOverrideSourceSpinner = _view.FindViewById<Spinner>(Resource.Id.searchOverrideSourceSpinner);
                //Tab4.SearchOverrideOffRb = _view.FindViewById<RadioButton>(Resource.Id.searchEngineOverrideOffRb);
                //Tab4.SearchOverrideOnRb = _view.FindViewById<RadioButton>(Resource.Id.searchEngineOverrideOnRb);
                //Tab4.SearchOverrideWithStaticBarRb = _view.FindViewById<RadioButton>(Resource.Id.searchEngineOverrideWithStaticBarRb);
                _tab3OverrideSpinner.ItemSelected += ExtSettingChanged;
                _tab3OverrideSpinner.ItemSelected += OnTab3OverrideSpinnerSelectionChanged;
                _tab4OverrideSpinner.ItemSelected += ExtSettingChanged;
                _tab4OverrideSpinner.ItemSelected += OnTab4OverrideSpinnerSelectionChanged;
                _tab3SpinOverrideAdapter = new ArrayAdapter<string>(Android.App.Application.Context,
                        Android.Resource.Layout.SimpleListItem1, _tabOverrideStringList);
                _tab3OverrideSpinner.Adapter = _tab3SpinOverrideAdapter;
                _tab4SpinOverrideAdapter = new ArrayAdapter<string>(Android.App.Application.Context,
                        Android.Resource.Layout.SimpleListItem1, _tabOverrideStringList);
                _tab4OverrideSpinner.Adapter = _tab4SpinOverrideAdapter;
                _versionTextView.Text = AppState.AppVersion;

                if (AppSettings.ZoomControl)
                {
                    Wv.Settings.BuiltInZoomControls = true;
                    Wv.Settings.DisplayZoomControls = false;
                }
                if (AppSettings.Debug.LoadWebViewsOnStart) { BitChute.Web.ViewClients.LoadInitialUrls(); }
                GetFragmentById(this.Id, this, TabId);


            }
            catch (Exception ex) { }
            try
            {
                SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.autoPlayOnMinimize).Click += ExtSettingChanged;
                SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.autoPlayFeedOnly).Click += ExtSettingChanged;
                SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.autoPlayOnMinimizeOff).Click += ExtSettingChanged;
                SettingsTabLayout.FindViewById<Button>(Resource.Id.clearLoginCredentialsButton).Click += ClearLoginCredentialsButton_OnClick;
            }
            catch
            {

            }
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
            catch
            {

            }
            return FragmentContainerLayout;
        }

        public void GoButton_OnClick(object sender, EventArgs e)
        {
            this.Wv.LoadUrl(ViewHelpers.Tab4.JavascriptInjectionTextBox.Text);
            ShowWebView();
        }

        public void ClearLoginCredentialsButton_OnClick(object sender, EventArgs e)
        {
            BitChute.Web.ExtWebInterface.ClearLoginCredentials();
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


        static bool LoginVisible;
        /// <summary>
        /// swaps the view for the test login layout
        /// </summary>
        /// <param name="v"></param>
        public override void SwapFragView(bool forceRemoveLoginLayout = false, bool forceWebViewLayout = false, bool forceShowLoginView = false)
        {
            if (forceRemoveLoginLayout)
            {
                ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
                ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(WebViewFragmentLayout);
                LoginVisible = false;
                return;
            }
            if (forceShowLoginView)
            {
                ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
                ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(LoginLayout);
                LoginVisible = true;
                return;
            }
            else
            {
                if (forceWebViewLayout)
                {
                    ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
                    ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(WebViewFragmentLayout);
                }
                else if (!LoginVisible)
                {
                    ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
                    ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(LoginLayout);
                }
                else
                {
                    ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
                    ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(WebViewFragmentLayout);
                }
            }
        }

        public static void NotificationsOnRb_OnClick(object sender, EventArgs e)
        {

        }

        public void ContinueWithoutLoginButton_OnClick(object sender, EventArgs e)
        {
            if (MainActivity.ViewPager.CurrentItem == 4)
            {
                ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
                ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(WebViewFragmentLayout);
            }
        }

        public void LoginButton_OnClick(object sender, EventArgs e)
        {
            BitChute.Web.Login.MakeLoginRequest(
                UserNameTextBox.Text,
                PasswordTextBox.Text);
            UserNameTextBox.Text = "";
            PasswordTextBox.Text = "";
        }

        public void LoginButtonShow_OnClick(object sender, EventArgs e)
        {
            ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
            ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(LoginLayout);
        }

        public static void SettingsButton_OnClick(object sender, EventArgs e)
        {
            if (_firstTimeLoad) { MainActivity.Fm4.SetCheckedState(); _firstTimeLoad = false; }
            ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
            ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(ViewHelpers.Tab4.SettingsTabLayout);

        }

        /// <summary>
        /// set to true when the application is initially 
        /// setting the checked state of the settings radiobuttons.
        /// 
        /// set to false after a one second delay to allow the user to change settings
        /// manually,
        /// 
        /// used to prevent loops in the checkchanged event mechanism. 
        /// 
        /// this whole system should soon be replaced with databinding
        /// to the android application preferences.
        /// </summary>
        public bool AppNowCheckingBoxes = false;
        public async void SetCheckedState()
        {
            AppNowCheckingBoxes = true;
            //SearchOverride.UI.SetupSearchOverrideControls(); // populate the search override controls
            if (AppSettings.ZoomControl) { _zconrb.Checked = true; }
            else { _zcoffrb.Checked = true; }
            if (AppSettings.Tab3OverrideEnabled) { _fmonrb.Checked = true; }
            else { _fmoffrb.Checked = true; }
            if (AppSettings.Tab0FeaturedOn) { _t1fonrb.Checked = true; }
            else { _t1foffrb.Checked = true; }
            if (AppSettings.Tab2Hide) { _t3honrb.Checked = true; }
            else { _t3hoffrb.Checked = true; }
            if (AppSettings.Tab3OverrideEnabled) { _fmonrb.Checked = true; }
            else { _fmoffrb.Checked = true; }
            switch (AppSettings.Tab3OverridePreference)
            {
                case "Home": _tab3OverrideSpinner.SetSelection(0); break;
                case "Subs": _tab3OverrideSpinner.SetSelection(1); break;
                case "Feed": _tab3OverrideSpinner.SetSelection(2); break;
                case "Explore": _tab3OverrideSpinner.SetSelection(3); break;
                case "Settings": _tab3OverrideSpinner.SetSelection(4); break;
                case "MyChannel": _tab3OverrideSpinner.SetSelection(5); break;
                case "Downloader": _tab3OverrideSpinner.SetSelection(6); break;
            }
            if (AppSettings.Tab4OverrideEnabled) { _stoverrideonrb.Checked = true; }
            else { _stoverrideoffrb.Checked = true; }
            switch (AppSettings.Tab4OverridePreference)
            {
                case "Home": _tab4OverrideSpinner.SetSelection(0); break;
                case "Subs": _tab4OverrideSpinner.SetSelection(1); break;
                case "Feed": _tab4OverrideSpinner.SetSelection(2); break;
                case "Explore": _tab4OverrideSpinner.SetSelection(3); break;
                case "Settings": _tab4OverrideSpinner.SetSelection(4); break;
                case "MyChannel": _tab4OverrideSpinner.SetSelection(5); break;
                case "Downloader": _tab3OverrideSpinner.SetSelection(6); break;
            }
            if (AppSettings.Notifying) { _notificationonrb.Checked = true; }
            else { _notificationoffrb.Checked = true; }
            if (AppSettings.HideHorizontalNavBar) { _hidehorizontalnavbaronrb.Checked = true; }
            else { _hidehorizontalnavbaroffrb.Checked = true; }
            if (AppSettings.HideVerticalNavBar) { _hideverticalnavbaronrb.Checked = true; }
            else { _hideverticalnavbaroffrb.Checked = true; }
            if (AppSettings.AutoPlayOnMinimized == "any") {
                SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.autoPlayOnMinimize).Checked = true; ;
            }
            else if (AppSettings.AutoPlayOnMinimized == "feed") {
                SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.autoPlayFeedOnly).Checked = true; ;
            }
            else if (AppSettings.AutoPlayOnMinimized == "off") {
                SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.autoPlayOnMinimizeOff).Checked = true; ;
            }
            try // something weird going on here.. dl fab setting selector is throwing resource error @TODO fix it
            {
                if (AppSettings.DlFabShowSetting == "onpress") { _showdlbuttononpress.Checked = true; }
                else if (AppSettings.DlFabShowSetting == "never") { _showdlbuttonnever.Checked = true; }
                else if (AppSettings.DlFabShowSetting == "always") { _showdlbuttonalways.Checked = true; }
            }
            catch { }
            await Task.Delay(1000);
            AppNowCheckingBoxes = false;
        }

        public static bool WebsiteSettingsVisible = true;

        /// <summary>
        /// swaps the view between android settings and site settings layouts
        /// </summary>
        /// <param name="v">nullable, the view to swap for</param>
        public void SwapSettingView()
        {
            if (WebsiteSettingsVisible)
            {
                if (_firstTimeLoad) { MainActivity.Fm4.SetCheckedState(); _firstTimeLoad = false; }
                ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
                ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(ViewHelpers.Tab4.SettingsTabLayout);

            }
            else if (!WebsiteSettingsVisible)
            {
                if (_firstTimeLoad) { MainActivity.Fm4.SetCheckedState(); _firstTimeLoad = false; }
                ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
                ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(WebViewFragmentLayout);
            }
            WebsiteSettingsVisible = !WebsiteSettingsVisible;
        }

        public static bool EncoderViewIsVisible = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public static void ShowEncoderView_OnClick(object sender, EventArgs e)
        {
            //VideoEncoding.EncoderDialog.ShowDialogQuestion(Android.App.Application.Context);
            ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
            ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(ViewHelpers.VideoEncoder.VideoEncoderLayout);
            EncoderViewIsVisible = true;
        }

        public void ShowWebView()
        {
            ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
            ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(WebViewFragmentLayout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void ShowWebView_OnClick(object sender, EventArgs e)
        {
            ShowWebView();
        }

        public void OnUserDeniedPreProcessing()
        {
            ShowWebViewButton.PerformClick();
            Wv.LoadUrl(JavascriptCommands.GetInjectable(JavascriptCommands.EnterUploadView));
        }
        
        public static void StartEncodingButton_OnClick(object sender, EventArgs e)
        {
            Task.Run(() => {
                try
                {
                    if (!FileBrowser.GetExternalPermissions()) { return; }
                    VideoEncoder.EncodeProgressBar.Progress = 0;
                    var codec = new MediaCodecHelper
                    .FileToMp4(Android.App.Application.Context, Convert.ToInt32(EncoderFpsEditText.Text), 1, 
                    Convert.ToInt32(VideoEncoder.EncoderWidthEditText.Text),
                    Convert.ToInt32(VideoEncoder.EncoderHeightEditText.Text),
                    Convert.ToInt32(VideoEncoder.EncoderBitRateEditText.Text /*int*/) * 1000 /* = kbps */);
                    codec.Progress += OnEncoderProgress;
                    string inputPath = "";
                    string outputPath = "";
                    var fileName = "";
                    Android.Net.Uri tempuri = null;
                    if (MediaCodecHelper.FileToMp4.InputUriToEncode == null)
                    {
                        inputPath = ViewHelpers.VideoEncoder.EncoderSourceEditText.Text;
                        tempuri = Android.Net.Uri.Parse(inputPath);
                        fileName = tempuri.LastPathSegment.Split(@"/").ToList<string>().Last();
                        //I had to trim this because bitchute will actually completely drop files that have long names into a 404 khole
                        outputPath = $"{MediaCodecHelper.FileToMp4.GetWorkingDirectory()}{fileName.Replace(".mp4", "")}_cp_{new System.Random().Next(0, 777)}.mp4";
                    }
                    else
                    {
                        tempuri = MediaCodecHelper.FileToMp4.InputUriToEncode;
                        fileName = MediaCodecHelper.FileToMp4.InputUriToEncode.LastPathSegment.Replace(":","");
                        outputPath = $"{MediaCodecHelper.FileToMp4.GetWorkingDirectory()}{fileName?.Replace(".mp4", "")}_cp_{new System.Random().Next(0, 777)}.mp4";
                    }
                    codec.Start(MediaCodecHelper.FileToMp4.InputUriToEncode, outputPath, inputPath);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            });
        }

        public static void EncoderSourceButton_OnClick(object sender, EventArgs e)
        {
            FileBrowser.ShowFileChooser("encoder");
        }

        public static void OnEncoderProgress(VideoEncoding.MinEventArgs.EncoderMinArgs e)
        {
            if (!VideoEncoding.MinEventArgs.Finished)
            {
                int r = 0;
                if (VideoEncoding.MinEventArgs.TotalData!=0)
                r = (int)((((decimal)VideoEncoding.MinEventArgs.EncodedData / (decimal)VideoEncoding.MinEventArgs.TotalData)) * 100);
                if (r > 100) { r = 100; }
                ViewHelpers.Main.UiHandler.Post(() =>
                {
                    ViewHelpers.VideoEncoder.EncodingStatusTextView.Text = $"Encoding video:{r}% done";
                    ViewHelpers.VideoEncoder.EncodeProgressBar.Progress = r;
                });
            }
            else
            {
                ViewHelpers.Main.UiHandler.Post(() => {
                    ViewHelpers.VideoEncoder.EncodeProgressBar.Progress = 100;
                    ViewHelpers.VideoEncoder.EncodingStatusTextView.Text = "Video finished encoding";
                    if (!FileToMp4.AudioEncodingInProgress){ViewHelpers.VideoEncoder.EncoderOutputFileEditText.Text= VideoEncoding.MinEventArgs.FilePath;}
                }); 
            }
        }

        public static void OnMuxerProgress(VideoEncoding.MuxerEventArgs e)
        {
            if (!e.Finished)
            {
                var r = (int)((((decimal)e.Time) / (decimal)e.Length) * 100);
                if (r <= 1)
                {
                    ViewHelpers.VideoEncoder.AudioEncodeProgressBar.Max = 100;
                    ViewHelpers.VideoEncoder.AudioEncodeProgressBar.Min = 0;
                }

                ViewHelpers.VideoEncoder.AudioEncodeProgressBar.Progress = r;
                ViewHelpers.VideoEncoder.AudioEncodingStatusTextView.Text = $"Muxing audio:{r}% done";
            }
            else
            {
                ViewHelpers.Main.UiHandler.Post(() =>
                {
                    ViewHelpers.VideoEncoder.AudioEncodeProgressBar.Max = 100;
                    ViewHelpers.VideoEncoder.AudioEncodeProgressBar.Min = 0;
                    ViewHelpers.VideoEncoder.AudioEncodeProgressBar.Progress = 100;
                    ViewHelpers.VideoEncoder.AudioEncodingStatusTextView.Text = $"Audio finished processing";
                    if (!MediaCodecHelper.FileToMp4.VideoEncodingInProgress) { VideoEncoder.EncoderOutputFileEditText.Text = e.FilePath; }
                });
            }
        }

        public async void LoadUrlWithDelay(string url, int delay)
        {
            await Task.Delay(delay);
            Wv.LoadUrl(url);
        }

        private static bool _systemCheckingRb = false;

        private static void OnVerticalNavbarRbChecked(object sender, EventArgs e)
        {
            if (_hideverticalnavbaronrb.Checked)
            {
                AppSettings.HideVerticalNavBar = true;
            }
            else
            {
                AppSettings.HideVerticalNavBar = false;
            }
            AppSettings.PrefEditor.PutBoolean("hideverticalnavbar", AppSettings.HideVerticalNavBar);
            AppSettings.PrefEditor.Commit();
            _systemCheckingRb = false;
        }

        private static void OnHorizontalNavbarRbChecked(object sender, EventArgs e)
        {
            if (!_systemCheckingRb)
            {
                if (_hidehorizontalnavbaronrb.Checked)
                {
                    AppSettings.HideHorizontalNavBar = true;
                    AppSettings.PrefEditor.PutBoolean("hidehorizontalnavbar", true);
                }
                else
                {
                    AppSettings.HideHorizontalNavBar = false;
                    AppSettings.PrefEditor.PutBoolean("hidehorizontalnavbar", false);
                }
                AppSettings.PrefEditor.Commit();
            }
        }

        public void OnSettingsChanged(List<object> settings)
        {
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

        bool _previousNotificationSetting;

        /// <summary>
        /// called when the .Checked state of radio buttons in the app settings fragment is changed
        /// sets the settings when this event occurs and calls a method to notify all fragments via mainactivity.
        ///writes the values to android preferences aswell using the api
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExtSettingChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                if (_notificationonrb.Checked) {
                    AppSettings.Notifying = true;
                    if (_previousNotificationSetting != AppSettings.Notifying)
                    {
                        BitChute.Services.MainPlaybackSticky.StartNotificationLoop(3000, null);
                    }
                }
                else { AppSettings.Notifying = false; }
                _previousNotificationSetting = AppSettings.Notifying;
                if (_zconrb.Checked) { AppSettings.ZoomControl = true; }
                else { AppSettings.ZoomControl = false; }
                if (_t3honrb.Checked) { AppSettings.Tab2Hide = true; }
                else { AppSettings.Tab2Hide = false; }
                if (!_t1foffrb.Checked) { AppSettings.Tab0FeaturedOn = true; }
                else { AppSettings.Tab0FeaturedOn = false; }
                if (_fmonrb.Checked) { AppSettings.Tab3OverrideEnabled = true; }
                else { AppSettings.Tab3OverrideEnabled = false; }
                if (_stoverrideonrb.Checked) { AppSettings.Tab4OverrideEnabled = true; }
                else { AppSettings.Tab4OverrideEnabled = false; }
                if (_showdlbuttononpress.Checked) { AppSettings.DlFabShowSetting = "onpress"; }
                else if (_showdlbuttonalways.Checked) { AppSettings.DlFabShowSetting = "always"; }
                else if (_showdlbuttonnever.Checked) { AppSettings.DlFabShowSetting = "never"; }
                if (SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.autoPlayOnMinimize).Checked)
                {
                    AppSettings.AutoPlayOnMinimized = "any";
                }
                else if (SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.autoPlayFeedOnly).Checked)
                {
                    AppSettings.AutoPlayOnMinimized = "feed";
                }
                else if (SettingsTabLayout.FindViewById<RadioButton>(Resource.Id.autoPlayOnMinimizeOff).Checked)
                {
                    AppSettings.AutoPlayOnMinimized = "off";
                }
                AppSettings.PrefEditor.PutString("autoplayonminimized", AppSettings.AutoPlayOnMinimized);
                AppSettings.PrefEditor.PutBoolean("zoomcontrol", AppSettings.ZoomControl);
                AppSettings.PrefEditor.PutBoolean("tab3overrideenabled", AppSettings.Tab3OverrideEnabled);
                AppSettings.PrefEditor.PutBoolean("tab3hide", AppSettings.Tab2Hide);
                AppSettings.PrefEditor.PutBoolean("t1featured", AppSettings.Tab0FeaturedOn);
                AppSettings.PrefEditor.PutBoolean("tab4overrideenabled", AppSettings.Tab4OverrideEnabled);
                AppSettings.PrefEditor.PutBoolean("notificationson", AppSettings.Notifying);
                AppSettings.PrefEditor.PutString("dlfabshowsetting", AppSettings.DlFabShowSetting);
                AppSettings.PrefEditor.Commit();
                _settingsList.Clear();
                _settingsList.Add(AppSettings.ZoomControl);
                _settingsList.Add(AppSettings.Tab3OverrideEnabled);
                _settingsList.Add(AppSettings.Tab2Hide);
                _settingsList.Add(AppSettings.Tab0FeaturedOn);
                _settingsList.Add(AppSettings.Tab4OverrideEnabled);
                MainActivity.Instance.OnSettingsChanged(_settingsList);
            }
        }

        private void OnFanModeRbCheckChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                if (_fmonrb.Checked)
                {
                    MainActivity.TabDetailChanger(3, AppSettings.Tab3OverridePreference);
                }
                else
                {
                    MainActivity.TabDetailChanger(3, "MyChannel");
                }
            }
        }

        private void OnTab3OverrideSpinnerSelectionChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                AppSettings.Tab3OverridePreference = _tab3OverrideSpinner.SelectedItem.ToString();
                if (_fmonrb.Checked)
                {
                    MainActivity.TabDetailChanger(3, AppSettings.Tab3OverridePreference);
                }
                else
                {
                }
                AppSettings.PrefEditor.PutString("tab3overridestring", AppSettings.Tab3OverridePreference);
                AppSettings.PrefEditor.Commit();
            }
        }

        private void OnSettingsRbCheckChanged(object sender, EventArgs e)
        {
            if (_stoverrideonrb.Checked){MainActivity.TabDetailChanger(4,Tab4OverridePreference);}
            else {  MainActivity.TabDetailChanger(4, "Settings"); }
        }

        private void OnTab4OverrideSpinnerSelectionChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                AppSettings.Tab4OverridePreference = _tab4OverrideSpinner.SelectedItem.ToString();
                if (_fmonrb.Checked)
                {
                    MainActivity.TabDetailChanger(4, AppSettings.Tab4OverridePreference);
                }
                else
                {

                }
                AppSettings.PrefEditor.PutString("tab4overridestring", AppSettings.Tab4OverridePreference);
                AppSettings.PrefEditor.Commit();
            }
        }

        static bool _firstTimeLoad = true;
        

        public void GetPendingIntent()
        {
            List<object> list = new List<object>();
            int zero = 0;
            list.Add(zero);
            var update = Android.App.PendingIntentFlags.UpdateCurrent;
            list.Add(update);
        }

        public void OnTab3OverrideChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                if (_fmonrb.Checked) {  AppSettings.Tab3OverrideEnabled = true; }
                else
                {
                    MainActivity.TabDetailChanger(3, "MyChannel");
                    AppSettings.Tab3OverrideEnabled = false;
                }
                var prefEditor = AppSettings.Prefs.Edit();
                AppSettings.PrefEditor.PutBoolean("tab3overrideenabled", AppSettings.Tab3OverrideEnabled);
                AppSettings.PrefEditor.Commit();
            }
        }

        public void OnTab4OverrideChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                if (_stoverrideonrb.Checked)
                {
                    AppSettings.Tab4OverrideEnabled = true;
                }
                else
                {
                    MainActivity.TabDetailChanger(4, "Settings");
                    AppSettings.Tab4OverrideEnabled = false;
                }

                AppSettings.PrefEditor.PutBoolean("tab4overrideenabled", AppSettings.Tab4OverrideEnabled);
                AppSettings.PrefEditor.Commit();
            }
        }

        public List<CustomNotification> GetNotifications()
        {
            return ExtNotifications.CustomNoteList;
        }

        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings.LinkOverflowFixDelay);
            Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
        }

        private async void HideWatchLabel()
        {
            await Task.Delay(1000);
            Wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }

        public static bool UserRequestedVideoPreProcessing = false;
    }
}
