using System;
using System.Collections.Generic;
using System.IO;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace BitChute.Classes
{
    public class FileBrowser
    {
        private static Android.Graphics.Color _darkGrey = new Android.Graphics.Color(20, 20, 20);
        public static FileRecyclerViewAdapter FileAdapter;
        public static string DefaultWorkingDirectory = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";

        public static void FileBrowserButton_OnClick(object sender, EventArgs e)
        {
            OpenFileBrowser();
        }

        public static void SaveFileToStorage(Java.IO.File f)
        {
            if (f == null)
            {
                return;
            }
            try
            {
                Java.IO.FileOutputStream fos = new Java.IO.FileOutputStream(f);
                fos.Close();
            }
            catch (FileNotFoundException e)
            { }
            catch (IOException e)
            { }
        }

        public static void OpenFileBrowser()
        {
            if (ViewHelpers.Tab3.FileLayoutManager == null)
            ViewHelpers.Tab3.FileLayoutManager = new LinearLayoutManager(Android.App.Application.Context);
            ViewHelpers.Tab3.FileRecyclerView.SetLayoutManager(ViewHelpers.Tab3.FileLayoutManager);
            if (ViewHelpers.Tab3.FileRecyclerView.GetAdapter() == null)
            {
                FileAdapter = new FileRecyclerViewAdapter(GetLocalVideos());
                ViewHelpers.Tab3.FileRecyclerView.SetAdapter(FileAdapter);
            }
            else
            {
                FileAdapter = new FileRecyclerViewAdapter(GetLocalVideos());
                FileAdapter.NotifyDataSetChanged();
            }
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

        public static List<string> GetLocalVideos()
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
            
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).
                                Inflate(Resource.Layout.FileCardView, parent, false);
                CardView cv = itemView.FindViewById<CardView>(Resource.Id.fileCardView);
                cv.SetBackgroundColor(_darkGrey);
                vh = new FileViewHolder(itemView, OnClick);

                return vh;
            }
            
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                vh = holder as FileViewHolder;
                if (FileTitleList != null)
                {
                    vh.Caption.Text = FileTitleList[position];
                }
            }
            
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
            
            void OnClick(int position)
            {
                var pos = position;
                if (ItemClick != null)
                    ItemClick(this, position);
            }
        }
    }
}
