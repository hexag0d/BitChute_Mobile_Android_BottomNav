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
using static BitChute.Classes.ExtWebChromeClient;
using static BitChute.Classes.ExtNotifications;
using static StartServices.Servicesclass.ExtStickyService;


namespace BitChute.Fragments
{
    public class TheFragment4 : Android.Support.V4.App.Fragment
    {
        string _title;
        string _icon;
        public static int TNo = 4;

        public static ServiceWebView Wv;
        public static LinearLayout WvLayout;
        public static LinearLayout AppSettingsLayout;
        public static View _view;

        public static List<object> _settingsList = new List<object>();
        public static Spinner _tab4OverrideSpinner;
        public static Spinner _tab5OverrideSpinner;

        bool tabLoaded = false;

        public static string Tab5Title = "Settings";
        public static string RootUrl = "https://www.bitchute.com/settings/";

        private static RadioButton _fmoffrb;
        private static RadioButton _fmonrb;

        private static RadioButton _zcoffrb;
        private static RadioButton _zconrb;

        private static RadioButton _t3honrb;
        private static RadioButton _t3hoffrb;

        private static RadioButton _t1fonrb;
        private static RadioButton _t1foffrb;

        private static RadioButton _stoverrideoffrb;
        private static RadioButton _stoverrideonrb;

        private static RadioButton _notificationonrb;
        private static RadioButton _notificationoffrb;

        private static RadioButton _hidehorizontalnavbaronrb;
        private static RadioButton _hidehorizontalnavbaroffrb;

        private static RadioButton _hideverticalnavbaronrb;
        private static RadioButton _hideverticalnavbaroffrb;

        private static RadioButton _showdlbuttononpress;
        private static RadioButton _showdlbuttonalways;
        private static RadioButton _showdlbuttonnever;

        private static RadioButton _backgroundkeyfeed;
        private static RadioButton _backgroundkeyany;

        private static RadioButton _autoplayminimizedon;
        private static RadioButton _autoplayfeedonly;
        private static RadioButton _autoplayminimizedoff;
        
        public static TextView _versionTextView;
        public static List<string> _tabOverrideStringList = new List<string>();
        ArrayAdapter<string> _tab4SpinOverrideAdapter;
        ArrayAdapter<string> _tab5SpinOverrideAdapter;

        public static ExtStickyService StickyService = new ExtStickyService();
        private static CookieCollection cookies = new CookieCollection();

        public static TheFragment4 Fm4;

        public static TheFragment4 NewInstance(string title, string icon)
        {
            var fragment = new TheFragment4();
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
            _view = inflater.Inflate(Resource.Layout.TheFragmentLayout5, container, false);
            Wv = (ServiceWebView)_view.FindViewById<ServiceWebView>(Resource.Id.webView5);
            WvLayout = _view.FindViewById<LinearLayout>(Resource.Id.webViewLayout);
            AppSettingsLayout = _view.FindViewById<LinearLayout>(Resource.Id.appSettingsMainLayout);
            if (AppSettings.SettingsTabOverride)
            {
                RootUrl = AppSettings.GetTabOverrideUrlPref("tab5overridestring");
            }

                Wv.SetWebViewClient(new ExtWebViewClient());
                Wv.SetWebChromeClient(new ExtendedChromeClient(Main));
                Wv.Settings.JavaScriptEnabled = true;
                Wv.Settings.DisplayZoomControls = false;
                Wv.Settings.MediaPlaybackRequiresUserGesture = false;

                //_wv.Settings.AllowFileAccess = true;

                //_wv.Settings.AllowContentAccess = true;

               AppSettings.Prefs = Android.App.Application.Context.GetSharedPreferences("BitChute", FileCreationMode.Private);
               AppSettings.PrefEditor =AppSettings.Prefs.Edit();

                _zcoffrb = _view.FindViewById<RadioButton>(Resource.Id._zoomControlOffBtn);
                _zconrb = _view.FindViewById<RadioButton>(Resource.Id._zoomControlOnBtn);
                _fmoffrb = _view.FindViewById<RadioButton>(Resource.Id._zoomControlOffBtn);
                _fmonrb = _view.FindViewById<RadioButton>(Resource.Id._fanModeOnBtn);
                _t3hoffrb = _view.FindViewById<RadioButton>(Resource.Id._tab3HideOverrideOff);
                _t3honrb = _view.FindViewById<RadioButton>(Resource.Id._tab3HideOverrideOn);
                _t1foffrb = _view.FindViewById<RadioButton>(Resource.Id._tab1FeaturedCreatorsOff);
                _t1fonrb = _view.FindViewById<RadioButton>(Resource.Id._tab1FeaturedCreatorsOn);
                _stoverrideoffrb = _view.FindViewById<RadioButton>(Resource.Id._stOverrideOffRb);
                _stoverrideonrb = _view.FindViewById<RadioButton>(Resource.Id._stOverrideOnRb);
                _tab4OverrideSpinner = _view.FindViewById<Spinner>(Resource.Id.tab4OverrideSpinner);
                _tab5OverrideSpinner = _view.FindViewById<Spinner>(Resource.Id.tab5OverrideSpinner);
                _notificationonrb = _view.FindViewById<RadioButton>(Resource.Id._notificationsOnRb);
                _notificationoffrb = _view.FindViewById<RadioButton>(Resource.Id._notificationsOffRb);
                _hidehorizontalnavbaronrb = _view.FindViewById<RadioButton>(Resource.Id._hideNavBarHorizontalOn);
                _hidehorizontalnavbaroffrb = _view.FindViewById<RadioButton>(Resource.Id._hideNavBarHorizontalOff);
                _hideverticalnavbaronrb = _view.FindViewById<RadioButton>(Resource.Id.verticalNavbarRbOn);
                _hideverticalnavbaroffrb = _view.FindViewById<RadioButton>(Resource.Id.verticalNavbarRbOff);
                _showdlbuttononpress = _view.FindViewById<RadioButton>(Resource.Id.showDlFabOnPress);
                _showdlbuttonalways = _view.FindViewById<RadioButton>(Resource.Id.alwaysShowDlFab);
                _showdlbuttonnever = _view.FindViewById<RadioButton>(Resource.Id.dlFabOff);

                _versionTextView = _view.FindViewById<TextView>(Resource.Id.versionTextView);
                //_notificationWebView = _view.FindViewById<WebView>(Resource.Id._notificationWebView);

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
                tabLoaded = true;
            
            if (AppSettings.ZoomControl)
            {
                Wv.Settings.BuiltInZoomControls = true;
                Wv.Settings.DisplayZoomControls = false;
            }
            CustomSetTouchListener(AppState.Display.Horizontal);
            AppSettingsLayout.Visibility = ViewStates.Gone;
            LoadUrlWithDelay(RootUrl, 5000);
            return _view;
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

        public class ExtTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                MainActivity.CustomOnTouch();
                return false;
            }
        }

        public void SetWebViewVis()
        {
            Wv.Visibility = ViewStates.Visible;
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

            if (AppSettings.ZoomControl)
            {
                _zconrb.Checked = true;
            }
            else
            {
                _zcoffrb.Checked = true;
            }
            if (AppSettings.FanMode)
            {
                _fmonrb.Checked = true;
            }
            else
            {
                _fmoffrb.Checked = true;
            }
            if (AppSettings.Tab1FeaturedOn)
            {
                _t1fonrb.Checked = true;
            }
            else
            {
                _t1foffrb.Checked = true;
            }
            if (AppSettings.Tab3Hide)
            {
                _t3honrb.Checked = true;
            }
            else
            {
                _t3hoffrb.Checked = true;
            }
            if (AppSettings.FanMode)
            {
                _fmonrb.Checked = true;
            }
            else
            {
                _fmoffrb.Checked = true;
            }

            switch (AppSettings.Tab4OverridePreference)
            {
                case "Home":
                    _tab4OverrideSpinner.SetSelection(0);
                    break;
                case "Subs":
                    _tab4OverrideSpinner.SetSelection(1);
                    break;
                case "Feed":
                    _tab4OverrideSpinner.SetSelection(2);
                    break;
                case "Explore":
                    _tab4OverrideSpinner.SetSelection(3);
                    break;
                case "Settings":
                    _tab4OverrideSpinner.SetSelection(4);
                    break;
                case "MyChannel":
                    _tab4OverrideSpinner.SetSelection(5);
                    break;
                case "Downloader":
                    _tab4OverrideSpinner.SetSelection(6);
                    break;
            }

            if (AppSettings.SettingsTabOverride)
            {
                _stoverrideonrb.Checked = true;
            }
            else
            {
                _stoverrideoffrb.Checked = true;
            }

            switch (AppSettings.Tab5OverridePreference)
            {
                case "Home":
                    _tab5OverrideSpinner.SetSelection(0);
                    break;
                case "Subs":
                    _tab5OverrideSpinner.SetSelection(1);
                    break;
                case "Feed":
                    _tab5OverrideSpinner.SetSelection(2);
                    break;
                case "Explore":
                    _tab5OverrideSpinner.SetSelection(3);
                    break;
                case "Settings":
                    _tab5OverrideSpinner.SetSelection(4);
                    break;
                case "MyChannel":
                    _tab5OverrideSpinner.SetSelection(5);
                    break;
                case "Downloader":
                    _tab4OverrideSpinner.SetSelection(6);
                    break;
            }
            if (AppSettings.Notifying)
            {
                _notificationonrb.Checked = true;
            }
            else
            {
                _notificationoffrb.Checked = true;
            }
            if (AppSettings.HideHorizontalNavBar)
            {
                _hidehorizontalnavbaronrb.Checked = true;
            }
            else
            {
                _hidehorizontalnavbaroffrb.Checked = true;
            }
            if (AppSettings.HideVerticalNavBar)
            {
                _hideverticalnavbaronrb.Checked = true;
            }
            else
            {
                _hideverticalnavbaroffrb.Checked = true;
            }
            if (AppSettings.DlFabShowSetting == "onpress")
            {
                _showdlbuttononpress.Checked = true;
            }
            else if (AppSettings.DlFabShowSetting == "never")
            {
                _showdlbuttonnever.Checked = true;
            }
            else if (AppSettings.DlFabShowSetting == "always")
            {
                _showdlbuttonalways.Checked = true;
            }
            await Task.Delay(1000);
            AppNowCheckingBoxes = false;
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
                if (_notificationonrb.Checked)
                {
                    AppSettings.Notifying = true;
                }
                else
                {
                    AppSettings.Notifying = false;
                }
                if (_zconrb.Checked)
                {
                    AppSettings.ZoomControl = true;
                }
                else
                {
                    AppSettings.ZoomControl = false;
                }
                if (_t3honrb.Checked)
                {
                    AppSettings.Tab3Hide = true;
                }
                else
                {
                    AppSettings.Tab3Hide = false;
                }
                if (!_t1foffrb.Checked)
                {
                    AppSettings.Tab1FeaturedOn = true;
                }
                else
                {
                    AppSettings.Tab1FeaturedOn = false;
                }
                if (_fmonrb.Checked)
                {
                    AppSettings.FanMode = true;
                }
                else
                {
                    AppSettings.FanMode = false;
                }
                if (_stoverrideonrb.Checked)
                {
                    AppSettings.SettingsTabOverride = true;
                }
                else
                {
                    AppSettings.SettingsTabOverride = false;
                }
                if (_showdlbuttononpress.Checked)
                {
                    AppSettings.DlFabShowSetting = "onpress";
                }
                else if (_showdlbuttonalways.Checked)
                {
                    AppSettings.DlFabShowSetting = "always";
                }
                else if (_showdlbuttonnever.Checked)
                {
                    AppSettings.DlFabShowSetting = "never";
                }
                //write the android prefs
               AppSettings.PrefEditor.PutBoolean("zoomcontrol", AppSettings.ZoomControl);
               AppSettings.PrefEditor.PutBoolean("fanmode", AppSettings.FanMode);
               AppSettings.PrefEditor.PutBoolean("tab3hide", AppSettings.Tab3Hide);
               AppSettings.PrefEditor.PutBoolean("t1featured", AppSettings.Tab1FeaturedOn);
               AppSettings.PrefEditor.PutBoolean("settingstaboverride", AppSettings.SettingsTabOverride);
               AppSettings.PrefEditor.PutBoolean("notificationson", AppSettings.Notifying);
               AppSettings.PrefEditor.PutString("dlfabshowsetting", AppSettings.DlFabShowSetting);
               AppSettings.PrefEditor.Commit();

                _settingsList.Clear();
                //then add the settings app settings list
                _settingsList.Add(AppSettings.ZoomControl);
                _settingsList.Add(AppSettings.FanMode);
                _settingsList.Add(AppSettings.Tab3Hide);
                _settingsList.Add(AppSettings.Tab1FeaturedOn);
                _settingsList.Add(AppSettings.SettingsTabOverride);

                Main.OnSettingsChanged(_settingsList);
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
            if (_stoverrideonrb.Checked)
            {
                MainActivity. TabDetailChanger(4, AppSettings.Tab5OverridePreference);
            }
            else
            {
                MainActivity.TabDetailChanger(4, "Settings");
            }
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

        /// <summary>
        /// shows the app specific settings menu
        /// when user long presses "settings" tab
        /// </summary>
        public void ShowAppSettingsMenu()
        {
            if (_firstTimeLoad)
            {
                SetCheckedState();
                _firstTimeLoad = false;
            }

            if (WvLayout.Visibility == ViewStates.Visible)
            {
                WvLayout.Visibility = ViewStates.Gone;
                AppSettingsLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                AppSettingsLayout.Visibility = ViewStates.Gone;
                WvLayout.Visibility = ViewStates.Visible;
            }
        }

        //public class ExtScrollListener : Java.Lang.Object, View.IOnScrollChangeListener
        //{
        //    public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        //    {
        //        MainActivity.CustomOnTouch();
        //    }
        //}

        public static void WebViewGoBack()
        {
            if (Wv.CanGoBack())
                Wv.GoBack();
        }

        public static int mysteryInt = 0;
        static bool _wvRl = true;

        public void Pop2Root()
        {
            mysteryInt++;

            if (mysteryInt == 6)
            {
                //what... you didn't think I was doing this for free, did you? ;]
                Wv.LoadUrl(@"https://www.soundcloud.com/vybemasterz/");
            }
            else
            {
                if (_wvRl)
                {
                    try
                    {
                        Wv.Reload();
                        _wvRl = false;
                    }
                    catch
                    { }
                }
                else
                {
                    Wv.LoadUrl(RootUrl);
                }
            }
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
                mysteryInt = 0;
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
                if (_fmonrb.Checked)
                {
                    AppSettings.FanMode = true;
                }
                else
                {
                    MainActivity.TabDetailChanger(3, "MyChannel");
                    AppSettings.FanMode = false;
                }
                var prefEditor =AppSettings.Prefs.Edit();
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

        public void LoadCustomUrl(string url)
        {
            Wv.LoadUrl(url);
        }
        
        private class ExtWebViewClient : WebViewClient
        {
            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                if (request.Url.ToString().Contains("pest."))
                {
                    WebResourceResponse w = new WebResourceResponse("text/css", "UTF-8", null);
                    return w;
                }
                return base.ShouldInterceptRequest(view, request);
            }

            public override void OnPageFinished(WebView view, string url)
            {
                WebViewHelpers.DelayedScrollToTop(TNo);
                if (AppSettings.SettingsTabOverride)
                {
                    //if (AppSettings._tab5OverridePreference != "Settings")
                    //{
                    ////    _wv.LoadUrl(JavascriptCommands._jsHideBanner);
                    ////    _wv.LoadUrl(JavascriptCommands._jsHideBuff);
                    //}
                    if (AppSettings.Tab5OverridePreference == "Feed" && AppSettings.SettingsTabOverride)
                    {
                        Wv.LoadUrl(JavascriptCommands._jsHideCarousel);

                        if (Wv.Url == "https://www.bitchute.com/")
                        {
                            //TheFragment4.SelectSubscribedTab(2000);
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
                catch
                {
                }
                HideLinkOverflow();
                //AdBlock.RemoveDiscusIFrame(TNo);
                base.OnPageFinished(view, url);
            }
        }
    }
}
