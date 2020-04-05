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
using BitChute.Models;
using BitChute.ViewHolders;
using static BitChute.Models.CommentModel;
using static BitChute.Models.CreatorModel;

namespace BitChute.Adapters
{
    public class CommentRecyclerViewAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> AvatarClick;

        public List<Comment> _commentList = SampleCommentList.GetSampleCommentList();
        public static View itemView;

        public static CommentSystemRecyclerViewHolder vh;

        public CommentRecyclerViewAdapter(List<Comment> comments)
        {
            if (_commentList == null)
            {

            }
            _commentList = comments;
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                            Inflate(Resource.Layout.CommentCardView, parent, false);

            CardView cv = itemView.FindViewById<CardView>(Resource.Id.commentSystemCardView);

            cv.SetBackgroundColor(AppSettings.Themes.SelectedTheme.CommentBackground);
            vh = new CommentSystemRecyclerViewHolder(itemView, OnClick, AvatarOnClick);

            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as CommentSystemRecyclerViewHolder;

            vh.CreatorNameTextView.Text = _commentList[position].CommenterName;
            vh.CommentTextView.Text = _commentList[position].CommentText;

            if (_commentList[position].CommenterAvatarDrawable != null)
            {
                vh.CreatorAvatar.SetImageDrawable(_commentList[position].CommenterAvatarDrawable);
            }
            else if (_commentList[position].Creator != null)
            {
                if (_commentList[position].Creator.CreatorThumbnailDrawable != null)
                vh.CreatorAvatar.SetImageDrawable(_commentList[position].Creator.CreatorThumbnailDrawable);
            }
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get
            {
                if (_commentList != null)
                    return _commentList.Count;
                else
                    return 6;
            }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        /// <summary>
        /// if the user clicks on an avatar then we'll take them to that creator's channel.  
        /// if the commentor doesn't have a channel then we'll display a message saying that
        /// </summary>
        /// <param name="position"></param>
        void AvatarOnClick(int position)
        {
            if (AvatarClick != null)
                AvatarClick(this, position);

            if (_commentList[position].Creator != null)
            {

                switch (MainActivity.ViewPager.CurrentItem)
                {
                    case 0:
                        break;
                    case 1:
                        TabStates.Tab1.Creator = TabStates.Tab1.CommentSystem.MainCommentList[position].Creator;
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }
                return;
            }
            Toast.MakeText(Android.App.Application.Context, "User has no channel; ask them to upload something!", ToastLength.Short).Show(); //do toast saying commenter has no channel ... maybe they should join BitChute
        }
    }
}