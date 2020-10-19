using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using static Android.Support.V4.Content.ContextCompat;
using static Android.Support.V4.App.ActivityCompat;
using static Android.Manifest.Permission;
using Android.Content.PM;
using BitChute;

namespace BitChute
{
    public class FileBrowser
    {
        private static Android.Graphics.Color _darkGrey = new Android.Graphics.Color(20, 20, 20);
        public static FileRecyclerViewAdapter FileAdapter;
        private static int _requestCode;
        public static FileBrowser StatBrowser = new FileBrowser();
        public delegate void FileChooserEventDelegate(FileChooserArgs _args);
        public event FileChooserEventDelegate Selected;
        public static string LatestFilePath = "";
        public static string LatestSendTo;
        public FileBrowser() { this.Selected += OnFileSelected; }
        public static string WorkingDirectory = Android.OS.Environment.ExternalStorageDirectory.Path + @"/";
        public static string FullDefaultSavePath = WorkingDirectory + @"Download/";

        public static void FileBrowserButton_OnClick(object sender, EventArgs e)
        {
            OpenFileBrowser();
        }

        /// <summary>
        /// sendto = "encoder", "static", "uploader"
        /// </summary>
        /// <param name="sendTo"></param>
        public static void ShowFileChooser(string sendTo = null)
        {
            Intent intent = new Intent(Intent.ActionGetContent);
            if (sendTo != null) { LatestSendTo = sendTo; intent.PutExtra("sendTo", sendTo); }
            intent.SetType("*/*");
            
            // Update with additional mime types here using a String[]. 
            //intent.PutExtra(Intent.ExtraMimeTypes, );
            
            intent.AddCategory(Intent.CategoryOpenable);
            intent.PutExtra(Intent.ExtraLocalOnly, true);
            
            MainActivity.Main.StartActivityForResult(intent, _requestCode);
        }

        /// <summary>
        /// sendto = "encoder", "uploader"
        /// </summary>
        public class FileChooserArgs : EventArgs
        {
            private string _path;
            private bool _cancelled;
            private string _sendTo;
            private Android.Net.Uri _uri;
            public FileChooserArgs(string path = null, string sendTo = null, bool cancelled = false, Android.Net.Uri uri = null)
            {
                if (path != null) { _path = path; }
                _cancelled = cancelled;
                if (sendTo != null) { _sendTo = sendTo; }
                if (uri != null) { _uri = uri; }
            }
            public string Path { get { return _path; } }
            public bool Cancelled { get { return _cancelled; } }
            public string SendTo { get { return _sendTo; } }
            public Android.Net.Uri Uri { get { return _uri; } }
        }

        public static void OnFileSelected(FileChooserArgs e)
        {
            if (e.Path != null || e.Path != "") { LatestFilePath = e.Path; }
            if (e.SendTo != null)
            {
                if (e.SendTo == "encoder") {
                    ViewHelpers.VideoEncoder.EncoderSourceEditText.Text = e?.Path;
                    ViewHelpers.VideoEncoder.EncoderSourceEditText.Text = e?.Uri.Path;
                    MediaCodecHelper.FileToMp4.InputUriToEncode = e?.Uri;
                }
                else if (e.SendTo == "uploader") {  }
            }
        }
        
        public static string ImportFileToString(Android.Net.Uri uri, string sendToIntent, Context ctx)
        {
            string decodedPath = "";
            decodedPath = UriDecoder.ConvertUriToString(uri);
            StatBrowser.Selected.Invoke(new FileChooserArgs(decodedPath, sendToIntent));
            return decodedPath;
        }

        public static Android.Net.Uri ImportFileToUri(Android.Net.Uri uri, string sendToIntent, Context ctx)
        {
            StatBrowser.Selected.Invoke(new FileChooserArgs(null, sendToIntent, false, uri));
            return uri;
        }

        public static void SaveFileToStorage(Java.IO.File f)
        {
            if (f == null) { return; }
            try
            {
                Java.IO.FileOutputStream fos = new Java.IO.FileOutputStream(f);
                fos.Close();
            }
            catch (FileNotFoundException e) { }
            catch (IOException e) { }
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

        public static bool GetExternalPermissions()
        {
            bool permissionGranted;
            if (CheckSelfPermission(MainActivity.Main, WriteExternalStorage) != (int)Permission.Granted)
            {
                RequestPermissions(MainActivity.Main, new string[] { WriteExternalStorage }, 0);
                permissionGranted = false;
            }
            else { permissionGranted = true; }
            if (CheckSelfPermission(MainActivity.Main, ReadExternalStorage) != (int)Permission.Granted)
            {
                RequestPermissions(MainActivity.Main, new string[] { ReadExternalStorage }, 0);
                permissionGranted = false;
            }
            else { permissionGranted = true; }
            return permissionGranted;
        }

        public static List<string> GetLocalVideos()
        {
            List<string> files = new List<string>();
            if (!GetExternalPermissions()) { return files; }
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
