using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BitChute.Fragments;

namespace BitChute.Classes
{
    public class TabStates
    {
        public class Tab3
        {
            private static bool _videoDownloaderViewEnabled = false;
            public static bool VideoDownloaderViewEnabled
            {
                get
                {
                    return _videoDownloaderViewEnabled;
                }
                set
                {
                    if (_videoDownloaderViewEnabled != value)
                    {
                        _videoDownloaderViewEnabled = value;
                        MyChannelFrag.SwapDownloaderView(_videoDownloaderViewEnabled);
                    }
                }
            }
        }

        /// <summary>
        /// tab 4 string should be set to strings like
        /// "Subs" "Home" or "Feed"
        /// </summary>
        public static string T4Is { get; set; }

        /// <summary>
        /// tab 5 string should be set to strings like
        /// "Subs" "Home" or "Feed"
        /// </summary>
        public static string T5Is { get; set; }
    }
}