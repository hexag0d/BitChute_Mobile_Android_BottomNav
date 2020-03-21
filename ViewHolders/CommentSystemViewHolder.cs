﻿using System;
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

namespace BitChute.ViewHolders
{    // Implement the ViewHolder pattern: each ViewHolder holds references
    // to the UI components (ImageView and TextView) within the CardView 
    // that is displayed in a row of the RecyclerView:
    public class CommentSystemRecyclerViewHolder : RecyclerView.ViewHolder
    {
        public ImageView CreatorAvatar { get; private set; }
        public TextView CommentTextView { get; private set; }
        public TextView CreatorNameTextView { get; private set; }

        public void GetControls()
        {
        }

        // Get references to the views defined in the CardView layout.
        public CommentSystemRecyclerViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            // Locate and cache view references:
            CreatorAvatar = itemView.FindViewById<ImageView>(Resource.Id.commenterImageView);
            CommentTextView = itemView.FindViewById<TextView>(Resource.Id.commentContentsTextView);
            CreatorNameTextView = itemView.FindViewById<TextView>(Resource.Id.commenterNameTextView);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}