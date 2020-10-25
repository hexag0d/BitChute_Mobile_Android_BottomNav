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

namespace BitChute.ViewHolders
{
    // Implement the ViewHolder pattern: each ViewHolder holds references
    // to the UI components (ImageView and TextView) within the CardView 
    // that is displayed in a row of the RecyclerView:
    public class FeedViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Caption { get; private set; }
        public TextView Caption2 { get; private set; }

        public void GetControls()
        {

        }

        // Get references to the views defined in the CardView layout.
        public FeedViewHolder(View itemView, Action<KeyValuePair<int, KeyValuePair<int, string>>> listener) : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.feedImageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.feedTitleCaptionTextView);
            Caption2 = itemView.FindViewById<TextView>(Resource.Id.feedNameCaptionTextView);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            //itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }

        // Get references to the views defined in the CardView layout.
        public FeedViewHolder(View itemView, Action<KeyValuePair<int, string>> listener) : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.feedImageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.feedTitleCaptionTextView);
            Caption2 = itemView.FindViewById<TextView>(Resource.Id.feedNameCaptionTextView);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            //itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }

        // Get references to the views defined in the CardView layout.
        public FeedViewHolder(View itemView, Action<string> listener) : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.feedImageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.feedTitleCaptionTextView);
            Caption2 = itemView.FindViewById<TextView>(Resource.Id.feedNameCaptionTextView);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            
        }

        // Get references to the views defined in the CardView layout.
        public FeedViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.feedImageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.feedTitleCaptionTextView);
            Caption2 = itemView.FindViewById<TextView>(Resource.Id.feedNameCaptionTextView);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}