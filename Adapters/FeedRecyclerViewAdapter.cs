
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
    public event EventHandler<KeyValuePair<int,VideoCard>> ItemClick;
    //public static VideoCardSet _videoCardSet;
    private List<VideoCard> _videoCardNoCreators;

    public void UpdateDataSet(VideoCard vc)
    {
        if (_videoCardNoCreators == null) { _videoCardNoCreators = new List<VideoCard>(); }
        _videoCardNoCreators.Add(vc);
        this.NotifyDataSetChanged();
    }

    public List<VideoCard> UpdateDataSet(List<VideoCard> vcl, bool addToList = false)
    {
        if (_videoCardNoCreators == null) { _videoCardNoCreators = new List<VideoCard>(); }
        if (addToList) { _videoCardNoCreators.AddRange(vcl); }
        this.NotifyDataSetChanged();
        return _videoCardNoCreators;
    }

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

    //public async System.Threading.Tasks.Task<FeedViewHolder> GetViewHolder(RecyclerView.ViewHolder holder, int position)
    //{
    //    //var bmp = BitChute.Web.Request.GetBitmapDrawable(_videoCardNoCreators[position].ThumbnailPath).Result;

    //         vh1 = holder as FeedViewHolder;
    //        vh1.Image.SetImageBitmap(_videoCardNoCreators[position].ThumbnailBitmap);
    //        vh1.Caption.Text = _videoCardNoCreators[position].Title;
    //        vh1.Caption2.Text = _videoCardNoCreators[position].CreatorName;
            

    //    return vh1;
    //}

    // Fill in the contents of the photo card (invoked by the layout manager):
    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        vh = holder as FeedViewHolder;

        //GetViewHolder(holder, position);
        if (_videoCardNoCreators != null)
        {
            vh.Image.SetImageBitmap(_videoCardNoCreators[position].ThumbnailBitmap);
            vh.Caption.Text = _videoCardNoCreators[position].Title;
            vh.Caption2.Text = _videoCardNoCreators[position].CreatorName;
        }
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
        if (ItemClick != null)
        {
            ItemClick(this, new KeyValuePair<int, VideoCard>(position, _videoCardNoCreators[position]));
               
        }           
    }
}
