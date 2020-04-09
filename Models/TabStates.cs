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
        private static MainActivity _main;
        private static int _mediaPlayerHasFocus = -1;
        private static List<bool> _initializedPlayers = new List<bool> { false, false, false, false, false };

        public static void SetMainActivity()
        {
            _main = MainActivity.Main;
        }

        /// <summary>
        /// Keeps track of which media player is currently playing media
        /// </summary>
        /// <param name="tab">use -1 to get the currently playing media player without setting a new one</param>
        /// <returns></returns>
        public static int MediaTabHasFocus (int tab)
        {
            // tab out of range, do nothing
            if (tab > 4 || tab < -1)
            {
                return _mediaPlayerHasFocus;
            }
            //app requested current media player
            if (tab != -1)
            {
                _mediaPlayerHasFocus = tab;
            }
            else
            {
                _mediaPlayerHasFocus = tab;
            }
            return _mediaPlayerHasFocus;
        }

        /// <summary>
        /// keeps track of which media players have been initialized
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static List<bool> InitializedPlayers(int tab)
        {
            if (tab == -1)
            {
                return _initializedPlayers;
            }
            else
            {
                _initializedPlayers[tab] = true;
            }
            return _initializedPlayers;
        } 
        

        public class Tab1
        {
            private static VideoDetail _videoDetail;
            private static VideoCard _videoSlimLoader;
            private static VideoCard _videoSlimLoaderFull;
            private static List<Comment> _mainVideoDetailCommentList;
            private static List<VideoCard> _rootVideoCardList;
            private static SubscriptionCardSet _rootSubCardSet;
            private static List<Models.CreatorModel.Creator> _creatorDetailSet;
            private static List<VideoModel.VideoCard> _creatorDetailVideoCardSet;
            private static List<VideoCard> _secondaryVidCardList;
            private static Creator _mainCreator;
            private static CommentRecyclerViewAdapter _commentSystemRecyclerViewAdapter;
            private static SubscriptionRecyclerViewAdapter _subscriptionViewAdapter;
            private static CreatorDetailRecyclerViewAdapter _creatorDetailRecyclerViewAdapter;

            public static bool MediaPlayerInitialized;
            public static bool VideoIsPlaying;

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

            public static VideoCard VideoCardSlimLoader
            {
                get { return _videoSlimLoader; }
                set
                {
                    _videoSlimLoader = value;
                    SubscriptionFragment.UpdateVideoDetailPageFromVideoCard(_videoSlimLoader);
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

            public static List<VideoModel.VideoCard> CreatorDetailVideoCardSet
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