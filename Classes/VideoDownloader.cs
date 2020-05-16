﻿
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using BottomNavigationViewPager.Fragments;
using HtmlAgilityPack;
using static BottomNavigationViewPager.Models.VideoModel;

namespace BottomNavigationViewPager.Classes
{
    public class VideoDownloader
    {
        public static bool LatestDownloadSucceeded;
        public static bool VideoDownloadInProgress;
        static System.Int64 bytes_total;
        private static System.Net.WebClient wc; 

        public static void VideoDownloadButton_OnClick(object sender, System.EventArgs e)
        {
            ViewHelpers.Tab3.DownloadProgressTextView.Text = "Initializing Download";
            InitializeVideoDownload(ViewHelpers.Tab3.DownloadLinkEditText.Text);
        }
        
        public static bool WriteFilePermissionGranted;
        public static bool ReadFilePermissionGranted;

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

        public static async void InitializeVideoDownload(string videoLink)
        {
            ViewHelpers.Tab3.DownloadProgressTextView.Text = "Getting permissions";
            GetExternalPermissions();
            if (!VideoDownloadInProgress)
            {
                VideoDownloadInProgress = true;
                VideoDownloader _vd = new VideoDownloader();
                if (videoLink != null && videoLink != "")
                {
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = "Getting video link";
                    Task<string> rawHtmlTask = ExtWebInterfaceGeneral.GetHtmlTextFromUrl(videoLink);
                    await rawHtmlTask;
                    Task<VideoCard> videoCardTask = _vd.DecodeHtmlVideoSource(rawHtmlTask.Result);
                    await videoCardTask;
                    if ((videoCardTask.Result).VideoUri.AbsolutePath == "" 
                        || (videoCardTask.Result).VideoUri.AbsolutePath == null)
                    {
                        ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Common.IO.VideoSourceMissing();
                        VideoDownloadInProgress = false;
                        return;
                    }
                    Task<bool> videoDownloadComplete = _vd.DownloadAndSaveVideo(videoCardTask.Result);
                    await videoDownloadComplete;
                }
                else
                {
                    await _vd.DownloadAndSaveVideo(null);
                }
            }
            else
            {
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
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    if (doc != null)
                    {
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
                catch
                {
                }
            });

            return vidCard;
        }

        private static int _progressRed = 50;
        private static int _progressBlue = 50;
        private static int _progressGreen = 20;
        private static int _progressStep = 0;
        private static string _progressText;
        private static string _progressString;
        private static Android.Graphics.Color _progressColor;
        private static bool _progressBlueUp = true;
        
        public static void OnVideoDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            decimal progress = ((decimal)e.BytesReceived/(decimal)bytes_total) * 100;
            if (progress < 100)
            _progressString = progress.ToString().Substring(0, 4);
            switch (_progressStep)
            {
                case 0:
                    _progressText = "Downloading   " + _progressString + @"%";
                    _progressStep++;
                    break;
                case 1:
                    _progressText = "Downloading.  " + _progressString + @"%";
                    _progressStep++;
                    break;
                case 2:
                    _progressText = "Downloading.. " + _progressString + @"%";
                    _progressStep++;
                    break;
                case 3:
                    _progressText = "Downloading..." + _progressString + @"%";
                    _progressStep = 0;
                    break;
            }
            ViewHelpers.Tab3.DownloadProgressTextView.Text = (_progressText);
            _progressColor = Android.Graphics.Color.Rgb(_progressRed, _progressGreen, _progressBlue);
            ViewHelpers.Tab3.DownloadProgressTextView.SetTextColor(_progressColor);
            ViewHelpers.Tab3.DownloadProgressBar.Progress = e.ProgressPercentage;
            ViewHelpers.Tab3.DownloadProgressBar.ProgressDrawable
                .SetColorFilter(_progressColor,
                Android.Graphics.PorterDuff.Mode.Multiply);
            ViewHelpers.Tab3.DownloadProgressBar.IndeterminateDrawable
                .SetColorFilter(_progressColor,
                Android.Graphics.PorterDuff.Mode.SrcAtop);
            if (_progressBlueUp)
            {
                if (_progressBlue <= 250)
                {
                    _progressBlue++;
                }
                else
                {
                    _progressBlue--;
                    _progressBlueUp = false;
                }
            }
            else
            {
                if (_progressBlue >= 50)
                {
                    _progressBlue--;
                }
                else
                {
                    _progressBlue++;
                    _progressBlueUp = true;
                }
            }
        }

        public static void OnVideoDownloadFinished(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _progressBlue = 150;
            _progressBlueUp = true;
            _progressColor = Android.Graphics.Color.Rgb(_progressRed, _progressGreen, _progressBlue);
            ViewHelpers.Tab3.DownloadProgressTextView.SetTextColor(_progressColor);
            ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Common.IO.FileDownloadSuccess();
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
            ViewHelpers.Tab3.DownloadProgressTextView.Text = "Starting download";
            wc = new System.Net.WebClient();
            wc.DownloadProgressChanged += OnVideoDownloadProgressChanged;
            wc.DownloadFileCompleted += OnVideoDownloadFinished;
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
                    ViewHelpers.Tab3.DownloadLinkEditText.Text = LanguageSupport.Common.IO.VideoSourceMissing();
                    return false;
                }
            }
            else
            {
                ViewHelpers.Tab3.DownloadLinkEditText.Text = LanguageSupport.Common.IO.VideoSourceMissing();
                return false;
            }
            try
            {
                if (vc.VideoUri != null)
                {
                    await Task.Run(() =>
                    {
                        wc.OpenRead(vc.VideoUri);
                        bytes_total = System.Convert.ToInt64(wc.ResponseHeaders["Content-Length"]);
                    });
                    await wc.DownloadFileTaskAsync(vc.VideoUri,
                        filePath);
                }
                else
                {
                    await Task.Run(() =>
                    {
                        wc.OpenRead(new System.Uri(vc.Link));
                        bytes_total = System.Convert.ToInt64(wc.ResponseHeaders["Content-Length"]);
                    });
                    await wc.DownloadFileTaskAsync(new System.Uri(vc.Link),
                        filePath);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            VideoDownloadInProgress = false;
            if (System.IO.File.Exists(filePath))
            {
                Toast.MakeText(Android.App.Application.Context, LanguageSupport.Common.IO.FileDownloadSuccess() ,ToastLength.Long);
                ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Common.IO.FileDownloadSuccess();
                return true;
            }
            else
            {
                Toast.MakeText(Android.App.Application.Context, LanguageSupport.Common.IO.FileDownloadFailed(), ToastLength.Long);
                ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Common.IO.FileDownloadFailed();
                return false;
            }
        }

        public static void CancelDownloadButton_OnClick(object sender, System.EventArgs e)
        {
            wc.CancelAsync();
        }
    }
}

//using (var streamWriter = new StreamWriter(filename, true))
//{
//    streamWriter.WriteLine(DateTime.UtcNow);
//}

//using (var streamReader = new StreamReader(filename))
//{
//    string content = streamReader.ReadToEnd();
//    System.Diagnostics.Debug.WriteLine(content);
//}