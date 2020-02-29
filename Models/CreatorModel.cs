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

namespace BottomNavigationViewPager.Models
{
    public class CreatorModel
    {
        /// <summary>
        /// lighter weight class that only contains the creator and link
        /// for getting the entire list of subs, doesn't contain video info
        /// </summary>
        public class Creator
        {
            string CreatorName { get; set; }
            string CreatorLinkToChannel { get; set; }
        }

        public class CreatorVideos
        {

        }
    }
}