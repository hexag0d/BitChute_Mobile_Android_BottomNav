using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitChute.Adapters;
using BitChute.Classes;
using BitChute.Fragments;
using static BitChute.MainActivity;
using static BitChute.Models.CommentModel;
using static BitChute.Models.CreatorModel;
using static BitChute.Models.SubscriptionModel;
using static BitChute.Models.VideoModel;

namespace BitChute.Models
{
    /// <summary>
    /// stores the tab specific states for easy setting and getting
    /// </summary>
    public class TabStates
    {
        public static MainActivity _main;

        public static void SetMainActivity()
        {
            _main = MainActivity._main;
        }

        public class Tab1
        {
            private static VideoDetail _videoDetail;
            private static VideoCardNoCreator _videoSlimLoader;
            private static VideoCard _videoSlimLoaderFull;
            private static List<Comment> _mainVideoDetailCommentList;
            private static List<VideoCard> _rootVideoCardList;
            private static SubscriptionCardSet _rootSubCardSet;
            private static List<Models.CreatorModel.Creator> _creatorDetailSet;
            private static List<VideoModel.VideoCardNoCreator> _creatorDetailVideoCardSet;
            private static List<VideoCard> _secondaryVidCardList;
            private static Creator _mainCreator;
            private static CommentRecyclerViewAdapter _commentSystemRecyclerViewAdapter;
            private static SubscriptionRecyclerViewAdapter _subscriptionViewAdapter;
            private static CreatorDetailRecyclerViewAdapter _creatorDetailRecyclerViewAdapter;

            private static Uri _mainVideoDataSource;

            /// <summary>
            /// when set, this class will automatically get and set the video details
            /// from bitchute remote API and then set the details to applicable tab
            /// </summary>
            public static VideoDetail MainVideoDetail
            {
                get { return _videoDetail; }
                set
                {
                    _videoDetail = value;
                    //CustomViewHelpers.Tab1.CreatorAvatarImageView.SetImageDrawable(_mainCreator.CreatorThumbnailDrawable);
                    //get the full video details first from BitChute
                    SubscriptionFragment.ReceiveFullVideoDetails(_videoDetail);
                }
            }

            public static VideoCardNoCreator VideoCardNoCreatorSlimLoader
            {
                get { return _videoSlimLoader; }
                set
                {
                    _videoSlimLoader = value;
                    SubscriptionFragment.UpdateVideoDetailPageFromVideoCard(null, _videoSlimLoader);
                }
            }

            public static VideoCard VideoCardSlimLoader
            {
                get { return _videoSlimLoaderFull; }
                set
                {

                }
            }

            public static SubscriptionRecyclerViewAdapter SubscriptionRecyclerViewAdapter
            {
                get { return _subscriptionViewAdapter; }
                set { _subscriptionViewAdapter = value; }
            }

            public static CreatorDetailRecyclerViewAdapter CreatorDetailRecyclerViewAdapter
            {
                get { return _creatorDetailRecyclerViewAdapter; }
                set { _creatorDetailRecyclerViewAdapter = value; }
            }

            public static CommentRecyclerViewAdapter CommentRecyclerViewAdapter
            {
                get { return _commentSystemRecyclerViewAdapter; }
                set { _commentSystemRecyclerViewAdapter = value; }
            }

            public static SubscriptionCardSet RootVideoCards
            {
                get { return _rootSubCardSet; }
                set
                {
                    _rootSubCardSet = value;
                    if (_subscriptionViewAdapter == null)
                    {
                        _subscriptionViewAdapter =
                            new SubscriptionRecyclerViewAdapter(_rootSubCardSet);
                        _subscriptionViewAdapter.ItemClick += SubscriptionFragment.RootVideoAdapter_ItemClick;

                        SubscriptionFragment.UpdateRootAdapter();
                        //SubscriptionFragment.UpdateRootAdapterResult();
                        //_subscriptionViewAdapter.NotifyDataSetChanged();
                    }
                    else
                    {
                        SubscriptionFragment.UpdateRootAdapter();
                    }
                }
            }

            public static Creator Creator
            {
                get { return _mainCreator; }
                set
                {
                    _mainCreator = value;
                    SubscriptionFragment.UpdateCreatorPage(_mainCreator);
                    Task.Factory.StartNew(() => CreatorDetailVideoCardSet = BitChuteAPI.Inbound.GetCreatorRecentVideos(_mainCreator).Result);
                }
            }

            public static List<VideoModel.VideoCardNoCreator> CreatorDetailVideoCardSet
            {
                get { return _creatorDetailVideoCardSet; }
                set
                {
                    _creatorDetailVideoCardSet = value;
                    if (_creatorDetailRecyclerViewAdapter == null)
                    {
                        _creatorDetailRecyclerViewAdapter =
                            new CreatorDetailRecyclerViewAdapter(_creatorDetailVideoCardSet);
                        _creatorDetailRecyclerViewAdapter.ItemClick += SubscriptionFragment.CreatorDetailAdapter_ItemClickInt;
                    }
                    SubscriptionFragment.UpdateCreatorDetailAdapter();
                }
            }
            
            public class CommentSystem
            {
                public static List<Comment> MainCommentList
                {
                    get { return _mainVideoDetailCommentList; }
                    set
                    {
                        _mainVideoDetailCommentList = value;
                        if (_commentSystemRecyclerViewAdapter == null)
                        {
                            _commentSystemRecyclerViewAdapter = 
                                new Adapters.CommentRecyclerViewAdapter(_mainVideoDetailCommentList);
                            _commentSystemRecyclerViewAdapter.ItemClick += SubscriptionFragment.CommentSystemViewAdapter_ItemClick;

                        }
                        SubscriptionFragment.UpdateVideoDetailCommentList();
                    }
                }
            }
        }
    }
}

//public static void SetVideoDetail(VideoDetail vd)
//{
//    if (CurrentVideo != null)
//    {
//        CurrentVideo.VideoId = videoId;
//    }
//    else
//    {
//        CurrentVideo = new VideoDetail();
//        CurrentVideo.VideoId = videoId;
//    }
//}
//public static string GetVideoId()
//{
//    if (CurrentVideo != null)
//    {
//        return CurrentVideo.VideoId;
//    }
//    else
//    {
//        return "No Video Loaded";
//    }
//}
//public static VideoDetail CurrentVideo { get; set; }