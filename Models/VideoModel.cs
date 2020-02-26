using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.ViewModels;

namespace BottomNavigationViewPager.Models
{
    public class VideoModel 
    {
        public class VideoInfo 
        {
            string creatorName { get; set; }
            string videoTitle { get; set; }
            string videoLocation { get; set; }
            string videoDescription { get; set; }
            Bitmap thumbnail { get; set; }
        }

        
    }
}