using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BottomNavigationViewPager.Classes;
using Java.Interop;
using Java.Net;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static BottomNavigationViewPager.Classes.ExtendedWebChromeClient;
using static BottomNavigationViewPager.Classes.ExtNotifications;
using static StartServices.Servicesclass.ExtStickyService;

namespace BottomNavigationViewPager.Fragments
{
    public class TheFragment4 : Android.Support.V4.App.Fragment
    {
        string _title;
        string _icon;

        public static ServiceWebView _wv;
        public static LinearLayout _wvLayout;
        public static LinearLayout _appSettingsLayout;
        public static View _view;

        public static List<object> _settingsList = new List<object>();
        public static Spinner _tab4OverrideSpinner;
        public static Spinner _tab5OverrideSpinner;

        bool tabLoaded = false;

        public static string _tab5Title = "Settings";
        public static string _url = "https://www.bitchute.com/settings/";

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

        public static Android.App.PendingIntentFlags _flags = new Android.App.PendingIntentFlags();
        public static int _count = 0;

        public static TheFragment4.ExtWebInterface _extWebInterface = new ExtWebInterface();
        public static TextView _versionTextView;
        public static bool _notificationHttpRequestInProgress = false;

        public static List<string> _tabOverrideStringList = new List<string>();
        ArrayAdapter<string> _tab4SpinOverrideAdapter;
        ArrayAdapter<string> _tab5SpinOverrideAdapter;

        public static ExtStickyService _stickyService = new ExtStickyService();
        private static CookieCollection cookies = new CookieCollection();

        public static TheFragment4 _fm5;

        public static TheFragment4 NewInstance(string title, string icon)
        {
            var fragment = new TheFragment4();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            _fm5 = fragment;
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
            _fm5 = this;
            _view = inflater.Inflate(Resource.Layout.TheFragmentLayout5, container, false);
            _wv = (ServiceWebView)_view.FindViewById<ServiceWebView>(Resource.Id.webView5);
            _wvLayout = _view.FindViewById<LinearLayout>(Resource.Id.webViewLayout);
            _appSettingsLayout = _view.FindViewById<LinearLayout>(Resource.Id.appSettingsMainLayout);
            if (AppSettings._settingsTabOverride)
            {
                _url = AppSettings.GetTabOverrideUrlPref("tab5overridestring");
            }
            if (!tabLoaded)
            {
                _wv.SetWebViewClient(new ExtWebViewClient());
                _wv.SetWebChromeClient(new ExtendedChromeClient(_main));
                _wv.Settings.JavaScriptEnabled = true;
                _wv.Settings.DisplayZoomControls = false;
                _wv.Settings.MediaPlaybackRequiresUserGesture = false;

                //_wv.Settings.AllowFileAccess = true;

                //_wv.Settings.AllowContentAccess = true;

                _prefs = Android.App.Application.Context.GetSharedPreferences("BitChute", FileCreationMode.Private);
                _prefEditor = _prefs.Edit();

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

                var _ctx = Android.App.Application.Context;

                _tab4OverrideSpinner.ItemSelected += ExtSettingChanged;
                _tab4OverrideSpinner.ItemSelected += OnTab4OverrideSpinnerSelectionChanged;
                _tab5OverrideSpinner.ItemSelected += ExtSettingChanged;
                _tab5OverrideSpinner.ItemSelected += OnSettingsTabOverrideSpinnerSelectionChanged;
                _tab4SpinOverrideAdapter = new ArrayAdapter<string>(_ctx,
                        Android.Resource.Layout.SimpleListItem1, _tabOverrideStringList);
                _tab4OverrideSpinner.Adapter = _tab4SpinOverrideAdapter;
                _tab5SpinOverrideAdapter = new ArrayAdapter<string>(_ctx,
                        Android.Resource.Layout.SimpleListItem1, _tabOverrideStringList);
                _tab5OverrideSpinner.Adapter = _tab5SpinOverrideAdapter;
                _versionTextView.Text = AppState._appVersion;
                tabLoaded = true;
            }
            if (AppSettings._zoomControl)
            {
                _wv.Settings.BuiltInZoomControls = true;
                _wv.Settings.DisplayZoomControls = false;
            }
            CustomSetTouchListener(AppState.Display._horizontal);
            _appSettingsLayout.Visibility = ViewStates.Gone;
            LoadUrlWithDelay(_url, 5000);
            return _view;
        }

        public void CustomSetTouchListener(bool landscape)
        {
            if (landscape)
            {
                _wv.SetOnTouchListener(new ExtTouchListener());
            }
            else
            {
                _wv.SetOnTouchListener(null);
            }
        }

        public static async void LoadUrlWithDelay(string url, int delay)
        {
            await Task.Delay(delay);
            _wv.LoadUrl(url);
        }

        private static bool _systemCheckingRb = false;

        private static void OnVerticalNavbarRbChecked(object sender, EventArgs e)
        {
            if (_hideverticalnavbaronrb.Checked)
            {
                AppSettings._hideVerticalNavbar = true;
            }
            else
            {
                AppSettings._hideVerticalNavbar = false;
            }
            _prefEditor.PutBoolean("hideverticalnavbar", AppSettings._hideVerticalNavbar);
            _prefEditor.Commit();
            _systemCheckingRb = false;
        }

        private static void OnHorizontalNavbarRbChecked(object sender, EventArgs e)
        {
            if (!_systemCheckingRb)
            {
                if (_hidehorizontalnavbaronrb.Checked)
                {
                    AppSettings._hideHorizontalNavbar = true;
                    _prefEditor.PutBoolean("hidehorizontalnavbar", true);
                }
                else
                {
                    AppSettings._hideHorizontalNavbar = false;
                    _prefEditor.PutBoolean("hidehorizontalnavbar", false);
                }
                _prefEditor.Commit();
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
            _wv.Visibility = ViewStates.Visible;
        }

        public void CustomLoadUrl(string url)
        {
            _wv.LoadUrl(url);
        }

        public void OnSettingsChanged(List<object> settings)
        {
            if (AppSettings._zoomControl)
            {
                _wv.Settings.BuiltInZoomControls = true;
                _wv.Settings.DisplayZoomControls = false;
            }
            else
            {
                _wv.Settings.BuiltInZoomControls = false;
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
        public bool _isNowCheckingBoxes = false;

        public async void SetCheckedState()
        {
            _isNowCheckingBoxes = true;

            if (AppSettings._zoomControl)
            {
                _zconrb.Checked = true;
            }
            else
            {
                _zcoffrb.Checked = true;
            }
            if (AppSettings._fanMode)
            {
                _fmonrb.Checked = true;
            }
            else
            {
                _fmoffrb.Checked = true;
            }
            if (AppSettings._tab1FeaturedOn)
            {
                _t1fonrb.Checked = true;
            }
            else
            {
                _t1foffrb.Checked = true;
            }
            if (AppSettings._tab3Hide)
            {
                _t3honrb.Checked = true;
            }
            else
            {
                _t3hoffrb.Checked = true;
            }
            if (AppSettings._fanMode)
            {
                _fmonrb.Checked = true;
            }
            else
            {
                _fmoffrb.Checked = true;
            }

            switch (AppSettings._tab4OverridePreference)
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

            if (AppSettings._settingsTabOverride)
            {
                _stoverrideonrb.Checked = true;
            }
            else
            {
                _stoverrideoffrb.Checked = true;
            }

            switch (AppSettings._tab5OverridePreference)
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
            if (AppSettings._notifying)
            {
                _notificationonrb.Checked = true;
            }
            else
            {
                _notificationoffrb.Checked = true;
            }
            if (AppSettings._hideHorizontalNavbar)
            {
                _hidehorizontalnavbaronrb.Checked = true;
            }
            else
            {
                _hidehorizontalnavbaroffrb.Checked = true;
            }
            if (AppSettings._hideVerticalNavbar)
            {
                _hideverticalnavbaronrb.Checked = true;
            }
            else
            {
                _hideverticalnavbaroffrb.Checked = true;
            }

            await Task.Delay(1000);
            _isNowCheckingBoxes = false;
        }

        public static Android.Content.ISharedPreferences _prefs;
        public static Android.Content.ISharedPreferencesEditor _prefEditor;

        /// <summary>
        /// called when the .Checked state of radio buttons in the app settings fragment is changed
        /// sets the settings when this event occurs and calls a method to notify all fragments via mainactivity.
        ///writes the values to android preferences aswell using the api
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExtSettingChanged(object sender, EventArgs e)
        {
            if (!_isNowCheckingBoxes)
            {
                if (_notificationonrb.Checked)
                {
                    AppSettings._notifying = true;
                }
                else
                {
                    AppSettings._notifying = false;
                }
                if (_zconrb.Checked)
                {
                    AppSettings._zoomControl = true;
                }
                else
                {
                    AppSettings._zoomControl = false;
                }
                if (_t3honrb.Checked)
                {
                    AppSettings._tab3Hide = true;
                }
                else
                {
                    AppSettings._tab3Hide = false;
                }
                if (!_t1foffrb.Checked)
                {
                    AppSettings._tab1FeaturedOn = true;
                }
                else
                {
                    AppSettings._tab1FeaturedOn = false;
                }
                if (_fmonrb.Checked)
                {
                    AppSettings._fanMode = true;
                }
                else
                {
                    AppSettings._fanMode = false;
                }
                if (_stoverrideonrb.Checked)
                {
                    AppSettings._settingsTabOverride = true;
                }
                else
                {
                    AppSettings._settingsTabOverride = false;
                }
                //write the android prefs
                _prefEditor.PutBoolean("zoomcontrol", AppSettings._zoomControl);
                _prefEditor.PutBoolean("fanmode", AppSettings._fanMode);
                _prefEditor.PutBoolean("tab3hide", AppSettings._tab3Hide);
                _prefEditor.PutBoolean("t1featured", AppSettings._tab1FeaturedOn);
                _prefEditor.PutBoolean("settingstaboverride", AppSettings._settingsTabOverride);
                _prefEditor.PutBoolean("notificationson", AppSettings._notifying);
                _prefEditor.Commit();

                _settingsList.Clear();
                //then add the settings app settings list
                _settingsList.Add(AppSettings._zoomControl);
                _settingsList.Add(AppSettings._fanMode);
                _settingsList.Add(AppSettings._tab3Hide);
                _settingsList.Add(AppSettings._tab1FeaturedOn);
                _settingsList.Add(AppSettings._settingsTabOverride);

                _main.OnSettingsChanged(_settingsList);
            }
        }

        private void OnFanModeRbCheckChanged(object sender, EventArgs e)
        {
            if (!_isNowCheckingBoxes)
            {
                if (_fmonrb.Checked)
                {
                    MainActivity.TabDetailChanger(3, AppSettings._tab4OverridePreference);
                }
                else
                {
                    MainActivity.TabDetailChanger(3, "MyChannel");
                }
            }
        }

        private void OnTab4OverrideSpinnerSelectionChanged(object sender, EventArgs e)
        {
            if (!_isNowCheckingBoxes)
            {
                AppSettings._tab4OverridePreference = _tab4OverrideSpinner.SelectedItem.ToString();
                if (_fmonrb.Checked)
                {
                    MainActivity.TabDetailChanger(3, AppSettings._tab4OverridePreference);
                }
                else
                {
                }
                _prefEditor.PutString("tab4overridestring", AppSettings._tab4OverridePreference);
                _prefEditor.Commit();
            }
        }

        private void OnSettingsRbCheckChanged(object sender, EventArgs e)
        {
            if (_stoverrideonrb.Checked)
            {
                MainActivity. TabDetailChanger(4, AppSettings._tab5OverridePreference);
            }
            else
            {
                MainActivity.TabDetailChanger(4, "Settings");
            }
        }

        private void OnSettingsTabOverrideSpinnerSelectionChanged(object sender, EventArgs e)
        {
            if (!_isNowCheckingBoxes)
            {
                AppSettings._tab5OverridePreference = _tab5OverrideSpinner.SelectedItem.ToString();
                if (_fmonrb.Checked)
                {
                    MainActivity.TabDetailChanger(4, AppSettings._tab5OverridePreference);
                }
                else
                {

                }
                _prefEditor.PutString("tab5overridestring", AppSettings._tab5OverridePreference);
                _prefEditor.Commit();
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

            if (_wvLayout.Visibility == ViewStates.Visible)
            {
                _wvLayout.Visibility = ViewStates.Gone;
                _appSettingsLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                _appSettingsLayout.Visibility = ViewStates.Gone;
                _wvLayout.Visibility = ViewStates.Visible;
            }
        }

        //public class ExtScrollListener : Java.Lang.Object, View.IOnScrollChangeListener
        //{
        //    public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        //    {
        //        MainActivity.CustomOnTouch();
        //    }
        //}

        public void WebViewGoBack()
        {
            if (_wv.CanGoBack())
                _wv.GoBack();
        }

        public static int mysteryInt = 0;
        static bool _wvRl = true;

        public void Pop2Root()
        {
            mysteryInt++;

            if (mysteryInt == 6)
            {
                _wv.LoadUrl(@"https://www.soundcloud.com/vybemasterz/");
            }
            else
            {
                if (_wvRl)
                {
                    try
                    {
                        _wv.Reload();
                        _wvRl = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    _wv.LoadUrl(_url);
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
                await Task.Delay(AppSettings._tabDelay);
                _wvRl = true;
                await Task.Delay(1666);
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
            if (!_isNowCheckingBoxes)
            {
                if (_fmonrb.Checked)
                {
                    AppSettings._fanMode = true;
                }
                else
                {
                    MainActivity.TabDetailChanger(3, "MyChannel");
                    AppSettings._fanMode = false;
                }
                var prefEditor = _prefs.Edit();
                _prefEditor.PutBoolean("fanmode", AppSettings._fanMode);
                _prefEditor.Commit();
            }
        }

        public void OnTab5OverrideChanged(object sender, EventArgs e)
        {
            if (!_isNowCheckingBoxes)
            {
                if (_stoverrideonrb.Checked)
                {
                    AppSettings._settingsTabOverride = true;
                }
                else
                {
                    MainActivity.TabDetailChanger(4, "Settings");
                    AppSettings._settingsTabOverride = false;
                }
                var prefEditor = _prefs.Edit();
                prefEditor.PutBoolean("settingstaboverride", AppSettings._settingsTabOverride);
                prefEditor.Commit();
            }
        }

        public static string _rawNoteText = "";

        public static string _cookieString { get; set; }
        internal static ExtNotifications ExtNotifications { get => _extNotifications; set => _extNotifications = value; }

        public List<CustomNotification> GetNotifications()
        {
            return ExtNotifications._customNoteList;
        }

        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(AppSettings._linkOverflowFixDelay);
            _wv.LoadUrl(JavascriptCommands._jsLinkFixer);
        }

        private static async void HideWatchLabel()
        {
            await Task.Delay(1000);
            _wv.LoadUrl(JavascriptCommands._jsHideTabInner);
        }

        public static string CookieHeader;
        private static ExtNotifications _extNotifications = new ExtNotifications();

        private static List<CustomNotification> _sentNotificationList = new List<CustomNotification>();
        private static NotificationManagerCompat _notificationManager;

        public async void SendNotifications(List<CustomNotification> notificationList)
        {
            await Task.Run(() =>
            {
                try
                {
                    var _ctx = Android.App.Application.Context;

                    if (_notificationManager == null)
                    {
                        _notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(_ctx);
                    }

                    if (notificationList.Count == 0)
                    {
                        return;
                    }
                    int notePos = 0;

                    // When the user clicks the notification, MainActivity will start up.

                    foreach (var note in notificationList)
                    {
                        var resultIntent = new Intent(_ctx, typeof(MainActivity));
                        var valuesForActivity = new Bundle();
                        valuesForActivity.PutInt(MainActivity.COUNT_KEY, _count);
                        valuesForActivity.PutString("URL", note._noteLink);
                        resultIntent.PutExtras(valuesForActivity);
                        var resultPendingIntent = PendingIntent.GetActivity(_ctx, MainActivity.NOTIFICATION_ID, resultIntent, PendingIntentFlags.UpdateCurrent);
                        resultIntent.AddFlags(ActivityFlags.SingleTop);

                        var alarmAttributes = new Android.Media.AudioAttributes.Builder()
                                .SetContentType(Android.Media.AudioContentType.Sonification)
                                .SetUsage(Android.Media.AudioUsageKind.Notification).Build();

                        if (!_sentNotificationList.Contains(note) && notePos == 0)
                        {
                            // Build the notification:
                            var builder = new Android.Support.V4.App.NotificationCompat.Builder(_ctx, MainActivity.CHANNEL_ID + 1)
                                    .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                                    .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                                    .SetContentTitle(note._noteText) // Set the title
                                    .SetNumber(1) // Display the count in the Content Info
                                                  //.SetLargeIcon(_notificationBMP) // This is the icon to display
                                    .SetSmallIcon(Resource.Drawable.bitchute_notification2)
                                    .SetContentText(note._noteType)
                                    .SetPriority(NotificationCompat.PriorityMin);

                            MainActivity.NOTIFICATION_ID++;

                            // publish the notification:
                            //var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(_ctx);
                            _notificationManager.Notify(MainActivity.NOTIFICATION_ID, builder.Build());
                            _sentNotificationList.Add(note);
                            notePos++;
                        }
                        else if (!_sentNotificationList.Contains(note))
                        {
                            var builder = new Android.Support.V4.App.NotificationCompat.Builder(_ctx, MainActivity.CHANNEL_ID)
                                .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                                .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                                .SetContentTitle(note._noteText) // Set the title
                                .SetNumber(1) // Display the count in the Content Info
                                              //.SetLargeIcon(_notificationBMP) // This is the icon to display
                                .SetSmallIcon(Resource.Drawable.bitchute_notification2)
                                .SetContentText(note._noteType)
                                .SetPriority(NotificationCompat.PriorityLow);

                            MainActivity.NOTIFICATION_ID++;


                            // publish the notification:
                            //var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(_ctx);
                            _notificationManager.Notify(MainActivity.NOTIFICATION_ID, builder.Build());
                            _sentNotificationList.Add(note);
                            notePos++;
                        }

                        ExtStickyService._notificationsHaveBeenSent = true;
                    }
                }
                catch
                {
                }
            });
        }

        public void LoadCustomUrl(string url)
        {
            _wv.LoadUrl(url);
        }

        public class ExtWebInterface
        {
            public static string _notificationRawText;
            public static CookieContainer _cookieCon = new CookieContainer();
            public static string _htmlCode = "";

            /// <summary>
            /// returns html source of url requested
            /// </summary>
            /// <param name="url">use the string you want to get html source from</param>
            /// <returns></returns>
            public async Task<string> GetNotificationText(string url)
            {
                await Task.Run(() =>
                {
                    _htmlCode = "";
                    HttpClientHandler handler = new HttpClientHandler() { UseCookies = false };

                    if (!_notificationHttpRequestInProgress)
                    {
                        try
                        {
                            Uri _notificationURI = new Uri("https://bitchute.com/notifications/");
                            var _cookieHeader = _cookieCon.GetCookieHeader(_notificationURI);

                            using (HttpClient _client = new HttpClient(handler))
                            {
                                _client.DefaultRequestHeaders.Add("Cookie", TheFragment4.CookieHeader);
                                _notificationHttpRequestInProgress = true;

                                var getRequest = _client.GetAsync("https://bitchute.com/notifications/").Result;
                                var resultContent = getRequest.Content.ReadAsStringAsync().Result;
                                _htmlCode = resultContent;
                                _notificationHttpRequestInProgress = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                });

                return _htmlCode;
            }
        }

        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView view, string url)
            {
                if (AppSettings._settingsTabOverride)
                {
                    //if (AppSettings._tab5OverridePreference != "Settings")
                    //{
                    ////    _wv.LoadUrl(JavascriptCommands._jsHideBanner);
                    ////    _wv.LoadUrl(JavascriptCommands._jsHideBuff);
                    //}
                    if (AppSettings._tab5OverridePreference == "Feed" && AppSettings._settingsTabOverride)
                    {
                        _wv.LoadUrl(JavascriptCommands._jsHideCarousel);

                        if (_wv.Url == "https://www.bitchute.com/")
                        {
                            TheFragment2.SelectSubscribedTab(2000);
                        }
                    }
                }

                //if (AppState.Display._horizontal)
                //{
                //    _wv.LoadUrl(JavascriptCommands._jsHideTitle);
                //    _wv.LoadUrl(JavascriptCommands._jsHidePageBar);
                //}

                _wv.LoadUrl(JavascriptCommands._jsLinkFixer);

                SetReload();

                try
                {
                    TheFragment4.CookieHeader = Android.Webkit.CookieManager.Instance.GetCookie("https://www.bitchute.com/");

                    Https.CookieString = TheFragment4.CookieHeader.ToString();
                    var cookiePairs = TheFragment4.CookieHeader.Split('&');

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
            }
        }
    }
}
