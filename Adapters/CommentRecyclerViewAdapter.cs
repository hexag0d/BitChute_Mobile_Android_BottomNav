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
using BitChute.Models;
using BitChute.ViewHolders;
using static BitChute.Models.CommentModel;

namespace BitChute.Adapters
{
        public class CommentRecyclerViewAdapter : RecyclerView.Adapter
        {
            public event EventHandler<int> ItemClick;
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

                vh = new CommentSystemRecyclerViewHolder(itemView, OnClick);

                return vh;
            }

            // Fill in the contents of the photo card (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                vh = holder as CommentSystemRecyclerViewHolder;

                // Set the ImageView and TextView in this ViewHolder's CardView 
                // from this position in the photo album:
                if (_commentList != null)
                {
                    vh.CreatorAvatar.SetImageResource(CommentModel.GetBlankAvatarInt());
                    vh.CreatorNameTextView.Text = _commentList[position].CommenterName;
                    vh.CommentTextView.Text = _commentList[position].CommentText;
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
                var pos = position;
                if (ItemClick != null)
                    ItemClick(this, position);
            }
        }
    
}