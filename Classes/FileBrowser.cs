using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BottomNavigationViewPager.Classes
{
    public class FileBrowser
    {
        public static void GetFolders()
        {
            var folder = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "download";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filesList = Directory.GetFiles(folder);
            foreach (var file in filesList)
            {
                var filename = Path.GetFileName(file);
            }
        }

        public class FileViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
        {
            public ImageView Image { get; private set; }
            public TextView Caption { get; private set; }
            public TextView Caption2 { get; private set; }

            public void GetControls()
            {

            }

            // Get references to the views defined in the CardView layout.
            public FileViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                // Locate and cache view references:
                //Image = itemView.FindViewById<ImageView>(Resource.Id.feedImageView);
                //Caption = itemView.FindViewById<TextView>(Resource.Id.feedTitleCaptionTextView);
                //Caption2 = itemView.FindViewById<TextView>(Resource.Id.feedNameCaptionTextView);

                // Detect user clicks on the item view and report which item
                // was clicked (by layout position) to the listener:
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}
