using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace BottomNavigationViewPager.Classes
{
    public class FileBrowser
    {
        private static Android.Graphics.Color _darkGrey = new Android.Graphics.Color(20, 20, 20);

        public static void FileBrowserButton_OnClick(object sender, EventArgs e)
        {
            OpenFileBrowser();
        }

        public static void OpenFileBrowser()
        {
            var mLayoutManager = new LinearLayoutManager(Android.App.Application.Context);
            ViewHelpers.Tab3.FileRecyclerView.SetLayoutManager(mLayoutManager);
            var adapter = new FileRecyclerViewAdapter(GetFolders());
            ViewHelpers.Tab3.FileRecyclerView.SetAdapter(adapter);
        }

        public static void GetExternalPermissions()
        {
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(MainActivity.Main, Android.Manifest.Permission.WriteExternalStorage) != (int)Android.Content.PM.Permission.Granted)
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(MainActivity.Main, new string[] { Android.Manifest.Permission.WriteExternalStorage }, 0);
            }

            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(MainActivity.Main, Android.Manifest.Permission.ReadExternalStorage) != (int)Android.Content.PM.Permission.Granted)
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(MainActivity.Main, new string[] { Android.Manifest.Permission.ReadExternalStorage }, 0);
            }
        }

        public static List<string> GetFolders()
        {
            GetExternalPermissions();
            List<string> files = new List<string>();
            var folder = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "download";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filesList = Directory.GetFiles(folder);
            foreach (var file in filesList)
            {
                var filename = Path.GetFileName(file);
                files.Add(filename);
            }
            return files;
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
                Caption = itemView.FindViewById<TextView>(Resource.Id.videoThumbnailTitle);
                // Detect user clicks on the item view and report which item
                // was clicked (by layout position) to the listener:
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

        public class FileRecyclerViewAdapter : RecyclerView.Adapter
        {
            public event EventHandler<int> ItemClick;
            public static List<string> FileTitleList = new List<string>();
            public static View itemView;
            public static FileViewHolder vh;

            public FileRecyclerViewAdapter(List<string> fileSet)
            {
                FileTitleList = fileSet;
            }

            // Create a new photo CardView (invoked by the layout manager): 
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                // Inflate the CardView for the photo:
                View itemView = LayoutInflater.From(parent.Context).
                                Inflate(Resource.Layout.FileCardView, parent, false);

                CardView cv = itemView.FindViewById<CardView>(Resource.Id.fileCardView);
                cv.SetBackgroundColor(_darkGrey);

                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                vh = new FileViewHolder(itemView, OnClick);

                return vh;
            }

            // Fill in the contents of the photo card (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                vh = holder as FileViewHolder;

                // Set the ImageView and TextView in this ViewHolder's CardView 
                // from this position in the photo album:
                if (FileTitleList != null)
                {
                    vh.Caption.Text = FileTitleList[position];
                }
            }

            // Return the number of photos available in the photo album:
            public override int ItemCount
            {
                get
                {
                    if (FileTitleList != null)
                        return FileTitleList.Count;
                    else
                        return 0;
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
}
