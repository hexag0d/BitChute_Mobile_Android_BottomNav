using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Classes;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Android.Views.View;
using static BitChute.Fragments.SettingsFragment;
using static StartServices.Servicesclass.ExtStickyService;

namespace BitChute.Fragments
{
    [Android.Runtime.Register("onWindowVisibilityChanged", "(I)V", "GetOnWindowVisibilityChanged_IHandler")]
    public class HomeFragment : Fragment
    {
        string _title;
        string _icon;
        
        protected static View _view;
        
        bool tabLoaded = false;

        public static HomeFragment NewInstance(string title, string icon) {
            var fragment = new HomeFragment();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
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
            _view = inflater.Inflate(Resource.Layout.Tab0FragmentLayout, container, false);

            if (!tabLoaded)
            {
                tabLoaded = true;
            }
            
            return _view;
        }

        public override void OnResume()
        {
            base.OnResume();
            IntentFilter intentFilter = new IntentFilter(Intent.ActionHeadsetPlug);

        }

        /// <summary>
        /// sets the touch listener when device is in landscape mode
        /// sets it to null when the device goes back into portrait mode
        /// </summary>
        /// <param name="landscape"></param>
        public void CustomSetTouchListener(bool landscape)
        {
            if (landscape)
            {
            }
            else
            {
            }
        }

        public void OnSettingsChanged()
        {
            _wv.Settings.SetSupportZoom(AppSettings.ZoomControl);

            if (!AppSettings.Tab1FeaturedOn)
            {
            }
            else
            {
            }

            if (AppSettings.ZoomControl)
            {
                _wv.Settings.BuiltInZoomControls = true;
            }
            else
            {
                _wv.Settings.BuiltInZoomControls = false;
            }
        }
        
        
        public class ExtTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                MainActivity.CustomOnTouch();
                CustomOnTouch();
                return false;
            }
        }

        private static async void CustomOnTouch()
        {
            if (AppState.Display.Horizontal)
            {
            }
        }
        

        /// <summary>
        /// tells the webview to GoBack, if it can
        /// </summary>
        public void WebViewGoBack()
        {
        }

        static bool _wvRl = true;

        /// <summary>
        /// one press refreshes the page; two presses pops back to the root
        /// </summary>
        public void Pop2Root()
        {
        }
    }
}
