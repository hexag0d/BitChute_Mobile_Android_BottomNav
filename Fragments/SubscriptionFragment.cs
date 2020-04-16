using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Classes;
using Java.IO;
using BitChute;
using StartServices.Servicesclass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static BitChute.MainActivity;
using static StartServices.Servicesclass.ExtStickyService;
using BitChute.Models;
using static BitChute.Models.VideoModel;

using static BitChute.Models.SubscriptionModel;
using static BitChute.Models.CreatorModel;
using static BitChute.Models.CommentModel;

namespace BitChute.Fragments 
{
    public class SubscriptionFragment : Fragment
    {
        string _title;
        string _icon;

        private static int _tabNo = 2;

        //video cards for the root view
        public static SubscriptionCardSet CreatorCardRootSet;
        //video cards for the channel view
        public static List<VideoModel.VideoCard> CreatorDetailVideoCardSet;

        //video cards for the related videos
        public static VideoCardSet _videoCardSetRelatedVideos;
        private static VideoDetailLoader _vidLoader = new VideoDetailLoader();
        public static Handler Tab1Handler = new Handler();

        public static Comment SelectedComment { get; set; }

        public static SubscriptionFragment NewInstance(string title, string icon)
        {
            var fragment = new SubscriptionFragment();
            fragment.Arguments = new Bundle();
            fragment.Arguments.PutString("title", title);
            fragment.Arguments.PutString("icon", icon);
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource  

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
            CustomViewHelpers.Tab1.Tab1FragmentLayout = inflater.Inflate(Resource.Layout.Tab1FragmentLayout, container, false);
            CustomViewHelpers.Tab1.RootRecyclerView = CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<RecyclerView>(Resource.Id.subscriptionRootRecyclerView);

            CustomViewHelpers.Tab1.CreatorDetailView = inflater.Inflate(Resource.Layout.Tab1CreatorDetail, container, false);
            CustomViewHelpers.Tab1.CreatorDetailRecyclerView = CustomViewHelpers.Tab1.CreatorDetailView.FindViewById<RecyclerView>(Resource.Id.creatorDetailRecyclerView);
            CustomViewHelpers.Tab1.CreatorDetailLayoutManager = new LinearLayoutManager(CustomViewHelpers.Tab1.CreatorDetailView.Context);

            //CustomViewHelpers.Tab1.CreatorDetailRecyclerView.SetAdapter(CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter);
            CustomViewHelpers.Tab1.VideoDetailView = inflater.Inflate(Resource.Layout.Tab1VideoDetail, container, false);
            CustomViewHelpers.Tab1.RootLayoutManager = new LinearLayoutManager(container.Context);
            CustomViewHelpers.Tab1.CreatorDetailLayoutManager = new LinearLayoutManager(CustomViewHelpers.Tab1.CreatorDetailView.Context);
            CustomViewHelpers.Tab1.VideoLayoutManager = new LinearLayoutManager(CustomViewHelpers.Tab1.VideoDetailView.Context);
            CustomViewHelpers.Tab1.RootRecyclerView.SetLayoutManager(CustomViewHelpers.Tab1.RootLayoutManager);
            CustomViewHelpers.Tab1.CreatorDetailRecyclerView.SetLayoutManager(CustomViewHelpers.Tab1.CreatorDetailLayoutManager);

            CustomViewHelpers.Tab1.VideoLayout = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<LinearLayout>(Resource.Id.videoLayout);
            CustomViewHelpers.Tab1.VideoView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<VideoView>(Resource.Id.videoView);
            CustomViewHelpers.Tab1.VideoMetaLayout = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<LinearLayout>(Resource.Id.videoMetaLayout);
            CustomViewHelpers.Tab1.VideoMetaLayoutLower = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<LinearLayout>(Resource.Id.videoMetaLowerLayout);
            
            CustomViewHelpers.Tab1.CreatorDetailAvatarImageView = CustomViewHelpers.Tab1.CreatorDetailView.FindViewById<ImageView>(Resource.Id.creatorDetailAvatarImageView);
            CustomViewHelpers.Tab1.CreatorNameTextView = CustomViewHelpers.Tab1.CreatorDetailView.FindViewById<TextView>(Resource.Id.creatorDetailNameTextView);

            CustomViewHelpers.Tab1.CommentRecyclerView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<RecyclerView>(Resource.Id.commentRecyclerView);
            CustomViewHelpers.Tab1.CommentRecyclerView.SetLayoutManager(CustomViewHelpers.Tab1.VideoLayoutManager);

            CustomViewHelpers.Tab1.Tab1ParentLayout = (ExtLinearLayout)CustomViewHelpers.Tab1.Tab1FragmentLayout.FindViewById<LinearLayout>(Resource.Id.tab1ParentFragmentLayout);
            CustomViewHelpers.Tab1.Container = container;
            CustomViewHelpers.Tab1.LayoutInflater = inflater;
            CustomViewHelpers.Tab1.VideoDetailTitle = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<TextView>(Resource.Id.videoDetailTitleTextView);
            CustomViewHelpers.Common.SetVideoTitles(_tabNo, CustomViewHelpers.Tab1.VideoDetailTitle);
            CustomViewHelpers.Tab1.VideoDetailScrollView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ScrollView>(Resource.Id.videoDetailScrollView);
            CustomViewHelpers.Tab1.VideoDetailCreatorAvatarImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.creatorAvatarImageView);
            CustomViewHelpers.Tab1.LikeButtonImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.likeButtonImageView);
            CustomViewHelpers.Tab1.DislikeButtonImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.dislikeButtonImageView);
            CustomViewHelpers.Tab1.SubscribeButton = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<Button>(Resource.Id.creatorSubscribeButton);
            CustomViewHelpers.Tab1.VideoViewCountTextView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<TextView>(Resource.Id.viewCountTextView);

            CustomViewHelpers.Tab1.FavoriteImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.favoriteImageView);
            CustomViewHelpers.Tab1.AddVideoToPlaylist = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.addToPlaylistImageView);
            CustomViewHelpers.Tab1.FlagMeImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.flagMeImageView);
            CustomViewHelpers.Tab1.P2pStatsImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.p2pStatsImageView);
            CustomViewHelpers.Tab1.ShareVideoImageView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<ImageView>(Resource.Id.shareImageView);

            CustomViewHelpers.Tab1.LikeCountTextView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<TextView>(Resource.Id.likeCountTextView);
            CustomViewHelpers.Tab1.DislikeCountTextView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<TextView>(Resource.Id.dislikeCountTextView);
            CustomViewHelpers.Tab1.SubCountTextView = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<TextView>(Resource.Id.creatorSubscriberCount);
            CustomViewHelpers.Tab1.VideoDescription = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<TextView>(Resource.Id.descriptionTextView);

            CustomViewHelpers.Tab1.LeaveACommentLayout = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<LinearLayout>(Resource.Id.commentLeaverLayout);
            CustomViewHelpers.Tab1.LeaveACommentTextBox = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<MultiAutoCompleteTextView>(Resource.Id.commentEntryText);
            CustomViewHelpers.Tab1.SendCommentButton = CustomViewHelpers.Tab1.VideoDetailView.FindViewById<Button>(Resource.Id.sendCommentButton);

            CustomViewHelpers.Tab1.VideoView.Click += VideoView_OnClick;
            CustomViewHelpers.Tab1.LikeButtonImageView.Click += VideoLikeImageView_OnClick;
            CustomViewHelpers.Tab1.DislikeButtonImageView.Click += VideoDislikeImageView_OnClick;
            CustomViewHelpers.Tab1.FavoriteImageView.Click += FavoriteImageView_OnClick;
            CustomViewHelpers.Tab1.AddVideoToPlaylist.Click += FavoriteImageView_OnClick;
            CustomViewHelpers.Tab1.FlagMeImageView.Click += FlagMeImageView_OnClick;
            CustomViewHelpers.Tab1.P2pStatsImageView.Click += P2PStatsImageView_OnClick;
            CustomViewHelpers.Tab1.ShareVideoImageView.Click += ShareVideoImageView_OnClick;
            CustomViewHelpers.Tab1.SubscribeButton.Click += SubscribeButton_OnClick;
            CustomViewHelpers.Tab1.SendCommentButton.Click += SendCommentButton_OnClick;

            //CustomViewHelpers.Tab1.LeaveACommentTextBox.Drag += LeaveACommentLayout_OnDrag;
            //CustomViewHelpers.Tab1.LeaveACommentLayout.Drag += LeaveACommentLayout_OnDrag;
            //CustomViewHelpers.Tab1.LeaveACommentTextBox.FocusChange += LeaveACommentTextBox_OnFocusChanged;
            //CustomViewHelpers.Tab1.Tab1ParentLayout.FocusChange += LeaveACommentTextBox_OnFocusChanged;
            //CustomViewHelpers.Tab1.Tab1FragmentLayout.FocusChange += LeaveACommentTextBox_OnFocusChanged;

            CustomViewHelpers.Tab1.VideoDetailView.ViewTreeObserver.AddOnGlobalLayoutListener(new ExtViewTreeListener());
            

            GetSubscriptionList();
            
            return CustomViewHelpers.Tab1.Tab1FragmentLayout;
        }
       
        public static void LeaveACommentTextBox_OnFocusChanged (object sender, View.FocusChangeEventArgs e)
        {

        }

        private static bool _commentBoxDragInProg;
        private static float _commentBoxYPos;

        public static void LeaveACommentLayout_OnDrag(object sender, View.DragEventArgs e)
        {
            if (e.Event.Action == DragAction.Location)
            {
                if (_commentBoxDragInProg)
                {
                    CustomViewHelpers.Tab1.LeaveACommentLayout.SetX(e.Event.GetY());
                }
            }
            if (e.Event.Action == DragAction.Entered)
            {
                _commentBoxDragInProg = true;
            }
            if (e.Event.Action == DragAction.Exited)
            {
                _commentBoxDragInProg = false;
            }
            _commentBoxYPos = e.Event.GetY();
        }

        public static bool MediaControllerInitialized;

        public void InitializeMediaController()
        {
            if (!VideoDetailLoader.MediaControllerDictionary.ContainsKey(MainActivity.ViewPager.CurrentItem))
            {
                VideoDetailLoader.MediaControllerDictionary.Add(MainActivity.ViewPager.CurrentItem, MainActivity.StickyService.InitializeMediaController(this.Context));
            }
            CustomViewHelpers.Tab1.VideoView.SetMediaController(VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem]);
            VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetAnchorView(CustomViewHelpers.Tab1.Tab1FragmentLayout);

            //VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetAnchorView(CustomViewHelpers.Tab1.VideoDetailView);
            //VideoDetailLoader.VideoViewDictionary[MainActivity.ViewPager.CurrentItem].SetMediaController(VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem]);

            // VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetMediaPlayer(VideoDetailLoader.VideoViewDictionary[MainActivity.ViewPager.CurrentItem]);
            //VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Enabled = true;
            try
            {
                VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Enabled = true;
                VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Show(6000);
                //VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Show();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            MediaControllerInitialized = true;
        }

        public static bool RootAdapterSet;

        public static void UpdateRootAdapter()
        {
            if (!RootAdapterSet)
            {
                Tab1Handler.Post(() =>
                {
                    CustomViewHelpers.Tab1.RootRecyclerView.SetAdapter(TabStates.Tab1.SubscriptionRecyclerViewAdapter);
                });
                RootAdapterSet = true;
            }
            else
            {
                Tab1Handler.Post(() =>
                {
                    TabStates.Tab1.SubscriptionRecyclerViewAdapter.NotifyDataSetChanged();
                });
            }
        }

        public static void UpdateCreatorPage (Creator c)
        {
            CustomViewHelpers.Tab1.CreatorDetailAvatarImageView.SetImageDrawable(c.CreatorThumbnailDrawable);
            CustomViewHelpers.Tab1.CreatorNameTextView.Text = c.Name;
            SwapView(CustomViewHelpers.Tab1.CreatorDetailView);
            NavigationStack.Tab1.AddToBackStack(c);
        }

        public static bool CreatorDetailAdapterSet;

        public static void UpdateCreatorDetailAdapter()
        {
            Tab1Handler.Post(() =>
            {
                if (!CreatorDetailAdapterSet)
                {
                    CustomViewHelpers.Tab1.CreatorDetailRecyclerView.SetAdapter(TabStates.Tab1.CreatorDetailRecyclerViewAdapter);
                    CreatorDetailAdapterSet = true;
                }
                TabStates.Tab1.CreatorDetailRecyclerViewAdapter.NotifyDataSetChanged();
            });
        }

        public static void UpdateVideoDetailPageFromVideoCard(VideoCard vc)
        {
            if (vc != null)
            {
                Tab1Handler.Post(() =>
                {
                    CustomViewHelpers.Tab1.VideoDetailTitle.Text = vc.Title;
                    CustomViewHelpers.Tab1.VideoDetailCreatorAvatarImageView.SetImageDrawable(vc.Creator?.CreatorThumbnailDrawable);
                    CustomViewHelpers.Tab1.CreatorNameTextView.Text = vc.Creator.Name;
                    CustomViewHelpers.Tab1.SubCountTextView.Text = vc.Creator?.SubCount.ToString();
                    //CustomViewHelpers.Tab1.VideoDetailCreatorAvatarImageView.SetImageBitmap(vc.Creator?.CreatorThumbnailBitmap);
                    SwapView(CustomViewHelpers.Tab1.VideoDetailView);
                });
                _vidLoader.LoadVideoFromCard(CustomViewHelpers.Tab1.VideoDetailView, null, vc, -1);
                NavigationStack.Tab1.AddToBackStack(vc);
                MainActivity.Fm1.InitializeMediaController();
            }
        }

        public static bool MainViewCommentAdapterSet;

        public static void UpdateVideoDetailCommentList()
        {
            Tab1Handler.Post(() => 
            {
                if (!MainViewCommentAdapterSet)
                {
                    CustomViewHelpers.Tab1.CommentRecyclerView.SetAdapter(TabStates.Tab1.CommentRecyclerViewAdapter);
                    MainViewCommentAdapterSet = true;
                }
                TabStates.Tab1.CreatorDetailRecyclerViewAdapter.NotifyDataSetChanged();
            });
        }
    
        public static void ReceiveCreatorDetailAdapterResult()
        {

        }

        public static void ReceiveCommentAdapterResult ()
        {

        }

        public static async Task<SubscriptionCardSet> GetSubscriptionList()
        {
            await Task.Run(() => {
                TabStates.Tab1.RootVideoCards = BitChuteAPI.Inbound.GetSubscriptionList().Result;
                });
            return CreatorCardRootSet;
        }

        public static Task<bool> ReceiveFullVideoDetails(VideoDetail vd)
        {
            Tab1Handler.Post(() =>
            {
                CustomViewHelpers.Tab1.VideoViewCountTextView.Text = vd.ViewCount.ToString();
                CustomViewHelpers.Tab1.LikeCountTextView.Text = vd.LikeCount.ToString();
                CustomViewHelpers.Tab1.DislikeCountTextView.Text = vd.DislikeCount.ToString();
                CustomViewHelpers.Tab1.VideoDescription.Text = vd.VideoDescription.ToString();
            });
            return Task.FromResult<bool>(true);
        }
        

        public static Task<bool> ReceiveVideoComments(List<Comment> commentList)
        {
            Tab1Handler.Post(() =>
            {
                CustomViewHelpers.Tab1.CommentRecyclerView.SetAdapter(
                    TabStates.Tab1.CommentRecyclerViewAdapter);
            });
            return Task.FromResult<bool>(true);
        }
        
        //public static Task<bool> ReceieveUpdatedVideoComments(List<Comment> updatedCommentList)
        //{
        //    Tab1Handler.Post(() =>
        //    {


        //        CustomViewHelpers.Tab1.CommentRecyclerView.SetAdapter(
        //            CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter);
        //    });
        //    return Task.FromResult<bool>(true);
        //}

        public static Task<bool> ReceiveVideoLikeDislikeViewCount(int likes, int dislikes, int viewCount)
        {
            Tab1Handler.Post(() =>
            {
                CustomViewHelpers.Tab1.VideoViewCountTextView.Text = viewCount.ToString();
                CustomViewHelpers.Tab1.LikeCountTextView.Text = likes.ToString();
                CustomViewHelpers.Tab1.DislikeCountTextView.Text = dislikes.ToString();
            });
            return Task.FromResult<bool>(true);
        }

        public static Task<bool> ReceiveMainCreatorSubscriberCount(int subscriberCount)
        {
            Tab1Handler.Post(() =>
            {
                CustomViewHelpers.Tab1.SubCountTextView.Text = subscriberCount.ToString();
            });
            return Task.FromResult<bool>(true);
        }

        public static bool VideoDetailChanged(List<Comment> comments, int likes, int dislikes)
        {
            if (likes != 0)
            {
                CustomViewHelpers.Tab1.LikeCountTextView.Text = likes.ToString();
            }
            if (dislikes != 0)
            {
                CustomViewHelpers.Tab1.DislikeCountTextView.Text = dislikes.ToString();
            }
            if (comments != null)
            {
                //CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter =
                //    new Adapters.CommentRecyclerViewAdapter(comments);
                //CustomViewHelpers.Tab1.CommentRecyclerView.SetAdapter(
                //    CustomViewHelpers.Tab1.CommentSystemRecyclerViewAdapter);
            }
            return false;
        }

        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void RootVideoAdapter_ItemClick(object sender, int e)
        {
            TabStates.Tab1.Creator = TabStates.Tab1.RootVideoCards[e].Creator;
            NavigationStack.Tab1.AddToBackStack(TabStates.Tab1.RootVideoCards[e].Creator);
        }

        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CreatorDetailAdapter_ItemClick(object sender, int i)
        {
            NavigateToNewPageFromVideoCard(CreatorDetailVideoCardSet[i], null);
        }

        /// <summary>
        /// click event for the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CreatorDetailAdapter_ItemClickInt(object sender, int pos)
        {
            TabStates.Tab1.VideoCardSlimLoader = TabStates.Tab1.CreatorDetailVideoCardSet[pos];
        }

        public void VideoView_OnClick(object sender, EventArgs e)
        {
            if (!VideoDetailLoader.MediaControllerDictionary.ContainsKey(MainActivity.ViewPager.CurrentItem))
            {
                VideoDetailLoader.MediaControllerDictionary.Add(MainActivity.ViewPager.CurrentItem, MainActivity.StickyService.InitializeMediaController(this.Context));
            }
            CustomViewHelpers.Tab1.VideoView.SetMediaController(VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem]);
            VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetAnchorView(CustomViewHelpers.Tab1.Tab1FragmentLayout);

            //VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetAnchorView(CustomViewHelpers.Tab1.VideoDetailView);
            //VideoDetailLoader.VideoViewDictionary[MainActivity.ViewPager.CurrentItem].SetMediaController(VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem]);

            // VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].SetMediaPlayer(VideoDetailLoader.VideoViewDictionary[MainActivity.ViewPager.CurrentItem]);
            //VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Enabled = true;
            try
            {
                VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Enabled = true;
                VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Show(6000);
                //VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Show();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        public void VideoLikeImageView_OnClick (object sender, EventArgs e)
        {

        }

        public void VideoDislikeImageView_OnClick(object sender, EventArgs e)
        {

        }

        public void FavoriteImageView_OnClick(object sender, EventArgs e)
        {

        }
        
        public void Add2PlaylistImageView_OnClick(object sender, EventArgs e)
        {

        }

        public void FlagMeImageView_OnClick(object sender, EventArgs e)
        {

        }
        
        public void P2PStatsImageView_OnClick(object sender, EventArgs e)
        {

        }

        public void ShareVideoImageView_OnClick(object sender, EventArgs e)
        {

        }

        public void SubscribeButton_OnClick(object sender, EventArgs e)
        {
            MainActivity.HideKeyboard();
        }
        
        public static void NavigateToCreatorPage(CreatorCard cc)
        {
            CreatorDetailLoader.LoadCreatorPage(CustomViewHelpers.Tab1.Tab1FragmentLayout, cc);
            //CustomViewHelpers.Tab1.CreatorDetailRecyclerViewAdapter = new Adapters.CreatorDetailRecyclerViewAdapter()
        }

        public static void CommentSystemViewAdapter_ItemClick(object sender, int e)
        {
        }

        public async static void SendCommentButton_OnClick(object sender, EventArgs e)
        {
            await BitChuteAPI.Outbound.SendComment(TabStates.Tab1.MainVideoDetail.VideoId,
                 SelectedComment.CommentId, "comment");
        }
 
        /// <summary>
        /// Navigates the selected tab to a new page
        /// </summary>
        /// <param name="view">the view you want to swap</param>
        /// <param name="videoCard">the videocard you want to load</param>
        /// <param name="creatorCard">optionally use a creatorcard for more detail</param>
        public static void NavigateToNewPageFromVideoCard(VideoCard vc, CreatorCard cc)
        {
            _vidLoader.LoadVideoFromCard(CustomViewHelpers.Tab1.VideoDetailView, cc, vc, -1);
            if (vc != null)
                NavigationStack.Tab1.AddToBackStack(vc);
            else if (cc != null)
                NavigationStack.Tab1.AddToBackStack(cc);
            
        }
        
        
        /// <summary>
        /// removes the child views and swaps for the View arg
        /// </summary>
        /// <param name="view"></param>
        public static void SwapView(View view)
        {
            CustomViewHelpers.Tab1.Tab1ParentLayout.RemoveAllViews();
            ScrollToTop();
            CustomViewHelpers.Tab1.Tab1ParentLayout.AddView(view);

        }

        public static bool ShowMediaController (bool showController)
        {
            if (VideoDetailLoader.MediaControllerDictionary.ContainsKey(MainActivity.ViewPager.CurrentItem))
            {
                if (MediaPlayerDictionary[MainActivity.ViewPager.CurrentItem].IsPlaying && CustomViewHelpers.Tab1.VideoDetailView.IsShown)
                {
                    VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Show(99999999);
                }
                else if ( CustomViewHelpers.Tab1.VideoDetailView.IsShown)
                {
                    VideoDetailLoader.MediaControllerDictionary[MainActivity.ViewPager.CurrentItem].Hide();
                    ScrollToTop();
                }
            }
            return true;
        }

        public static void ScrollToTop()
        {
            Tab1Handler.Post(() =>
            {
                CustomViewHelpers.Tab1.CreatorDetailRecyclerView.ScrollTo(0, 0);
                CustomViewHelpers.Tab1.VideoDetailScrollView.ScrollTo(0, 0);
            });
        }

        /// <summary>
        /// Navigates the selected tab to a new page
        /// </summary>
        /// <param name="view">the view you want to swap</param>
        /// <param name="type"></param>
        /// <param name=""></param>
        public static void NavigateToNewPage(View view, string linkId)
        {
            // VideoDetailLoader.LoadVideoFromDetail()
            SwapView(CustomViewHelpers.Tab1.VideoDetailView);


            //// the link will most likely be tagged with an identifier
            ////this will tell the app what type of layout to load the page link in
            //switch (linkId.Substring(0, 2))
            //{
            //    //for example, link id starts with vd for videodetail
            //    case "vd":
            //        // pass the link details to the video detail view
            //        SwapView(_videoDetailView);
            //        break;

            //}
        }

        public void CustomSetTouchListener(bool landscape)
        {
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

        public void OnSettingsChanged()
        {
            if (AppSettings.ZoomControl)
            {

            }
            else
            {

            }
        }
        
        public void SubsTabGoBack()
        {
            if (!CustomViewHelpers.Common.SoftKeyboardIsVisible)
            {
                NavigationStack.Tab1.NavigateBackFromStack();
            }
            else
            {
                CustomViewHelpers.Common.FocusCommentTextView();
            }
        }
        
        /// <summary>
        /// pops back to the root of the subscription tab
        /// </summary>
        public static void Pop2Root()
        {
            GetSubscriptionList();
            SwapView(CustomViewHelpers.Tab1.RootRecyclerView);
            NavigationStack.Tab1.AddToBackStack(true);
        }
    }
}
///// <summary>
///// click event for the adapter
///// </summary>
///// <param name="sender"></param>
///// <param name="e"></param>
//private void MAdapter_ItemClick(object sender, int e, VideoCard vc)
//{
//    SwapView(_videoDetailView);
//    var check = e;
//}