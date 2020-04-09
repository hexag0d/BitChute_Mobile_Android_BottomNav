/* A̬̤̣̹̱̜ͭ̉̋͊̑͊͑
 based off the template by hnabbasi

 bitchute.com/channel/hexagod
soundcloud.com/vybemasterz

*/
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using BitChute.Adapters;
using BitChute.Classes;
using BitChute.Fragments;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Android.Views.View;
using static BitChute.Fragments.SettingsFragment;
using static BitChute.Models.VideoModel;
using static BitChute.Models.SubscriptionModel;
using static BitChute.Models.CommentModel;
using BitChute.ViewHolders;
using BitChute.Models;

namespace BitChute
{
    //we need to set the intent filter so that links can open inapp
    [Android.App.IntentFilter(new[] { Intent.ActionView },
              Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
              DataScheme = "http",
              DataHost = "bitchute.com",
              DataPathPrefix = "",
              AutoVerify = true)]
    [Android.App.Activity(LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Label = "BitChute", Theme = "@style/AppTheme", MainLauncher = true,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize,
        ParentActivity = typeof(MainActivity))]

    public class MainActivity : FragmentActivity
    {
        int _tabSelected;

        public static ViewPager ViewPager;
        public static BottomNavigationView _navigationView;
        public static List<BottomNavigationItemView> _navViewItemList
            = new List<BottomNavigationItemView>();

        IMenuItem _menu;
        private static Drawable _tab4Icon;
        private static Drawable _tab5Icon;
        private static AssetManager _assets;
        Android.Support.V4.App.Fragment[] _fragments;

        public static MainActivity Main;
        public static Bundle _bundle;
        
        public static ExtNotifications notifications = new ExtNotifications();
        public static bool _navBarHideTimeout = false;

        //notification items:
        public static int NOTIFICATION_ID = 1000;
        public static readonly string CHANNEL_ID = "notification";
        public static readonly string COUNT_KEY = "count";

        public static Window _window;
        public static View _mainView;
        
        public static ExtStickyService _service = new ExtStickyService();
        readonly WindowManagerFlags _winFlagUseHw = WindowManagerFlags.HardwareAccelerated;

        public static HeadphoneIntent.MusicIntentReceiver _musicIntentReceiver;

        // Underlying data set (a photo album):
        public static VideoCardSet _photoAlbum;

        public static Android.Graphics.Color _darkGrey = new Android.Graphics.Color(20, 20, 20);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppSettings.LoadAllPrefsFromSettings();
            DisplayMetrics metrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(metrics);

            AppState.Display.ScreenHeight = metrics.HeightPixels;
            AppState.Display.ScreenWidth = metrics.WidthPixels;

            _assets = Resources.Assets;
            if (Resources.Configuration.Orientation == Orientation.Landscape)
            {
                AppState.Display.Horizontal = true;
            }
            else
            {
                AppState.Display.Horizontal = false;
            }
            Main = this;
            _window = this.Window;
            
            var mServiceIntent = new Intent(this, typeof(ExtStickyService));
            StartService(mServiceIntent);

            //get the intent incase a notification started the activity
            Intent _sentIntent = Intent;

            if (_sentIntent != null)
            {
                //if it's not null then set the fragment1 url to our intent url string
                try
                {
                    //var _tempUrl = _sentIntent.Extras.GetString("URL");
                    var _tempUrl = "";
                    if (_tempUrl != "" && _tempUrl != null)
                    {
                    }
                }
                catch
                {
                }
            }
            try
            {
                StartService(mServiceIntent);
            }
            catch
            {
            }
            
            base.OnCreate(savedInstanceState);
            _window.AddFlags(_winFlagUseHw);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            InitializeTabs();
            
            ViewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            ViewPager.PageSelected += ViewPager_PageSelected;
            ViewPager.Adapter = new ViewPagerAdapter(SupportFragmentManager, _fragments);

            _navigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            RemoveShiftMode(_navigationView);
            _navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            ViewPager.OffscreenPageLimit = 4;
            CreateNotificationChannel();
            ExtStickyService.StartNotificationLoop(90000);
            _musicIntentReceiver = new HeadphoneIntent.MusicIntentReceiver();
            
            //debug subs
            ViewPager.CurrentItem = 1;
        }

        public static HomeFragment _fm1 = HomeFragment.NewInstance("Home", "tab_home");
        public static SubscriptionFragment _fm2 = SubscriptionFragment.NewInstance("Subs", "tab_subs");
        public static FeedFragment _fm3 = FeedFragment.NewInstance("Feed", "tab_playlist");
        public static MyChannelFragment _fm4 = MyChannelFragment.NewInstance("MyChannel", "tab_mychannel");
        public static SettingsFragment _fm5 = SettingsFragment.NewInstance("Settings", "tab_home");

        void InitializeTabs()
        {
            _fragments = new Android.Support.V4.App.Fragment[] {
                _fm1,
                _fm2,
                _fm3,
                _fm4,
                _fm5
            };
        }
        

        internal static ExtNotifications Notifications { get => notifications; set => notifications = value; }
        public static bool _navHidden = false;
        public static bool _navTimeout = true;
        public static int _navTimer = 0;

        public async void CustomOnSwipe()
        {
            await Task.Delay(1);

            if (_navTimer != 0)
                _navTimer = 0;

            if (!_navTimeout || AppState.Display.Horizontal)
            {
                _navigationView.Visibility = ViewStates.Visible;
                _navHidden = false;
                NavBarRemove();
                _navTimeout = true;
            }
        }

        /// <summary>
        /// invoked on scroll events and hides the navbar after x seconds
        /// .. timer resets every time it's called
        /// . works with a custom scroll listener
        /// </summary>
        public static async void CustomOnTouch()
        {
            await Task.Delay(1);

                if (_navTimer != 0)
                    _navTimer = 0;

            if (AppState.Display.Horizontal && !AppSettings._hideHorizontalNavbar)
            {
                if (!_navTimeout)
                {
                    _navigationView.Visibility = ViewStates.Visible;
                    _navHidden = false;
                    NavBarRemove();
                    _navTimeout = true;
                }
            }
            else if (!AppState.Display.Horizontal)
            {
                if (_navigationView.Visibility == ViewStates.Gone)
                {
                    _navigationView.Visibility = ViewStates.Visible;
                    _navHidden = false;
                    NavBarRemove();
                    _navTimeout = true;
                }
            }
        }

        public static async void NavBarRemove()
        {
            while (!_navHidden)
            {
                await Task.Delay(1000);

                _navTimer++;

                if (_navTimer >= 8)
                {
                    if (AppState.Display.Horizontal)
                    {
                        _navigationView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        if (AppSettings._hideVerticalNavbar)
                        {
                            _navigationView.Visibility = ViewStates.Gone;
                        }
                    }
                    _navTimeout = false;
                    _navHidden = true;
                }
            }
        }

        public override bool OnKeyDown(Android.Views.Keycode keyCode, KeyEvent e)
        {
            if (e.KeyCode == Android.Views.Keycode.Back)
            {
                switch (ViewPager.CurrentItem)
                {
                    case 0:
                        _fm1.WebViewGoBack();
                        break;
                    case 1:
                        _fm2.SubsTabGoBack();
                        break;
                    case 2:
                        _fm3.WebViewGoBack();
                        break;
                    case 3:
                        break;
                    case 4:
                        _fm5.WebViewGoBack();
                        break;
                }
            }
            return false;
        }
        
        void NavigationView_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            _navTimer = 0;

            if (_tabSelected == e.Item.Order)
            {
                switch (ViewPager.CurrentItem)
                {
                    case 0:
                        _fm1.Pop2Root();
                        break;
                    case 1:
                        _fm2.Pop2Root();
                        break;
                    case 2:
                        _fm3.Pop2Root();
                        break;
                    case 3:
                        _fm4.Pop2Root();
                        break;
                    case 4:
                        _fm5.Pop2Root();
                        break;
                }
            }
            else
            {
                ViewPager.SetCurrentItem(e.Item.Order, true);
            }
        }

        private void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            _navTimer = 0;
            _menu = _navigationView.Menu.GetItem(e.Position);
            _navigationView.SelectedItemId = _menu.ItemId;
            _tabSelected = ViewPager.CurrentItem;

            if (AppSettings._fanMode)
            {
                if (AppSettings._tab4OverridePreference != null && _tab4Icon != null)
                {
                    _navViewItemList[3].SetTitle(AppSettings._tab4OverridePreference);
                    _navViewItemList[3].SetIcon(_tab4Icon);
                }
            }
            if (AppSettings._settingsTabOverride)
            {
                if (AppSettings._tab4OverridePreference != null && _tab5Icon != null)
                {
                    _navViewItemList[4].SetTitle(AppSettings._tab4OverridePreference);
                    _navViewItemList[4].SetIcon(_tab5Icon);
                }
            }
            CustomOnSwipe();
        }

        //BottomNavigationView.NavigationItemReselectedEventArgs

        void RemoveShiftMode(BottomNavigationView view)
        {
            var menuView = (BottomNavigationMenuView)view.GetChildAt(0);
            
            try
            {
                for (int i = 0; i < menuView.ChildCount; i++)
                {
                    var item = (BottomNavigationItemView)menuView.GetChildAt(i);
                    View label = item.FindViewById(Resource.Id.largeLabel);
                    if (label != null)
                    {
                        ((Android.Widget.TextView)label).SetPadding(0, 0, 0, 0);
                    }
                    //api_8
                    //item.SetShiftingMode(false);

                    // set once again checked value, so view will be updated
                    item.SetChecked(item.ItemData.IsChecked);

                    if (i == 2)
                    {
                        item.LongClick += FeedTabLongClickListener;
                    }
                    if (i == 4)
                    {
                        item.LongClick += SettingsTabLongClickListener; 
                    }
                    if (_navViewItemList != null)
                    {
                        _navViewItemList.Add(item);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine((ex.InnerException ?? ex).Message);
            }
            try
            {
                if (!AppSettings._fanMode)
                {
                    _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_mychannel);
                }
                else
                {
                    _navViewItemList[3].SetTitle(AppSettings._tab4OverridePreference);
                    _navViewItemList[3].SetIcon(GetTabIconFromString(AppSettings._tab4OverridePreference));
                    _tab4Icon = GetTabIconFromString(AppSettings._tab4OverridePreference);
                }
                if (!AppSettings._settingsTabOverride)
                {
                    _tab5Icon = Main.GetDrawable(Resource.Drawable.tab_settings);
                }
                else
                {
                    _navViewItemList[4].SetTitle(AppSettings._tab4OverridePreference);
                    _navViewItemList[4].SetIcon(GetTabIconFromString(AppSettings._tab4OverridePreference));
                    _tab5Icon = GetTabIconFromString(AppSettings._tab4OverridePreference);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// forwards the settings to fragments
        /// </summary>
        /// <param name="oa"></param>
        public static void OnSettingsChanged()
        {
            _fm1.OnSettingsChanged();
            _fm3.OnSettingsChanged();
            _fm4.OnSettingsChanged();
            _fm5.OnSettingsChanged();
        }

        /// <summary>
        /// changes tabs 4 and 5
        /// int use 3 for MyChannel tab and 4 for Settings tab
        /// string use strings like "Home" "Subs" "Feed" "MyChannel" "Settings" "WatchL8r" "Playlists"
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="changeDetails"></param>
        public static void TabDetailChanger(int tab, string changeDetails)
        {
            switch (tab)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3: 
                        if (changeDetails == "" || changeDetails == null)
                        {
                            _navViewItemList[tab].SetTitle("MyChannel");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_mychannel));
                            _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_mychannel);

                        }
                        if (changeDetails == "Home")
                        {
                            _navViewItemList[tab].SetTitle("Home");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_home));
                            _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_home);

                        }
                        if (changeDetails == "Subs")
                        {
                            _navViewItemList[tab].SetTitle("Subs");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_subs));
                            _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_subs);

                        }
                        if (changeDetails == "Feed")
                        {
                            _navViewItemList[tab].SetTitle("Feed");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_playlists));
                            _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_playlists);
 
                        }
                        if (changeDetails == "MyChannel")
                        {
                            _navViewItemList[tab].SetTitle("MyChannel");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_mychannel));
                            _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_mychannel);
                        }
                        if (changeDetails == "Explore")
                        {
                            _navViewItemList[tab].SetTitle("Explore");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_subs));
                            _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_subs);
                        }
                        if (changeDetails == "Settings")
                        {
                            _navViewItemList[tab].SetTitle("Settings");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_settings));
                            _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_settings);
                        }
                        if (changeDetails == "WatchL8r")
                        {
                            _navViewItemList[tab].SetTitle("WatchL8r");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_mychannel));
                            _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_mychannel);
                        }
                    break;
                case 4:
                        if (changeDetails == "" || changeDetails == null)
                        {
                            _navViewItemList[tab].SetTitle("Settings");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_settings));
                            _tab4Icon = Main.GetDrawable(Resource.Drawable.tab_settings);
                            SettingsFragment._url = Https.Urls._settings;
                        }
                        if (changeDetails == "Home")
                        {
                            _navViewItemList[tab].SetTitle("Home");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_home));
                            _tab5Icon = Main.GetDrawable(Resource.Drawable.tab_home);
                            SettingsFragment._url = Https.Urls._homepage;
                        }
                        if (changeDetails == "Subs")
                        {
                            _navViewItemList[tab].SetTitle("Subs");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_subs));
                            _tab5Icon = Main.GetDrawable(Resource.Drawable.tab_subs);
                            SettingsFragment._url = Https.Urls._subspage;
                        }
                        if (changeDetails == "Feed")
                        {
                            _navViewItemList[tab].SetTitle("Feed");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_playlists));
                            _tab5Icon = Main.GetDrawable(Resource.Drawable.tab_playlists);
                            SettingsFragment._url = Https.Urls._homepage;
                        }
                        if (changeDetails == "Explore")
                        {
                            _navViewItemList[tab].SetTitle("Explore");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_subs));
                            _tab5Icon = Main.GetDrawable(Resource.Drawable.tab_subs);
                            SettingsFragment._url = Https.Urls._explore;
                        }
                        if (changeDetails == "Settings")
                        {
                            _navViewItemList[tab].SetTitle("Settings");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_settings));
                            _tab5Icon = Main.GetDrawable(Resource.Drawable.tab_settings);
                            SettingsFragment._url = Https.Urls._settings;
                        }
                        if (changeDetails == "WatchL8r")
                        {
                            _navViewItemList[tab].SetTitle("WatchL8r");
                            _navViewItemList[tab].SetIcon(Main.GetDrawable(Resource.Drawable.tab_mychannel));
                            _tab5Icon = Main.GetDrawable(Resource.Drawable.tab_mychannel);
                            SettingsFragment._url = Https.Urls._watchLater;
                        }
                        SettingsFragment.LoadUrlWithDelay(SettingsFragment._url, 0);
                    
                    break;
            }
        }

        /// <summary>
        /// Listens for long click events on tab 4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SettingsTabLongClickListener(object sender, LongClickEventArgs e)
        {
            if (ViewPager.CurrentItem != 4)
            {
                ViewPager.CurrentItem = 4;
            }
            _fm5.ShowAppSettingsMenu();
        }

        public static bool _backgroundRequested = false;

        /// <summary>
        /// Listens for long click events on tab 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FeedTabLongClickListener(object sender, LongClickEventArgs e)
        {
            _backgroundRequested = true;
            Main.MoveTaskToBack(true);
        }

        public Android.App.ActivityManager GetCustomActivityManager()
        {
            Android.App.ActivityManager _am = (Android.App.ActivityManager)Android.App.Application
                    .Context.GetSystemService(Context.ActivityService);
            return _am;
        }

        public static int _serviceTimer = 0;
        public static int _backgroundTimeoutInt = 0;
        public static int _backgroundLoopMsDelayInt = 3600;
        public static ExtWebInterface _extWebInterface = new ExtWebInterface();
        public static ExtNotifications _extNotifications = new ExtNotifications();
        
        void CreateNotificationChannel()
        {
            var alarmAttributes = new Android.Media.AudioAttributes.Builder()
                .SetContentType(Android.Media.AudioContentType.Unknown)
                    .SetUsage(Android.Media.AudioUsageKind.NotificationRingtone).Build();
            
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }
            
            var name = "BitChute";
            var description = "BitChute for Android";
            var channelSilent = new Android.App.NotificationChannel(CHANNEL_ID, name + " Silent", Android.App.NotificationImportance.High)
            {
                Description = description
            };
            
            var channel = new Android.App.NotificationChannel(CHANNEL_ID + 1, name, Android.App.NotificationImportance.High)
            {
                Description = description
            };

            channel.LockscreenVisibility = NotificationVisibility.Private;
            var notificationManager = (Android.App.NotificationManager)GetSystemService(NotificationService);
            channelSilent.SetSound(null, null);
            notificationManager.CreateNotificationChannel(channel);
            notificationManager.CreateNotificationChannel(channelSilent);
        }

        private Action<int, Result, Intent> resultCallbackvalue;

        public void StartActivity(Intent intent, int requestCode, Action<int, Result, Intent> resultCallback)
        {
            this.resultCallbackvalue = resultCallback;
            StartActivityForResult(intent, requestCode);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (this.resultCallbackvalue != null)
            {
                this.resultCallbackvalue(requestCode, resultCode, data);
                this.resultCallbackvalue = null;
            }
        }
        
        protected override void OnNewIntent(Intent intent)
        {
            string url = "";
            base.OnNewIntent(intent);

            if (intent != null)
            {
                try
                {
                    url = intent.Extras.GetString("URL");
                }
                catch
                {
                }
            }
            
            try
            {
                switch (ViewPager.CurrentItem)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }
            }
            catch 
            {
            }
        }

        readonly WindowManagerFlags _winflagfullscreen = WindowManagerFlags.Fullscreen;
        readonly WindowManagerFlags _winflagnotfullscreen = WindowManagerFlags.ForceNotFullscreen;

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            var tempHeight = AppState.Display.ScreenHeight;
            AppState.Display.ScreenHeight = AppState.Display.ScreenWidth;
            AppState.Display.ScreenWidth = tempHeight;
            if (newConfig.Orientation == Orientation.Landscape)
            {
                AppState.Display.Horizontal = true;
                VideoDetailLoader.OnRotation(AppState.Display.GetCurrentVideoContainerLayout());
                _navigationView.Visibility = ViewStates.Gone;
                switch (ViewPager.CurrentItem)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }
                _window.ClearFlags(_winflagnotfullscreen);
                _window.AddFlags(_winflagfullscreen);
            }
            if (newConfig.Orientation == Orientation.Portrait)
            {
                AppState.Display.Horizontal = false;
                VideoDetailLoader.OnRotation(AppState.Display.GetCurrentVideoContainerLayout());
                switch (ViewPager.CurrentItem)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }
                _window.ClearFlags(_winflagfullscreen);
                _window.AddFlags(_winflagnotfullscreen);
                CustomOnTouch();
            }

            if (!AppSettings._hideHorizontalNavbar || newConfig.Orientation == Orientation.Portrait)
            {
                _navTimeout = false;
            }

            //app seems to be lagging ontouch so removing the touch listener when app is portrait
            _fm1.CustomSetTouchListener(AppState.Display.Horizontal);
            _fm2.CustomSetTouchListener(AppState.Display.Horizontal);
            _fm3.CustomSetTouchListener(AppState.Display.Horizontal);
            _fm4.CustomSetTouchListener(AppState.Display.Horizontal);
            _fm5.CustomSetTouchListener(AppState.Display.Horizontal);

            base.OnConfigurationChanged(newConfig);
        }

        /// <summary>
        /// returns a Drawable icon to be used for tab detail changing
        /// takes string arguments like "Home" "Subs" "Feed" "MyChannel" "Settings"
        /// </summary>
        /// <param name="tabString"></param>
        /// <returns></returns>
        public static Drawable GetTabIconFromString(string tabString)
        {
            Drawable icon;

            switch (tabString)
            {
                case "Home":
                    icon = Main.GetDrawable(Resource.Drawable.tab_home);
                    return icon;
                case "Subs":
                    icon = Main.GetDrawable(Resource.Drawable.tab_subs);
                    return icon;
                case "Feed":
                    icon = Main.GetDrawable(Resource.Drawable.tab_playlists);
                    return icon;
                case "MyChannel":
                    icon = Main.GetDrawable(Resource.Drawable.tab_mychannel);
                    return icon;
                case "Settings":
                    icon = Main.GetDrawable(Resource.Drawable.tab_settings);
                    return icon;
                case "Playlists":
                    icon = Main.GetDrawable(Resource.Drawable.tab_playlists);
                    return icon;
                case "WatchL8r":
                    icon = Main.GetDrawable(Resource.Drawable.tab_playlists);
                    return icon;
                case "Explore":
                    icon = Main.GetDrawable(Resource.Drawable.tab_home);
                    return icon;
            }
            return Main.GetDrawable(Resource.Drawable.tab_home);
        }

        public class SubscriptionRecyclerViewAdapter : RecyclerView.Adapter
        {
            public event EventHandler<int> ItemClick;
            public static SubscriptionCardSet _subscriptionCardSet;
            public static View itemView;

            public SubscriptionRecyclerViewAdapter(SubscriptionCardSet videoCardSet)
            {
                if (videoCardSet == null)
                {
                    videoCardSet = new SubscriptionCardSet();
                }
                _subscriptionCardSet = videoCardSet;
            }

            // Create a new photo CardView (invoked by the layout manager): 
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                // Inflate the CardView for the photo:
                View itemView = LayoutInflater.From(parent.Context).
                            Inflate(Resource.Layout.SubscriptionsCardView, parent, false);

                Android.Graphics.Color _darkGrey = new Android.Graphics.Color(20, 20, 20);
                CardView cv = itemView.FindViewById<CardView>(Resource.Id.subscriptionCardView);
                cv.SetBackgroundColor(_darkGrey);

                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                SubscriptionViewHolder vh = new SubscriptionViewHolder(itemView, OnClick);

                return vh;
            }

            // Fill in the contents of the photo card (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                SubscriptionViewHolder vh = holder as SubscriptionViewHolder;
                
                // Set the ImageView and TextView in this ViewHolder's CardView 
                // from this position in the photo album:
                vh.Image.SetImageResource(_subscriptionCardSet[position].PhotoID);

                try
                {
                    vh.Title.Text = _subscriptionCardSet[position].Title;
                    vh.CreatorName.Text = _subscriptionCardSet[position].CreatorName;
                }
                catch (Exception ex)
                {
                }
            }

            // Return the number of photos available in the photo album:
            public override int ItemCount
            {
                get
                {
                    if (_subscriptionCardSet != null)
                        return _subscriptionCardSet.NumPhotos;
                    else
                        return 36;
                }
            }

            // Raise an event when the item-click takes place:
            void OnClick(int position)
            {
                var pos = position;
                if (ItemClick != null)
                    ItemClick(this, position);
            }
        }
        
        public class FeedRecyclerViewAdapter : RecyclerView.Adapter
        {
            public event EventHandler<int> ItemClick;
            public static VideoCardSet _videoCardSet;
            public static List<VideoCardNoCreator> _videoCardNoCreators = new List<VideoCardNoCreator>();
            public static View itemView;

            public static FeedViewHolder vh;
            
            public FeedRecyclerViewAdapter(VideoCardSet videoCardSet)
            {
                if (videoCardSet == null)
                {
                    videoCardSet = new VideoCardSet();
                }
                _videoCardSet = videoCardSet;
            }

            public FeedRecyclerViewAdapter(List<VideoCardNoCreator> videoCardNoCreators)
            {
                _videoCardNoCreators = videoCardNoCreators;
            }

            public FeedRecyclerViewAdapter(List<Models.CreatorModel.Creator> creatorSet)
            {
                foreach (var creator in creatorSet)
                {
                    foreach (var vidcard in creator.RecentVideoCards)
                    {
                        vidcard.CreatorName = creator.Name;
                        _videoCardNoCreators.Add(vidcard);
                    }
                }
            }

            // Create a new photo CardView (invoked by the layout manager): 
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                // Inflate the CardView for the photo:
                View itemView = LayoutInflater.From(parent.Context).
                                Inflate(Resource.Layout.FeedCardView, parent, false);
                
                CardView cv = itemView.FindViewById<CardView>(Resource.Id.feedCardView);

                cv.SetBackgroundColor(_darkGrey);

                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                vh = new FeedViewHolder(itemView, OnClick);

                return vh;
            }

            // Fill in the contents of the photo card (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                vh = holder as FeedViewHolder;

                // Set the ImageView and TextView in this ViewHolder's CardView 
                // from this position in the photo album:
                if (_videoCardSet != null)
                {
                    vh.Image.SetImageResource(_videoCardSet[position].PhotoID);
                    vh.Caption.Text = _videoCardSet[position].Title;
                    vh.Caption2.Text = _videoCardSet[position].CreatorName;
                }
                else
                {
                    vh.Image.SetImageResource(_videoCardNoCreators[position].VideoThumbnail);
                    vh.Caption.Text = _videoCardNoCreators[position].Title;
                    vh.Caption2.Text = _videoCardNoCreators[position].CreatorName;
                }
            }

            // Return the number of photos available in the photo album:
            public override int ItemCount
            {
                get
                {
                    if (_videoCardSet != null)
                        return _videoCardSet.NumPhotos;
                    else
                        return 6;
                }
            }

            // Raise an event when the item-click takes place:
            void OnClick(int position)
            {
                var pos = position;
                if (ItemClick != null)
                    ItemClick(this, position);
            }
        }
        

        public static Android.Graphics.Drawables.Drawable UniversalGetDrawable(int id)
        {
            Drawable drawable = Main.GetDrawable(id);
            return drawable;
        }

        public void UpdateViewsFromAdapter(RecyclerView.Adapter a)
        {
            RunOnUiThread(() => a.NotifyDataSetChanged());
        }
        

        protected override void OnResume()
        {
            base.OnResume();

            try {
                IntentFilter filter = new IntentFilter(Intent.ActionHeadsetPlug);
                RegisterReceiver(_musicIntentReceiver, filter);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        protected override void OnDestroy()
        {
            ViewPager.PageSelected -= ViewPager_PageSelected;
            _navigationView.NavigationItemSelected -= NavigationView_NavigationItemSelected;
            
            base.OnDestroy();
        }
    }
}
