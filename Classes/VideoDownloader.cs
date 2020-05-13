
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

namespace BottomNavigationViewPager.Classes
{
    public class VideoDownloader
    {
        public static bool LatestDownloadSucceeded;

        public static void VideoDownloadButton_OnClick(object sender, System.EventArgs e)
        {
            DownloadVideo(ViewHelpers.Tab3.DownloadLinkEditText.Text);
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

        public static async void DownloadVideo(string videoLink)
        {
            GetExternalPermissions();
            
            VideoDownloader _vd = new VideoDownloader();
            if (videoLink != null && videoLink != "")
            {
                Task<string> rawHtmlTask = ExtWebInterfaceGeneral.GetHtmlTextFromUrl(videoLink);
                await rawHtmlTask;
                Task<string> videoUrlDecode = _vd.DecodeHtmlVideoSource(rawHtmlTask.Result);
                await videoUrlDecode;
                if (videoUrlDecode.Result == "" || videoUrlDecode.Result == null)
                {
                    LatestDownloadSucceeded = false;
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Common.IO.VideoSourceMissing();
                    return;
                }
                Task<bool> videoDownloadComplete = _vd.DownloadAndSaveVideo(videoUrlDecode.Result);
                await videoDownloadComplete;
            }
            else
            {
                await _vd.DownloadAndSaveVideo(null);
            }

        }

        public async Task<string> DecodeHtmlVideoSource(string html)
        {
            string videoLink = "";
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
                        }
                    }
                }
                catch
                {
                }
                //_fm5.SendNotifications();
            });

            return videoLink;
        }

        private static int _progressRed = 50;
        private static int _progressBlue = 50;
        private static int _progressGreen = 20;
        private static int _progressStep = 0;
        private static string _progressText;
        private static Android.Graphics.Color _progressColor;

        public static void OnVideoDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            switch (_progressStep)
            {
                case 0:
                    _progressText = "Downloading   " + e.ProgressPercentage.ToString();
                    _progressStep++;
                    break;
                case 1:
                    _progressText = "Downloading.  " + e.ProgressPercentage.ToString();
                    _progressStep++;
                    break;
                case 2:
                    _progressText = "Downloading.. " + e.ProgressPercentage.ToString();
                    _progressStep++;
                    break;
                case 3:
                    _progressText = "Downloading..." + e.ProgressPercentage.ToString();
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

            if (_progressBlue <= 250)
            { 
                _progressBlue++;
            }
            else
            {
                _progressBlue = 50;
            }
        }

        public static void OnVideoDownloadFinished(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _progressBlue = 50;
        }

        public async Task<bool> DownloadAndSaveVideo(string url)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadProgressChanged += OnVideoDownloadProgressChanged;
            wc.DownloadFileCompleted += OnVideoDownloadFinished;
            var documentsPath = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";
            string filePath = documentsPath + ViewHelpers.Tab3.DownloadFileNameEditText.Text;
            if (url == null || url == "")
            {
                //debug
                url = @"https://file-examples.com/wp-content/uploads/2017/04/file_example_MP4_1280_10MG.mp4";
            }
            try
            {
                await wc.DownloadFileTaskAsync(
                    new System.Uri(url),
                    filePath);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            if (System.IO.File.Exists(filePath))
            {
                Toast.MakeText(Android.App.Application.Context, LanguageSupport.Common.IO.FileDownloadSuccess() ,ToastLength.Long);
                ViewHelpers.Tab3.DownloadLinkEditText.Text = LanguageSupport.Common.IO.FileDownloadSuccess();
                LatestDownloadSucceeded = true;
                return true;
            }
            else
            {
                Toast.MakeText(Android.App.Application.Context, LanguageSupport.Common.IO.FileDownloadFailed(), ToastLength.Long);
                ViewHelpers.Tab3.DownloadLinkEditText.Text = LanguageSupport.Common.IO.FileDownloadFailed();
                LatestDownloadSucceeded = false;
                return false;
            }
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