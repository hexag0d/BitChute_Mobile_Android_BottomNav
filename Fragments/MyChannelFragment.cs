using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Classes;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static StartServices.Servicesclass.ExtStickyService;

namespace BitChute.Fragments
{
    public class MyChannelFragment : Fragment
    {
        string _title;
        string _icon;
        

        bool tabLoaded = false;

        public static MyChannelFragment NewInstance(string title, string icon)
        {
            var fragment = new MyChannelFragment();
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

        public void GoBack()
        {
        }

        public void Pop2Root()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _view = inflater.Inflate(Resource.Layout.Tab3FragmentLayout, container, false);

            if (AppSettings.FanMode)
            {
            }
            if (!tabLoaded)
            {


                //_wv.Settings.AllowFileAccess = true;
                //_wv.Settings.AllowContentAccess = true;
                tabLoaded = true;
            }
            if (AppSettings.ZoomControl)
            {
            }

            return _view;
        }

        public void CustomSetTouchListener(bool landscape)
        {
            if (landscape)
            {
            }
            else
            {
            }
        }

        public static async void LoadUrlWithDelay(string url, int delay)
        {

        }

        public class ExtTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                return false;
            }
        }

        /// <summary>
        /// this is to allow faster phones and connections the ability to Pop2Root
        /// used to be set without delay inside OnPageFinished but I don't think 
        /// that would work on faster phones
        /// </summary>
        public static async void SetReload()
        {
        }

        public void OnSettingsChanged()
        {
            if (AppSettings.FanMode && AppSettings.Tab4OverridePreference == "Feed")
            {
            }

            if (AppSettings.ZoomControl)
            {
            }
            else
            {
            }
        }
    }
}
