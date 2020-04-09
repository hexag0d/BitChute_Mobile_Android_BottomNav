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
using BitChute.Classes;
using BitChute.ViewHolders;
using static BitChute.Models.CreatorModel;
using static BitChute.Models.VideoModel;

namespace BitChute.Adapters
{

    public class CreatorDetailRecyclerViewAdapter : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public static VideoCardSet _videoCardSet;
        public static List<VideoCardNoCreator> VideoCardNoCreators = new List<VideoCardNoCreator>();
        public static View itemView;

        public static CreatorDetailViewHolder vh;

        public CreatorDetailRecyclerViewAdapter(VideoCardSet videoCardSet)
        {
            if (videoCardSet == null)
            {
                videoCardSet = new VideoCardSet();
            }
            _videoCardSet = videoCardSet;
        }

        public CreatorDetailRecyclerViewAdapter(List<VideoCardNoCreator> videoCardNoCreators)
        {
            VideoCardNoCreators = videoCardNoCreators;
        }

        public CreatorDetailRecyclerViewAdapter(List<Models.CreatorModel.Creator> creatorSet)
        {
            foreach (var creator in creatorSet)
            {
                foreach (var vidcard in creator.RecentVideoCards)
                {
                    vidcard.CreatorName = creator.Name;
                    VideoCardNoCreators.Add(vidcard);
                }
            }
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                            Inflate(Resource.Layout.CreatorDetailCardView, parent, false);

            CardView cv = itemView.FindViewById<CardView>(Resource.Id.creatorDetailVideoCardView);
            cv.SetBackgroundColor(AppSettings.Themes.Colors.DarkGrey);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            vh = new CreatorDetailViewHolder(itemView, OnClick);

            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as CreatorDetailViewHolder;

            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the photo album:
            if (_videoCardSet != null)
            {
                vh.Image.SetImageResource(_videoCardSet[position].PhotoID);
                vh.Caption.Text = _videoCardSet[position].Title;
                vh.Caption2.Text = _videoCardSet[position].CreatorName;
            }
            else
            {
                vh.Image.SetImageResource(VideoCardNoCreators[position].VideoThumbnail);
                vh.Caption.Text = VideoCardNoCreators[position].Title;
                vh.Caption2.Text = VideoCardNoCreators[position].CreatorName;
            }
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get
            {
                if (_videoCardSet != null)
                    return _videoCardSet.NumPhotos;
                else
                    return 16;
            }
        }

        void OnClick (int position)
        {
            var pos = position;
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}