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
using BitChute.Classes;
using static BitChute.MainActivity;

namespace BitChute.Models
{
    /// <summary>
    /// This class is an easier way to access static view resources. 
    /// It essentially acts as a link within the app to easily get and set views
    /// </summary>
    public class CustomViewHelpers 
    {
        public class Comment
        {

        }

        public class Common
        {
            public static Dictionary<int, VideoView> VideoViewDictionary = new Dictionary<int, VideoView>();
            public static Dictionary<int, TextView> VideoDetailTitles = new Dictionary<int, TextView>();

            public static void SetVideoTitles (int tabNo, TextView tv)
            {
                VideoDetailTitles.Add(tabNo, tv);
            }

            public static void OnRotation(LinearLayout.LayoutParams layoutParams)
            {
                foreach (var vv in VideoViewDictionary)
                {
                    vv.Value.LayoutParameters = layoutParams;
                }
                if (AppState.Display.Horizontal)
                {
                    foreach (var tv in VideoDetailTitles)
                    {
                        tv.Value.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    foreach (var tv in VideoDetailTitles)
                    {
                        tv.Value.Visibility = ViewStates.Visible;
                    }
                }
            }

        //    public class Video
        //    {
        //        public class PictureInPicture
        //        {
        //            private static VideoView _vidView;
        //            public static VideoView VideoView
        //            {
        //                get
        //                {
        //                    if (_vidView == null)
        //                    {
        //                        return new VideoView(Android.App.Application.Context);
        //                    }
        //                    else
        //                    {
        //                        return _vidView;
        //                    }
        //                }
        //                set
        //                {
        //                    _vidView = value;
        //                }
        //            }
        //        }

        //        public static VideoView GetVideoView(int tab)
        //        {
        //            switch (tab)
        //            {
        //                case 0:
        //                    break;
        //                case 1:
        //                    if (Tab1.VideoView == null)
        //                    {

        //                    }
        //                    return Tab1.VideoView;
        //                case 2:
        //                    break;

        //            }
        //            return Tab1.VideoView;
        //        }
        //    }
        }

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
            public static LinearLayout VideoLayout { get; set; }
            public static LinearLayout VideoMetaLayout { get; set; }
            public static LinearLayout VideoMetaLayoutLower { get; set; }
            
            public static Android.Widget.ScrollView VideoDetailScrollView { get; set; }

            public static Android.Support.V7.Widget.RecyclerView RootRecyclerView { get; set; }
            public static Android.Support.V7.Widget.RecyclerView CreatorDetailRecyclerView { get; set; }
            public static Android.Support.V7.Widget.RecyclerView CommentRecyclerView { get; set; }

            public static Android.Support.V7.Widget.RecyclerView.LayoutManager RootLayoutManager { get; set; }
            public static Android.Support.V7.Widget.RecyclerView.LayoutManager CreatorDetailLayoutManager { get; set; }
            public static Android.Support.V7.Widget.RecyclerView.LayoutManager VideoLayoutManager { get; set; }
            //public static VideoView VideoView { get; set; }

            public static VideoView VideoView
            {
                get { return Common.VideoViewDictionary[TabNo]; }
                set
                {
                    if (Common.VideoViewDictionary.ContainsKey(TabNo))
                    {
                        Common.VideoViewDictionary[TabNo] = value;
                    }
                    else
                    {
                        Common.VideoViewDictionary.Add(TabNo, value);
                    }
                }
            }
            public static ImageView CreatorDetailAvatarImageView { get; set; }
            public static TextView VideoDetailTitle { get; set; }
            //public static TextView VideoDetailTitle
            //{
            //    get { return VideoDetailTitle; }
            //    set
            //    {
            //        if (!Common.TitleTextViewDictionary.ContainsKey(TabNo))
            //            Common.TitleTextViewDictionary.Add(TabNo, value);
            //        else
            //            Common.TitleTextViewDictionary[TabNo] = value;
            //    }
            //}
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
            public static LinearLayout LeaveACommentLayout { get; set; }
            public static MultiAutoCompleteTextView LeaveACommentTextBox { get; set; }
            public static Button SendCommentButton { get; set; }

            private static int TabNo = 1;
        }
    }
}