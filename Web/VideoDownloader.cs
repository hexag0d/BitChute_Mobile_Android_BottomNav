﻿using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Widget;
using BitChute.Fragments;
using HtmlAgilityPack;
using BitChute.Services;
using static BitChute.Models.VideoModel;
using BitChute.Web;

namespace BitChute
{
    [Service(Exported = true)]
    public class VideoDownloader : Service
    {
        public static bool LatestDownloadSucceeded;
        public static bool VideoDownloadInProgress;
        static System.Int64 bytes_total;
        private static ExtWebClient _wc;
        static bool _videoDownloadCancelledByUser = false;
        //private static System.Net.WebClient _wc;
        

        public static void VideoDownloadButton_OnClick(object sender, System.EventArgs e)
        {
            ViewHelpers.Tab3.DownloadProgressTextView.Text = "Initializing Download";
            Toast.MakeText(Android.App.Application.Context, "Download started",ToastLength.Long);
            InitializeVideoDownload(ViewHelpers.Tab3.DownloadLinkEditText.Text);
        }

        public static void DownloadFAB_OnClick(object sender, System.EventArgs e)
        {
            InitializeVideoDownload(GetVideoUrlByTab(MainActivity.ViewPager.CurrentItem));
        }

        public static string GetVideoUrlByTab(int tab)
        {
            string taburl = "";
            taburl = CommonFrag.GetFragmentById(-1, null, tab)?.Wv.OriginalUrl;
            return taburl;
        }

        public static async void InitializeVideoDownload(string videoLink)
        {
            if (!VideoDownloadInProgress)
            {
                try
                {
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = "Getting permissions";
                    if (!FileBrowser.GetExternalPermissions()) { return; }
                    VideoDownloadInProgress = true;
                    VideoDownloader _vd = new VideoDownloader();
                    if (videoLink != null && videoLink != "")
                    {
                        ViewHelpers.Tab3.DownloadProgressTextView.Text = "Getting video link";
                        Task<string> rawHtmlTask = ExtWebInterface.GetHtmlTextFromUrl(videoLink);
                        var html =  await rawHtmlTask;
                        Task<VideoCard> videoCardTask = _vd.DecodeHtmlVideoSource(html);
                        var vidCard = await videoCardTask;
                        if ((vidCard).VideoUri.AbsolutePath == ""
                            || (vidCard).VideoUri.AbsolutePath == null)
                        {
                            ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.VideoSourceMissing();
                            Toast.MakeText(Android.App.Application.Context, LanguageSupport.Main.IO.VideoSourceMissing(), ToastLength.Long);
                            VideoDownloadInProgress = false;
                            return;
                        }
                        Task<bool> videoDownloadComplete = _vd.DownloadAndSaveVideo(vidCard);
                        await videoDownloadComplete;
                    }
                    else
                    {
                        await _vd.DownloadAndSaveVideo(null);
                    }
                }
                catch (Exception ex)
                {
                    VideoDownloadInProgress = false;
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = $"An error occured : {ex.Message} "; 
                }
            }
            else
            {
                try
                {
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = "Video download already in progress, stop it first";
                    Toast.MakeText(Android.App.Application.Context, "Video download already in progress, stop it first", ToastLength.Long);
                }
                catch { }
            }
        }

        public async Task<VideoCard> DecodeHtmlVideoSource(string html)
        {
            VideoCard vidCard = new VideoCard();
            string videoLink = "";
            string vidTitle = "";
            await Task.Run(() =>
            {
                try
                {
                    //if the source ends with .mp4 then we've got a raw link
                    //and can immediately return
                    if (html.EndsWith(@".mp4"))
                    {
                        vidCard.VideoUri = new System.Uri(html);
                        vidCard.Title = html.Replace(@".mp4", "");
                    }
                    //source didn't end in .mp4 so we need to decode the link
                    else
                    {
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(html);
                        
                        if (doc != null)
                        {
                            var check = doc.DocumentNode;
                            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//source"))
                            {
                                videoLink = node.Attributes["src"].Value.ToString();
                                if (videoLink != null || videoLink != "")
                                {
                                    vidCard.VideoUri = new System.Uri(videoLink);
                                }
                            }
                            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//title"))
                            {
                                vidTitle = node.InnerText;
                                if (vidTitle != null || vidTitle != "")
                                {
                                    vidCard.Title = vidTitle;
                                }
                            }
                        }
                    }
                }
                catch
                { 
                }
            });
            return vidCard;
        }



        public VideoDownloader(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public VideoDownloader()
        {
        }

        /// <summary>
        /// these are just for fun, they allow the progress bar to change colors
        /// </summary>
        private static int _progRed = 50;
        private static int _progBlue = 50;
        private static int _progGreen = 20;
        private static Android.Graphics.Color _progColor;
        private static bool _progBlueUp = true;

        private static int _progStep = 0;
        private static string _progText;
        private static string _progString;
        static decimal _progress;

        public static void OnVideoDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (bytes_total > 0)
            {
                 _progress = ((decimal)e.BytesReceived / (decimal)bytes_total) * 100;
            }
            if (_progress < 100)
            _progString = _progress.ToString().Substring(0, 4);
            switch (_progStep)
            {
                case 0:
                    _progText = "Downloading   " + _progString + @"%";
                    _progStep++;
                    break;
                case 1:
                    _progText = "Downloading.  " + _progString + @"%";
                    _progStep++;
                    break;
                case 2:
                    _progText = "Downloading.. " + _progString + @"%";
                    _progStep++;
                    break;
                case 3:
                    _progText = "Downloading..." + _progString + @"%";
                    _progStep = 0;
                    break;
            }
            ViewHelpers.Tab3.DownloadProgressTextView.Text = (_progText);
            _progColor = Android.Graphics.Color.Rgb(_progRed, _progGreen, _progBlue);
            ViewHelpers.Tab3.DownloadProgressTextView.SetTextColor(_progColor);
            ViewHelpers.Main.DownloadFAB.SetColorFilter(_progColor);
            ViewHelpers.Tab3.DownloadProgressBar.Progress = e.ProgressPercentage;
            ViewHelpers.Tab3.DownloadProgressBar.ProgressDrawable
                .SetColorFilter(_progColor,
                Android.Graphics.PorterDuff.Mode.Multiply);
            ViewHelpers.Tab3.DownloadProgressBar.IndeterminateDrawable
                .SetColorFilter(_progColor,
                Android.Graphics.PorterDuff.Mode.SrcAtop);
            if (_progBlueUp)
            {
                if (_progBlue <= 250)
                {
                    _progBlue++;
                }
                else
                {
                    _progBlue--;
                    _progBlueUp = false;
                }
            }
            else
            {
                if (_progBlue >= 50)
                {
                    _progBlue--;
                }
                else
                {
                    _progBlue++;
                    _progBlueUp = true;
                }
            }
        }

        public static void OnVideoDownloadFinished(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ViewHelpers.Tab3.DownloadProgressBar.Indeterminate = true;
            _progBlue = 150;
            _progBlueUp = true;
            _progColor = Android.Graphics.Color.Rgb(_progRed, _progGreen, _progBlue);
            ViewHelpers.Main.DownloadFAB.SetColorFilter(null); 
            ViewHelpers.Tab3.DownloadProgressTextView.SetTextColor(_progColor);
            if (e.Cancelled) { ViewHelpers.Tab3.DownloadProgressTextView.Text = "video download aborted by user"; }
            else if (e.Error != null) { }
            else { ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.FileDownloadSuccess(); }
        }

        /// <summary>
        /// uri or url can be null but not both.
        /// 
        /// returns true if the download succeded or file already exists
        /// </summary>
        /// <param name="url"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<bool> DownloadAndSaveVideoWebRequest(VideoCard vc)
        {
            ViewHelpers.Tab3.DownloadProgressTextView.Text = "Starting download";
            _wc = new ExtWebClient();
            _wc.DownloadProgressChanged += OnVideoDownloadProgressChanged;
            _wc.DownloadFileCompleted += OnVideoDownloadFinished;
            var documentsPath = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";
            string filePath;
            if (ViewHelpers.Tab3.AutoFillVideoTitleText.Checked)
            {
                filePath = string.Join("_", vc.Title.Split(System.IO.Path.GetInvalidFileNameChars()));
                ViewHelpers.Tab3.DownloadFileNameEditText.Text = filePath;
                filePath = documentsPath + filePath;
            }
            else
            {
                filePath = documentsPath + ViewHelpers.Tab3.DownloadFileNameEditText.Text;
            }
            if (!filePath.EndsWith(@".mp4"))
            {
                filePath = filePath + @".mp4";
            }
            if (vc != null)
            {
                if ((vc.Link == null || vc.Link == "") && (vc.VideoUri == null || vc.VideoUri.AbsolutePath == ""))
                {
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.VideoSourceMissing();
                    Toast.MakeText(Android.App.Application.Context, LanguageSupport.Main.IO.VideoSourceMissing(), ToastLength.Long);
                    return false;
                }
            }
            try
            {
                if (vc.VideoUri != null)
                {
                    await Task.Run(() =>
                    {
                        _wc.OpenRead(vc.VideoUri);
                        bytes_total = System.Convert.ToInt64(_wc.ResponseHeaders["Content-Length"]);
                    });
                    ViewHelpers.Tab3.DownloadProgressBar.Indeterminate = false;
                    await _wc.DownloadFileTaskAsync(vc.VideoUri,
                        filePath);
                }
                else
                {
                    await Task.Run(() =>
                    {
                        _wc.OpenRead(new System.Uri(vc.Link));
                        bytes_total = System.Convert.ToInt64(_wc.ResponseHeaders["Content-Length"]);
                    });
                    ViewHelpers.Tab3.DownloadProgressBar.Indeterminate = false;
                    await _wc.DownloadFileTaskAsync(new System.Uri(vc.Link),
                        filePath);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.InnerException);
                ViewHelpers.Tab3.DownloadProgressBar.Indeterminate = true;
            }
            VideoDownloadInProgress = false;
            if (System.IO.File.Exists(filePath))
            {
                Toast.MakeText(Android.App.Application.Context, LanguageSupport.Main.IO.FileDownloadSuccess(), ToastLength.Long);
                ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.FileDownloadSuccess();
                return true;
            }
            else
            {
                Toast.MakeText(Android.App.Application.Context, LanguageSupport.Main.IO.FileDownloadFailed(), ToastLength.Long);
                ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.FileDownloadFailed();
                return false;
            }
        }

        /// <summary>
        /// uri or url can be null but not both.
        /// 
        /// returns true if the download succeded or file already exists
        /// </summary>
        /// <param name="url"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<bool> DownloadAndSaveVideo(VideoCard vc)
        {
            string filePath = "";
            try
            {
                ViewHelpers.Tab3.DownloadProgressTextView.Text = "Starting download";
                _wc = new ExtWebClient();
                _wc.DownloadProgressChanged += OnVideoDownloadProgressChanged;
                _wc.DownloadFileCompleted += OnVideoDownloadFinished;

                var documentsPath = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";

                if (ViewHelpers.Tab3.AutoFillVideoTitleText.Checked)
                {
                    filePath = string.Join("_", vc.Title.Split(System.IO.Path.GetInvalidFileNameChars()));
                    ViewHelpers.Tab3.DownloadFileNameEditText.Text = filePath;
                    filePath = documentsPath + filePath;
                }
                else
                {
                    filePath = documentsPath + ViewHelpers.Tab3.DownloadFileNameEditText.Text;
                }
                if (!filePath.EndsWith(@".mp4"))
                {
                    filePath = filePath + @".mp4";
                }
                if (vc != null)
                {
                    if ((vc.Link == null || vc.Link == "") && (vc.VideoUri == null || vc.VideoUri.AbsolutePath == ""))
                    {
                        ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.VideoSourceMissing();
                        Toast.MakeText(Android.App.Application.Context, LanguageSupport.Main.IO.VideoSourceMissing(), ToastLength.Long);
                        return false;
                    }
                }
            }
            catch { }
            try
            {
                if (vc.VideoUri != null)
                {
                    await Task.Run(() =>
                    {
                        _wc.OpenRead(vc.VideoUri);
                        bytes_total = System.Convert.ToInt64(_wc.ResponseHeaders["Content-Length"]);
                    });
                    await _wc.DownloadFileTaskAsync(vc.VideoUri, filePath);
                }
                else
                {
                    await Task.Run(() =>
                    {
                        _wc.OpenRead(new System.Uri(vc.Link));
                        bytes_total = System.Convert.ToInt64(_wc.ResponseHeaders["Content-Length"]);
                    });
                    await _wc.DownloadFileTaskAsync(new System.Uri(vc.Link),  filePath);
                }
                VideoDownloadInProgress = false;
                if (System.IO.File.Exists(filePath))
                {
                    Toast.MakeText(Android.App.Application.Context, LanguageSupport.Main.IO.FileDownloadSuccess(), ToastLength.Long);
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.FileDownloadSuccess();
                    return true;
                }
                else
                {
                    Toast.MakeText(Android.App.Application.Context, LanguageSupport.Main.IO.FileDownloadFailed(), ToastLength.Long);
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.FileDownloadFailed();
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                try {
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = $"An error occured getting from {vc.VideoUri} try " +
                     $"another seed?";
                    ViewHelpers.Tab3.DownloadLinkEditText.Text = vc.VideoUri.ToString();
                }
                catch { }
                System.Console.WriteLine(ex.InnerException);
            }
            return false;
        }

        public static void CancelDownloadButton_OnClick(object sender, System.EventArgs e)
        {
            try { _wc.CancelAsync(); }
            catch{}
            VideoDownloadInProgress = false;
        }

        public override IBinder OnBind(Intent intent) { return null; }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                if (MainPlaybackSticky.Pm == null)
                {
                    MainPlaybackSticky.Pm = (PowerManager)GetSystemService(Context.PowerService);
                }
                PowerManager.WakeLock _wl = MainPlaybackSticky.Pm.NewWakeLock(WakeLockFlags.Partial, "Wakelock_BC");
                _wl.Acquire();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// When we start on the foreground we will present a notification to the user
        /// When they press the notification it will take them to the main page so they can control the music
        /// </summary>
        public void StartForeground()
        {
            try
            {
                var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0,
                                new Intent(ApplicationContext, typeof(MainActivity)),
                                PendingIntentFlags.UpdateCurrent);

                var builder = new Android.Support.V4.App.NotificationCompat.Builder(Android.App.Application.Context, MainActivity.CHANNEL_ID)
                                .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                                .SetContentTitle("BitChute video download started")
                                .SetSmallIcon(Resource.Drawable.bitchute_notification2)
                                .SetPriority(NotificationCompat.PriorityLow);

                StartForeground(-7777, builder.Build());
            }
            catch
            {

            }
        }
    }
}
