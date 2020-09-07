using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Classes;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static BitChute.Classes.AppSettings;
using static BitChute.Classes.ExtWebChromeClient;
using static BitChute.Classes.ExtNotifications;
using static StartServices.Servicesclass.ExtStickyService;
using static BitChute.Classes.ViewHelpers;
using static BitChute.Classes.ViewHelpers.Tab4;
using static Android.Widget.TabHost;
using System.Linq;
using MediaCodecHelper;

namespace BitChute.Fragments
{
    public class Tab4Frag : Android.Support.V4.App.Fragment
    {
        string _title;
        string _icon;
        public static int TNo = 4;
        public static string Tab5Title = "Settings";
        public static string RootUrl = "https://www.bitchute.com/settings/";
        public static ExtStickyService StickyService = new ExtStickyService();
        private static CookieCollection cookies = new CookieCollection();
        public static Tab4Frag Fm4;
        public static ServiceWebView Wv;

        public static Tab4Frag NewInstance(string title, string icon)
        {
            var fragment = new Tab4Frag();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            Fm4 = fragment;
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
            _tabOverrideStringList.Add("WatchL8r");
            _tabOverrideStringList.Add("Playlists");
            _tabOverrideStringList.Add("Downloader");
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
            Fm4 = this;
            FragmentContainerLayout = inflater.Inflate(Resource.Layout.Tab4FragLayout, container, false);
            WebViewFragmentLayout = inflater.Inflate(Resource.Layout.Tab4WebView, container, false);
            InternalTabbedLayout = inflater.Inflate(Resource.Layout.InternalEncoderTabLayout, container, false);
            SettingsTabLayout = inflater.Inflate(Resource.Layout.SettingsTabLayout, container, false);
            TabFragmentLinearLayout = (LinearLayout)ViewHelpers.Tab4.FragmentContainerLayout.FindViewById<LinearLayout>(Resource.Id.tab4LinearLayout);
            ViewHelpers.Tab4.TabFragmentLinearLayout.RemoveAllViews();
            ViewHelpers.Tab4.TabFragmentLinearLayout.AddView(ViewHelpers.Tab4.InternalTabbedLayout);
            Wv = (ServiceWebView)WebViewFragmentLayout.FindViewById<ServiceWebView>(Resource.Id.webView4Swapable);
            if (AppSettings.SettingsTabOverride) { RootUrl = AppSettings.GetTabOverrideUrlPref("tab5overridestring"); }
            Wv.SetWebViewClient(new ExtWebViewClient());
            Wv.SetWebChromeClient(new ExtendedChromeClient(MainActivity.Main));
            Wv.Settings.JavaScriptEnabled = true;
            Wv.Settings.DisplayZoomControls = false;
            Wv.Settings.MediaPlaybackRequiresUserGesture = false;
            LoadUrlWithDelay(RootUrl, 3000);
            ViewHelpers.VideoEncoder.VideoEncoderLayout = inflater.Inflate(Resource.Layout.VideoEncodingLayout, container, false);
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
            EncoderFlexLinearLayout = InternalTabbedLayout.FindViewById<LinearLayout>(Resource.Id.encoderFlexLinearLayout);
            ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(ViewHelpers.Tab4.WebViewFragmentLayout);
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
            _tab4OverrideSpinner = SettingsTabLayout.FindViewById<Spinner>(Resource.Id.tab4OverrideSpinner);
            _tab5OverrideSpinner = SettingsTabLayout.FindViewById<Spinner>(Resource.Id.tab5OverrideSpinner);
            _notificationonrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._notificationsOnRb);
            _notificationoffrb = SettingsTabLayout.FindViewById<RadioButton>(Resource.Id._notificationsOffRb);
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
            //ViewHelpers.Tab4.JavascriptInjectionTextBox = _view.FindViewById<EditText>(Resource.Id.javascriptDebugInjectionTextBox);
            //Tab4.SearchOverrideSourceSpinner = _view.FindViewById<Spinner>(Resource.Id.searchOverrideSourceSpinner);
            //Tab4.SearchOverrideOffRb = _view.FindViewById<RadioButton>(Resource.Id.searchEngineOverrideOffRb);
            //Tab4.SearchOverrideOnRb = _view.FindViewById<RadioButton>(Resource.Id.searchEngineOverrideOnRb);
            //Tab4.SearchOverrideWithStaticBarRb = _view.FindViewById<RadioButton>(Resource.Id.searchEngineOverrideWithStaticBarRb);
            _tab4OverrideSpinner.ItemSelected += ExtSettingChanged;
            _tab4OverrideSpinner.ItemSelected += OnTab4OverrideSpinnerSelectionChanged;
            _tab5OverrideSpinner.ItemSelected += ExtSettingChanged;
            _tab5OverrideSpinner.ItemSelected += OnSettingsTabOverrideSpinnerSelectionChanged;
            _tab4SpinOverrideAdapter = new ArrayAdapter<string>(Android.App.Application.Context,
                    Android.Resource.Layout.SimpleListItem1, _tabOverrideStringList);
            _tab4OverrideSpinner.Adapter = _tab4SpinOverrideAdapter;
            _tab5SpinOverrideAdapter = new ArrayAdapter<string>(Android.App.Application.Context,
                    Android.Resource.Layout.SimpleListItem1, _tabOverrideStringList);
            _tab5OverrideSpinner.Adapter = _tab5SpinOverrideAdapter;
            _versionTextView.Text = AppState.AppVersion;

            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            return FragmentContainerLayout;
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
            SearchOverride.UI.SetupSearchOverrideControls(); // populate the search override controls
            if (AppSettings.ZoomControl) { _zconrb.Checked = true; }
            else { _zcoffrb.Checked = true; }
            if (AppSettings.FanMode) { _fmonrb.Checked = true; }
            else { _fmoffrb.Checked = true; }
            if (AppSettings.Tab1FeaturedOn) { _t1fonrb.Checked = true; }
            else { _t1foffrb.Checked = true; }
            if (AppSettings.Tab3Hide) { _t3honrb.Checked = true; }
            else { _t3hoffrb.Checked = true; }
            if (AppSettings.FanMode) { _fmonrb.Checked = true; }
            else { _fmoffrb.Checked = true; }
            switch (AppSettings.Tab4OverridePreference)
            {
                case "Home": _tab4OverrideSpinner.SetSelection(0); break;
                case "Subs": _tab4OverrideSpinner.SetSelection(1); break;
                case "Feed": _tab4OverrideSpinner.SetSelection(2); break;
                case "Explore": _tab4OverrideSpinner.SetSelection(3); break;
                case "Settings": _tab4OverrideSpinner.SetSelection(4); break;
                case "MyChannel": _tab4OverrideSpinner.SetSelection(5); break;
                case "Downloader": _tab4OverrideSpinner.SetSelection(6); break;
            }
            if (AppSettings.SettingsTabOverride) { _stoverrideonrb.Checked = true; }
            else { _stoverrideoffrb.Checked = true; }
            switch (AppSettings.Tab5OverridePreference)
            {
                case "Home": _tab5OverrideSpinner.SetSelection(0); break;
                case "Subs": _tab5OverrideSpinner.SetSelection(1); break;
                case "Feed": _tab5OverrideSpinner.SetSelection(2); break;
                case "Explore": _tab5OverrideSpinner.SetSelection(3); break;
                case "Settings": _tab5OverrideSpinner.SetSelection(4); break;
                case "MyChannel": _tab5OverrideSpinner.SetSelection(5); break;
                case "Downloader": _tab4OverrideSpinner.SetSelection(6); break;
            }
            if (AppSettings.Notifying) { _notificationonrb.Checked = true; }
            else { _notificationoffrb.Checked = true; }
            if (AppSettings.HideHorizontalNavBar) { _hidehorizontalnavbaronrb.Checked = true; }
            else { _hidehorizontalnavbaroffrb.Checked = true; }
            if (AppSettings.HideVerticalNavBar) { _hideverticalnavbaronrb.Checked = true; }
            else { _hideverticalnavbaroffrb.Checked = true; }
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
        public static void SwapSettingView()
        {
            
            if (!WebsiteSettingsVisible)
            {
                ViewHelpers.Tab4.TabFragmentLinearLayout.RemoveAllViews();
                ViewHelpers.Tab4.TabFragmentLinearLayout.AddView(InternalTabbedLayout);
            }
            else
            {
                ViewHelpers.Tab4.TabFragmentLinearLayout.RemoveAllViews();
                ViewHelpers.Tab4.TabFragmentLinearLayout.AddView(ViewHelpers.Tab4.SettingsTabLayout);
            }
            WebsiteSettingsVisible = !WebsiteSettingsVisible;
            if (_firstTimeLoad) { Fm4.SetCheckedState(); _firstTimeLoad = false; }
        }

        public static bool EncoderViewIsVisible = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public static void ShowEncoderView_OnClick(object sender, EventArgs e)
        {
            ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
            ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(ViewHelpers.VideoEncoder.VideoEncoderLayout);
            EncoderViewIsVisible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public static void ShowWebView_OnClick(object sender, EventArgs e)
        {
            ViewHelpers.Tab4.EncoderFlexLinearLayout.RemoveAllViews();
            ViewHelpers.Tab4.EncoderFlexLinearLayout.AddView(ViewHelpers.Tab4.WebViewFragmentLayout);
            EncoderViewIsVisible = false;
        }
        
        public static void StartEncodingButton_OnClick(object sender, EventArgs e)
        {
            FileBrowser.GetExternalPermissions();
            var codec = new MediaCodecHelper.FileToMp4(Android.App.Application.Context, 24, 1, null);
            Task.Run(() => {
                try
                {
                    codec.Progress += OnEncoderProgress;
                    string inputPath = ViewHelpers.VideoEncoder.EncoderSourceEditText.Text;
                    Android.Net.Uri tempuri = Android.Net.Uri.Parse(inputPath);
                    var fileName = tempuri.LastPathSegment.Split(@"/").ToList<string>().Last();
                    string outputPath = $"{MediaCodecHelper.FileToMp4.GetWorkingDirectory()}{fileName.Replace(".mp4", "")}_encoded{new System.Random().Next(0, 66666666)}.mp4";
                    codec.Start(inputPath, outputPath);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            });
        }

        public static void EncoderSourceButton_OnClick(object sender, EventArgs e)
        {
            FileBrowser.ShowFileChooser("encoder");
        }

        public static void OnEncoderProgress(VideoEncoding.EncoderEventArgs e)
        {
            if (!e.Finished)
            {
                var r = (int)((((decimal)e.EncodedData / (decimal)e.TotalData)) * 100);
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
                    if (!FileToMp4.AudioEncodingInProgress){ViewHelpers.VideoEncoder.EncoderOutputFileEditText.Text=e.FilePath;}
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

        public static async void LoadUrlWithDelay(string url, int delay)
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

        public void CustomLoadUrl(string url)
        {
            Wv.LoadUrl(url);
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
                if (_notificationonrb.Checked) { AppSettings.Notifying = true; }
                else { AppSettings.Notifying = false; }
                if (_zconrb.Checked) { AppSettings.ZoomControl = true; }
                else { AppSettings.ZoomControl = false; }
                if (_t3honrb.Checked) { AppSettings.Tab3Hide = true; }
                else { AppSettings.Tab3Hide = false; }
                if (!_t1foffrb.Checked) { AppSettings.Tab1FeaturedOn = true; }
                else { AppSettings.Tab1FeaturedOn = false; }
                if (_fmonrb.Checked) { AppSettings.FanMode = true; }
                else { AppSettings.FanMode = false; }
                if (_stoverrideonrb.Checked) { AppSettings.SettingsTabOverride = true; }
                else { AppSettings.SettingsTabOverride = false; }
                if (_showdlbuttononpress.Checked) { AppSettings.DlFabShowSetting = "onpress"; }
                else if (_showdlbuttonalways.Checked) { AppSettings.DlFabShowSetting = "always"; }
                else if (_showdlbuttonnever.Checked) { AppSettings.DlFabShowSetting = "never"; }
                AppSettings.PrefEditor.PutBoolean("zoomcontrol", AppSettings.ZoomControl);
                AppSettings.PrefEditor.PutBoolean("fanmode", AppSettings.FanMode);
                AppSettings.PrefEditor.PutBoolean("tab3hide", AppSettings.Tab3Hide);
                AppSettings.PrefEditor.PutBoolean("t1featured", AppSettings.Tab1FeaturedOn);
                AppSettings.PrefEditor.PutBoolean("settingstaboverride", AppSettings.SettingsTabOverride);
                AppSettings.PrefEditor.PutBoolean("notificationson", AppSettings.Notifying);
                AppSettings.PrefEditor.PutString("dlfabshowsetting", AppSettings.DlFabShowSetting);
                AppSettings.PrefEditor.Commit();
                _settingsList.Clear();
                _settingsList.Add(AppSettings.ZoomControl);
                _settingsList.Add(AppSettings.FanMode);
                _settingsList.Add(AppSettings.Tab3Hide);
                _settingsList.Add(AppSettings.Tab1FeaturedOn);
                _settingsList.Add(AppSettings.SettingsTabOverride);
                MainActivity.Main.OnSettingsChanged(_settingsList);
            }
        }

        private void OnFanModeRbCheckChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                if (_fmonrb.Checked)
                {
                    MainActivity.TabDetailChanger(3, AppSettings.Tab4OverridePreference);
                }
                else
                {
                    MainActivity.TabDetailChanger(3, "MyChannel");
                }
            }
        }

        private void OnTab4OverrideSpinnerSelectionChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                AppSettings.Tab4OverridePreference = _tab4OverrideSpinner.SelectedItem.ToString();
                if (_fmonrb.Checked)
                {
                    MainActivity.TabDetailChanger(3, AppSettings.Tab4OverridePreference);
                }
                else
                {
                }
                AppSettings.PrefEditor.PutString("tab4overridestring", AppSettings.Tab4OverridePreference);
                AppSettings.PrefEditor.Commit();
            }
        }

        private void OnSettingsRbCheckChanged(object sender, EventArgs e)
        {
            if (_stoverrideonrb.Checked){MainActivity.TabDetailChanger(4,Tab5OverridePreference);}
            else {  MainActivity.TabDetailChanger(4, "Settings"); }
        }

        private void OnSettingsTabOverrideSpinnerSelectionChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                AppSettings.Tab5OverridePreference = _tab5OverrideSpinner.SelectedItem.ToString();
                if (_fmonrb.Checked)
                {
                    MainActivity.TabDetailChanger(4, AppSettings.Tab5OverridePreference);
                }
                else
                {

                }
                AppSettings.PrefEditor.PutString("tab5overridestring", AppSettings.Tab5OverridePreference);
                AppSettings.PrefEditor.Commit();
            }
        }

        static bool _firstTimeLoad = true;

        public static void WebViewGoBack() {if (Wv.CanGoBack()) Wv.GoBack();}
        static bool _wvRl = true;
        public void Pop2Root()
        {
            if(_wvRl){try{Wv.Reload();_wvRl=false;}catch{}}
            else { Wv.LoadUrl(RootUrl); }
        }
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
                await Task.Delay(6);
                _wvRling = false;
            }
        }

        public void GetPendingIntent()
        {
            List<object> list = new List<object>();
            int zero = 0;
            list.Add(zero);
            var update = Android.App.PendingIntentFlags.UpdateCurrent;
            list.Add(update);
        }

        public void OnTab4OverrideChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                if (_fmonrb.Checked) {  AppSettings.FanMode = true; }
                else
                {
                    MainActivity.TabDetailChanger(3, "MyChannel");
                    AppSettings.FanMode = false;
                }
                var prefEditor = AppSettings.Prefs.Edit();
                AppSettings.PrefEditor.PutBoolean("fanmode", AppSettings.FanMode);
                AppSettings.PrefEditor.Commit();
            }
        }

        public void OnTab5OverrideChanged(object sender, EventArgs e)
        {
            if (!AppNowCheckingBoxes)
            {
                if (_stoverrideonrb.Checked)
                {
                    AppSettings.SettingsTabOverride = true;
                }
                else
                {
                    MainActivity.TabDetailChanger(4, "Settings");
                    AppSettings.SettingsTabOverride = false;
                }

                AppSettings.PrefEditor.PutBoolean("settingstaboverride", AppSettings.SettingsTabOverride);
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
        public static async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings.LinkOverflowFixDelay);
            Wv.LoadUrl(JavascriptCommands._jsLinkFixer);
        }

        private static async void HideWatchLabel()
        {
            await Task.Delay(1000);
            Wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }

        public void LoadCustomUrl(string url){ Wv.LoadUrl(url); }

        public static async void SelectSubscribedTab(int delay)
        {
            await Task.Delay(delay);
            Wv.LoadUrl(JavascriptCommands._jsSelectSubscribed);
        }

        public static bool UserRequestedVideoPreProcessing = false;

        private class ExtWebViewClient : WebViewClient
        {
            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().StartsWith("https://www.bitchute.com/myupload"))
                {
                    //WebResourceResponse w = new WebResourceResponse("text/css", "UTF-8", null);
                    //return w;
                }
                if (AppSettings.SearchFeatureOverride && !SearchOverride.SearchOverrideInProg)
                {
                    if (!request.Url.ToString().Contains(@"https://www.bitchute.com/search?q="))
                    { 
                        return base.ShouldInterceptRequest(view, request);
                    }
                    if (request.Url.ToString().Contains(@"https://www.bitchute.com/search?q="))
                    {
                        SearchOverride.SearchOverrideInProg = true;
                        MainActivity.Main.RunOnUiThread(() => { Wv.StopLoading(); });
                        var ro = SearchOverride.ReRouteSearch(request.Url.ToString());
                        SearchOverride.UI.WvSearchOverride(view, ro);
                        WebResourceResponse w = new WebResourceResponse("text/css", "UTF-8", null);
                        return w;
                    }
                }
                return base.ShouldInterceptRequest(view, request);
            }
            
            public override void OnPageFinished(WebView view, string url)
            {
                WebViewHelpers.DelayedScrollToTop(TNo);
                if (AppSettings.SettingsTabOverride)
                {
                    //if (AppSettings._tab5OverridePreference != "Settings") // these js commands are disabled because settings tab uses the banner
                    //{
                    ////    _wv.LoadUrl(JavascriptCommands._jsHideBanner);
                    ////    _wv.LoadUrl(JavascriptCommands._jsHideBuff);
                    //}
                    if (AppSettings.Tab5OverridePreference == "Feed" && AppSettings.SettingsTabOverride)
                    {
                        Wv.LoadUrl(JavascriptCommands._jsHideCarousel);

                        if (Wv.Url == "https://www.bitchute.com/")
                        {
                            Tab4Frag.SelectSubscribedTab(2000);
                        }
                    }
                }

                //if (AppState.Display._horizontal)
                //{
                //    _wv.LoadUrl(JavascriptCommands._jsHideTitle);
                //    _wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                //}

                Wv.LoadUrl(JavascriptCommands._jsLinkFixer);

                SetReload();

                try
                {
                    ExtWebInterface.CookieHeader = Android.Webkit.CookieManager.Instance.GetCookie("https://www.bitchute.com/");

                    Https.CookieString = ExtWebInterface.CookieHeader.ToString();
                    var cookiePairs = ExtWebInterface.CookieHeader.Split('&');

                    Https.CookieString = "";

                    foreach (var cookiePair in cookiePairs)
                    {
                        var cookiePieces = cookiePair.Split('=');
                        if (cookiePieces[0].Contains(":"))
                            cookiePieces[0] = cookiePieces[0].Substring(0, cookiePieces[0].IndexOf(":"));
                        cookies.Add(new Cookie
                        {
                            Name = cookiePieces[0],
                            Value = cookiePieces[1]
                        });
                    }

                    foreach (Cookie c in cookies)
                    {
                        c.Domain = "https://bitchute.com/";
                        if (Https.CookieString == "")
                        {
                            Https.CookieString = c.ToString();
                        }
                        else
                        {
                            Https.CookieString += c.ToString();
                        }
                    }
                }
                catch{  }
                HideLinkOverflow();
                base.OnPageFinished(view, url);
            }
        }
    }
}
