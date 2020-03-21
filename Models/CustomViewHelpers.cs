using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BitChute.Adapters;
using static BitChute.MainActivity;

namespace BitChute.Models
{
    /// <summary>
    /// This class is an easier way to access static view resources. 
    /// It essentially acts as a link within the app to easily get and set views
    /// </summary>
    public class CustomViewHelpers
    {
        public class Tab1
        {
            public static View VideoDetailView { get; set; }
            public static ViewGroup Container { get; set; }
            public static SubscriptionRecyclerViewAdapter SubscriptionViewAdapter { get; set; }
            public static CommentRecyclerViewAdapter CommentSystemRecyclerViewAdapter { get; set; }
            public static LayoutInflater LayoutInflater { get; set; }
            public static LinearLayout SubscriptionRecyclerView { get; set; }
            public static LinearLayout Tab1ParentLayout { get; set; }
            public static Android.Support.V7.Widget.RecyclerView RecyclerView { get; set; }
            public static Android.Support.V7.Widget.RecyclerView.LayoutManager LayoutManager { get; set; }
        }
    }
}