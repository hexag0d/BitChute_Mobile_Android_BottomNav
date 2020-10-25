
using Android;
using Android.Support.V7.Widget;
using Android.Views;
using BitChute;
using BitChute.ViewHolders;
using System;
using System.Collections.Generic;
using static BitChute.Models.VideoModel;

public class FeedRecyclerViewAdapter : RecyclerView.Adapter
{
    public event EventHandler<int> ItemClick;
    //public static VideoCardSet _videoCardSet;
    public static List<VideoCard> _videoCardNoCreators = new List<VideoCard>();
    public static View itemView;

    public static FeedViewHolder vh;

    //public FeedRecyclerViewAdapter(VideoCardSet videoCardSet)
    //{
    //    if (videoCardSet == null)
    //    {
    //        videoCardSet = new VideoCardSet();
    //    }
    //    _videoCardSet = videoCardSet;
    //}

    public FeedRecyclerViewAdapter(List<VideoCard> videoCardNoCreators)
    {
        _videoCardNoCreators = videoCardNoCreators;
    }

    public FeedRecyclerViewAdapter(List<BitChute.Models.CreatorModel.Creator> creatorSet)
    {
        foreach (var creator in creatorSet)
        {
            foreach (var vidcard in creator.RecentVideoCards)
            {
                vidcard.Creator.Name = creator.Name;
                _videoCardNoCreators.Add(vidcard);
            }
        }
    }

    // Create a new photo CardView (invoked by the layout manager): 
    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        
        // Inflate the CardView for the photo:
        View itemView = LayoutInflater.From(BitChute.MainActivity.GetMainContext()).Inflate(BitChute.Resource.Layout.FeedCardView, parent, false);

        CardView cv = itemView.FindViewById<CardView>(BitChute.Resource.Id.feedCardView);

        cv.SetBackgroundColor(AppState.Display.DarkGrey);

        // Create a ViewHolder to find and hold these view references, and 
        // register OnClick with the view holder:
        vh = new FeedViewHolder(itemView, OnClick);

        return vh;
    }

    // Fill in the contents of the photo card (invoked by the layout manager):
    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        vh = holder as FeedViewHolder;

        if (_videoCardNoCreators != null)
        {
            //vh.Image.SetImageURI(_videoCardNoCreators[position].ThumbnailUri);
            vh.Image.SetImageBitmap(_videoCardNoCreators[position].ThumbnailBitmap);
            vh.Caption.Text = _videoCardNoCreators[position].Title;
            vh.Caption2.Text = _videoCardNoCreators[position].CreatorName;
        }
        // Set the ImageView and TextView in this ViewHolder's CardView 
        // from this position in the photo album:
        //if (_videoCardSet != null)
        //{
        //    vh.Image.SetImageBitmap()
        //    vh.Caption.Text = _videoCardSet[position].Title;
        //    vh.Caption2.Text = _videoCardSet[position].Creator.Name;
        //}
    }

    // Return the number of photos available in the photo album:
    public override int ItemCount
    {
        get
        {
            if (_videoCardNoCreators != null)
                return _videoCardNoCreators.Count;
            else
                return 6;
        }
    }

    // Raise an event when the item-click takes place:
    void OnClick(int position)
    {
        var pos = position;
        if (ItemClick != null)
            ItemClick(this, position);
    }
}
