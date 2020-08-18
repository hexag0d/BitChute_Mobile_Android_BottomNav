using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Widget;
using BitChute.Fragments;
using HtmlAgilityPack;
using StartServices.Servicesclass;
using static BitChute.Models.VideoModel;

namespace BitChute.Classes
{
    [Service(Exported = true)]
    public class VideoDownloader : Service
    {
        public static bool LatestDownloadSucceeded;
        public static bool VideoDownloadInProgress;
        static System.Int64 bytes_total;
        private static ExtWebClient _wc; 
        //private static System.Net.WebClient _wc;
        

        public static void VideoDownloadButton_OnClick(object sender, System.EventArgs e)
        {
            ViewHelpers.Tab3.DownloadProgressTextView.Text = "Initializing Download";
            Toast.MakeText(Android.App.Application.Context, "Download started",ToastLength.Long);
            InitializeVideoDownload(ViewHelpers.Tab3.DownloadLinkEditText.Text);
        }

        public static void DownloadFAB_OnClick(object sender, System.EventArgs e)
        {
            ViewHelpers.InjectJavascriptIntoWebView(MainActivity.ViewPager.CurrentItem, null);
            //ViewHelpers.RestoreDisqusIFrame(MainActivity.ViewPager.CurrentItem); // @DEBUG using this button to test the initial js command, because I haven't pinpointed exactly how to recover the missing iFrame at this time; this needs to be switched back to the download button and an iFrame event inspection needs implemented before golive
            //InitializeVideoDownload(GetVideoUrlByTab(MainActivity.ViewPager.CurrentItem));  // @TODO restore
        }

        public static string GetVideoUrlByTab(int tab)
        {
            string taburl = "";
            switch (tab)
            {
                case 0: taburl = Tab0Frag.Wv.OriginalUrl; break;
                case 1: taburl = Tab1Frag.Wv.OriginalUrl; break;
                case 2: taburl = Tab2Frag.Wv.OriginalUrl; break;
                case 3: taburl = Tab3Frag.Wv.OriginalUrl; break;
                case 4: taburl = Tab4Frag.Wv.OriginalUrl; break;
            }
            return taburl;
        }

        public static bool WriteFilePermissionGranted;
        public static bool ReadFilePermissionGranted;
        public static bool GetExternalPermissions()
        {
            try
            {
                if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(MainActivity.Main, Android.Manifest.Permission.WriteExternalStorage) != (int)Android.Content.PM.Permission.Granted)
                {
                    Android.Support.V4.App.ActivityCompat.RequestPermissions(MainActivity.Main, new string[] { Android.Manifest.Permission.WriteExternalStorage }, 0);
                }

                if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(MainActivity.Main, Android.Manifest.Permission.ReadExternalStorage) != (int)Android.Content.PM.Permission.Granted)
                {
                    Android.Support.V4.App.ActivityCompat.RequestPermissions(MainActivity.Main, new string[] { Android.Manifest.Permission.ReadExternalStorage }, 0);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async void InitializeVideoDownload(string videoLink)
        {
            if (!VideoDownloadInProgress)
            {
                ViewHelpers.Tab3.DownloadProgressTextView.Text = "Getting permissions";
                Task<bool> pTask = Task.FromResult<bool>(GetExternalPermissions());
                await pTask;
                VideoDownloadInProgress = true;
                VideoDownloader _vd = new VideoDownloader();
                if (videoLink != null && videoLink != "")
                {
                    ViewHelpers.Tab3.DownloadProgressTextView.Text = "Getting video link";
                    Task<string> rawHtmlTask = ExtWebInterface.GetHtmlTextFromUrl(videoLink);
                    await rawHtmlTask;
                    Task<VideoCard> videoCardTask = _vd.DecodeHtmlVideoSource(rawHtmlTask.Result);
                    await videoCardTask;
                    if ((videoCardTask.Result).VideoUri.AbsolutePath == "" 
                        || (videoCardTask.Result).VideoUri.AbsolutePath == null)
                    {
                        ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.VideoSourceMissing();
                        Toast.MakeText(Android.App.Application.Context, LanguageSupport.Main.IO.VideoSourceMissing(), ToastLength.Long);
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
                ViewHelpers.Tab3.DownloadProgressTextView.Text = "Video download already in progress, stop it first";
                Toast.MakeText(Android.App.Application.Context, "Video download already in progress, stop it first", ToastLength.Long);
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

        public static void OnVideoDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            decimal progress = ((decimal)e.BytesReceived/(decimal)bytes_total) * 100;
            if (progress < 100)
            _progString = progress.ToString().Substring(0, 4);
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
            _progBlue = 150;
            _progBlueUp = true;
            _progColor = Android.Graphics.Color.Rgb(_progRed, _progGreen, _progBlue);
            ViewHelpers.Main.DownloadFAB.SetColorFilter(null); 
            ViewHelpers.Tab3.DownloadProgressTextView.SetTextColor(_progColor);
            ViewHelpers.Tab3.DownloadProgressTextView.Text = LanguageSupport.Main.IO.FileDownloadSuccess();
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
            //_wc = new System.Net.WebClient();
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
                        //_wc.DownloadFileAsync(vc.VideoUri, filePath);
                        _wc.OpenRead(vc.VideoUri);
                        bytes_total = System.Convert.ToInt64(_wc.ResponseHeaders["Content-Length"]);
                    });
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
                    await _wc.DownloadFileTaskAsync(new System.Uri(vc.Link),
                        filePath);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.InnerException);
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
            ViewHelpers.Tab3.DownloadProgressTextView.Text = "Starting download";
            //_wc = new System.Net.WebClient();
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
                       // _wc.DownloadFileAsync(vc.VideoUri, filePath);
                        _wc.OpenRead(vc.VideoUri);
                        bytes_total = System.Convert.ToInt64(_wc.ResponseHeaders["Content-Length"]);
                    });
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
                    await _wc.DownloadFileTaskAsync(new System.Uri(vc.Link),
                        filePath);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.InnerException);
            }
            VideoDownloadInProgress = false;
            if (System.IO.File.Exists(filePath))
            {
                Toast.MakeText(Android.App.Application.Context, LanguageSupport.Main.IO.FileDownloadSuccess() ,ToastLength.Long);
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

        public static void CancelDownloadButton_OnClick(object sender, System.EventArgs e)
        {
            _wc.CancelAsync();
            VideoDownloadInProgress = false;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                if (ExtStickyService.Pm == null)
                {
                    ExtStickyService.Pm = (PowerManager)GetSystemService(Context.PowerService);
                }
                PowerManager.WakeLock _wl = ExtStickyService.Pm.NewWakeLock(WakeLockFlags.Partial, "My Tag");
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
                                .SetContentTitle("BitChute streaming in background")
                                .SetSmallIcon(Resource.Drawable.bitchute_notification2)
                                .SetPriority(NotificationCompat.PriorityMax);

                StartForeground(-6666, builder.Build());
            }
            catch
            {

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