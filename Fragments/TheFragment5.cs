﻿using Android.App;
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
using static BottomNavigationViewPager.Classes.ExtNotifications;

namespace BottomNavigationViewPager.Fragments
{
        //[Android.Runtime.Register("onShowFileChooser", 
        //"Landroid/webkit/WebView;Landroid/webkit/ValueCallback;Landroid/webkit/WebChromeClient$FileChooserParams;)Z",
        //"GetOnShowFileChooser_Landroid_webkit_WebView_Landroid_webkit_ValueCallback_Landroid_webkit_WebChromeClient_FileChooserParams_Handler")]
    public class TheFragment5 : Android.Support.V4.App.Fragment
    {
        string _title;
        string _icon;

        public static WebView _wv;
        public static LinearLayout _wvLayout;
        public static LinearLayout _appSettingsLayout;
        public static View _view;

        public static List<object> _settingsList = new List<object>();
        public static Spinner _tab4OverrideSpinner;
        public static Spinner _tab5OverrideSpinner;


        public static bool _zoomControl { get; set; }
        public static bool _tab1FeaturedOn { get; set; }
        public static bool _fanMode { get; set; }
        public static bool _tab3Hide { get; set; }
        public static bool _settingsTabOverride { get; set; }

        public static string _tab4OverridePreference { get; set; }
        public static string _tab5OverridePreference { get; set; }

        bool tabLoaded = false;

        public static string _tab5Title = "Settings";
        public static string _url = "https://www.bitchute.com/settings/";

        public static RadioButton _fmoffrb;
        public static RadioButton _fmonrb;

        public static RadioButton _zcoffrb;
        public static RadioButton _zconrb;

        public static RadioButton _t3honrb;
        public static RadioButton _t3hoffrb;

        public static RadioButton _t1fonrb;
        public static RadioButton _t1foffrb;

        public static RadioButton _stoverrideoffrb;
        public static RadioButton _stoverrideonrb;

        public static RadioButton _notificationonrb;
        public static RadioButton _notificationoffrb;

        private static RadioButton _hidehorizontalnavbaronrb;
        private static RadioButton _hidehorizontalnavbaroffrb;

        public static Android.App.PendingIntentFlags _flags = new Android.App.PendingIntentFlags();
        public static int _count = 0;
        //public ExtNotifications _extNotifications = new ExtNotifications();
        public static TheFragment5.ExtWebInterface _extWebInterface = new ExtWebInterface();
        public static TextView _versionTextView;
        public static bool _notificationHttpRequestInProgress = false;
        public static List<string> _notificationList = new List<string>();

        public static List<string> _tabOverrideStringList = new List<string>();
        ArrayAdapter<string> _tab4SpinOverrideAdapter;
        ArrayAdapter<string> _tab5SpinOverrideAdapter;

        public static CustomStickyService _stickyService = new CustomStickyService();

        private static CookieCollection cookies = new CookieCollection();

        public static TheFragment5 _fm5;

        public static TheFragment5 NewInstance(string title, string icon)
        {
            var fragment = new TheFragment5();
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

            _wv = _view.FindViewById<WebView>(Resource.Id.webView5);

            _wvLayout = _view.FindViewById<LinearLayout>(Resource.Id.webViewLayout);
            _appSettingsLayout = _view.FindViewById<LinearLayout>(Resource.Id.appSettingsMainLayout);
            //var _view2 = inflater.Inflate(Resource.Layout.SettingsFragmentLayout, container, false);

            var _ctx = Android.App.Application.Context;

            if (!tabLoaded)
            {
                _wv.SetWebViewClient(new ExtWebViewClient());
                //_wv.SetWebChromeClient(new ExtWebChromeClient());
                _wv.Settings.JavaScriptEnabled = true;
                _wv.Settings.DisplayZoomControls = false;
                _wv.Settings.MediaPlaybackRequiresUserGesture = false;

                _wv.LoadUrl(_url);

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

                _versionTextView = _view.FindViewById<TextView>(Resource.Id.versionTextView);
                //_notificationWebView = _view.FindViewById<WebView>(Resource.Id._notificationWebView);

                _zcoffrb.CheckedChange += ExtSettingChanged;
                _fmonrb.CheckedChange += ExtSettingChanged;
                _t3hoffrb.CheckedChange += ExtSettingChanged;
                _t1foffrb.CheckedChange += ExtSettingChanged;
                _stoverrideonrb.CheckedChange += OnTab5OverrideChanged;
                _notificationonrb.CheckedChange += OnNotificationRbChecked;
                _hidehorizontalnavbaronrb.CheckedChange += OnHorizontalNavBarRbChecked;

                _tab4OverrideSpinner.ItemSelected += OnTab4OverrideSelectionChanged;
                _tab5OverrideSpinner.ItemSelected += OnTab5OverrideSelectionChanged;
                //_notificationTestButton.Click += ExtNotificationEvents;

                _tab4SpinOverrideAdapter = new ArrayAdapter<string>(_ctx,
                        Android.Resource.Layout.SimpleListItem1, _tabOverrideStringList);

                _tab4OverrideSpinner.Adapter = _tab4SpinOverrideAdapter;

                _tab5SpinOverrideAdapter = new ArrayAdapter<string>(_ctx,
                        Android.Resource.Layout.SimpleListItem1, _tabOverrideStringList);

                _tab5OverrideSpinner.Adapter = _tab5SpinOverrideAdapter;

                _versionTextView.Text = Globals._appVersion;

                tabLoaded = true;
            }
            _wv.SetOnTouchListener(new ExtTouchListener());
            _appSettingsLayout.Visibility = ViewStates.Gone;

            GetNotificationSetting();

            SetCheckedState();
            
            //_notificationWebView.SetWebViewClient(new NotificationWebClient());
            //_notificationWebView.Settings.DomStorageEnabled = true;
            //_notificationWebView.AddJavascriptInterface(new Foo(_ctx), "Foo");

            //_notificationWebView.LoadUrl("https://www.bitchute.com/notifications/");

            return _view;
        }

        private void GetNavBarPrefs()
        {
            Globals.AppSettings._hideHorizontalNavBar = _prefs.GetBoolean("hidehoriztonalnavbar", true);
        }

        private static void OnHorizontalNavBarRbChecked(object sender, EventArgs e)
        {
            if (_hidehorizontalnavbaronrb.Checked)
            {
                Globals.AppSettings._hideHorizontalNavBar = true;
                _prefEditor.PutBoolean("hidehorizontalnavbar", true);
            }
            else
            {
                Globals.AppSettings._hideHorizontalNavBar = false;
                _prefEditor.PutBoolean("hidehorizontalnavbar", false);
            }
        }


        public class ExtTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                _main.CustomOnTouch();

                return false;
            }
        }

        public void SetWebViewVis()
        {
            _wv.Visibility = ViewStates.Visible;
        }

        public bool GetNotificationSetting()
        {
            Globals.AppSettings._notifying = _prefs.GetBoolean("notificationson", true);

            if (Globals.AppSettings._notifying)
            {
                _notificationonrb.Checked = true;
            }
            else
            {
                _notificationonrb.Checked = false;
            }

            return Globals.AppSettings._notifying;
        }

        public void OnNotificationRbChecked(object sender, EventArgs e)
        {
            if (_notificationonrb.Checked)
            {
                Globals.AppSettings._notifying = true;
                //start the notification timer as setting _notifying false breaks the loop
                NotificationTimer();
                _prefEditor.PutBoolean("notificationson", Globals.AppSettings._notifying);
                _appIsNotifying = true;
            }
            else
            {
                Globals.AppSettings._notifying = false;
                _prefEditor.PutBoolean("notificationson", Globals.AppSettings._notifying);
                _appIsNotifying = false;
            }
        }
        
        public void CustomLoadUrl(string url)
        {
            _wv.LoadUrl(url);
        }

        public void OnSettingsChanged(List<object> settings)
        {
            _wv.Settings.SetSupportZoom(Convert.ToBoolean(settings[0]));

            if (Convert.ToBoolean(settings[2]))
            {
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab1);
                _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab2);
                _wv.LoadUrl(Globals.JavascriptCommands._jsSelectTab3);
                //_wv.LoadUrl(Globals.JavascriptCommands._jsHideLabel);
            }
        }

        public bool _isNowCheckingBoxes = false;

        public void SetCheckedState()
        {
            _tab4OverridePreference = _prefs.GetString("tab4overridestring", "MyChannel");
            _tab5OverridePreference = _prefs.GetString("settingstaboverridestring", "Settings");

            _zoomControl = _prefs.GetBoolean("zoomcontrol", false);
            _fanMode = _prefs.GetBoolean("fanmode", false);
            _tab3Hide = _prefs.GetBoolean("tab3hide", true);
            _tab1FeaturedOn = _prefs.GetBoolean("t1featured", true);
            _settingsTabOverride = _prefs.GetBoolean("settingstaboverride", false);
            

            _isNowCheckingBoxes = true;
            
            if (_zoomControl)
            {
                _zconrb.Checked = true;
            }
            else
            {
                _zconrb.Checked = false;
            }
            if (_fanMode)
            {
                _fmonrb.Checked = true;
            }
            else
            {
                _fmonrb.Checked = false;
            }
            if (_tab1FeaturedOn)
            {
                _t1fonrb.Checked = true;
            }
            else
            {
                _t1fonrb.Checked = true;
            }
            if (_tab3Hide)
            {
                _t3honrb.Checked = true;
            }
            else
            {
                _t3honrb.Checked = false;
            }
            if (_settingsTabOverride)
            {
                _stoverrideonrb.Checked = true;
            }
            else
            {
                _stoverrideonrb.Checked = false;
            }

            switch (_tab4OverridePreference)
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
            }

            switch (_tab5OverridePreference)
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
            }
            _isNowCheckingBoxes = false;
        }

        /// <summary>
        /// shows the app specific settings menu
        /// when user long presses "settings" tab
        /// </summary>
        public void ShowAppSettingsMenu()
        {
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

        public static MainActivity _main = new MainActivity();

        //public class ExtScrollListener : Java.Lang.Object, View.IOnScrollChangeListener
        //{
        //    public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        //    {
        //        _main.CustomOnTouch();
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

                await Task.Delay(Globals.AppSettings._tabDelay);

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
        
        public void OnTab5OverrideChanged(object sender, EventArgs e)
        {
            if (_stoverrideonrb.Checked)
            {
                _settingsTabOverride = true;
            }
            else
            {
                _settingsTabOverride = false;
            }
            var prefEditor = _prefs.Edit();

            prefEditor.PutBoolean("settingstaboverride", _settingsTabOverride);
        }

        public void OnTab4OverrideSelectionChanged(object sender, EventArgs e)
        {
            _tab4OverrideSpinner = _view.FindViewById<Spinner>(Resource.Id.tab4OverrideSpinner);

            if (_tab4OverrideSpinner != null)
            {
                _tab4OverridePreference = _tab4OverrideSpinner.SelectedItem.ToString();
                _main.TabDetailChanger(3, _tab4OverrideSpinner.SelectedItem.ToString());
            }

            _prefEditor.PutString("tab4overridestring", _tab4OverridePreference);
            _prefEditor.Commit();
        }

        public void OnTab5OverrideSelectionChanged(object sender, EventArgs e)
        {
            _tab5OverrideSpinner = _view.FindViewById<Spinner>(Resource.Id.tab5OverrideSpinner);

            if (_tab5OverrideSpinner != null)
            {
                _tab5OverridePreference = _tab5OverrideSpinner.SelectedItem.ToString();
                _main.TabDetailChanger(4, _tab5OverrideSpinner.SelectedItem.ToString());
            }
            
            _prefEditor.PutString("settingstaboverridestring", _tab5OverridePreference);
            _prefEditor.Commit();
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
                if (_tab4OverrideSpinner.SelectedItem == null)
                {
                    return;
                }
                //_tab4OverridePreference = _tab4OverrideSpinner.SelectedItem.ToString();
                //_tab5OverridePreference = _tab5OverrideSpinner.SelectedItem.ToString();
                
                if (tabLoaded)
                {
                    if (_zconrb.Checked)
                    {
                        _zoomControl = true;
                    }
                    else
                    {
                        _zoomControl = false;
                    }

                    if (_fmonrb.Checked)
                    {
                        _fanMode = true;

                        _main.TabDetailChanger(3, _tab4OverridePreference);
                    }
                    else
                    {
                        _fanMode = false;
                    }
                    if (_t3honrb.Checked)
                    {
                        _tab3Hide = true;
                    }
                    else
                    {
                        _tab3Hide = false;
                    }
                    if (_t1fonrb.Checked)
                    {
                        _tab1FeaturedOn = true;
                    }
                    else
                    {
                        _tab1FeaturedOn = false;
                    }
                    if (_stoverrideonrb.Checked)
                    {
                        _settingsTabOverride = true;

                        _main.TabDetailChanger(4, _tab5OverridePreference);
                    }
                    else
                    {
                        _settingsTabOverride = false;

                    }
                    _prefEditor.PutBoolean("zoomcontrol", _zoomControl);
                    _prefEditor.PutBoolean("fanmode", _fanMode);
                    _prefEditor.PutBoolean("tab3hide", _tab3Hide);
                    _prefEditor.PutBoolean("t1featured", _tab1FeaturedOn);
                    _prefEditor.PutBoolean("settingstaboverride", _settingsTabOverride);
                    _prefEditor.Commit();

                    _settingsList.Clear();
                    _settingsList.Add(_zoomControl);
                    _settingsList.Add(_fanMode);
                    _settingsList.Add(_tab3Hide);
                    _settingsList.Add(_tab1FeaturedOn);
                    _settingsList.Add(_settingsTabOverride);
                    _settingsList.Add(_tab4OverridePreference);
                    _settingsList.Add(_tab5OverridePreference);
                    //we don't need to add the notification preference because it's only used in frag5

                    _main.OnSettingsChanged(_settingsList);
                }
            }
        }

        public static string _rawNoteText = "";

        public static string _cookieString { get; set; }
        internal static ExtNotifications ExtNotifications { get => _extNotifications; set => _extNotifications = value; }

        /// <summary>
        /// we have to set this with a delay or it won't fix the link overflow
        /// </summary>
        public static async void HideLinkOverflow()
        {
            await Task.Delay(Globals.AppSettings._linkOverflowFixDelay);

            _wv.LoadUrl(Globals.JavascriptCommands._jsLinkFixer);
        }

        public static bool _appNotifications = true;

        public static bool _appIsNotifying = true;

        public async void NotificationTimer()
        {
            while (_appIsNotifying)
            {
                if (!_notificationHttpRequestInProgress)
                {
                    _extWebInterface.GetNotificationText("https://www.bitchute.com/notifications/");
                }
                await Task.Delay(Globals.AppSettings._notificationDelay);
            }
        }

        private static async void HideWatchLabel()
        {
            await Task.Delay(1000);
            _wv.LoadUrl(Globals.JavascriptCommands._jsHideTabInner);
        }
      
        public static string _cookieHeader;
        private static ExtNotifications _extNotifications = new ExtNotifications();

        private static int _noteCount = 0;

        private static List<CustomNotification> _sentNotificationList = new List<CustomNotification>();

        public Task<bool> SendNotifications(List<CustomNotification> notificationList)
        {
            TheFragment5._notificationHttpRequestInProgress = true;
                try
                {
                   var _ctx = Android.App.Application.Context;

                    // When the user clicks the notification, MainActivity will start up.

                   MainActivity._NotificationURLList.Clear();

                   foreach (var note in notificationList)
                   {
                       var resultIntent = new Intent(_ctx, typeof(MainActivity));
                       var valuesForActivity = new Bundle();
                       valuesForActivity.PutInt(MainActivity.COUNT_KEY, _count);
                       MainActivity._NotificationURLList.Add(note._noteLink);
                       valuesForActivity.PutString("URL", note._noteLink);
                       resultIntent.PutExtras(valuesForActivity);
                        
                       var resultPendingIntent = PendingIntent.GetActivity(_ctx, MainActivity.NOTIFICATION_ID, resultIntent, PendingIntentFlags.UpdateCurrent);

                       resultIntent.AddFlags(ActivityFlags.SingleTop);

                       if (!_sentNotificationList.Contains(note))
                       {
                            // Build the notification:
                            var builder = new Android.Support.V4.App.NotificationCompat.Builder(_ctx, MainActivity.CHANNEL_ID)
                                     .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                                     .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                                     .SetContentTitle(note._noteText) // Set the title
                                     .SetNumber(1) // Display the count in the Content Info
                                     .SetSmallIcon(2130837590) // This is the icon to display
                                     .SetContentText(note._noteType);
                        
                           MainActivity.NOTIFICATION_ID++;

                            // Finally, publish the notification:
                            var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(_ctx);
                            notificationManager.Notify(MainActivity.NOTIFICATION_ID, builder.Build());

                            _sentNotificationList.Add(note);
                       }

                   CustomStickyService._notificationsHaveBeenSent = true;
                   }
                }
                catch
                {

                }

            return Task.FromResult<bool>(false);
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

            public string GetNotificationText(string url)
            {
                _htmlCode = "";
                

                    HttpClientHandler handler = new HttpClientHandler() { UseCookies = false };
                    
                    _notificationHttpRequestInProgress = true;

                    try
                    {
                        Uri _notificationURI = new Uri("https://bitchute.com/notifications/");
                        
                        var _cookieHeader = _cookieCon.GetCookieHeader(_notificationURI);

                        using (HttpClient _client = new HttpClient(handler))
                        {
                            _client.DefaultRequestHeaders.Add("Cookie", TheFragment5._cookieHeader);
                            var getRequest = _client.GetAsync("https://bitchute.com/notifications/").Result;
                            var resultContent = getRequest.Content.ReadAsStringAsync().Result;

                            _htmlCode = resultContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                _notificationHttpRequestInProgress = false;

                return _htmlCode;
            }
        }

        private class ExtWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView view, string url)
            {
                HideWatchLabel();

                if (_settingsTabOverride)
                {
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideBanner);

                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideBuff);

                    if (_tab5OverridePreference == "Feed")
                    {
                        _wv.LoadUrl(Globals.JavascriptCommands._jsHideCarousel);
                        _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab1);
                        _wv.LoadUrl(Globals.JavascriptCommands._jsHideTab2);
                        _wv.LoadUrl(Globals.JavascriptCommands._jsSelectTab3);
                        _wv.LoadUrl(Globals.JavascriptCommands._jsHideTrending);
                    }
                }

                if (Globals.AppState.Display._horizontal)
                {
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideTitle);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHideWatchTab);
                    _wv.LoadUrl(Globals.JavascriptCommands._jsHidePageBar);
                }

                _wv.LoadUrl(Globals.JavascriptCommands._jsLinkFixer);

                SetReload();

                TheFragment5._cookieHeader = Android.Webkit.CookieManager.Instance.GetCookie(url);
                Globals._cookieString = TheFragment5._cookieHeader.ToString();
                var cookiePairs = TheFragment5._cookieHeader.Split('&');

                Globals._cookieString = "";

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
                    c.Domain = "https://bitchute.com/notifications/";

                    if (Globals._cookieString == "")
                    {
                        Globals._cookieString = c.ToString();
                    }
                    else
                    {
                        Globals._cookieString += c.ToString();
                    }
                }
                HideLinkOverflow();
            }
        }

        //private class ExtWebChromeClient : WebChromeClient
        //{
        //    public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        //    {
                

        //        return base.OnShowFileChooser(webView, filePathCallback, fileChooserParams);
        //    }
        //}
    }
}
