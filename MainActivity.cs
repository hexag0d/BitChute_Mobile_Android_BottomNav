/* App by:
 

.__                                             .___
|  |__   ____ ___  ________     ____   ____   __| _/
|  |  \_/ __ \\  \/  /\__  \   / ___\ /  _ \ / __ | 
|   Y  \  ___/ >    <  / __ \_/ /_/  >  <_> ) /_/ | 
|___|  /\___  >__/\_ \(____  /\___  / \____/\____ | 
     \/     \/      \/     \//_____/             \/ 

bitchute.com/channel/hexagod
soundcloud.com/vybemasterz

twitter @vybeypantelonez
minds @hexagod
steemit @vybemasterz
gab.ai @hexagod

based off the template by hnabbasi
https://github.com/hnabbasi/BottomNavigationViewPager
 
 */

using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.Design.Internal;
using Android.Support.V4.View;
using BottomNavigationViewPager.Adapters;
using System.Collections.Generic;
using Android.Support.V4.App;
using BottomNavigationViewPager.Fragments;
using Android.InputMethodServices;
using Android.Views;
using Android.Content;
using Android.Webkit;
using Android.Support.V4.Content;
using static Android.Support.Design.Widget.BottomNavigationView;

//app:layout_behavior="@string/hide_bottom_view_on_scroll_behavior"

namespace BottomNavigationViewPager
{
    [Android.App.Activity(Label = "BitChute", Theme = "@style/AppTheme", MainLauncher = true,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize,
        ParentActivity = typeof(MainActivity))]
        
    public class MainActivity : FragmentActivity
    {
        int _tabSelected;

        ViewPager _viewPager;
        BottomNavigationView _navigationView;
        IMenuItem _menu;
        Fragment[] _fragments;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            

            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            InitializeTabs();
            
            _viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            _viewPager.PageSelected += ViewPager_PageSelected;
            _viewPager.Adapter = new ViewPagerAdapter(SupportFragmentManager, _fragments);
			
            _navigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            RemoveShiftMode(_navigationView);
            _navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            _viewPager.OffscreenPageLimit = 4;
        }

        TheFragment1 _fm1 = new TheFragment1();
        TheFragment2 _fm2 = new TheFragment2();
        TheFragment3 _fm3 = new TheFragment3();
        TheFragment4 _fm4 = new TheFragment4();
        TheFragment5 _fm5 = new TheFragment5();

        public override bool OnKeyDown(Android.Views.Keycode keyCode, KeyEvent e)
        {
            if (e.KeyCode == Android.Views.Keycode.Back)
            {
                switch(_viewPager.CurrentItem)
                {
                    case 0:
                        _fm1.WebViewGoBack();
                        break;
                    case 1:
                        _fm2.WebViewGoBack();
                        break;
                    case 2:
                        _fm3.WebViewGoBack();
                        break;
                    case 3:
                        _fm4.WebViewGoBack();
                        break;
                    case 4:
                        _fm5.WebViewGoBack();
                        break;
                }
            }
            
            return false;
        }
        
        void InitializeTabs() {
            _fragments = new Fragment[] {
                TheFragment1.NewInstance("Home", "tab_home"),
                TheFragment2.NewInstance("Subs", "tab_subs"),
                TheFragment3.NewInstance("Playlist", "tab_playlist"),
                TheFragment4.NewInstance("MyChannel", "tab_mychannel"),
                TheFragment5.NewInstance("Settings", "tab_home")
            };
        }

        void NavigationView_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (_tabSelected == e.Item.Order)
            {
                switch (_viewPager.CurrentItem)
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
                //_navigationView.SelectedItemId = e.Item.Order;
                //_menu = _navigationView.Menu.GetItem(e2.Position);
                //_navigationView.SelectedItemId = _menu.ItemId;
                _viewPager.SetCurrentItem(e.Item.Order, true);
            } 
        }

        private void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {

            _menu = _navigationView.Menu.GetItem(e.Position);
            _navigationView.SelectedItemId = _menu.ItemId;

            _tabSelected = _viewPager.CurrentItem;
        }

        //BottomNavigationView.NavigationItemReselectedEventArgs

        void RemoveShiftMode(BottomNavigationView view)
        {
            var menuView = (BottomNavigationMenuView) view.GetChildAt(0);

            try
            {
                var shiftingMode = menuView.Class.GetDeclaredField("mShiftingMode");
				shiftingMode.Accessible = true;
				shiftingMode.SetBoolean(menuView, false);
				shiftingMode.Accessible = false;

				for (int i = 0; i < menuView.ChildCount; i++)
				{
					var item = (BottomNavigationItemView)menuView.GetChildAt(i);
					item.SetShiftingMode(false);
					// set once again checked value, so view will be updated
					item.SetChecked(item.ItemData.IsChecked);
				}
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine((ex.InnerException??ex).Message);
            }
        }

        protected override void OnDestroy()
        {
			_viewPager.PageSelected -= ViewPager_PageSelected;
            _navigationView.NavigationItemSelected -= NavigationView_NavigationItemSelected;
            base.OnDestroy();


        }
     }
}

