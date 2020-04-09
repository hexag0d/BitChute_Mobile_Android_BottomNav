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
            public static View Tab1FragmentLayout { get; set; }
            public static View CreatorDetailView { get; set; }
            public static View VideoDetailView { get; set; }
            
            public static ViewGroup Container { get; set; }

            //public static SubscriptionRecyclerViewAdapter SubscriptionViewAdapter { get; set; }
            //public static CreatorDetailRecyclerViewAdapter CreatorDetailRecyclerViewAdapter { get; set; }
            //public static CommentRecyclerViewAdapter CommentSystemRecyclerViewAdapter { get; set; }

            public static LayoutInflater LayoutInflater { get; set; }
            public static LinearLayout SubscriptionRecyclerView { get; set; }
            public static LinearLayout Tab1ParentLayout { get; set; }
            public static Android.Widget.ScrollView VideoDetailScrollView { get; set; }

            public static Android.Support.V7.Widget.RecyclerView RootRecyclerView { get; set; }
            public static Android.Support.V7.Widget.RecyclerView CreatorDetailRecyclerView { get; set; }
            public static Android.Support.V7.Widget.RecyclerView CommentRecyclerView { get; set; }

            public static Android.Support.V7.Widget.RecyclerView.LayoutManager RootLayoutManager { get; set; }
            public static Android.Support.V7.Widget.RecyclerView.LayoutManager CreatorDetailLayoutManager { get; set; }
            public static Android.Support.V7.Widget.RecyclerView.LayoutManager VideoLayoutManager { get; set; }

            public static ImageView CreatorDetailAvatarImageView { get; set; }
            public static TextView VideoDetailTitle { get; set; }
            public static TextView VideoViewCountTextView { get; set; }
            public static ImageView LikeButtonImageView { get; set; }
            public static TextView LikeCountTextView { get; set; }
            public static ImageView DislikeButtonImageView { get; set; }
            public static TextView DislikeCountTextView { get; set; }
            public static Button SubscribeButton { get; set; }
            public static ImageView FavoriteImageView { get; set; }
            public static ImageView AddVideoToPlaylist { get; set; }
            public static ImageView P2pStatsImageView { get; set; }
            public static ImageView ShareVideoImageView { get; set; }
            public static ImageView VideoDetailCreatorAvatarImageView { get; set; }
            public static TextView VideoDescription { get; set; }
            public static TextView CreatorNameTextView { get; set; }
            public static TextView SubCountTextView { get; set; }
            public static ImageView FlagMeImageView { get; set; }
        }
    }
}